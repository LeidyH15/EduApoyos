using EduApoyos.Application.Common.Models;

namespace EduApoyos.Application.Solicitudes;

public interface ISolicitudApoyoService
{
    Task<ResultadoPaginado<SolicitudResponse>> ListarAsync(
        SolicitudFiltroRequest filtro,
        CancellationToken cancellationToken = default);

    Task<SolicitudResponse> ObtenerPorIdAsync(
        Guid solicitudId,
        Guid usuarioActualId,
        bool esAsesor,
        CancellationToken cancellationToken = default);

    Task<SolicitudResponse> CrearAsync(
        Guid usuarioActualId,
        bool esAsesor,
        CrearSolicitudRequest request,
        CancellationToken cancellationToken = default);

    Task<SolicitudResponse> ActualizarAsync(
        Guid solicitudId,
        Guid usuarioActualId,
        bool esAsesor,
        ActualizarSolicitudRequest request,
        CancellationToken cancellationToken = default);

    Task<SolicitudResponse> CambiarEstadoAsync(
        Guid solicitudId,
        Guid asesorId,
        CambiarEstadoSolicitudRequest request,
        CancellationToken cancellationToken = default);

    Task<ResultadoPaginado<SolicitudResponse>>
        ListarPorEstudianteAsync(
            Guid estudianteId,
            Guid usuarioActualId,
            bool esAsesor,
            int pagina,
            int tamanoPagina,
            CancellationToken cancellationToken = default);
}