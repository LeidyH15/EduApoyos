namespace EduApoyos.Application.Abstractions.Authentication;

public interface IUsuarioActualService
{
    Guid UsuarioId { get; }

    bool EstaAutenticado { get; }

    bool EsAsesor { get; }

    bool EsEstudiante { get; }
}