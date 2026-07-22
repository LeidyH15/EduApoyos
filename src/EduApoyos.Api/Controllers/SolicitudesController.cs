using EduApoyos.Application.Abstractions.Authentication;
using EduApoyos.Application.Common.Models;
using EduApoyos.Application.Solicitudes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduApoyos.Api.Controllers;

[ApiController]
[Route("api/solicitudes")]
[Authorize]
public class SolicitudesController : ControllerBase
{
    private readonly ISolicitudApoyoService
        _solicitudService;

    private readonly IUsuarioActualService
        _usuarioActual;

    public SolicitudesController(
        ISolicitudApoyoService solicitudService,
        IUsuarioActualService usuarioActual)
    {
        _solicitudService = solicitudService;
        _usuarioActual = usuarioActual;
    }

    /// <summary>
    /// Lista las solicitudes aplicando filtros y paginación.
    /// Disponible únicamente para asesores.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Asesor")]
    [ProducesResponseType(
        typeof(ResultadoPaginado<SolicitudResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<
        ResultadoPaginado<SolicitudResponse>>> Listar(
        [FromQuery] SolicitudFiltroRequest filtro,
        CancellationToken cancellationToken = default)
    {
        var resultado =
            await _solicitudService.ListarAsync(
                filtro,
                cancellationToken);

        return Ok(resultado);
    }

    /// <summary>
    /// Obtiene el detalle de una solicitud y su historial.
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Asesor,Estudiante")]
    [ProducesResponseType(
        typeof(SolicitudResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status403Forbidden)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SolicitudResponse>>
        ObtenerPorId(
            Guid id,
            CancellationToken cancellationToken = default)
    {
        var solicitud =
            await _solicitudService.ObtenerPorIdAsync(
                id,
                _usuarioActual.UsuarioId,
                _usuarioActual.EsAsesor,
                cancellationToken);

        return Ok(solicitud);
    }

    /// <summary>
    /// Crea una solicitud de apoyo.
    /// Un asesor indica el estudiante; un estudiante utiliza su perfil.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Asesor,Estudiante")]
    [ProducesResponseType(
        typeof(SolicitudResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(ValidationProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status403Forbidden)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SolicitudResponse>> Crear(
        [FromBody] CrearSolicitudRequest request,
        CancellationToken cancellationToken = default)
    {
        var solicitud =
            await _solicitudService.CrearAsync(
                _usuarioActual.UsuarioId,
                _usuarioActual.EsAsesor,
                request,
                cancellationToken);

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = solicitud.Id },
            solicitud);
    }

    /// <summary>
    /// Actualiza la información de una solicitud pendiente.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Asesor,Estudiante")]
    [ProducesResponseType(
        typeof(SolicitudResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ValidationProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status403Forbidden)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<SolicitudResponse>>
        Actualizar(
            Guid id,
            [FromBody] ActualizarSolicitudRequest request,
            CancellationToken cancellationToken = default)
    {
        var solicitud =
            await _solicitudService.ActualizarAsync(
                id,
                _usuarioActual.UsuarioId,
                _usuarioActual.EsAsesor,
                request,
                cancellationToken);

        return Ok(solicitud);
    }

    /// <summary>
    /// Cambia el estado de una solicitud aplicando Strategy.
    /// Disponible únicamente para asesores.
    /// </summary>
    [HttpPatch("{id:guid}/estado")]
    [Authorize(Roles = "Asesor")]
    [ProducesResponseType(
        typeof(SolicitudResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ValidationProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status403Forbidden)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<SolicitudResponse>>
        CambiarEstado(
            Guid id,
            [FromBody] CambiarEstadoSolicitudRequest request,
            CancellationToken cancellationToken = default)
    {
        var solicitud =
            await _solicitudService.CambiarEstadoAsync(
                id,
                _usuarioActual.UsuarioId,
                request,
                cancellationToken);

        return Ok(solicitud);
    }

    /// <summary>
    /// Descarga una constancia de la solicitud en formato de texto
    /// El estudiante solo puede descargar constancias propias
    /// </summary>
    [HttpGet("{id:guid}/constancia")]
    [Authorize(Roles = "Asesor,Estudiante")]
    [ProducesResponseType(
        typeof(FileContentResult),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status403Forbidden)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DescargarConstancia(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var constancia =
            await _solicitudService.GenerarConstanciaAsync(
                id,
                _usuarioActual.UsuarioId,
                _usuarioActual.EsAsesor,
                cancellationToken);

        return File(
            constancia.Contenido,
            constancia.TipoContenido,
            constancia.NombreArchivo);
    }
}