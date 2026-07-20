using EduApoyos.Application.Abstractions.Persistence;
using EduApoyos.Domain.Strategies;
using EduApoyos.Infrastructure.Identity;
using EduApoyos.Infrastructure.Persistence;
using EduApoyos.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        return services;
    }
}