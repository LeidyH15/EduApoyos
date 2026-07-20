using EduApoyos.Domain.Entities;
using EduApoyos.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduApoyos.Infrastructure.Persistence.Configurations;

public class SolicitudApoyoConfiguration
    : IEntityTypeConfiguration<SolicitudApoyo>
{
    public void Configure(
        EntityTypeBuilder<SolicitudApoyo> builder)
    {
        builder.ToTable("SolicitudesApoyo");

        builder.HasKey(solicitud => solicitud.Id);

        builder.Property(solicitud => solicitud.TipoApoyo)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(solicitud => solicitud.MontoSolicitado)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(solicitud => solicitud.Descripcion)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(solicitud => solicitud.Estado)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(solicitud => solicitud.FechaSolicitud)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(solicitud => solicitud.FechaActualizacion)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(solicitud => solicitud.AsesorId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(solicitud => new
        {
            solicitud.Estado,
            solicitud.FechaActualizacion
        })
            .IsClustered(false)
            .HasDatabaseName(
                "IX_SolicitudesApoyo_Estado_FechaActualizacion");
    }
}