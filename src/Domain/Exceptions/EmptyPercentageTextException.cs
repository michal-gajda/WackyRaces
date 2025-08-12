namespace WackyRaces.Domain.Exceptions;

public sealed class EmptyPercentageTextException : DomainException
{
    public EmptyPercentageTextException(Exception? innerException = default) : base("Percentage text cannot be null, empty, or contain only whitespace characters", innerException)
    {
    }
}
