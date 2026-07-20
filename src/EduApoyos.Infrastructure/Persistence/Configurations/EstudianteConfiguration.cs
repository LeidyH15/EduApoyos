using EduApoyos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduApoyos.Infrastructure.Persistence.Configurations;

public class EstudianteConfiguration
    : IEntityTypeConfiguration<Estudiante>
{
    public void Configure(
        EntityTypeBuilder<Estudiante> builder)
    {
        builder.ToTable("Estudiantes");

        builder.HasKey(estudiante => estudiante.Id);

        builder.Property(estudiante => estudiante.NumeroDocumento)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(estudiante => estudiante.TipoDocumento)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(estudiante => estudiante.ProgramaAcademico)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(estudiante => estudiante.Semestre)
            .IsRequired();

        builder.HasIndex(estudiante => estudiante.NumeroDocumento)
            .IsUnique()
            .HasDatabaseName("UX_Estudiantes_NumeroDocumento");

        builder.HasMany(estudiante => estudiante.Solicitudes)
            .WithOne(solicitud => solicitud.Estudiante)
            .HasForeignKey(solicitud => solicitud.EstudianteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}