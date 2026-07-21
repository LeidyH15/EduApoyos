using EduApoyos.Domain.Entities;
using EduApoyos.Domain.Enums;

namespace EduApoyos.UnitTests.Domain.Entities;

public class SolicitudApoyoTests
{
    private static readonly Guid EstudianteId =
        Guid.NewGuid();

    [Fact]
    public void ConstructorValido_CreaSolicitudPendiente()
    {
        var solicitud = new SolicitudApoyo(
            EstudianteId,
            TipoApoyo.Beca,
            2500000m,
            "Solicitud para cubrir los costos de matrícula.");

        Assert.NotEqual(
            Guid.Empty,
            solicitud.Id);

        Assert.Equal(
            EstudianteId,
            solicitud.EstudianteId);

        Assert.Equal(
            TipoApoyo.Beca,
            solicitud.TipoApoyo);

        Assert.Equal(
            2500000m,
            solicitud.MontoSolicitado);

        Assert.Equal(
            EstadoSolicitud.Pendiente,
            solicitud.Estado);

        Assert.Null(
            solicitud.AsesorId);

        Assert.Empty(
            solicitud.HistorialEstados);

        Assert.Equal(
            DateTimeKind.Utc,
            solicitud.FechaSolicitud.Kind);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-500000)]
    public void MontoNoValido_LanzaExcepcion(int monto)
    {
        var accion = () => new SolicitudApoyo(
            EstudianteId,
            TipoApoyo.Credito,
            monto,
            "Solicitud de crédito educativo.");

        Assert.Throws<ArgumentOutOfRangeException>(
            accion);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void DescripcionVacia_LanzaExcepcion(string descripcion)
    {
        var accion = () => new SolicitudApoyo(
            EstudianteId,
            TipoApoyo.Subsidio,
            500000m,
            descripcion);

        Assert.Throws<ArgumentException>(
            accion);
    }

    [Fact]
    public void EstudianteVacio_LanzaExcepcion()
    {
        var accion = () => new SolicitudApoyo(
            Guid.Empty,
            TipoApoyo.Beca,
            1000000m,
            "Solicitud con estudiante inválido.");

        Assert.Throws<ArgumentException>(
            accion);
    }

    [Fact]
    public void ActualizarPendiente_ActualizaDatos()
    {
        var solicitud = new SolicitudApoyo(
            EstudianteId,
            TipoApoyo.Beca,
            1000000m,
            "Descripción inicial de la solicitud.");

        var fechaAnterior =
            solicitud.FechaActualizacion;

        solicitud.ActualizarInformacion(
            TipoApoyo.Subsidio,
            1500000m,
            "Descripción actualizada de la solicitud.");

        Assert.Equal(
            TipoApoyo.Subsidio,
            solicitud.TipoApoyo);

        Assert.Equal(
            1500000m,
            solicitud.MontoSolicitado);

        Assert.Equal(
            "Descripción actualizada de la solicitud.",
            solicitud.Descripcion);

        Assert.True(
            solicitud.FechaActualizacion >=
            fechaAnterior);
    }

    [Fact]
    public void ActualizarConMontoCero_LanzaExcepcion()
    {
        var solicitud = new SolicitudApoyo(
            EstudianteId,
            TipoApoyo.Beca,
            1000000m,
            "Descripción inicial válida.");

        var accion = () =>
            solicitud.ActualizarInformacion(
                TipoApoyo.Credito,
                0,
                "Descripción actualizada válida.");

        Assert.Throws<ArgumentOutOfRangeException>(
            accion);
    }

    [Fact]
    public void ActualizarConDescripcionVacia_LanzaExcepcion()
    {
        var solicitud = new SolicitudApoyo(
            EstudianteId,
            TipoApoyo.Beca,
            1000000m,
            "Descripción inicial válida.");

        var accion = () =>
            solicitud.ActualizarInformacion(
                TipoApoyo.Credito,
                1200000m,
                " ");

        Assert.Throws<ArgumentException>(
            accion);
    }
}