using EduApoyos.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace EduApoyos.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string NombreCompleto { get; set; } = string.Empty;

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    public Estudiante? Estudiante { get; set; }
}