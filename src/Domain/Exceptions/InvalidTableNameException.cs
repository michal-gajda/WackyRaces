namespace WackyRaces.Domain.Exceptions;

public sealed class InvalidTableNameException : DomainException
{
    public InvalidTableNameException(string? value, Exception? innerException = default) : base($"The table name '{value}' is not valid - it cannot be null, empty, or contain only whitespace characters", innerException)
    {
    }
}
