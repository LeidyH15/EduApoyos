namespace EduApoyos.Application.Abstractions.Persistence;

public interface IUnidadTrabajo
{
    Task<int> GuardarCambiosAsync(
        CancellationToken cancellationToken = default);
}