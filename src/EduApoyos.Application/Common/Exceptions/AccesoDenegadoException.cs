namespace EduApoyos.Application.Common.Exceptions;

public class AccesoDenegadoException : Exception
{
    public AccesoDenegadoException(string message)
        : base(message)
    {
    }
}