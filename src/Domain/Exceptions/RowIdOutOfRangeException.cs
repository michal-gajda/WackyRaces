namespace WackyRaces.Domain.Exceptions;

public sealed class RowIdOutOfRangeException : DomainException
{
    public RowIdOutOfRangeException(int value, Exception? innerException = default) : base($"The '{value}' is out of range", innerException)
    {
    }
}
