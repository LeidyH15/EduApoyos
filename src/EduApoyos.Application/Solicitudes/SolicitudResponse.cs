using EduApoyos.Domain.Enums;

namespace EduApoyos.Application.Solicitudes;

public class SolicitudResponse
{
    public Guid Id { get; set; }

    public Guid EstudianteId { get; set; }

    public Guid UsuarioEstudianteId { get; set; }

    public string NombreEstudiante { get; set; } = string.Empty;

    public string NumeroDocumento { get; set; } = string.Empty;

    public TipoApoyo TipoApoyo { get; set; }

    public decimal MontoSolicitado { get; set; }

    public string Descripcion { get; set; } = string.Empty;

    public EstadoSolicitud Estado { get; set; }

    public DateTime FechaSolicitud { get; set; }

    public DateTime FechaActualizacion { get; set; }

    public Guid? AsesorId { get; set; }

    public IReadOnlyCollection<HistorialEstadoResponse> Historial { get; set; }
        = Array.Empty<HistorialEstadoResponse>();
}