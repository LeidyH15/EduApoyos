namespace EduApoyos.Application.Authentication.Models;

public class AutenticacionResponse
{
    public Guid UsuarioId { get; init; }

    public string NombreCompleto { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Rol { get; init; } = string.Empty;

    public string Token { get; init; } = string.Empty;

    public DateTime Expiracion { get; init; }
}