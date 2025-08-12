namespace WackyRaces.Domain;

public abstract class DomainException : Exception
{
    protected DomainException(string message, Exception? innerException = default) : base(message, innerException)
    {
    }
}
