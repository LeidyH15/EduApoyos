using EduApoyos.Application.Abstractions.Authentication;
using EduApoyos.Application.Abstractions.Persistence;
using EduApoyos.Application.Authentication.Models;
using EduApoyos.Application.Common.Exceptions;
using EduApoyos.Domain.Entities;
using EduApoyos.Domain.Enums;
using EduApoyos.Infrastructure.Identity;
using EduApoyos.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace EduApoyos.Infrastructure.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<ApplicationUser>
        _userManager;

    private readonly SignInManager<ApplicationUser>
        _signInManager;

    private readonly IEstudianteRepository
        _estudianteRepository;

    private readonly IUnidadTrabajo
        _unidadTrabajo;

    private readonly ApplicationDbContext
        _context;

    private readonly IJwtTokenGenerator
        _tokenGenerator;

    public AuthenticationService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEstudianteRepository estudianteRepository,
        IUnidadTrabajo unidadTrabajo,
        ApplicationDbContext context,
        IJwtTokenGenerator tokenGenerator)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _estudianteRepository = estudianteRepository;
        _unidadTrabajo = unidadTrabajo;
        _context = context;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<AutenticacionResponse>
        RegistrarEstudianteAsync(
            RegistroRequest request,
            CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim();

        var numeroDocumento =
            request.NumeroDocumento.Trim();

        var usuarioExistente =
            await _userManager.FindByEmailAsync(
                email);

        if (usuarioExistente is not null)
        {
            throw new ConflictoException(
                "Ya existe un usuario registrado con este correo.");
        }

        var documentoExiste =
            await _estudianteRepository
                .ExisteDocumentoAsync(
                    numeroDocumento,
                    cancellationToken:
                        cancellationToken);

        if (documentoExiste)
        {
            throw new ConflictoException(
                "Ya existe un estudiante con este documento.");
        }

        await using var transaction =
            await _context.Database
                .BeginTransactionAsync(
                    cancellationToken);

        try
        {
            var usuario = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = email,
                Email = email,
                NombreCompleto =
                    request.NombreCompleto.Trim(),
                FechaRegistro = DateTime.UtcNow
            };

            var resultadoCreacion =
                await _userManager.CreateAsync(
                    usuario,
                    request.Password);

            if (!resultadoCreacion.Succeeded)
            {
                var errores = string.Join(
                    " ",
                    resultadoCreacion.Errors.Select(
                        error =>
                            error.Description));

                throw new ConflictoException(
                    errores);
            }

            var rol =
                RolUsuario.Estudiante.ToString();

            var resultadoRol =
                await _userManager.AddToRoleAsync(
                    usuario,
                    rol);

            if (!resultadoRol.Succeeded)
            {
                var errores = string.Join(
                    " ",
                    resultadoRol.Errors.Select(
                        error =>
                            error.Description));

                throw new InvalidOperationException(
                    $"No se pudo asignar el rol: {errores}");
            }

            var estudiante = new Estudiante(
                usuario.Id,
                numeroDocumento,
                request.TipoDocumento,
                request.ProgramaAcademico,
                request.Semestre);

            await _estudianteRepository
                .AgregarAsync(
                    estudiante,
                    cancellationToken);

            await _unidadTrabajo
                .GuardarCambiosAsync(
                    cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);

            return CrearRespuesta(
                usuario,
                [rol],
                estudiante.Id);
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken);

            throw;
        }
    }

    public async Task<AutenticacionResponse>
        LoginAsync(
            LoginRequest request,
            CancellationToken cancellationToken = default)
    {
        var usuario =
            await _userManager.FindByEmailAsync(
                request.Email.Trim());

        if (usuario is null)
        {
            throw new NoAutorizadoException(
                "El correo o la contraseña son incorrectos.");
        }

        var resultado =
            await _signInManager
                .CheckPasswordSignInAsync(
                    usuario,
                    request.Password,
                    lockoutOnFailure: true);

        if (!resultado.Succeeded)
        {
            throw new NoAutorizadoException(
                "El correo o la contraseña son incorrectos.");
        }

        var roles =
            await _userManager.GetRolesAsync(
                usuario);

        Guid? estudianteId = null;

        if (
            roles.Contains(
                RolUsuario.Estudiante.ToString(),
                StringComparer.OrdinalIgnoreCase)
        )
        {
            var estudiante =
                await _estudianteRepository
                    .ObtenerPorUsuarioIdAsync(
                        usuario.Id,
                        cancellationToken);

            if (estudiante is null)
            {
                throw new InvalidOperationException(
                    "El usuario estudiante no tiene un perfil académico asociado.");
            }

            estudianteId = estudiante.Id;
        }

        return CrearRespuesta(
            usuario,
            roles,
            estudianteId);
    }

    private AutenticacionResponse CrearRespuesta(
        ApplicationUser usuario,
        IEnumerable<string> roles,
        Guid? estudianteId)
    {
        var listaRoles =
            roles.ToList();

        var token =
            _tokenGenerator.Generar(
                usuario,
                listaRoles);

        return new AutenticacionResponse
        {
            UsuarioId = usuario.Id,
            EstudianteId = estudianteId,
            NombreCompleto =
                usuario.NombreCompleto,
            Email =
                usuario.Email ??
                string.Empty,
            Rol =
                listaRoles.FirstOrDefault() ??
                string.Empty,
            Token = token.Token,
            Expiracion = token.Expiracion
        };
    }
}