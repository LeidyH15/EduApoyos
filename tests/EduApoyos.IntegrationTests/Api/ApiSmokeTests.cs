using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using EduApoyos.IntegrationTests.Infrastructure;
using System.Net.Http.Headers;

namespace EduApoyos.IntegrationTests.Api;

public class ApiSmokeTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ApiSmokeTests(
        CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Swagger_DevuelveDocumentoOpenApi()
    {
        var response =
            await _client.GetAsync(
                "/swagger/v1/swagger.json");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        Assert.Equal(
            "application/json",
            response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task EstudiantesSinToken_DevuelveNoAutorizado()
    {
        var response =
            await _client.GetAsync(
                "/api/estudiantes?pagina=1&tamanoPagina=10");

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task LoginIncorrecto_DevuelveProblemDetails()
    {
        var request = new
        {
            email = "asesor.tests@eduapoyos.local",
            password = "Password_Incorrecto1!"
        };

        var response =
            await _client.PostAsJsonAsync(
                "/api/auth/login",
                request);

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);

        Assert.Equal(
            "application/problem+json",
            response.Content.Headers.ContentType?.MediaType);

        var contenido =
            await response.Content.ReadAsStringAsync();

        using var documento =
            JsonDocument.Parse(contenido);

        var raiz = documento.RootElement;

        Assert.Equal(
            401,
            raiz.GetProperty("status").GetInt32());

        Assert.Equal(
            "No autorizado",
            raiz.GetProperty("title").GetString());

        Assert.True(
            raiz.TryGetProperty(
                "traceId",
                out _));
    }

    [Fact]
    public async Task LoginCorrecto_DevuelveTokenJwt()
    {
        var request = new
        {
            email = "asesor.tests@eduapoyos.local",
            password = "Asesor_Test2026!"
        };

        var response =
            await _client.PostAsJsonAsync(
                "/api/auth/login",
                request);

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var contenido =
            await response.Content.ReadAsStringAsync();

        using var documento =
            JsonDocument.Parse(contenido);

        var raiz = documento.RootElement;

        var token =
            raiz.GetProperty("token").GetString();

        Assert.False(
            string.IsNullOrWhiteSpace(token));

        Assert.Equal(
            "Asesor",
            raiz.GetProperty("rol").GetString());

        Assert.Equal(
            "asesor.tests@eduapoyos.local",
            raiz.GetProperty("email").GetString());
    }

    [Fact]
    public async Task ListadoConToken_DevuelveOk()
    {
        var token =
            await ObtenerTokenAsesorAsync();

        using var request =
            new HttpRequestMessage(
                HttpMethod.Get,
                "/api/estudiantes?pagina=1&tamanoPagina=10");

        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);

        var response =
            await _client.SendAsync(request);

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        Assert.Equal(
            "application/json",
            response.Content.Headers.ContentType?.MediaType);

        var contenido =
            await response.Content.ReadAsStringAsync();

        using var documento =
            JsonDocument.Parse(contenido);

        var raiz = documento.RootElement;

        Assert.Equal(
            1,
            raiz.GetProperty("pagina").GetInt32());

        Assert.Equal(
            10,
            raiz.GetProperty("tamanoPagina").GetInt32());

        Assert.True(
            raiz.TryGetProperty(
                "elementos",
                out _));

        Assert.True(
            raiz.TryGetProperty(
                "totalElementos",
                out _));

        Assert.True(
            raiz.TryGetProperty(
                "totalPaginas",
                out _));
    }

    private async Task<string> ObtenerTokenAsesorAsync()
    {
        var request = new
        {
            email = "asesor.tests@eduapoyos.local",
            password = "Asesor_Test2026!"
        };

        var response =
            await _client.PostAsJsonAsync(
                "/api/auth/login",
                request);

        response.EnsureSuccessStatusCode();

        var contenido =
            await response.Content.ReadAsStringAsync();

        using var documento =
            JsonDocument.Parse(contenido);

        return documento.RootElement
            .GetProperty("token")
            .GetString()
            ?? throw new InvalidOperationException(
                "La API no devolvió el token JWT.");
    }

}