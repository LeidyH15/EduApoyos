using EduApoyos.Application.Abstractions.Persistence;
using EduApoyos.Domain.Entities;
using EduApoyos.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EduApoyos.Infrastructure.Persistence;

public class ApplicationDbContext
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>,
      IUnidadTrabajo
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Estudiante> Estudiantes => Set<Estudiante>();

    public DbSet<SolicitudApoyo> SolicitudesApoyo =>
        Set<SolicitudApoyo>();

    public DbSet<HistorialEstado> HistorialesEstado =>
        Set<HistorialEstado>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);
    }

    public Task<int> GuardarCambiosAsync(
        CancellationToken cancellationToken = default)
    {
        return SaveChangesAsync(cancellationToken);
    }
}