namespace WackyRaces.Domain.Types;

using WackyRaces.Domain.Exceptions;

public readonly record struct RowId
{
    public int Value { get; private init; }

    public RowId(int value)
    {
        if (value < 1)
        {
            throw new RowIdOutOfRangeException(value);
        }

        this.Value = value;
    }
}
