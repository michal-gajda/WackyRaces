namespace WackyRaces.Domain.Exceptions;

using WackyRaces.Domain.Types;

public sealed class CircularReferenceException : DomainException
{
    public CircularReferenceException(Coordinate coordinate)
        : base($"Circular reference detected at cell {coordinate}")
    {
    }
}
