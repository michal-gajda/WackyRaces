namespace WackyRaces.Domain.Exceptions;

public sealed class UnknownOperatorException : DomainException
{
    public UnknownOperatorException(string operatorToken, Exception? innerException = default) : base($"Unknown operator: '{operatorToken}'. Supported operators are: +, -, *, /", innerException)
    {
    }
}
