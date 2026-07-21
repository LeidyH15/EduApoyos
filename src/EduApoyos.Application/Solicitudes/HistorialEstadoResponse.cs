using EduApoyos.Domain.Enums;

namespace EduApoyos.Application.Solicitudes;

public class HistorialEstadoResponse
{
    public Guid Id { get; set; }

    public EstadoSolicitud EstadoAnterior { get; set; }

    public EstadoSolicitud EstadoNuevo { get; set; }

    public DateTime FechaCambio { get; set; }

    public Guid UsuarioId { get; set; }

    public string? Observacion { get; set; }
}