namespace WackyRaces.Domain.Exceptions;

public sealed class UnsupportedDataValueOperationException : DomainException
{
    public UnsupportedDataValueOperationException(string operation, string typeName, Exception? innerException = default) : base($"{operation} operation not supported for {typeName} type", innerException)
    {
    }
}
