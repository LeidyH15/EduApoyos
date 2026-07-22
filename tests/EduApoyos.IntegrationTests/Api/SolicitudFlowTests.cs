using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using EduApoyos.IntegrationTests.Infrastructure;

namespace EduApoyos.IntegrationTests.Api;

public class SolicitudFlowTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public SolicitudFlowTests(
        CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task FlujoSolicitud_CompletaProceso()
    {
        var identificador =
            Guid.NewGuid().ToString("N");

        var emailEstudiante =
            $"estudiante.{identificador}@tests.local";

        var registro = new
        {
            nombreCompleto =
                "Estudiante Integración",
            email = emailEstudiante,
            password =
                "Estudiante_Test2026!",
            numeroDocumento =
                $"T-{identificador[..18]}",
            tipoDocumento = 1,
            programaAcademico =
                "Ingeniería de Sistemas",
            semestre = 5
        };

        var registroResponse =
            await _client.PostAsJsonAsync(
                "/api/auth/register",
                registro);

        var errorRegistro =
            await registroResponse.Content
                .ReadAsStringAsync();

        Assert.True(
            registroResponse.IsSuccessStatusCode,
            $"Falló el registro: {errorRegistro}");

        var tokenEstudiante =
            await ObtenerTokenAsync(
                registroResponse);

        var crearSolicitud = new
        {
            tipoApoyo = 1,
            montoSolicitado = 1800000m,
            descripcion =
                "Solicitud de beca creada desde una prueba de integración."
        };

        using var crearRequest =
            CrearRequestAutorizado(
                HttpMethod.Post,
                "/api/solicitudes",
                tokenEstudiante,
                crearSolicitud);

        var crearResponse =
            await _client.SendAsync(
                crearRequest);

        Assert.Equal(
            HttpStatusCode.Created,
            crearResponse.StatusCode);

        var solicitudJson =
            await LeerJsonAsync(
                crearResponse);

        var solicitudId =
            solicitudJson
                .GetProperty("id")
                .GetGuid();

        var estudianteId =
            solicitudJson
                .GetProperty("estudianteId")
                .GetGuid();

        Assert.Equal(
            1,
            solicitudJson
                .GetProperty("estado")
                .GetInt32());

        using var portalRequest =
            CrearRequestAutorizado(
                HttpMethod.Get,
                $"/api/estudiantes/{estudianteId}/solicitudes",
                tokenEstudiante);

        var portalResponse =
            await _client.SendAsync(
                portalRequest);

        Assert.Equal(
            HttpStatusCode.OK,
            portalResponse.StatusCode);

        var portalJson =
            await LeerJsonAsync(
                portalResponse);

        Assert.Equal(
            1,
            portalJson
                .GetProperty("totalElementos")
                .GetInt32());

        using var constanciaRequest =
            CrearRequestAutorizado(
                HttpMethod.Get,
                $"/api/solicitudes/{solicitudId}/constancia",
                tokenEstudiante);

        var constanciaResponse =
            await _client.SendAsync(
                constanciaRequest);

        Assert.Equal(
            HttpStatusCode.OK,
            constanciaResponse.StatusCode);

        Assert.Equal(
            "text/plain",
            constanciaResponse
                .Content
                .Headers
                .ContentType?
                .MediaType);

        Assert.Equal(
            "utf-8",
            constanciaResponse
                .Content
                .Headers
                .ContentType?
                .CharSet);

        Assert.Equal(
            "attachment",
            constanciaResponse
                .Content
                .Headers
                .ContentDisposition?
                .DispositionType);

        var nombreArchivo =
            constanciaResponse
                .Content
                .Headers
                .ContentDisposition?
                .FileNameStar
            ?? constanciaResponse
                .Content
                .Headers
                .ContentDisposition?
                .FileName;

        Assert.NotNull(nombreArchivo);

        Assert.Contains(
            solicitudId.ToString(),
            nombreArchivo!,
            StringComparison.OrdinalIgnoreCase);

        var contenidoConstancia =
            await constanciaResponse.Content
                .ReadAsStringAsync();

        Assert.Contains(
            "CONSTANCIA DE SOLICITUD DE APOYO ECONÓMICO",
            contenidoConstancia);

        Assert.Contains(
            solicitudId.ToString(),
            contenidoConstancia);

        Assert.Contains(
            "Estudiante Integración",
            contenidoConstancia);

        Assert.Contains(
            "Solicitud de beca creada desde una prueba de integración.",
            contenidoConstancia);

        var tokenAsesor =
            await LoginAsesorAsync();

        using var listadoRequest =
            CrearRequestAutorizado(
                HttpMethod.Get,
                "/api/solicitudes?pagina=1&tamanoPagina=10",
                tokenAsesor);

        var listadoResponse =
            await _client.SendAsync(
                listadoRequest);

        Assert.Equal(
            HttpStatusCode.OK,
            listadoResponse.StatusCode);

        var cambioEstado = new
        {
            nuevoEstado = 2,
            observacion =
                "Solicitud recibida para revisión."
        };

        using var cambioRequest =
            CrearRequestAutorizado(
                HttpMethod.Patch,
                $"/api/solicitudes/{solicitudId}/estado",
                tokenAsesor,
                cambioEstado);

        var cambioResponse =
            await _client.SendAsync(
                cambioRequest);

        Assert.Equal(
            HttpStatusCode.OK,
            cambioResponse.StatusCode);

        var cambioJson =
            await LeerJsonAsync(
                cambioResponse);

        Assert.Equal(
            2,
            cambioJson
                .GetProperty("estado")
                .GetInt32());

        Assert.Equal(
            1,
            cambioJson
                .GetProperty("historial")
                .GetArrayLength());
    }

    [Fact]
    public async Task ConstanciaAjena_DevuelveProhibido()
    {
        var tokenPropietario =
            await RegistrarEstudianteAsync(
                "Estudiante Propietario");

        var crearSolicitud = new
        {
            tipoApoyo = 1,
            montoSolicitado = 1200000m,
            descripcion =
                "Solicitud perteneciente al primer estudiante."
        };

        using var crearRequest =
            CrearRequestAutorizado(
                HttpMethod.Post,
                "/api/solicitudes",
                tokenPropietario,
                crearSolicitud);

        var crearResponse =
            await _client.SendAsync(
                crearRequest);

        Assert.Equal(
            HttpStatusCode.Created,
            crearResponse.StatusCode);

        var solicitudJson =
            await LeerJsonAsync(
                crearResponse);

        var solicitudId =
            solicitudJson
                .GetProperty("id")
                .GetGuid();

        var tokenOtroEstudiante =
            await RegistrarEstudianteAsync(
                "Estudiante Sin Acceso");

        using var constanciaRequest =
            CrearRequestAutorizado(
                HttpMethod.Get,
                $"/api/solicitudes/{solicitudId}/constancia",
                tokenOtroEstudiante);

        var constanciaResponse =
            await _client.SendAsync(
                constanciaRequest);

        Assert.Equal(
            HttpStatusCode.Forbidden,
            constanciaResponse.StatusCode);

        Assert.Equal(
            "application/problem+json",
            constanciaResponse
                .Content
                .Headers
                .ContentType?
                .MediaType);

        var errorJson =
            await LeerJsonAsync(
                constanciaResponse);

        Assert.Equal(
            403,
            errorJson
                .GetProperty("status")
                .GetInt32());

        Assert.True(
            errorJson.TryGetProperty(
                "traceId",
                out _));
    }

    private async Task<string> RegistrarEstudianteAsync(
    string nombreCompleto)
    {
        var identificador =
            Guid.NewGuid().ToString("N");

        var registro = new
        {
            nombreCompleto,
            email =
                $"estudiante.{identificador}@tests.local",
            password =
                "Estudiante_Test2026!",
            numeroDocumento =
                $"T-{identificador[..18]}",
            tipoDocumento = 1,
            programaAcademico =
                "Ingeniería de Sistemas",
            semestre = 4
        };

        var response =
            await _client.PostAsJsonAsync(
                "/api/auth/register",
                registro);

        var contenido =
            await response.Content
                .ReadAsStringAsync();

        Assert.True(
            response.IsSuccessStatusCode,
            $"Falló el registro del estudiante: {contenido}");

        return await ObtenerTokenAsync(
            response);
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

        response.EnsureSuccessStatusCode();

        return await ObtenerTokenAsync(
            response);
    }

    private static async Task<string> ObtenerTokenAsync(
        HttpResponseMessage response)
    {
        var json =
            await LeerJsonAsync(response);

        return json
            .GetProperty("token")
            .GetString()
            ?? throw new InvalidOperationException(
                "La respuesta no contiene un token.");
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
}