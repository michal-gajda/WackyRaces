namespace WackyRaces.Domain.Exceptions;

public sealed class InvalidNumberTokenException : DomainException
{
    public InvalidNumberTokenException(string token, Exception? innerException = default) : base($"Invalid number token: '{token}'. Expected a valid integer, decimal, or percentage value", innerException)
    {
    }
}
