namespace WackyRaces.Domain.Types;

using System.Text.RegularExpressions;
using WackyRaces.Domain.Exceptions;

public readonly partial record struct Coordinate
{
    [GeneratedRegex(@"^(?<columnId>[A-Z])(?<rowId>[0-9]+)$", RegexOptions.IgnoreCase, 100)]
    private static partial Regex CoordinateParseRegex();

    public RowId RowId { get; private init; }
    public ColumnId ColumnId { get; init; }

    public Coordinate(int rowId, int columnId)
    {
        this.RowId = new RowId(rowId);
        this.ColumnId = new ColumnId(columnId);
    }

    public Coordinate(RowId rowId, ColumnId columnId)
    {
        this.RowId = rowId;
        this.ColumnId = columnId;
    }

    public override string ToString()
    {
        return $"{this.ColumnId.Value}{this.RowId.Value}";
    }

    public static Coordinate Parse(string source)
    {
        var match = CoordinateParseRegex().Match(source);

        if (match.Success)
        {
            var columnId = Convert.ToChar(match.Groups["columnId"].Value);
            var rowId = Convert.ToInt32(match.Groups["rowId"].Value);

            return new Coordinate(new RowId(rowId), new ColumnId(columnId));
        }

        throw new CoordinateSourceException(source);
    }
}
