using System.Globalization;
using System.Text;
using EduApoyos.Application.Solicitudes;
using EduApoyos.Application.Solicitudes.Constancias;

namespace EduApoyos.Infrastructure.Solicitudes.Constancias;

public class ConstanciaSolicitudTextoFactory
    : IConstanciaSolicitudFactory
{
    public ConstanciaSolicitudArchivo Crear(
        SolicitudResponse solicitud)
    {
        ArgumentNullException.ThrowIfNull(solicitud);

        var cultura =
            CultureInfo.GetCultureInfo("es-CO");

        var contenido = new StringBuilder();

        contenido.AppendLine(
            "INSTITUCIÓN DE EDUCACIÓN SUPERIOR");

        contenido.AppendLine(
            "CONSTANCIA DE SOLICITUD DE APOYO ECONÓMICO");

        contenido.AppendLine(
            new string('=', 52));

        contenido.AppendLine();

        contenido.AppendLine(
            $"Número de solicitud: {solicitud.Id}");

        contenido.AppendLine(
            $"Estudiante: {solicitud.NombreEstudiante}");

        contenido.AppendLine(
            $"Documento: {solicitud.NumeroDocumento}");

        contenido.AppendLine(
            $"Tipo de apoyo: {solicitud.TipoApoyo}");

        contenido.AppendLine(
            $"Monto solicitado: " +
            $"{solicitud.MontoSolicitado.ToString("C0", cultura)}");

        contenido.AppendLine(
            $"Estado actual: {solicitud.Estado}");

        contenido.AppendLine(
            $"Fecha de solicitud: " +
            $"{solicitud.FechaSolicitud:dd/MM/yyyy HH:mm} UTC");

        contenido.AppendLine(
            $"Última actualización: " +
            $"{solicitud.FechaActualizacion:dd/MM/yyyy HH:mm} UTC");

        contenido.AppendLine();

        contenido.AppendLine("Descripción:");

        contenido.AppendLine(
            solicitud.Descripcion);

        contenido.AppendLine();

        contenido.AppendLine(
            new string('-', 52));

        contenido.AppendLine(
            "Esta constancia acredita el registro de la solicitud");

        contenido.AppendLine(
            "en el sistema EduApoyos. No constituye una aprobación");

        contenido.AppendLine(
            "ni un compromiso de desembolso por parte de la institución.");

        contenido.AppendLine();

        contenido.AppendLine(
            $"Constancia generada: " +
            $"{DateTime.UtcNow:dd/MM/yyyy HH:mm} UTC");

        var bytes =
            Encoding.UTF8.GetBytes(
                contenido.ToString());

        return new ConstanciaSolicitudArchivo(
            bytes,
            "text/plain; charset=utf-8",
            $"constancia-solicitud-{solicitud.Id}.txt");
    }
}