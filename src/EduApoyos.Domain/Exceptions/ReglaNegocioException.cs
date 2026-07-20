namespace EduApoyos.Domain.Exceptions;

public class ReglaNegocioException : Exception
{
    public ReglaNegocioException(string message)
        : base(message)
    {
    }
}