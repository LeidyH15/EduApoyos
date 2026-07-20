using EduApoyos.Application.Abstractions.Persistence;
using EduApoyos.Application.Common.Models;
using EduApoyos.Domain.Entities;
using EduApoyos.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EduApoyos.Infrastructure.Persistence.Repositories;

public class SolicitudApoyoRepository : ISolicitudApoyoRepository
{
    private readonly ApplicationDbContext _context;

    public SolicitudApoyoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SolicitudApoyo?> ObtenerPorIdAsync(
        Guid id,
        bool incluirHistorial = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<SolicitudApoyo> consulta =
            _context.SolicitudesApoyo;

        consulta = consulta.Include(
            solicitud => solicitud.Estudiante);

        if (incluirHistorial)
        {
            consulta = consulta.Include(
                solicitud => solicitud.HistorialEstados);
        }

        return await consulta.FirstOrDefaultAsync(
            solicitud => solicitud.Id == id,
            cancellationToken);
    }

    public async Task<ResultadoPaginado<SolicitudApoyo>> ListarAsync(
        EstadoSolicitud? estado,
        TipoApoyo? tipoApoyo,
        DateTime? fechaDesde,
        DateTime? fechaHasta,
        int pagina,
        int tamanoPagina,
        CancellationToken cancellationToken = default)
    {
        var consulta = _context.SolicitudesApoyo
            .AsNoTracking()
            .Include(solicitud => solicitud.Estudiante)
            .AsQueryable();

        if (estado.HasValue)
        {
            consulta = consulta.Where(
                solicitud => solicitud.Estado == estado.Value);
        }

        if (tipoApoyo.HasValue)
        {
            consulta = consulta.Where(
                solicitud => solicitud.TipoApoyo == tipoApoyo.Value);
        }

        if (fechaDesde.HasValue)
        {
            consulta = consulta.Where(
                solicitud =>
                    solicitud.FechaSolicitud >= fechaDesde.Value);
        }

        if (fechaHasta.HasValue)
        {
            consulta = consulta.Where(
                solicitud =>
                    solicitud.FechaSolicitud <= fechaHasta.Value);
        }

        var totalElementos = await consulta.CountAsync(
            cancellationToken);

        var solicitudes = await consulta
            .OrderByDescending(
                solicitud => solicitud.FechaSolicitud)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync(cancellationToken);

        return new ResultadoPaginado<SolicitudApoyo>(
            solicitudes,
            pagina,
            tamanoPagina,
            totalElementos);
    }

    public async Task<ResultadoPaginado<SolicitudApoyo>>
        ListarPorEstudianteAsync(
            Guid estudianteId,
            int pagina,
            int tamanoPagina,
            CancellationToken cancellationToken = default)
    {
        var consulta = _context.SolicitudesApoyo
            .AsNoTracking()
            .Where(
                solicitud =>
                    solicitud.EstudianteId == estudianteId);

        var totalElementos = await consulta.CountAsync(
            cancellationToken);

        var solicitudes = await consulta
            .OrderByDescending(
                solicitud => solicitud.FechaSolicitud)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync(cancellationToken);

        return new ResultadoPaginado<SolicitudApoyo>(
            solicitudes,
            pagina,
            tamanoPagina,
            totalElementos);
    }

    public Task AgregarAsync(
        SolicitudApoyo solicitud,
        CancellationToken cancellationToken = default)
    {
        return _context.SolicitudesApoyo.AddAsync(
            solicitud,
            cancellationToken).AsTask();
    }

    public void Actualizar(SolicitudApoyo solicitud)
    {
        _context.SolicitudesApoyo.Update(solicitud);
    }
}