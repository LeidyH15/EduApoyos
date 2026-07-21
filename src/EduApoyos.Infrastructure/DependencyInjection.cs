using Microsoft.Extensions.Logging;
using EduApoyos.Application.Abstractions.Authentication;
using EduApoyos.Application.Abstractions.Persistence;
using EduApoyos.Application.Estudiantes;
using EduApoyos.Domain.Strategies;
using EduApoyos.Infrastructure.Authentication;
using EduApoyos.Infrastructure.Estudiantes;
using EduApoyos.Infrastructure.Identity;
using EduApoyos.Infrastructure.Persistence;
using EduApoyos.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace EduApoyos.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "No se configuró la cadena de conexión DefaultConnection.");

        var jwtSection = configuration.GetSection(
            JwtSettings.SectionName);

        var jwtSettings = jwtSection.Get<JwtSettings>()
            ?? throw new InvalidOperationException(
                "No se configuró la sección Jwt.");

        if (string.IsNullOrWhiteSpace(jwtSettings.Key) ||
            Encoding.UTF8.GetByteCount(jwtSettings.Key) < 32)
        {
            throw new InvalidOperationException(
                "La clave JWT debe tener al menos 32 bytes.");
        }

        if (string.IsNullOrWhiteSpace(jwtSettings.Issuer) ||
            string.IsNullOrWhiteSpace(jwtSettings.Audience) ||
            jwtSettings.ExpirationMinutes <= 0)
        {
            throw new InvalidOperationException(
                "La configuración JWT no es válida.");
        }

        services.Configure<JwtSettings>(jwtSection);

        services.Configure<SeedAsesorSettings>(
            configuration.GetSection(
                SeedAsesorSettings.SectionName));

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                connectionString,
                sqlServerOptions =>
                    sqlServerOptions.MigrationsAssembly(
                        typeof(ApplicationDbContext)
                            .Assembly.FullName)));

        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(
                options =>
                {
                    options.User.RequireUniqueEmail = true;

                    options.Password.RequiredLength = 8;
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireNonAlphanumeric = true;

                    options.Lockout.MaxFailedAccessAttempts = 5;

                    options.Lockout.DefaultLockoutTimeSpan =
                        TimeSpan.FromMinutes(15);
                })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme =
                JwtBearerDefaults.AuthenticationScheme;

            options.DefaultChallengeScheme =
                JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(options =>
            {
                
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,

                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(
                                    jwtSettings.Key)),

                        ClockSkew = TimeSpan.Zero
                    };
            });

        services.AddAuthorization();

        services.AddScoped<
            IJwtTokenGenerator,
            JwtTokenGenerator>();

        services.AddScoped<
            IAuthenticationService,
            AuthenticationService>();

        services.AddScoped<IdentityDataSeeder>();

        services.AddScoped<
            IEstudianteRepository,
            EstudianteRepository>();

        services.AddScoped<
            ISolicitudApoyoRepository,
            SolicitudApoyoRepository>();

        services.AddScoped<IUnidadTrabajo>(
            provider =>
                provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<
            IEstrategiaEstadoSolicitud,
            EstrategiaSolicitudPendiente>();

        services.AddScoped<
            IEstrategiaEstadoSolicitud,
            EstrategiaSolicitudEnRevision>();

        services.AddScoped<
           IEstudianteService,
           EstudianteService>();

        return services;
    }
}