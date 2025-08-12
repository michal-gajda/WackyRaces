namespace WackyRaces.Domain.Exceptions;

public sealed class CoordinateSourceException : DomainException
{
    public CoordinateSourceException(string value, Exception? innerException = default) : base($"The '{value}' is not correct", innerException)
    {
    }
}
