using EduApoyos.Application.Abstractions.Authentication;
using EduApoyos.Application.Common.Models;
using EduApoyos.Application.Solicitudes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduApoyos.Api.Controllers;

[ApiController]
[Route("api/estudiantes/{estudianteId:guid}/solicitudes")]
[Authorize(Roles = "Asesor,Estudiante")]
public class EstudianteSolicitudesController : ControllerBase
{
    private readonly ISolicitudApoyoService
        _solicitudService;

    private readonly IUsuarioActualService
        _usuarioActual;

    public EstudianteSolicitudesController(
        ISolicitudApoyoService solicitudService,
        IUsuarioActualService usuarioActual)
    {
        _solicitudService = solicitudService;
        _usuarioActual = usuarioActual;
    }

    /// <summary>
    /// Lista las solicitudes de un estudiante.
    /// El estudiante solamente puede consultar sus propias solicitudes.
    /// </summary>
    [HttpGet]
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
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    public async Task<ActionResult<
        ResultadoPaginado<SolicitudResponse>>> Listar(
        Guid estudianteId,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanoPagina = 10,
        CancellationToken cancellationToken = default)
    {
        var resultado =
            await _solicitudService
                .ListarPorEstudianteAsync(
                    estudianteId,
                    _usuarioActual.UsuarioId,
                    _usuarioActual.EsAsesor,
                    pagina,
                    tamanoPagina,
                    cancellationToken);

        return Ok(resultado);
    }
}