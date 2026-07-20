using EduApoyos.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace EduApoyos.Infrastructure.Identity;

public class IdentityDataSeeder
{
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SeedAsesorSettings _settings;

    public IdentityDataSeeder(
        RoleManager<IdentityRole<Guid>> roleManager,
        UserManager<ApplicationUser> userManager,
        IOptions<SeedAsesorSettings> options)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _settings = options.Value;
    }

    public async Task InicializarAsync()
    {
        await CrearRolSiNoExisteAsync(
            RolUsuario.Asesor.ToString());

        await CrearRolSiNoExisteAsync(
            RolUsuario.Estudiante.ToString());

        await CrearAsesorSiNoExisteAsync();
    }

    private async Task CrearRolSiNoExisteAsync(
        string nombreRol)
    {
        if (await _roleManager.RoleExistsAsync(nombreRol))
        {
            return;
        }

        var resultado = await _roleManager.CreateAsync(
            new IdentityRole<Guid>
            {
                Id = Guid.NewGuid(),
                Name = nombreRol
            });

        if (!resultado.Succeeded)
        {
            throw new InvalidOperationException(
                ConstruirMensajeErrores(
                    $"No se pudo crear el rol {nombreRol}",
                    resultado.Errors));
        }
    }

    private async Task CrearAsesorSiNoExisteAsync()
    {
        if (string.IsNullOrWhiteSpace(_settings.Email) ||
            string.IsNullOrWhiteSpace(_settings.Password) ||
            string.IsNullOrWhiteSpace(_settings.NombreCompleto))
        {
            throw new InvalidOperationException(
                "La configuración SeedAsesor no está completa.");
        }

        var email = _settings.Email.Trim();

        var usuarioExistente =
            await _userManager.FindByEmailAsync(email);

        if (usuarioExistente is not null)
        {
            if (!await _userManager.IsInRoleAsync(
                    usuarioExistente,
                    RolUsuario.Asesor.ToString()))
            {
                var resultadoRol =
                    await _userManager.AddToRoleAsync(
                        usuarioExistente,
                        RolUsuario.Asesor.ToString());

                if (!resultadoRol.Succeeded)
                {
                    throw new InvalidOperationException(
                        ConstruirMensajeErrores(
                            "No se pudo asignar el rol Asesor",
                            resultadoRol.Errors));
                }
            }

            return;
        }

        var asesor = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email,
            NombreCompleto = _settings.NombreCompleto.Trim(),
            FechaRegistro = DateTime.UtcNow,
            EmailConfirmed = true
        };

        var resultadoCreacion =
            await _userManager.CreateAsync(
                asesor,
                _settings.Password);

        if (!resultadoCreacion.Succeeded)
        {
            throw new InvalidOperationException(
                ConstruirMensajeErrores(
                    "No se pudo crear el asesor inicial",
                    resultadoCreacion.Errors));
        }

        var resultadoAsignacion =
            await _userManager.AddToRoleAsync(
                asesor,
                RolUsuario.Asesor.ToString());

        if (!resultadoAsignacion.Succeeded)
        {
            throw new InvalidOperationException(
                ConstruirMensajeErrores(
                    "No se pudo asignar el rol Asesor",
                    resultadoAsignacion.Errors));
        }
    }

    private static string ConstruirMensajeErrores(
        string mensaje,
        IEnumerable<IdentityError> errores)
    {
        return $"{mensaje}: {string.Join(
            " ",
            errores.Select(error => error.Description))}";
    }
}