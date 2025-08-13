namespace WackyRaces.Domain.Types;

using WackyRaces.Domain.Exceptions;

public readonly record struct ColumnId
{
    public char Value { get; private init; }

    public ColumnId(int value)
    {
        if (value < 1 || value > 26)
        {
            throw new ColumnIdOutOfRangeException(value);
        }

        this.Value = (char)(64 + value);
    }

    public ColumnId(char value)
    {
        char upper = char.ToUpper(value);

        if (upper < 'A' || upper > 'Z')
        {
            throw new ColumnIdOutOfRangeException(value);
        }

        this.Value = upper;
    }

    public ColumnId NextValue()
    {
        if (this.Value == 'Z')
        {
            throw new ColumnIdOutOfRangeException('Z');
        }

        return new ColumnId((char)(this.Value + 1));
    }

    public ColumnId PreviousValue()
    {
        if (this.Value == 'A')
        {
            throw new ColumnIdOutOfRangeException('A');
        }

        return new ColumnId((char)(this.Value - 1));
    }
}
