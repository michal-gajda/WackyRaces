namespace WackyRaces.Domain.Exceptions;

public sealed class UnknownFunctionException : DomainException
{
    public UnknownFunctionException(string functionName, Exception? innerException = default) : base($"Unknown function: '{functionName}'. Supported functions are: SUM, AVG, COUNT", innerException)
    {
    }
}
