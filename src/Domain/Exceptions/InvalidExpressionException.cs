namespace WackyRaces.Domain.Exceptions;

public sealed class InvalidExpressionException : DomainException
{
    public InvalidExpressionException(Exception? innerException = default) : base("Invalid expression: The formula could not be evaluated. Check for balanced parentheses and proper syntax", innerException)
    {
    }
}
