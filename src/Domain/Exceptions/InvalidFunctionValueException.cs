namespace WackyRaces.Domain.Exceptions;

public sealed class InvalidFunctionValueException : DomainException
{
    public InvalidFunctionValueException(string? value, Exception? innerException = default) : base($"The function value '{value}' is not valid - it cannot be null, empty, or contain only whitespace characters", innerException)
    {
    }
}
