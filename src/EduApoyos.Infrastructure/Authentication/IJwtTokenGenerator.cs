using EduApoyos.Infrastructure.Identity;

namespace EduApoyos.Infrastructure.Authentication;

public interface IJwtTokenGenerator
{
    JwtTokenResult Generar(
        ApplicationUser usuario,
        IEnumerable<string> roles);
}