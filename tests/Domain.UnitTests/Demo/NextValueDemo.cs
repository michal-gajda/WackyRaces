using Shouldly;
using WackyRaces.Domain.Types;

namespace WackyRaces.Domain.UnitTests.Demo;

[TestClass]
public class NextValueDemo
{
    [TestMethod]
    public void ShouldDemonstrate_RowIdNextValue()
    {
        // Demonstrate RowId.NextValue() - returns next integer
        var row = new RowId(1);

        Console.WriteLine($"Starting row: {row.Value}");

        for (int i = 0; i < 5; i++)
        {
            row = row.NextValue();
            Console.WriteLine($"Next row: {row.Value}");
        }

        // Expected sequence: 1 -> 2 -> 3 -> 4 -> 5 -> 6
        row.Value.ShouldBe(6);
    }

    [TestMethod]
    public void ShouldDemonstrate_ColumnIdNextValue()
    {
        // Demonstrate ColumnId.NextValue() - returns next letter A->B->C
        var column = new ColumnId('A');

        Console.WriteLine($"Starting column: {column.Value}");

        for (int i = 0; i < 5; i++)
        {
            column = column.NextValue();
            Console.WriteLine($"Next column: {column.Value}");
        }

        // Expected sequence: A -> B -> C -> D -> E -> F
        column.Value.ShouldBe('F');
    }

    [TestMethod]
    public void ShouldDemonstrate_SpreadsheetStyleIncrement()
    {
        // Demonstrate typical spreadsheet usage
        var currentRow = new RowId(1);
        var currentColumn = new ColumnId('A');

        // Simulate moving through cells: A1, A2, A3, then B1, B2, B3
        for (int row = 1; row <= 3; row++)
        {
            Console.WriteLine($"Cell: {currentColumn.Value}{currentRow.Value}");
            if (row < 3) currentRow = currentRow.NextValue();
        }

        // Move to next column, reset row
        currentColumn = currentColumn.NextValue();
        currentRow = new RowId(1);

        for (int row = 1; row <= 3; row++)
        {
            Console.WriteLine($"Cell: {currentColumn.Value}{currentRow.Value}");
            if (row < 3) currentRow = currentRow.NextValue();
        }

        currentColumn.Value.ShouldBe('B');
        currentRow.Value.ShouldBe(3);
    }

    [TestMethod]
    public void ShouldDemonstrate_EdgeCases()
    {
        // Test edge cases

        // RowId can increment indefinitely (until int.MaxValue)
        var largeRow = new RowId(int.MaxValue - 1);
        var nextLargeRow = largeRow.NextValue();
        nextLargeRow.Value.ShouldBe(int.MaxValue);

        // ColumnId stops at Z
        var columnY = new ColumnId('Y');
        var columnZ = columnY.NextValue();
        columnZ.Value.ShouldBe('Z');

        // Trying to go beyond Z should throw exception
        Should.Throw<WackyRaces.Domain.Exceptions.ColumnIdOutOfRangeException>(() => columnZ.NextValue());
    }

    [TestMethod]
    public void ShouldDemonstrate_RowIdPreviousValue()
    {
        // Demonstrate RowId.PreviousValue() - returns previous integer
        var row = new RowId(6);

        Console.WriteLine($"Starting row: {row.Value}");

        for (int i = 0; i < 5; i++)
        {
            row = row.PreviousValue();
            Console.WriteLine($"Previous row: {row.Value}");
        }

        // Expected sequence: 6 -> 5 -> 4 -> 3 -> 2 -> 1
        row.Value.ShouldBe(1);
    }

    [TestMethod]
    public void ShouldDemonstrate_ColumnIdPreviousValue()
    {
        // Demonstrate ColumnId.PreviousValue() - returns previous letter F->E->D
        var column = new ColumnId('F');

        Console.WriteLine($"Starting column: {column.Value}");

        for (int i = 0; i < 5; i++)
        {
            column = column.PreviousValue();
            Console.WriteLine($"Previous column: {column.Value}");
        }

        // Expected sequence: F -> E -> D -> C -> B -> A
        column.Value.ShouldBe('A');
    }

    [TestMethod]
    public void ShouldDemonstrate_PreviousValueEdgeCases()
    {
        // Test edge cases for PreviousValue

        // RowId stops at 1
        var row2 = new RowId(2);
        var row1 = row2.PreviousValue();
        row1.Value.ShouldBe(1);

        // Trying to go below 1 should throw exception
        Should.Throw<WackyRaces.Domain.Exceptions.RowIdOutOfRangeException>(() => row1.PreviousValue());

        // ColumnId stops at A
        var columnB = new ColumnId('B');
        var columnA = columnB.PreviousValue();
        columnA.Value.ShouldBe('A');

        // Trying to go before A should throw exception
        Should.Throw<WackyRaces.Domain.Exceptions.ColumnIdOutOfRangeException>(() => columnA.PreviousValue());
    }

    [TestMethod]
    public void ShouldDemonstrate_BidirectionalNavigation()
    {
        // Demonstrate going forward and backward
        var row = new RowId(5);
        var column = new ColumnId('M');

        // Go forward
        row = row.NextValue().NextValue(); // 5 -> 6 -> 7
        column = column.NextValue().NextValue(); // M -> N -> O

        row.Value.ShouldBe(7);
        column.Value.ShouldBe('O');

        // Go backward
        row = row.PreviousValue().PreviousValue().PreviousValue(); // 7 -> 6 -> 5 -> 4
        column = column.PreviousValue().PreviousValue().PreviousValue(); // O -> N -> M -> L

        row.Value.ShouldBe(4);
        column.Value.ShouldBe('L');
    }
}
