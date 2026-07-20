namespace EduApoyos.Infrastructure.Authentication;

public record JwtTokenResult(
    string Token,
    DateTime Expiracion);