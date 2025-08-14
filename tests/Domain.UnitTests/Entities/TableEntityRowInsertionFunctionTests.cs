namespace WackyRaces.Domain.UnitTests.Entities;

using WackyRaces.Domain.Entities;
using WackyRaces.Domain.Types;

[TestClass]
public sealed class TableEntityRowInsertionFunctionTests
{
    private TableEntity CreateTestTable()
    {
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "Function Test Table");
        return table;
    }

    [TestMethod]
    public void InsertRowAfter_WithFunctionCoordinates_UpdatesReferencesCorrectly()
    {
        // Arrange
        var table = CreateTestTable();

        // Set up test data
        table.SetCell(Coordinate.Parse("A1"), new DataValue(10));
        table.SetCell(Coordinate.Parse("A2"), new DataValue(20));
        table.SetCell(Coordinate.Parse("A3"), new DataValue(30));
        table.SetCell(Coordinate.Parse("A4"), new DataValue(40));
        table.SetCell(Coordinate.Parse("A5"), new DataValue(50));

        // Create Function objects with coordinate references
        table.SetCell(Coordinate.Parse("B1"), new DataValue(new Function("SUM(A1,A3)", typeof(decimal))));
        table.SetCell(Coordinate.Parse("B2"), new DataValue(new Function("AVG(A2:A4)", typeof(decimal))));
        table.SetCell(Coordinate.Parse("B3"), new DataValue(new Function("COUNT(A1:A5)", typeof(int))));
        table.SetCell(Coordinate.Parse("B4"), new DataValue(new Function("SUM(A3,A4,A5)", typeof(decimal))));

        Console.WriteLine("Before insertion:");
        Console.WriteLine($"B1: {table.GetCell(Coordinate.Parse("B1")).AsT6.Value}");
        Console.WriteLine($"B2: {table.GetCell(Coordinate.Parse("B2")).AsT6.Value}");
        Console.WriteLine($"B3: {table.GetCell(Coordinate.Parse("B3")).AsT6.Value}");
        Console.WriteLine($"B4: {table.GetCell(Coordinate.Parse("B4")).AsT6.Value}");

        // Act - Insert row after row 2
        table.InsertRowAfter(2);

        Console.WriteLine("\nAfter insertion:");
        var b1Cell = table.GetCell(Coordinate.Parse("B1"));
        var b2Cell = table.GetCell(Coordinate.Parse("B2"));  // B2 should stay at B2
        var b4Cell = table.GetCell(Coordinate.Parse("B4"));  // B3 moved to B4
        var b5Cell = table.GetCell(Coordinate.Parse("B5"));  // B4 moved to B5

        Console.WriteLine($"B1: {(b1Cell.IsT6 ? b1Cell.AsT6.Value : b1Cell.AsT0)}"); // Should stay same with updated coords
        Console.WriteLine($"B2: {(b2Cell.IsT6 ? b2Cell.AsT6.Value : b2Cell.AsT0)}"); // Should stay at B2 with updated coords
        Console.WriteLine($"B4: {(b4Cell.IsT6 ? b4Cell.AsT6.Value : b4Cell.AsT0)}"); // B3 moved to B4
        Console.WriteLine($"B5: {(b5Cell.IsT6 ? b5Cell.AsT6.Value : b5Cell.AsT0)}"); // B4 moved to B5

        // Assert - Check that coordinates in functions are updated properly

        // B1 should stay the same since it only references A1 and A3, but A3 becomes A4
        if (b1Cell.IsT6)
        {
            b1Cell.AsT6.Value.ShouldBe("SUM(A1,A4)");
        }
        else
        {
            b1Cell.AsT0.ShouldBe("=SUM(A1,A4)");
        }

        // B2 should stay at B2, but A4 becomes A5 (A2 and below insertion point stay same)
        if (b2Cell.IsT6)
        {
            b2Cell.AsT6.Value.ShouldBe("AVG(A2:A5)");
        }
        else
        {
            b2Cell.AsT0.ShouldBe("=AVG(A2:A5)");
        }

        // Original B3 moved to B4, and its range A1:A5 becomes A1:A6 (since A5->A6)
        if (b4Cell.IsT6)
        {
            b4Cell.AsT6.Value.ShouldBe("COUNT(A1:A6)");
        }
        else
        {
            b4Cell.AsT0.ShouldBe("=COUNT(A1:A6)");
        }

        // Original B4 moved to B5, and its references A3,A4,A5 become A4,A5,A6
        if (b5Cell.IsT6)
        {
            b5Cell.AsT6.Value.ShouldBe("SUM(A4,A5,A6)");
        }
        else
        {
            b5Cell.AsT0.ShouldBe("=SUM(A4,A5,A6)");
        }
    }

    [TestMethod]
    public void InsertRowAfter_WithConcatFunction_UpdatesStringReferences()
    {
        // Arrange
        var table = CreateTestTable();

        table.SetCell(Coordinate.Parse("A1"), new DataValue("Hello"));
        table.SetCell(Coordinate.Parse("A2"), new DataValue("World"));
        table.SetCell(Coordinate.Parse("A3"), new DataValue("Test"));

        // CONCAT function with coordinate references
        table.SetCell(Coordinate.Parse("B2"), new DataValue(new Function("CONCAT(A1,\" \",A2)", typeof(string))));
        table.SetCell(Coordinate.Parse("B3"), new DataValue(new Function("CONCAT(A2,\" - \",A3)", typeof(string))));

        Console.WriteLine("Before insertion:");
        Console.WriteLine($"B2: {table.GetCell(Coordinate.Parse("B2")).AsT6.Value}");
        Console.WriteLine($"B3: {table.GetCell(Coordinate.Parse("B3")).AsT6.Value}");

        // Act - Insert row after row 1
        table.InsertRowAfter(1);

        Console.WriteLine("\nAfter insertion:");
        Console.WriteLine($"B3: {table.GetCell(Coordinate.Parse("B3")).AsT6.Value}"); // B2 moved to B3
        Console.WriteLine($"B4: {table.GetCell(Coordinate.Parse("B4")).AsT6.Value}"); // B3 moved to B4

        // Assert - Check CONCAT function coordinate updates

        // Original B2 moved to B3, A1 stays A1, A2 becomes A3
        table.GetCell(Coordinate.Parse("B3")).AsT6.Value.ShouldBe("CONCAT(A1,\" \",A3)");

        // Original B3 moved to B4, A2 becomes A3, A3 becomes A4
        table.GetCell(Coordinate.Parse("B4")).AsT6.Value.ShouldBe("CONCAT(A3,\" - \",A4)");
    }

    [TestMethod]
    public void InsertRowAfter_WithComplexFunctionExpression_UpdatesAllCoordinates()
    {
        // Arrange
        var table = CreateTestTable();

        table.SetCell(Coordinate.Parse("A1"), new DataValue(10));
        table.SetCell(Coordinate.Parse("A2"), new DataValue(20));
        table.SetCell(Coordinate.Parse("A3"), new DataValue(30));
        table.SetCell(Coordinate.Parse("B1"), new DataValue(5));
        table.SetCell(Coordinate.Parse("B2"), new DataValue(15));
        table.SetCell(Coordinate.Parse("B3"), new DataValue(25));

        // Complex function with multiple coordinate references
        table.SetCell(Coordinate.Parse("C3"), new DataValue(new Function("SUM(A1,A2)+AVG(B2:B3)*COUNT(A1:A3)", typeof(decimal))));

        Console.WriteLine("Before insertion:");
        Console.WriteLine($"C3: {table.GetCell(Coordinate.Parse("C3")).AsT6.Value}");

        // Act - Insert row after row 1
        table.InsertRowAfter(1);

        Console.WriteLine("\nAfter insertion:");
        Console.WriteLine($"C4: {table.GetCell(Coordinate.Parse("C4")).AsT6.Value}"); // C3 moved to C4

        // Assert - All coordinates should be updated
        // Original: SUM(A1,A2)+AVG(B2:B3)*COUNT(A1:A3)
        // After inserting after row 1: A2->A3, B2->B3, B3->B4, A2->A3, A3->A4
        // Expected: SUM(A1,A3)+AVG(B3:B4)*COUNT(A1:A4)
        table.GetCell(Coordinate.Parse("C4")).AsT6.Value.ShouldBe("SUM(A1,A3)+AVG(B3:B4)*COUNT(A1:A4)");
    }

    [TestMethod]
    public void InsertRowAfter_WithFunctionReferencingInsertedRow_DoesNotChangeUnaffectedReferences()
    {
        // Arrange
        var table = CreateTestTable();

        table.SetCell(Coordinate.Parse("A1"), new DataValue(10));
        table.SetCell(Coordinate.Parse("A5"), new DataValue(50));

        // Function that references rows both above and below the insertion point
        table.SetCell(Coordinate.Parse("B1"), new DataValue(new Function("SUM(A1,A5)", typeof(decimal))));

        Console.WriteLine("Before insertion:");
        Console.WriteLine($"B1: {table.GetCell(Coordinate.Parse("B1")).AsT6.Value}");

        // Act - Insert row after row 2 (between A1 and A5)
        table.InsertRowAfter(2);

        Console.WriteLine("\nAfter insertion:");
        Console.WriteLine($"B1: {table.GetCell(Coordinate.Parse("B1")).AsT6.Value}");

        // Assert - A1 should stay A1, A5 should become A6
        table.GetCell(Coordinate.Parse("B1")).AsT6.Value.ShouldBe("SUM(A1,A6)");
    }
}
