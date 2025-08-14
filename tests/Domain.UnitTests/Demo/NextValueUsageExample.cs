namespace WackyRaces.Domain.UnitTests.Demo;

using WackyRaces.Domain.Types;
using WackyRaces.Domain.Entities;

[TestClass]
public class NextValueUsageExample
{
    [TestMethod]
    public void ShouldDemonstrate_PracticalUsageInTableEntity()
    {
        // Practical example: Fill a table with sequential data using NextValue
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "NextValue Example");

        // Start from A1
        var currentColumn = new ColumnId('A');
        var currentRow = new RowId(1);

        // Fill cells A1, A2, A3 with values 10, 20, 30
        for (int i = 1; i <= 3; i++)
        {
            var coordinate = new Coordinate(currentRow, currentColumn);
            table.SetCell(coordinate, new DataValue(i * 10));

            if (i < 3)
            {
                currentRow = currentRow.NextValue(); // Move to next row
            }
        }

        // Move to column B, reset to row 1
        currentColumn = currentColumn.NextValue();
        currentRow = new RowId(1);

        // Fill cells B1, B2, B3 with values 100, 200, 300
        for (int i = 1; i <= 3; i++)
        {
            var coordinate = new Coordinate(currentRow, currentColumn);
            table.SetCell(coordinate, new DataValue(i * 100));

            if (i < 3) currentRow = currentRow.NextValue(); // Move to next row
        }

        // Verify the values
        table.GetValue(Coordinate.Parse("A1")).AsT1.ShouldBe(10);
        table.GetValue(Coordinate.Parse("A2")).AsT1.ShouldBe(20);
        table.GetValue(Coordinate.Parse("A3")).AsT1.ShouldBe(30);

        table.GetValue(Coordinate.Parse("B1")).AsT1.ShouldBe(100);
        table.GetValue(Coordinate.Parse("B2")).AsT1.ShouldBe(200);
        table.GetValue(Coordinate.Parse("B3")).AsT1.ShouldBe(300);
    }

    [TestMethod]
    public void ShouldDemonstrate_LoopingThroughRange()
    {
        // Example: Loop through a range of cells
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "Range Loop Example");

        var startColumn = new ColumnId('A');
        var endColumn = new ColumnId('C');
        var startRow = new RowId(1);
        var endRow = new RowId(2);

        int valueCounter = 1;

        // Loop through columns A to C
        var currentColumn = startColumn;
        while (currentColumn.Value <= endColumn.Value)
        {
            // Loop through rows 1 to 2
            var currentRow = startRow;
            while (currentRow.Value <= endRow.Value)
            {
                var coordinate = new Coordinate(currentRow, currentColumn);
                table.SetCell(coordinate, new DataValue(valueCounter++));

                if (currentRow.Value < endRow.Value)
                    currentRow = currentRow.NextValue();
                else
                    break;
            }

            if (currentColumn.Value < endColumn.Value)
                currentColumn = currentColumn.NextValue();
            else
                break;
        }

        // Verify: Should have filled A1=1, A2=2, B1=3, B2=4, C1=5, C2=6
        table.GetValue(Coordinate.Parse("A1")).AsT1.ShouldBe(1);
        table.GetValue(Coordinate.Parse("A2")).AsT1.ShouldBe(2);
        table.GetValue(Coordinate.Parse("B1")).AsT1.ShouldBe(3);
        table.GetValue(Coordinate.Parse("B2")).AsT1.ShouldBe(4);
        table.GetValue(Coordinate.Parse("C1")).AsT1.ShouldBe(5);
        table.GetValue(Coordinate.Parse("C2")).AsT1.ShouldBe(6);
    }

    [TestMethod]
    public void ShouldShow_NextValueChaining()
    {
        // Demonstrate method chaining for multiple increments
        var row = new RowId(1);
        var column = new ColumnId('A');

        // Chain multiple NextValue calls
        var row5 = row.NextValue().NextValue().NextValue().NextValue();
        var columnE = column.NextValue().NextValue().NextValue().NextValue();

        row5.Value.ShouldBe(5); // 1 -> 2 -> 3 -> 4 -> 5
        columnE.Value.ShouldBe('E'); // A -> B -> C -> D -> E
    }
}
