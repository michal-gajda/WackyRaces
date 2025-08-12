namespace WackyRaces.Domain.Exceptions;

public sealed class UnsupportedComplexRangeException : DomainException
{
    public UnsupportedComplexRangeException(string range, Exception? innerException = default) : base($"Complex ranges not yet supported: '{range}'. Currently only single column ranges like 'A1:A3' are supported", innerException)
    {
    }
}
