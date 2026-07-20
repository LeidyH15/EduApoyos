using EduApoyos.Domain.Enums;

namespace EduApoyos.Domain.Entities;

public class Estudiante
{
    private Estudiante()
    {
    }

    public Estudiante(
        Guid usuarioId,
        string numeroDocumento,
        TipoDocumento tipoDocumento,
        string programaAcademico,
        int semestre)
    {
        if (usuarioId == Guid.Empty)
        {
            throw new ArgumentException(
                "El identificador del usuario es obligatorio.",
                nameof(usuarioId));
        }

        ValidarDatos(numeroDocumento, programaAcademico, semestre);

        Id = Guid.NewGuid();
        UsuarioId = usuarioId;
        NumeroDocumento = numeroDocumento.Trim();
        TipoDocumento = tipoDocumento;
        ProgramaAcademico = programaAcademico.Trim();
        Semestre = semestre;
    }

    public Guid Id { get; private set; }

    public Guid UsuarioId { get; private set; }

    public string NumeroDocumento { get; private set; } = string.Empty;

    public TipoDocumento TipoDocumento { get; private set; }

    public string ProgramaAcademico { get; private set; } = string.Empty;

    public int Semestre { get; private set; }

    public ICollection<SolicitudApoyo> Solicitudes { get; private set; }
        = new List<SolicitudApoyo>();

    public void ActualizarDatosAcademicos(
        string numeroDocumento,
        TipoDocumento tipoDocumento,
        string programaAcademico,
        int semestre)
    {
        ValidarDatos(numeroDocumento, programaAcademico, semestre);

        NumeroDocumento = numeroDocumento.Trim();
        TipoDocumento = tipoDocumento;
        ProgramaAcademico = programaAcademico.Trim();
        Semestre = semestre;
    }

    private static void ValidarDatos(
        string numeroDocumento,
        string programaAcademico,
        int semestre)
    {
        if (string.IsNullOrWhiteSpace(numeroDocumento))
        {
            throw new ArgumentException(
                "El número de documento es obligatorio.",
                nameof(numeroDocumento));
        }

        if (string.IsNullOrWhiteSpace(programaAcademico))
        {
            throw new ArgumentException(
                "El programa académico es obligatorio.",
                nameof(programaAcademico));
        }

        if (semestre is < 1 or > 12)
        {
            throw new ArgumentOutOfRangeException(
                nameof(semestre),
                "El semestre debe estar entre 1 y 12.");
        }
    }
}