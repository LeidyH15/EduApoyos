namespace EduApoyos.Application.Solicitudes.Constancias;

public sealed record ConstanciaSolicitudArchivo(
    byte[] Contenido,
    string TipoContenido,
    string NombreArchivo);