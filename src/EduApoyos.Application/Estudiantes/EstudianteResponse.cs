using EduApoyos.Domain.Enums;

namespace EduApoyos.Application.Estudiantes;

public class EstudianteResponse
{
    public Guid Id { get; set; }

    public Guid UsuarioId { get; set; }

    public string NombreCompleto { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string NumeroDocumento { get; set; } = string.Empty;

    public TipoDocumento TipoDocumento { get; set; }

    public string ProgramaAcademico { get; set; } = string.Empty;

    public int Semestre { get; set; }

    public DateTime FechaRegistro { get; set; }
}