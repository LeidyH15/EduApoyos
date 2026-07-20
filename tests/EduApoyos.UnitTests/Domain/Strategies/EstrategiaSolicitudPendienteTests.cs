using EduApoyos.Domain.Entities;
using EduApoyos.Domain.Enums;
using EduApoyos.Domain.Exceptions;
using EduApoyos.Domain.Strategies;

namespace EduApoyos.UnitTests.Domain.Strategies;

public class EstrategiaSolicitudPendienteTests
{
    [Fact]
    public void CambiarEstado_DePendienteAEnRevision_DebeActualizarSolicitud()
    {
        // Arrange
        var solicitud = CrearSolicitud();
        var estrategia = new EstrategiaSolicitudPendiente();
        var asesorId = Guid.NewGuid();

        // Act
        estrategia.CambiarEstado(
            solicitud,
            EstadoSolicitud.EnRevision,
            asesorId,
            "Documentación recibida.");

        // Assert
        Assert.Equal(EstadoSolicitud.EnRevision, solicitud.Estado);
        Assert.Equal(asesorId, solicitud.AsesorId);

        var historial = Assert.Single(solicitud.HistorialEstados);

        Assert.Equal(EstadoSolicitud.Pendiente, historial.EstadoAnterior);
        Assert.Equal(EstadoSolicitud.EnRevision, historial.EstadoNuevo);
        Assert.Equal(asesorId, historial.UsuarioId);
        Assert.Equal("Documentación recibida.", historial.Observacion);
    }

    [Fact]
    public void CambiarEstado_DePendienteAAprobada_DebeLanzarExcepcion()
    {
        // Arrange
        var solicitud = CrearSolicitud();
        var estrategia = new EstrategiaSolicitudPendiente();

        // Act
        var excepcion = Assert.Throws<ReglaNegocioException>(() =>
            estrategia.CambiarEstado(
                solicitud,
                EstadoSolicitud.Aprobada,
                Guid.NewGuid(),
                null));

        // Assert
        Assert.Equal(
            "Una solicitud pendiente solo puede pasar a en revisión.",
            excepcion.Message);

        Assert.Equal(EstadoSolicitud.Pendiente, solicitud.Estado);
        Assert.Empty(solicitud.HistorialEstados);
    }

    private static SolicitudApoyo CrearSolicitud()
    {
        return new SolicitudApoyo(
            Guid.NewGuid(),
            TipoApoyo.Beca,
            1_500_000m,
            "Solicitud de apoyo para el pago de matrícula.");
    }
}