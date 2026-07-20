using EduApoyos.Domain.Entities;
using EduApoyos.Domain.Enums;

namespace EduApoyos.Domain.Strategies;

public interface IEstrategiaEstadoSolicitud
{
    EstadoSolicitud EstadoOrigen { get; }

    void CambiarEstado(
        SolicitudApoyo solicitud,
        EstadoSolicitud nuevoEstado,
        Guid usuarioId,
        string? observacion);
}