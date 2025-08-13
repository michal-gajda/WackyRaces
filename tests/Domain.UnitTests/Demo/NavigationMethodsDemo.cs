using Shouldly;
using WackyRaces.Domain.Types;

namespace WackyRaces.Domain.UnitTests.Demo;

[TestClass]
public class NavigationMethodsDemo
{
    [TestMethod]
    public void ShouldDemonstrate_CompleteNavigationAPI()
    {
        // Complete demonstration of all navigation methods

        Console.WriteLine("=== RowId Navigation ===");
        var row = new RowId(5);
        Console.WriteLine($"Start: {row.Value}");

        // Forward navigation
        row = row.NextValue();
        Console.WriteLine($"Next: {row.Value}"); // 6

        row = row.NextValue();
        Console.WriteLine($"Next: {row.Value}"); // 7

        // Backward navigation
        row = row.PreviousValue();
        Console.WriteLine($"Previous: {row.Value}"); // 6

        row = row.PreviousValue();
        Console.WriteLine($"Previous: {row.Value}"); // 5

        Console.WriteLine("\n=== ColumnId Navigation ===");
        var column = new ColumnId('M');
        Console.WriteLine($"Start: {column.Value}");

        // Forward navigation
        column = column.NextValue();
        Console.WriteLine($"Next: {column.Value}"); // N

        column = column.NextValue();
        Console.WriteLine($"Next: {column.Value}"); // O

        // Backward navigation
        column = column.PreviousValue();
        Console.WriteLine($"Previous: {column.Value}"); // N

        column = column.PreviousValue();
        Console.WriteLine($"Previous: {column.Value}"); // M

        // Assert final positions
        row.Value.ShouldBe(5);
        column.Value.ShouldBe('M');
    }

    [TestMethod]
    public void ShouldDemonstrate_BoundaryBehavior()
    {
        Console.WriteLine("=== Boundary Testing ===");

        // RowId boundaries
        var minRow = new RowId(1);
        var maxTestRow = new RowId(int.MaxValue);

        Console.WriteLine($"Min RowId: {minRow.Value}");
        Console.WriteLine($"Max RowId test: {maxTestRow.Value}");

        // Can go forward from min
        var nextFromMin = minRow.NextValue();
        nextFromMin.Value.ShouldBe(2);

        // Can go backward to min
        var backToMin = nextFromMin.PreviousValue();
        backToMin.Value.ShouldBe(1);

        // Cannot go backward from min
        Should.Throw<WackyRaces.Domain.Exceptions.RowIdOutOfRangeException>(() => minRow.PreviousValue());

        // ColumnId boundaries
        var minColumn = new ColumnId('A');
        var maxColumn = new ColumnId('Z');

        Console.WriteLine($"Min ColumnId: {minColumn.Value}");
        Console.WriteLine($"Max ColumnId: {maxColumn.Value}");

        // Can go forward from min
        var nextFromMinCol = minColumn.NextValue();
        nextFromMinCol.Value.ShouldBe('B');

        // Can go backward to min
        var backToMinCol = nextFromMinCol.PreviousValue();
        backToMinCol.Value.ShouldBe('A');

        // Cannot go backward from min
        Should.Throw<WackyRaces.Domain.Exceptions.ColumnIdOutOfRangeException>(() => minColumn.PreviousValue());

        // Can go backward from max
        var prevFromMax = maxColumn.PreviousValue();
        prevFromMax.Value.ShouldBe('Y');

        // Can go forward to max
        var backToMax = prevFromMax.NextValue();
        backToMax.Value.ShouldBe('Z');

        // Cannot go forward from max
        Should.Throw<WackyRaces.Domain.Exceptions.ColumnIdOutOfRangeException>(() => maxColumn.NextValue());
    }

    [TestMethod]
    public void ShouldDemonstrate_PracticalSpreadsheetNavigation()
    {
        // Practical example: Navigate like in a real spreadsheet
        var currentCell = (Row: new RowId(10), Column: new ColumnId('E'));

        Console.WriteLine($"Starting at: {currentCell.Column.Value}{currentCell.Row.Value}");

        // Navigate in a pattern: Right, Down, Left, Up
        var path = new List<string>();

        // Add starting position
        path.Add($"{currentCell.Column.Value}{currentCell.Row.Value}");

        // Move right 3 cells
        for (int i = 0; i < 3; i++)
        {
            currentCell.Column = currentCell.Column.NextValue();
            path.Add($"{currentCell.Column.Value}{currentCell.Row.Value}");
        }
        // Now at H10

        // Move down 3 cells
        for (int i = 0; i < 3; i++)
        {
            currentCell.Row = currentCell.Row.NextValue();
            path.Add($"{currentCell.Column.Value}{currentCell.Row.Value}");
        }
        // Now at H13

        // Move left 3 cells
        for (int i = 0; i < 3; i++)
        {
            currentCell.Column = currentCell.Column.PreviousValue();
            path.Add($"{currentCell.Column.Value}{currentCell.Row.Value}");
        }
        // Now at E13

        // Move up 3 cells
        for (int i = 0; i < 3; i++)
        {
            currentCell.Row = currentCell.Row.PreviousValue();
            path.Add($"{currentCell.Column.Value}{currentCell.Row.Value}");
        }
        // Back to E10

        Console.WriteLine($"Path taken: {string.Join(" -> ", path)}");

        // Should be back where we started
        currentCell.Row.Value.ShouldBe(10);
        currentCell.Column.Value.ShouldBe('E');

        // Path should form a square
        path.Count.ShouldBe(13); // Start + 12 moves
        path[0].ShouldBe("E10");  // Start
        path[12].ShouldBe("E10"); // End (back to start)
    }
}
