using EduApoyos.Application.Abstractions.Persistence;
using EduApoyos.Application.Common.Exceptions;
using EduApoyos.Application.Common.Models;
using EduApoyos.Application.Estudiantes;
using EduApoyos.Domain.Entities;
using EduApoyos.Domain.Enums;
using EduApoyos.Domain.Exceptions;
using EduApoyos.Infrastructure.Identity;
using EduApoyos.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EduApoyos.Infrastructure.Estudiantes;

public class EstudianteService : IEstudianteService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEstudianteRepository _estudianteRepository;
    private readonly IUnidadTrabajo _unidadTrabajo;
    private readonly ApplicationDbContext _context;

    public EstudianteService(
        UserManager<ApplicationUser> userManager,
        IEstudianteRepository estudianteRepository,
        IUnidadTrabajo unidadTrabajo,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _estudianteRepository = estudianteRepository;
        _unidadTrabajo = unidadTrabajo;
        _context = context;
    }

    public async Task<ResultadoPaginado<EstudianteResponse>> ListarAsync(
        int pagina,
        int tamanoPagina,
        CancellationToken cancellationToken = default)
    {
        ValidarPaginacion(pagina, tamanoPagina);

        var resultado =
            await _estudianteRepository.ListarAsync(
                pagina,
                tamanoPagina,
                cancellationToken);

        var usuariosIds = resultado.Elementos
            .Select(estudiante => estudiante.UsuarioId)
            .ToList();

        var usuarios = await _context.Users
            .AsNoTracking()
            .Where(usuario => usuariosIds.Contains(usuario.Id))
            .ToDictionaryAsync(
                usuario => usuario.Id,
                cancellationToken);

        var estudiantes = resultado.Elementos
            .Select(estudiante =>
            {
                if (!usuarios.TryGetValue(
                        estudiante.UsuarioId,
                        out var usuario))
                {
                    throw new InvalidOperationException(
                        "El estudiante no tiene un usuario asociado.");
                }

                return CrearRespuesta(estudiante, usuario);
            })
            .ToList();

        return new ResultadoPaginado<EstudianteResponse>(
            estudiantes,
            resultado.Pagina,
            resultado.TamanoPagina,
            resultado.TotalElementos);
    }

    public async Task<EstudianteResponse> ObtenerPorIdAsync(
        Guid estudianteId,
        CancellationToken cancellationToken = default)
    {
        var estudiante =
            await ObtenerEstudianteAsync(
                estudianteId,
                cancellationToken);

        var usuario =
            await ObtenerUsuarioAsync(estudiante.UsuarioId);

        return CrearRespuesta(estudiante, usuario);
    }

    public async Task<EstudianteResponse> CrearAsync(
        CrearEstudianteRequest request,
        CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim();
        var numeroDocumento = request.NumeroDocumento.Trim();

        var usuarioExistente =
            await _userManager.FindByEmailAsync(email);

        if (usuarioExistente is not null)
        {
            throw new ConflictoException(
                "Ya existe un usuario registrado con este correo.");
        }

        var documentoExiste =
            await _estudianteRepository.ExisteDocumentoAsync(
                numeroDocumento,
                cancellationToken: cancellationToken);

        if (documentoExiste)
        {
            throw new ConflictoException(
                "Ya existe un estudiante con este documento.");
        }

        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            var usuario = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = email,
                Email = email,
                NombreCompleto = request.NombreCompleto.Trim(),
                FechaRegistro = DateTime.UtcNow
            };

            var resultadoCreacion =
                await _userManager.CreateAsync(
                    usuario,
                    request.Password);

            ValidarResultadoIdentity(
                resultadoCreacion,
                "No se pudo crear el usuario.");

            var resultadoRol =
                await _userManager.AddToRoleAsync(
                    usuario,
                    RolUsuario.Estudiante.ToString());

            ValidarResultadoIdentity(
                resultadoRol,
                "No se pudo asignar el rol Estudiante.");

            var estudiante = new Estudiante(
                usuario.Id,
                numeroDocumento,
                request.TipoDocumento,
                request.ProgramaAcademico,
                request.Semestre);

            await _estudianteRepository.AgregarAsync(
                estudiante,
                cancellationToken);

            await _unidadTrabajo.GuardarCambiosAsync(
                cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return CrearRespuesta(estudiante, usuario);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<EstudianteResponse> ActualizarAsync(
        Guid estudianteId,
        ActualizarEstudianteRequest request,
        CancellationToken cancellationToken = default)
    {
        var estudiante =
            await ObtenerEstudianteAsync(
                estudianteId,
                cancellationToken);

        var usuario =
            await ObtenerUsuarioAsync(estudiante.UsuarioId);

        var email = request.Email.Trim();
        var numeroDocumento = request.NumeroDocumento.Trim();

        var documentoExiste =
            await _estudianteRepository.ExisteDocumentoAsync(
                numeroDocumento,
                estudianteId,
                cancellationToken);

        if (documentoExiste)
        {
            throw new ConflictoException(
                "Ya existe otro estudiante con este documento.");
        }

        var usuarioConEmail =
            await _userManager.FindByEmailAsync(email);

        if (usuarioConEmail is not null &&
            usuarioConEmail.Id != usuario.Id)
        {
            throw new ConflictoException(
                "Ya existe otro usuario registrado con este correo.");
        }

        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            estudiante.ActualizarDatosAcademicos(
                numeroDocumento,
                request.TipoDocumento,
                request.ProgramaAcademico,
                request.Semestre);

            usuario.NombreCompleto =
                request.NombreCompleto.Trim();

            usuario.Email = email;
            usuario.UserName = email;

            _estudianteRepository.Actualizar(estudiante);

            var resultadoUsuario =
                await _userManager.UpdateAsync(usuario);

            ValidarResultadoIdentity(
                resultadoUsuario,
                "No se pudo actualizar el usuario.");

            await _unidadTrabajo.GuardarCambiosAsync(
                cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return CrearRespuesta(estudiante, usuario);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task EliminarAsync(
        Guid estudianteId,
        CancellationToken cancellationToken = default)
    {
        var estudiante =
            await ObtenerEstudianteAsync(
                estudianteId,
                cancellationToken);

        var tieneSolicitudes =
            await _context.SolicitudesApoyo.AnyAsync(
                solicitud =>
                    solicitud.EstudianteId == estudianteId,
                cancellationToken);

        if (tieneSolicitudes)
        {
            throw new ReglaNegocioException(
                "No se puede eliminar un estudiante que tenga solicitudes de apoyo.");
        }

        var usuario =
            await ObtenerUsuarioAsync(estudiante.UsuarioId);

        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            _estudianteRepository.Eliminar(estudiante);

            var resultadoEliminacion =
                await _userManager.DeleteAsync(usuario);

            ValidarResultadoIdentity(
                resultadoEliminacion,
                "No se pudo eliminar el usuario.");

            await _unidadTrabajo.GuardarCambiosAsync(
                cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private async Task<Estudiante> ObtenerEstudianteAsync(
        Guid estudianteId,
        CancellationToken cancellationToken)
    {
        if (estudianteId == Guid.Empty)
        {
            throw new ArgumentException(
                "El identificador del estudiante no es válido.",
                nameof(estudianteId));
        }

        var estudiante =
            await _estudianteRepository.ObtenerPorIdAsync(
                estudianteId,
                cancellationToken);

        return estudiante ??
            throw new KeyNotFoundException(
                "No se encontró el estudiante solicitado.");
    }

    private async Task<ApplicationUser> ObtenerUsuarioAsync(
        Guid usuarioId)
    {
        var usuario =
            await _userManager.FindByIdAsync(
                usuarioId.ToString());

        return usuario ??
            throw new KeyNotFoundException(
                "No se encontró el usuario asociado al estudiante.");
    }

    private static EstudianteResponse CrearRespuesta(
        Estudiante estudiante,
        ApplicationUser usuario)
    {
        return new EstudianteResponse
        {
            Id = estudiante.Id,
            UsuarioId = estudiante.UsuarioId,
            NombreCompleto = usuario.NombreCompleto,
            Email = usuario.Email ?? string.Empty,
            NumeroDocumento = estudiante.NumeroDocumento,
            TipoDocumento = estudiante.TipoDocumento,
            ProgramaAcademico =
                estudiante.ProgramaAcademico,
            Semestre = estudiante.Semestre,
            FechaRegistro = usuario.FechaRegistro
        };
    }

    private static void ValidarPaginacion(
        int pagina,
        int tamanoPagina)
    {
        if (pagina < 1)
        {
            throw new ArgumentException(
                "La página debe ser mayor o igual a 1.",
                nameof(pagina));
        }

        if (tamanoPagina is < 1 or > 100)
        {
            throw new ArgumentException(
                "El tamaño de página debe estar entre 1 y 100.",
                nameof(tamanoPagina));
        }
    }

    private static void ValidarResultadoIdentity(
        IdentityResult resultado,
        string mensaje)
    {
        if (resultado.Succeeded)
        {
            return;
        }

        var errores = string.Join(
            " ",
            resultado.Errors.Select(
                error => error.Description));

        throw new ConflictoException(
            $"{mensaje} {errores}");
    }
}