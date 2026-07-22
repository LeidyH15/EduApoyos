namespace EduApoyos.Application.Solicitudes.Constancias;

public interface IConstanciaSolicitudFactory
{
    ConstanciaSolicitudArchivo Crear(
        SolicitudResponse solicitud);
}