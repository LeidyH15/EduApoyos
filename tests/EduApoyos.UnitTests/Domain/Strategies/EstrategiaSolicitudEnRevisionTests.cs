using EduApoyos.Domain.Entities;
using EduApoyos.Domain.Enums;
using EduApoyos.Domain.Exceptions;
using EduApoyos.Domain.Strategies;

namespace EduApoyos.UnitTests.Domain.Strategies;

public class EstrategiaSolicitudEnRevisionTests
{
    [Theory]
    [InlineData(EstadoSolicitud.Aprobada)]
    [InlineData(EstadoSolicitud.Rechazada)]
    public void CambiarEstado_DesdeEnRevisionAEstadoFinal_DebeActualizarSolicitud(
        EstadoSolicitud estadoFinal)
    {
        // Arrange
        var solicitud = CrearSolicitudEnRevision();
        var estrategia = new EstrategiaSolicitudEnRevision();
        var asesorId = Guid.NewGuid();

        // Act
        estrategia.CambiarEstado(
            solicitud,
            estadoFinal,
            asesorId,
            "Evaluación finalizada.");

        // Assert
        Assert.Equal(estadoFinal, solicitud.Estado);
        Assert.Equal(asesorId, solicitud.AsesorId);
        Assert.Equal(2, solicitud.HistorialEstados.Count);

        var ultimoCambio = solicitud.HistorialEstados.Last();

        Assert.Equal(
            EstadoSolicitud.EnRevision,
            ultimoCambio.EstadoAnterior);

        Assert.Equal(estadoFinal, ultimoCambio.EstadoNuevo);
    }

    [Fact]
    public void CambiarEstado_DeEnRevisionAPendiente_DebeLanzarExcepcion()
    {
        // Arrange
        var solicitud = CrearSolicitudEnRevision();
        var estrategia = new EstrategiaSolicitudEnRevision();

        // Act
        var excepcion = Assert.Throws<ReglaNegocioException>(() =>
            estrategia.CambiarEstado(
                solicitud,
                EstadoSolicitud.Pendiente,
                Guid.NewGuid(),
                null));

        // Assert
        Assert.Equal(
            "Una solicitud en revisión solo puede aprobarse o rechazarse.",
            excepcion.Message);

        Assert.Equal(EstadoSolicitud.EnRevision, solicitud.Estado);
        Assert.Single(solicitud.HistorialEstados);
    }

    private static SolicitudApoyo CrearSolicitudEnRevision()
    {
        var solicitud = new SolicitudApoyo(
            Guid.NewGuid(),
            TipoApoyo.Subsidio,
            800_000m,
            "Solicitud de subsidio de sostenimiento.");

        var estrategiaPendiente = new EstrategiaSolicitudPendiente();

        estrategiaPendiente.CambiarEstado(
            solicitud,
            EstadoSolicitud.EnRevision,
            Guid.NewGuid(),
            "Solicitud recibida.");

        return solicitud;
    }
}