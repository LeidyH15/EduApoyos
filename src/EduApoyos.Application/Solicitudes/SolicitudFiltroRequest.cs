using System.ComponentModel.DataAnnotations;
using EduApoyos.Domain.Enums;

namespace EduApoyos.Application.Solicitudes;

public class SolicitudFiltroRequest
{
    [EnumDataType(
        typeof(EstadoSolicitud),
        ErrorMessage = "El estado indicado no es válido.")]
    public EstadoSolicitud? Estado { get; set; }

    [EnumDataType(
        typeof(TipoApoyo),
        ErrorMessage = "El tipo de apoyo no es válido.")]
    public TipoApoyo? TipoApoyo { get; set; }

    public DateTime? FechaDesde { get; set; }

    public DateTime? FechaHasta { get; set; }

    [Range(
        1,
        int.MaxValue,
        ErrorMessage = "La página debe ser mayor o igual a 1.")]
    public int Pagina { get; set; } = 1;

    [Range(
        1,
        100,
        ErrorMessage = "El tamaño de página debe estar entre 1 y 100.")]
    public int TamanoPagina { get; set; } = 10;
}