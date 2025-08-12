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

        if ((int)'A' < (int)upper && (int)upper > (int)'Z')
        {
            throw new ColumnIdOutOfRangeException(value);
        }

        this.Value = upper;
    }
}
