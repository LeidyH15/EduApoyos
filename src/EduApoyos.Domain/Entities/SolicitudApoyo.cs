using EduApoyos.Domain.Enums;

namespace EduApoyos.Domain.Entities;

public class SolicitudApoyo
{
    private SolicitudApoyo()
    {
    }

    public SolicitudApoyo(
        Guid estudianteId,
        TipoApoyo tipoApoyo,
        decimal montoSolicitado,
        string descripcion)
    {
        if (estudianteId == Guid.Empty)
        {
            throw new ArgumentException(
                "El identificador del estudiante es obligatorio.",
                nameof(estudianteId));
        }

        ValidarDatos(montoSolicitado, descripcion);

        Id = Guid.NewGuid();
        EstudianteId = estudianteId;
        TipoApoyo = tipoApoyo;
        MontoSolicitado = montoSolicitado;
        Descripcion = descripcion.Trim();
        Estado = EstadoSolicitud.Pendiente;
        FechaSolicitud = DateTime.UtcNow;
        FechaActualizacion = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid EstudianteId { get; private set; }

    public TipoApoyo TipoApoyo { get; private set; }

    public decimal MontoSolicitado { get; private set; }

    public string Descripcion { get; private set; } = string.Empty;

    public EstadoSolicitud Estado { get; private set; }

    public DateTime FechaSolicitud { get; private set; }

    public DateTime FechaActualizacion { get; private set; }

    public Guid? AsesorId { get; private set; }

    public Estudiante Estudiante { get; private set; } = null!;

    public ICollection<HistorialEstado> HistorialEstados { get; private set; }
        = new List<HistorialEstado>();

    public void ActualizarInformacion(
        TipoApoyo tipoApoyo,
        decimal montoSolicitado,
        string descripcion)
    {
        if (Estado != EstadoSolicitud.Pendiente)
        {
            throw new InvalidOperationException(
                "Solo se puede actualizar una solicitud pendiente.");
        }

        ValidarDatos(montoSolicitado, descripcion);

        TipoApoyo = tipoApoyo;
        MontoSolicitado = montoSolicitado;
        Descripcion = descripcion.Trim();
        FechaActualizacion = DateTime.UtcNow;
    }

    internal void AplicarCambioEstado(
        EstadoSolicitud nuevoEstado,
        Guid usuarioId,
        string? observacion)
    {
        if (usuarioId == Guid.Empty)
        {
            throw new ArgumentException(
                "El usuario que realiza el cambio es obligatorio.",
                nameof(usuarioId));
        }

        if (Estado == nuevoEstado)
        {
            throw new InvalidOperationException(
                "La solicitud ya se encuentra en el estado indicado.");
        }

        var estadoAnterior = Estado;

        Estado = nuevoEstado;
        AsesorId = usuarioId;
        FechaActualizacion = DateTime.UtcNow;

        HistorialEstados.Add(
            new HistorialEstado(
                Id,
                estadoAnterior,
                nuevoEstado,
                usuarioId,
                observacion));
    }

    private static void ValidarDatos(
        decimal montoSolicitado,
        string descripcion)
    {
        if (montoSolicitado <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(montoSolicitado),
                "El monto solicitado debe ser mayor que cero.");
        }

        if (string.IsNullOrWhiteSpace(descripcion))
        {
            throw new ArgumentException(
                "La descripción es obligatoria.",
                nameof(descripcion));
        }
    }
}