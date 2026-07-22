using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using EduApoyos.IntegrationTests.Infrastructure;

namespace EduApoyos.IntegrationTests.Api;

public class EstudianteFlowTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public EstudianteFlowTests(
        CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CrudEstudiante_CompletaProceso()
    {
        var token = await LoginAsesorAsync();

        var identificador =
            Guid.NewGuid().ToString("N");

        var email =
            $"estudiante.{identificador}@tests.local";

        var numeroDocumento =
            $"DOC-{identificador[..16]}";

        var crearEstudiante = new
        {
            nombreCompleto = "Estudiante Integración",
            email,
            password = "Estudiante_Test2026!",
            numeroDocumento,
            tipoDocumento = 1,
            programaAcademico = "Ingeniería de Sistemas",
            semestre = 5
        };

        using var crearRequest =
            CrearRequestAutorizado(
                HttpMethod.Post,
                "/api/estudiantes",
                token,
                crearEstudiante);

        var crearResponse =
            await _client.SendAsync(crearRequest);

        await VerificarEstadoAsync(
            crearResponse,
            HttpStatusCode.Created,
            "creación del estudiante");

        var estudianteCreado =
            await LeerJsonAsync(crearResponse);

        var estudianteId =
            estudianteCreado
                .GetProperty("id")
                .GetGuid();

        Assert.NotEqual(
            Guid.Empty,
            estudianteId);

        Assert.Equal(
            "Estudiante Integración",
            estudianteCreado
                .GetProperty("nombreCompleto")
                .GetString());

        Assert.Equal(
            email,
            estudianteCreado
                .GetProperty("email")
                .GetString());

        Assert.Equal(
            numeroDocumento,
            estudianteCreado
                .GetProperty("numeroDocumento")
                .GetString());

        Assert.Equal(
            5,
            estudianteCreado
                .GetProperty("semestre")
                .GetInt32());

        using var obtenerRequest =
            CrearRequestAutorizado(
                HttpMethod.Get,
                $"/api/estudiantes/{estudianteId}",
                token);

        var obtenerResponse =
            await _client.SendAsync(obtenerRequest);

        await VerificarEstadoAsync(
            obtenerResponse,
            HttpStatusCode.OK,
            "consulta del estudiante");

        var estudianteConsultado =
            await LeerJsonAsync(obtenerResponse);

        Assert.Equal(
            estudianteId,
            estudianteConsultado
                .GetProperty("id")
                .GetGuid());

        using var listadoRequest =
            CrearRequestAutorizado(
                HttpMethod.Get,
                "/api/estudiantes?pagina=1&tamanoPagina=10",
                token);

        var listadoResponse =
            await _client.SendAsync(listadoRequest);

        await VerificarEstadoAsync(
            listadoResponse,
            HttpStatusCode.OK,
            "listado de estudiantes");

        var listado =
            await LeerJsonAsync(listadoResponse);

        Assert.Equal(
            1,
            listado
                .GetProperty("pagina")
                .GetInt32());

        Assert.Equal(
            10,
            listado
                .GetProperty("tamanoPagina")
                .GetInt32());

        Assert.True(
            listado
                .GetProperty("totalElementos")
                .GetInt32() >= 1);

        var elementos =
            listado.GetProperty("elementos");

        Assert.Contains(
            elementos.EnumerateArray(),
            elemento =>
                elemento
                    .GetProperty("id")
                    .GetGuid() == estudianteId);

        var actualizarEstudiante = new
        {
            nombreCompleto =
                "Estudiante Integración Actualizado",
            email,
            numeroDocumento,
            tipoDocumento = 1,
            programaAcademico =
                "Ingeniería de Software",
            semestre = 6
        };

        using var actualizarRequest =
            CrearRequestAutorizado(
                HttpMethod.Put,
                $"/api/estudiantes/{estudianteId}",
                token,
                actualizarEstudiante);

        var actualizarResponse =
            await _client.SendAsync(actualizarRequest);

        await VerificarEstadoAsync(
            actualizarResponse,
            HttpStatusCode.OK,
            "actualización del estudiante");

        var estudianteActualizado =
            await LeerJsonAsync(actualizarResponse);

        Assert.Equal(
            "Estudiante Integración Actualizado",
            estudianteActualizado
                .GetProperty("nombreCompleto")
                .GetString());

        Assert.Equal(
            "Ingeniería de Software",
            estudianteActualizado
                .GetProperty("programaAcademico")
                .GetString());

        Assert.Equal(
            6,
            estudianteActualizado
                .GetProperty("semestre")
                .GetInt32());

        using var eliminarRequest =
            CrearRequestAutorizado(
                HttpMethod.Delete,
                $"/api/estudiantes/{estudianteId}",
                token);

        var eliminarResponse =
            await _client.SendAsync(eliminarRequest);

        await VerificarEstadoAsync(
            eliminarResponse,
            HttpStatusCode.NoContent,
            "eliminación del estudiante");

        using var obtenerEliminadoRequest =
            CrearRequestAutorizado(
                HttpMethod.Get,
                $"/api/estudiantes/{estudianteId}",
                token);

        var obtenerEliminadoResponse =
            await _client.SendAsync(
                obtenerEliminadoRequest);

        await VerificarEstadoAsync(
            obtenerEliminadoResponse,
            HttpStatusCode.NotFound,
            "consulta del estudiante eliminado");

        Assert.Equal(
            "application/problem+json",
            obtenerEliminadoResponse
                .Content
                .Headers
                .ContentType?
                .MediaType);
    }

    private async Task<string> LoginAsesorAsync()
    {
        var login = new
        {
            email =
                "asesor.tests@eduapoyos.local",
            password =
                "Asesor_Test2026!"
        };

        var response =
            await _client.PostAsJsonAsync(
                "/api/auth/login",
                login);

        await VerificarEstadoAsync(
            response,
            HttpStatusCode.OK,
            "inicio de sesión del asesor");

        var json =
            await LeerJsonAsync(response);

        return json
            .GetProperty("token")
            .GetString()
            ?? throw new InvalidOperationException(
                "La respuesta no contiene el token JWT.");
    }

    private static HttpRequestMessage CrearRequestAutorizado(
        HttpMethod method,
        string url,
        string token,
        object? body = null)
    {
        var request =
            new HttpRequestMessage(
                method,
                url);

        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);

        if (body is not null)
        {
            request.Content =
                JsonContent.Create(body);
        }

        return request;
    }

    private static async Task<JsonElement> LeerJsonAsync(
        HttpResponseMessage response)
    {
        var contenido =
            await response.Content
                .ReadAsStringAsync();

        using var documento =
            JsonDocument.Parse(contenido);

        return documento.RootElement.Clone();
    }

    private static async Task VerificarEstadoAsync(
        HttpResponseMessage response,
        HttpStatusCode esperado,
        string operacion)
    {
        if (response.StatusCode == esperado)
        {
            return;
        }

        var contenido =
            await response.Content
                .ReadAsStringAsync();

        Assert.Fail(
            $"Falló la {operacion}. " +
            $"Esperado: {(int)esperado} {esperado}. " +
            $"Recibido: {(int)response.StatusCode} " +
            $"{response.StatusCode}. " +
            $"Respuesta: {contenido}");
    }
}