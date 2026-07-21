using System.ComponentModel.DataAnnotations;
using EduApoyos.Domain.Enums;

namespace EduApoyos.Application.Solicitudes;

public class CambiarEstadoSolicitudRequest
{
    [Required(ErrorMessage = "El nuevo estado es obligatorio.")]
    [EnumDataType(
        typeof(EstadoSolicitud),
        ErrorMessage = "El estado indicado no es válido.")]
    public EstadoSolicitud NuevoEstado { get; set; }

    [StringLength(
        500,
        ErrorMessage = "La observación no puede superar 500 caracteres.")]
    public string? Observacion { get; set; }
}