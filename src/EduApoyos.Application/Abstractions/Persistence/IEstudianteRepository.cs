using EduApoyos.Application.Common.Models;
using EduApoyos.Domain.Entities;

namespace EduApoyos.Application.Abstractions.Persistence;

public interface IEstudianteRepository
{
    Task<Estudiante?> ObtenerPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Estudiante?> ObtenerPorUsuarioIdAsync(
        Guid usuarioId,
        CancellationToken cancellationToken = default);

    Task<ResultadoPaginado<Estudiante>> ListarAsync(
        int pagina,
        int tamanoPagina,
        CancellationToken cancellationToken = default);

    Task<bool> ExisteDocumentoAsync(
        string numeroDocumento,
        Guid? estudianteExcluidoId = null,
        CancellationToken cancellationToken = default);

    Task AgregarAsync(
        Estudiante estudiante,
        CancellationToken cancellationToken = default);

    void Actualizar(Estudiante estudiante);

    void Eliminar(Estudiante estudiante);
}