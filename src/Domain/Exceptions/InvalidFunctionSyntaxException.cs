namespace WackyRaces.Domain.Exceptions;

public sealed class InvalidFunctionSyntaxException : DomainException
{
    public InvalidFunctionSyntaxException(string expression, Exception? innerException = default) : base($"Invalid function syntax: '{expression}'. Functions must have matching parentheses and valid structure", innerException)
    {
    }
}
