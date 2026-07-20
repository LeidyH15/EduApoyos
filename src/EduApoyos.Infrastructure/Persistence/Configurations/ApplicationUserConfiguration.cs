using EduApoyos.Domain.Entities;
using EduApoyos.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduApoyos.Infrastructure.Persistence.Configurations;

public class ApplicationUserConfiguration
    : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(
        EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("Usuarios");

        builder.Property(usuario => usuario.NombreCompleto)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(usuario => usuario.Email)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(usuario => usuario.FechaRegistro)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.HasOne(usuario => usuario.Estudiante)
            .WithOne()
            .HasForeignKey<Estudiante>(
                estudiante => estudiante.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}