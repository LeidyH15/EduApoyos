using System.Text;
using EduApoyos.Infrastructure.Authentication;
using EduApoyos.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EduApoyos.IntegrationTests.Infrastructure;

public class CustomWebApplicationFactory
    : WebApplicationFactory<Program>
{
    private const string JwtKey =
        "IntegrationTests_Jwt_Key_2026_Segura_123456789";

    private const string JwtIssuer =
        "EduApoyos.Api.Tests";

    private const string JwtAudience =
        "EduApoyos.Client.Tests";

    private readonly string _databaseName =
        $"EduApoyosTests-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration(
            (_, configuration) =>
            {
                var configuracionPruebas =
                    new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:DefaultConnection"] =
                            "Server=localhost;Database=EduApoyosTests;",

                        ["Jwt:Key"] =
                            JwtKey,

                        ["Jwt:Issuer"] =
                            JwtIssuer,

                        ["Jwt:Audience"] =
                            JwtAudience,

                        ["Jwt:ExpirationMinutes"] =
                            "60",

                        ["SeedAsesor:NombreCompleto"] =
                            "Asesor Pruebas",

                        ["SeedAsesor:Email"] =
                            "asesor.tests@eduapoyos.local",

                        ["SeedAsesor:Password"] =
                            "Asesor_Test2026!"
                    };

                configuration.AddInMemoryCollection(
                    configuracionPruebas);
            });

        builder.ConfigureServices(services =>
        {
            var configuracionesDbContext =
                services
                    .Where(descriptor =>
                        descriptor.ServiceType ==
                            typeof(
                                DbContextOptions<
                                    ApplicationDbContext>) ||
                        descriptor.ServiceType.Name.Contains(
                            "IDbContextOptionsConfiguration"))
                    .ToList();

            foreach (
                var descriptor in configuracionesDbContext)
            {
                services.Remove(descriptor);
            }

            services.RemoveAll<ApplicationDbContext>();

            services.AddDbContext<ApplicationDbContext>(
    options =>
        options
            .UseInMemoryDatabase(
                _databaseName)
            .ConfigureWarnings(warnings =>
                warnings.Ignore(
                    InMemoryEventId
                        .TransactionIgnoredWarning)));

            services.Configure<JwtSettings>(
                options =>
                {
                    options.Key = JwtKey;
                    options.Issuer = JwtIssuer;
                    options.Audience = JwtAudience;
                    options.ExpirationMinutes = 60;
                });

            services.PostConfigure<JwtBearerOptions>(
                JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,

                            ValidIssuer = JwtIssuer,
                            ValidAudience = JwtAudience,

                            IssuerSigningKey =
                                new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(
                                        JwtKey)),

                            ClockSkew = TimeSpan.Zero
                        };
                });
        });
    }
}