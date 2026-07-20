namespace EduApoyos.Application.Common.Exceptions;

public class ConflictoException : Exception
{
    public ConflictoException(string message)
        : base(message)
    {
    }
}