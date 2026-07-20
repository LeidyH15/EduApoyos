using EduApoyos.Domain.Entities;
using EduApoyos.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduApoyos.Infrastructure.Persistence.Configurations;

public class HistorialEstadoConfiguration
    : IEntityTypeConfiguration<HistorialEstado>
{
    public void Configure(
        EntityTypeBuilder<HistorialEstado> builder)
    {
        builder.ToTable("HistorialEstados");

        builder.HasKey(historial => historial.Id);

        builder.Property(historial => historial.EstadoAnterior)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(historial => historial.EstadoNuevo)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(historial => historial.FechaCambio)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(historial => historial.Observacion)
            .HasMaxLength(500);

        builder.HasOne(historial => historial.Solicitud)
            .WithMany(solicitud => solicitud.HistorialEstados)
            .HasForeignKey(historial => historial.SolicitudId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(historial => historial.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}