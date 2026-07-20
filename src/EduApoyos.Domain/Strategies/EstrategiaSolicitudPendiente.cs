using EduApoyos.Domain.Entities;
using EduApoyos.Domain.Enums;
using EduApoyos.Domain.Exceptions;

namespace EduApoyos.Domain.Strategies;

public class EstrategiaSolicitudPendiente : IEstrategiaEstadoSolicitud
{
    public EstadoSolicitud EstadoOrigen => EstadoSolicitud.Pendiente;

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
                "Esta estrategia solo puede procesar solicitudes pendientes.");
        }

        if (nuevoEstado != EstadoSolicitud.EnRevision)
        {
            throw new ReglaNegocioException(
                "Una solicitud pendiente solo puede pasar a en revisión.");
        }

        solicitud.AplicarCambioEstado(
            nuevoEstado,
            usuarioId,
            observacion);
    }
}