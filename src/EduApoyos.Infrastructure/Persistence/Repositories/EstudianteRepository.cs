using EduApoyos.Application.Abstractions.Persistence;
using EduApoyos.Application.Common.Models;
using EduApoyos.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduApoyos.Infrastructure.Persistence.Repositories;

public class EstudianteRepository : IEstudianteRepository
{
    private readonly ApplicationDbContext _context;

    public EstudianteRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Estudiante?> ObtenerPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _context.Estudiantes
            .FirstOrDefaultAsync(
                estudiante => estudiante.Id == id,
                cancellationToken);
    }

    public Task<Estudiante?> ObtenerPorUsuarioIdAsync(
        Guid usuarioId,
        CancellationToken cancellationToken = default)
    {
        return _context.Estudiantes
            .FirstOrDefaultAsync(
                estudiante => estudiante.UsuarioId == usuarioId,
                cancellationToken);
    }

    public async Task<ResultadoPaginado<Estudiante>> ListarAsync(
        int pagina,
        int tamanoPagina,
        CancellationToken cancellationToken = default)
    {
        var consulta = _context.Estudiantes
            .AsNoTracking()
            .OrderBy(estudiante => estudiante.NumeroDocumento);

        var totalElementos = await consulta.CountAsync(
            cancellationToken);

        var estudiantes = await consulta
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync(cancellationToken);

        return new ResultadoPaginado<Estudiante>(
            estudiantes,
            pagina,
            tamanoPagina,
            totalElementos);
    }

    public Task<bool> ExisteDocumentoAsync(
        string numeroDocumento,
        Guid? estudianteExcluidoId = null,
        CancellationToken cancellationToken = default)
    {
        return _context.Estudiantes.AnyAsync(
            estudiante =>
                estudiante.NumeroDocumento == numeroDocumento &&
                (!estudianteExcluidoId.HasValue ||
                 estudiante.Id != estudianteExcluidoId.Value),
            cancellationToken);
    }

    public Task AgregarAsync(
        Estudiante estudiante,
        CancellationToken cancellationToken = default)
    {
        return _context.Estudiantes.AddAsync(
            estudiante,
            cancellationToken).AsTask();
    }

    public void Actualizar(Estudiante estudiante)
    {
        _context.Estudiantes.Update(estudiante);
    }

    public void Eliminar(Estudiante estudiante)
    {
        _context.Estudiantes.Remove(estudiante);
    }
}