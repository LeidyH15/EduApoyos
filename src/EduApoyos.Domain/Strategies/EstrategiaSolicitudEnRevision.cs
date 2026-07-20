using EduApoyos.Domain.Entities;
using EduApoyos.Domain.Enums;
using EduApoyos.Domain.Exceptions;

namespace EduApoyos.Domain.Strategies;

public class EstrategiaSolicitudEnRevision : IEstrategiaEstadoSolicitud
{
    public EstadoSolicitud EstadoOrigen => EstadoSolicitud.EnRevision;

    public void CambiarEstado(
        SolicitudApoyo solicitud,
        EstadoSolicitud nuevoEstado,
        Guid usuarioId,
        string? observacion)
    {
        ArgumentNullException.ThrowIfNull(solicitud);

        if (solicitud.Estado != EstadoOrigen)
        {
            throw new ReglaNegocioException(
                "Esta estrategia solo puede procesar solicitudes en revisión.");
        }

        if (nuevoEstado is not EstadoSolicitud.Aprobada
            and not EstadoSolicitud.Rechazada)
        {
            throw new ReglaNegocioException(
                "Una solicitud en revisión solo puede aprobarse o rechazarse.");
        }

        solicitud.AplicarCambioEstado(
            nuevoEstado,
            usuarioId,
            observacion);
    }
}