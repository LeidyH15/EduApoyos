using EduApoyos.Domain.Enums;

namespace EduApoyos.Domain.Entities;

public class HistorialEstado
{
    private HistorialEstado()
    {
    }

    internal HistorialEstado(
        Guid solicitudId,
        EstadoSolicitud estadoAnterior,
        EstadoSolicitud estadoNuevo,
        Guid usuarioId,
        string? observacion)
    {
        if (solicitudId == Guid.Empty)
        {
            throw new ArgumentException(
                "El identificador de la solicitud es obligatorio.",
                nameof(solicitudId));
        }

        if (usuarioId == Guid.Empty)
        {
            throw new ArgumentException(
                "El usuario que realiza el cambio es obligatorio.",
                nameof(usuarioId));
        }

        Id = Guid.NewGuid();
        SolicitudId = solicitudId;
        EstadoAnterior = estadoAnterior;
        EstadoNuevo = estadoNuevo;
        FechaCambio = DateTime.UtcNow;
        UsuarioId = usuarioId;
        Observacion = string.IsNullOrWhiteSpace(observacion)
            ? null
            : observacion.Trim();
    }

    public Guid Id { get; private set; }

    public Guid SolicitudId { get; private set; }

    public EstadoSolicitud EstadoAnterior { get; private set; }

    public EstadoSolicitud EstadoNuevo { get; private set; }

    public DateTime FechaCambio { get; private set; }

    public Guid UsuarioId { get; private set; }

    public string? Observacion { get; private set; }

    public SolicitudApoyo Solicitud { get; private set; } = null!;
}