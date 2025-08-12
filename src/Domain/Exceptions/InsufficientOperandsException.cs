namespace WackyRaces.Domain.Exceptions;

public sealed class InsufficientOperandsException : DomainException
{
    public InsufficientOperandsException(string operatorToken, Exception? innerException = default) : base($"Insufficient operands for operator '{operatorToken}'. Binary operators require two operands", innerException)
    {
    }
}
