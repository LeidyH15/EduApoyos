using EduApoyos.Api.Controllers;
using EduApoyos.Application.Common.Models;
using EduApoyos.Application.Estudiantes;
using EduApoyos.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace EduApoyos.UnitTests.Api.Controllers;

public class EstudiantesControllerTests
{
    [Fact]
    public async Task Listar_DevuelveResultadoPaginado()
    {
        var estudianteId =
            Guid.NewGuid();

        var estudiantes =
            new List<EstudianteResponse>
            {
                new()
                {
                    Id = estudianteId,
                    UsuarioId = Guid.NewGuid(),
                    NombreCompleto =
                        "Estudiante de Prueba",
                    Email =
                        "estudiante@tests.local",
                    NumeroDocumento =
                        "DOC-123456",
                    TipoDocumento =
                        TipoDocumento.CedulaCiudadania,
                    ProgramaAcademico =
                        "Ingeniería de Sistemas",
                    Semestre = 5,
                    FechaRegistro =
                        DateTime.UtcNow
                }
            };

        var resultadoEsperado =
            new ResultadoPaginado<EstudianteResponse>(
                estudiantes,
                pagina: 1,
                tamanoPagina: 10,
                totalElementos: 1);

        var servicioMock =
            new Mock<IEstudianteService>();

        servicioMock
            .Setup(servicio =>
                servicio.ListarAsync(
                    1,
                    10,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(resultadoEsperado);

        var controller =
            new EstudiantesController(
                servicioMock.Object);

        var respuesta =
            await controller.Listar(
                pagina: 1,
                tamanoPagina: 10,
                cancellationToken:
                    CancellationToken.None);

        var okResult =
            Assert.IsType<OkObjectResult>(
                respuesta.Result);

        var resultadoObtenido =
            Assert.IsType<
                ResultadoPaginado<EstudianteResponse>>(
                    okResult.Value);

        Assert.Same(
            resultadoEsperado,
            resultadoObtenido);

        Assert.Single(
            resultadoObtenido.Elementos);

        Assert.Equal(
            estudianteId,
            resultadoObtenido
                .Elementos
                .Single()
                .Id);

        Assert.Equal(
            1,
            resultadoObtenido.Pagina);

        Assert.Equal(
            10,
            resultadoObtenido.TamanoPagina);

        Assert.Equal(
            1,
            resultadoObtenido.TotalElementos);

        Assert.Equal(
            1,
            resultadoObtenido.TotalPaginas);

        servicioMock.Verify(
            servicio =>
                servicio.ListarAsync(
                    1,
                    10,
                    It.IsAny<CancellationToken>()),
            Times.Once);
    }
}