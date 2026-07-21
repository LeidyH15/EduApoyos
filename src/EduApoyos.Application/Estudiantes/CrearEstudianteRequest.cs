using System.ComponentModel.DataAnnotations;
using EduApoyos.Domain.Enums;

namespace EduApoyos.Application.Estudiantes;

public class CrearEstudianteRequest
{
    [Required(ErrorMessage = "El nombre completo es obligatorio.")]
    [StringLength(
        150,
        MinimumLength = 3,
        ErrorMessage = "El nombre debe tener entre 3 y 150 caracteres.")]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(
        100,
        MinimumLength = 8,
        ErrorMessage = "La contraseña debe tener mínimo 8 caracteres.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "El número de documento es obligatorio.")]
    [StringLength(
        30,
        MinimumLength = 5,
        ErrorMessage = "El número de documento debe tener entre 5 y 30 caracteres.")]
    public string NumeroDocumento { get; set; } = string.Empty;

    [Required(ErrorMessage = "El tipo de documento es obligatorio.")]
    [EnumDataType(
        typeof(TipoDocumento),
        ErrorMessage = "El tipo de documento no es válido.")]
    public TipoDocumento TipoDocumento { get; set; }

    [Required(ErrorMessage = "El programa académico es obligatorio.")]
    [StringLength(
        150,
        MinimumLength = 2,
        ErrorMessage = "El programa académico debe tener entre 2 y 150 caracteres.")]
    public string ProgramaAcademico { get; set; } = string.Empty;

    [Range(
        1,
        12,
        ErrorMessage = "El semestre debe estar entre 1 y 12.")]
    public int Semestre { get; set; }
}