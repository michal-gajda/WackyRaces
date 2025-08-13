using Shouldly;
using WackyRaces.Domain.Types;

namespace WackyRaces.Domain.UnitTests.Demo;

[TestClass]
public class PreviousValueDemo
{
    [TestMethod]
    public void ShouldDemonstrate_BidirectionalCellNavigation()
    {
        // Demonstrate navigating a spreadsheet-like grid both ways
        var currentRow = new RowId(5);
        var currentColumn = new ColumnId('E');

        Console.WriteLine($"Starting at cell: {currentColumn.Value}{currentRow.Value}");

        // Move right 2 columns and down 2 rows
        currentColumn = currentColumn.NextValue().NextValue(); // E -> F -> G
        currentRow = currentRow.NextValue().NextValue(); // 5 -> 6 -> 7

        Console.WriteLine($"After moving right+down: {currentColumn.Value}{currentRow.Value}");
        currentColumn.Value.ShouldBe('G');
        currentRow.Value.ShouldBe(7);

        // Move left 3 columns and up 3 rows
        currentColumn = currentColumn.PreviousValue().PreviousValue().PreviousValue(); // G -> F -> E -> D
        currentRow = currentRow.PreviousValue().PreviousValue().PreviousValue(); // 7 -> 6 -> 5 -> 4

        Console.WriteLine($"After moving left+up: {currentColumn.Value}{currentRow.Value}");
        currentColumn.Value.ShouldBe('D');
        currentRow.Value.ShouldBe(4);
    }

    [TestMethod]
    public void ShouldDemonstrate_SequentialBackwardIteration()
    {
        // Start from Z10 and move backward through the alphabet and rows
        var column = new ColumnId('Z');
        var row = new RowId(10);

        var cells = new List<string>();

        // Collect cells while moving backward
        for (int i = 0; i < 5; i++)
        {
            cells.Add($"{column.Value}{row.Value}");

            // Move to previous column if possible
            if (column.Value > 'A')
            {
                column = column.PreviousValue();
            }

            // Move to previous row if possible
            if (row.Value > 1)
            {
                row = row.PreviousValue();
            }
        }

        // Should have: Z10, Y9, X8, W7, V6
        cells[0].ShouldBe("Z10");
        cells[1].ShouldBe("Y9");
        cells[2].ShouldBe("X8");
        cells[3].ShouldBe("W7");
        cells[4].ShouldBe("V6");
    }

    [TestMethod]
    public void ShouldDemonstrate_BoundaryChecking()
    {
        // Demonstrate proper boundary checking for PreviousValue

        // Test RowId boundary (minimum is 1)
        var row1 = new RowId(1);
        Should.Throw<WackyRaces.Domain.Exceptions.RowIdOutOfRangeException>(() => row1.PreviousValue());

        var row2 = new RowId(2);
        var prevRow = row2.PreviousValue(); // Should work
        prevRow.Value.ShouldBe(1);

        // Test ColumnId boundary (minimum is 'A')
        var colA = new ColumnId('A');
        Should.Throw<WackyRaces.Domain.Exceptions.ColumnIdOutOfRangeException>(() => colA.PreviousValue());

        var colB = new ColumnId('B');
        var prevCol = colB.PreviousValue(); // Should work
        prevCol.Value.ShouldBe('A');
    }

    [TestMethod]
    public void ShouldDemonstrate_RangeIteration()
    {
        // Iterate through a range from C5 to A3 (backward)
        var startColumn = new ColumnId('C');
        var startRow = new RowId(5);
        var endColumn = new ColumnId('A');
        var endRow = new RowId(3);

        var currentColumn = startColumn;
        var currentRow = startRow;
        var cellsVisited = new List<string>();

        // Navigate backward through the range
        while (currentColumn.Value >= endColumn.Value && currentRow.Value >= endRow.Value)
        {
            cellsVisited.Add($"{currentColumn.Value}{currentRow.Value}");

            // Move to previous position
            if (currentRow.Value > endRow.Value)
            {
                currentRow = currentRow.PreviousValue();
            }
            else if (currentColumn.Value > endColumn.Value)
            {
                currentColumn = currentColumn.PreviousValue();
                currentRow = startRow; // Reset to start row for new column
            }
            else
            {
                break;
            }
        }

        // Should visit: C5, C4, C3, B5, B4, B3, A5, A4, A3
        cellsVisited.Count.ShouldBe(9);
        cellsVisited.ShouldContain("C5");
        cellsVisited.ShouldContain("A3");
    }

    [TestMethod]
    public void ShouldDemonstrate_MethodChaining()
    {
        // Demonstrate chaining PreviousValue calls
        var row = new RowId(10);
        var column = new ColumnId('J');

        // Chain multiple PreviousValue calls
        var row5 = row.PreviousValue().PreviousValue().PreviousValue().PreviousValue().PreviousValue();
        var columnE = column.PreviousValue().PreviousValue().PreviousValue().PreviousValue().PreviousValue();

        row5.Value.ShouldBe(5); // 10 -> 9 -> 8 -> 7 -> 6 -> 5
        columnE.Value.ShouldBe('E'); // J -> I -> H -> G -> F -> E

        // Mix NextValue and PreviousValue
        var finalRow = row5.NextValue().NextValue().PreviousValue(); // 5 -> 6 -> 7 -> 6
        var finalColumn = columnE.NextValue().NextValue().PreviousValue(); // E -> F -> G -> F

        finalRow.Value.ShouldBe(6);
        finalColumn.Value.ShouldBe('F');
    }
}
