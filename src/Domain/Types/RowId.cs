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

    public RowId NextValue()
    {
        return new RowId(this.Value + 1);
    }

    public RowId PreviousValue()
    {
        if (this.Value == 1)
        {
            throw new RowIdOutOfRangeException(0);
        }

        return new RowId(this.Value - 1);
    }
}
