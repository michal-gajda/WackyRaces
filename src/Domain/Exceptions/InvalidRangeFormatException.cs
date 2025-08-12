namespace WackyRaces.Domain.Exceptions;

public sealed class InvalidRangeFormatException : DomainException
{
    public InvalidRangeFormatException(string range, Exception? innerException = default) : base($"Invalid range format: '{range}'. Expected format is 'A1:A3' or single cell reference like 'A1'", innerException)
    {
    }
}
