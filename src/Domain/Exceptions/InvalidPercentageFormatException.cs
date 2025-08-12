namespace WackyRaces.Domain.Exceptions;

public sealed class InvalidPercentageFormatException : DomainException
{
    public InvalidPercentageFormatException(string value, Exception? innerException = default) : base($"Invalid percentage format: '{value}'. Percentage must end with '%' and contain a valid numeric value", innerException)
    {
    }
}
