using EduApoyos.Application.Common.Models;
using EduApoyos.Application.Estudiantes;
using EduApoyos.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduApoyos.Api.Controllers;

[ApiController]
[Route("api/estudiantes")]
[Authorize(Roles = nameof(RolUsuario.Asesor))]
public class EstudiantesController : ControllerBase
{
    private readonly IEstudianteService _estudianteService;

    public EstudiantesController(
        IEstudianteService estudianteService)
    {
        _estudianteService = estudianteService;
    }

    /// <summary>
    /// Lista los estudiantes de forma paginada.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(
        typeof(ResultadoPaginado<EstudianteResponse>),
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
        ResultadoPaginado<EstudianteResponse>>> Listar(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanoPagina = 10,
        CancellationToken cancellationToken = default)
    {
        var resultado =
            await _estudianteService.ListarAsync(
                pagina,
                tamanoPagina,
                cancellationToken);

        return Ok(resultado);
    }

    /// <summary>
    /// Obtiene un estudiante por su identificador.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(
        typeof(EstudianteResponse),
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
    public async Task<ActionResult<EstudianteResponse>>
        ObtenerPorId(
            Guid id,
            CancellationToken cancellationToken = default)
    {
        var estudiante =
            await _estudianteService.ObtenerPorIdAsync(
                id,
                cancellationToken);

        return Ok(estudiante);
    }

    /// <summary>
    /// Crea un estudiante y su usuario de acceso.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(
        typeof(EstudianteResponse),
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
        StatusCodes.Status409Conflict)]
    public async Task<ActionResult<EstudianteResponse>> Crear(
        [FromBody] CrearEstudianteRequest request,
        CancellationToken cancellationToken = default)
    {
        var estudiante =
            await _estudianteService.CrearAsync(
                request,
                cancellationToken);

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = estudiante.Id },
            estudiante);
    }

    /// <summary>
    /// Actualiza los datos personales y académicos de un estudiante.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(
        typeof(EstudianteResponse),
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
        StatusCodes.Status409Conflict)]
    public async Task<ActionResult<EstudianteResponse>>
        Actualizar(
            Guid id,
            [FromBody] ActualizarEstudianteRequest request,
            CancellationToken cancellationToken = default)
    {
        var estudiante =
            await _estudianteService.ActualizarAsync(
                id,
                request,
                cancellationToken);

        return Ok(estudiante);
    }

    /// <summary>
    /// Elimina un estudiante que no tenga solicitudes de apoyo.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
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
    public async Task<IActionResult> Eliminar(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await _estudianteService.EliminarAsync(
            id,
            cancellationToken);

        return NoContent();
    }
}