namespace WackyRaces.Domain.Exceptions;

public sealed class ColumnIdOutOfRangeException : DomainException
{
    public ColumnIdOutOfRangeException(char value, Exception? innerException = default) : base($"The '{value}' is out of range", innerException)
    {
    }

    public ColumnIdOutOfRangeException(int value, Exception? innerException = default) : base($"The '{value}' is out of range", innerException)
    {
    }
}
