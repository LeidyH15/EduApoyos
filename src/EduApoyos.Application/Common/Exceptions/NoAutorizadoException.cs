namespace EduApoyos.Application.Common.Exceptions;

public class NoAutorizadoException : Exception
{
    public NoAutorizadoException(string message)
        : base(message)
    {
    }
}