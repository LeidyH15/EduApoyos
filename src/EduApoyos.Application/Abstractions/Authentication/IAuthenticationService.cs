using EduApoyos.Application.Authentication.Models;

namespace EduApoyos.Application.Abstractions.Authentication;

public interface IAuthenticationService
{
    Task<AutenticacionResponse> RegistrarEstudianteAsync(
        RegistroRequest request,
        CancellationToken cancellationToken = default);

    Task<AutenticacionResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default);
}