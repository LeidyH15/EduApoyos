using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EduApoyos.Infrastructure.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EduApoyos.Infrastructure.Authentication;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _settings;

    public JwtTokenGenerator(
        IOptions<JwtSettings> options)
    {
        _settings = options.Value;
    }

    public JwtTokenResult Generar(
        ApplicationUser usuario,
        IEnumerable<string> roles)
    {
        var expiracion = DateTime.UtcNow.AddMinutes(
            _settings.ExpirationMinutes);

        var claims = new List<Claim>
        {
            new(
                JwtRegisteredClaimNames.Sub,
                usuario.Id.ToString()),

            new(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString()),

            new(
                JwtRegisteredClaimNames.Email,
                usuario.Email ?? string.Empty),

            new(
                ClaimTypes.NameIdentifier,
                usuario.Id.ToString()),

            new(
                ClaimTypes.Name,
                usuario.NombreCompleto)
        };

        claims.AddRange(
            roles.Select(
                rol => new Claim(ClaimTypes.Role, rol)));

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_settings.Key));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expiracion,
            signingCredentials: credentials);

        var tokenText = new JwtSecurityTokenHandler()
            .WriteToken(token);

        return new JwtTokenResult(
            tokenText,
            expiracion);
    }
}