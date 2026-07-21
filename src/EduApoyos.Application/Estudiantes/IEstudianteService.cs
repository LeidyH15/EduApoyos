using EduApoyos.Application.Common.Models;

namespace EduApoyos.Application.Estudiantes;

public interface IEstudianteService
{
    Task<ResultadoPaginado<EstudianteResponse>> ListarAsync(
        int pagina,
        int tamanoPagina,
        CancellationToken cancellationToken = default);

    Task<EstudianteResponse> ObtenerPorIdAsync(
        Guid estudianteId,
        CancellationToken cancellationToken = default);

    Task<EstudianteResponse> CrearAsync(
        CrearEstudianteRequest request,
        CancellationToken cancellationToken = default);

    Task<EstudianteResponse> ActualizarAsync(
        Guid estudianteId,
        ActualizarEstudianteRequest request,
        CancellationToken cancellationToken = default);

    Task EliminarAsync(
        Guid estudianteId,
        CancellationToken cancellationToken = default);
}