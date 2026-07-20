using EduApoyos.Application.Common.Models;
using EduApoyos.Domain.Entities;
using EduApoyos.Domain.Enums;

namespace EduApoyos.Application.Abstractions.Persistence;

public interface ISolicitudApoyoRepository
{
    Task<SolicitudApoyo?> ObtenerPorIdAsync(
        Guid id,
        bool incluirHistorial = false,
        CancellationToken cancellationToken = default);

    Task<ResultadoPaginado<SolicitudApoyo>> ListarAsync(
        EstadoSolicitud? estado,
        TipoApoyo? tipoApoyo,
        DateTime? fechaDesde,
        DateTime? fechaHasta,
        int pagina,
        int tamanoPagina,
        CancellationToken cancellationToken = default);

    Task<ResultadoPaginado<SolicitudApoyo>> ListarPorEstudianteAsync(
        Guid estudianteId,
        int pagina,
        int tamanoPagina,
        CancellationToken cancellationToken = default);

    Task AgregarAsync(
        SolicitudApoyo solicitud,
        CancellationToken cancellationToken = default);

    void Actualizar(SolicitudApoyo solicitud);
}