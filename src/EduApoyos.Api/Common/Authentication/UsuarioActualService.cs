using System.Security.Claims;
using EduApoyos.Application.Abstractions.Authentication;
using EduApoyos.Application.Common.Exceptions;
using EduApoyos.Domain.Enums;

namespace EduApoyos.Api.Common.Authentication;

public class UsuarioActualService : IUsuarioActualService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UsuarioActualService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? Usuario =>
        _httpContextAccessor.HttpContext?.User;

    public bool EstaAutenticado =>
        Usuario?.Identity?.IsAuthenticated == true;

    public bool EsAsesor =>
        Usuario?.IsInRole(
            nameof(RolUsuario.Asesor)) == true;

    public bool EsEstudiante =>
        Usuario?.IsInRole(
            nameof(RolUsuario.Estudiante)) == true;

    public Guid UsuarioId
    {
        get
        {
            if (!EstaAutenticado)
            {
                throw new NoAutorizadoException(
                    "El usuario no se encuentra autenticado.");
            }

            var valorUsuarioId =
                Usuario?.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(
                    valorUsuarioId,
                    out var usuarioId))
            {
                throw new NoAutorizadoException(
                    "El token no contiene un identificador de usuario válido.");
            }

            return usuarioId;
        }
    }
}