using System.ComponentModel.DataAnnotations;
using EduApoyos.Domain.Enums;

namespace EduApoyos.Application.Solicitudes;

public class ActualizarSolicitudRequest
{
    [Required(ErrorMessage = "El tipo de apoyo es obligatorio.")]
    [EnumDataType(
        typeof(TipoApoyo),
        ErrorMessage = "El tipo de apoyo no es válido.")]
    public TipoApoyo TipoApoyo { get; set; }

    [Range(
    0.01,
    999999999999.99,
    ErrorMessage = "El monto solicitado debe ser mayor que cero.")]
    public decimal MontoSolicitado { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(
        1000,
        MinimumLength = 10,
        ErrorMessage = "La descripción debe tener entre 10 y 1000 caracteres.")]
    public string Descripcion { get; set; } = string.Empty;
}