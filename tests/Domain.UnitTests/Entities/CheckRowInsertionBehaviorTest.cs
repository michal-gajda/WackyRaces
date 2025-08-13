using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using WackyRaces.Domain.Entities;
using WackyRaces.Domain.Types;

namespace WackyRaces.Domain.UnitTests.Entities;

[TestClass]
public sealed class CheckRowInsertionBehaviorTest
{
    public TestContext TestContext { get; set; } = null!;

    [TestMethod]
    public void CheckSpecificScenario_InsertRowAfter3()
    {
        // Arrange - exactly as provided but with proper DataValue wrapping
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "Sample");

        table.SetCell(Coordinate.Parse("A1"), new DataValue(1));
        table.SetCell(Coordinate.Parse("A2"), new DataValue(1));
        table.SetCell(Coordinate.Parse("A3"), new DataValue(1));
        table.SetCell(Coordinate.Parse("B3"), new DataValue(new Function("SUM(A1:A3)", typeof(int))));

        TestContext.WriteLine("Before insertion:");
        TestContext.WriteLine($"A1: {table.GetCell(Coordinate.Parse("A1"))}");
        TestContext.WriteLine($"A2: {table.GetCell(Coordinate.Parse("A2"))}");
        TestContext.WriteLine($"A3: {table.GetCell(Coordinate.Parse("A3"))}");
        TestContext.WriteLine($"B3: {table.GetCell(Coordinate.Parse("B3"))}");

        // Check the function value before insertion
        var functionValueBefore = table.GetCell(Coordinate.Parse("B3"));
        TestContext.WriteLine($"B3 Function before: {(functionValueBefore.IsT6 ? functionValueBefore.AsT6.Value : "Not a function")}");

        // Act
        table.InsertRowAfter(3);

        TestContext.WriteLine("\nAfter insertion (InsertRowAfter(3) inserts new row at position 4):");
        TestContext.WriteLine($"A1: {table.GetCell(Coordinate.Parse("A1"))}");
        TestContext.WriteLine($"A2: {table.GetCell(Coordinate.Parse("A2"))}");
        TestContext.WriteLine($"A3: {table.GetCell(Coordinate.Parse("A3"))}");
        TestContext.WriteLine($"A4: {table.GetCell(Coordinate.Parse("A4"))}"); // Should be empty (new row)
        TestContext.WriteLine($"B3: {table.GetCell(Coordinate.Parse("B3"))}"); // Should stay with function
        TestContext.WriteLine($"B4: {table.GetCell(Coordinate.Parse("B4"))}"); // Should be empty (new row)

        // Check the function value after insertion
        var functionValueAfter = table.GetCell(Coordinate.Parse("B3"));
        TestContext.WriteLine($"B3 Function after: {(functionValueAfter.IsT6 ? functionValueAfter.AsT6.Value : "Not a function")}");

        // Act - Get the value as requested (this will be an empty cell)
        var value = table.GetValue(Coordinate.Parse("B4"));
        TestContext.WriteLine($"Evaluated value at B4: {value}");

        // Assert - Verify the actual behavior
        // B3 should still have the function (it doesn't move when inserting after row 3)
        table.GetCell(Coordinate.Parse("B3")).IsT6.ShouldBeTrue();
        table.GetCell(Coordinate.Parse("B3")).AsT6.Value.ShouldBe("SUM(A1:A3)");

        // B4 should be empty (new inserted row)
        table.GetCell(Coordinate.Parse("B4")).AsT0.ShouldBe(string.Empty);

        // The evaluated value at B4 should be empty string
        value.IsT0.ShouldBeTrue();
        value.AsT0.ShouldBe(string.Empty);

        // The function at B3 should still evaluate to 3
        var b3Value = table.GetValue(Coordinate.Parse("B3"));
        b3Value.IsT1.ShouldBeTrue();
        b3Value.AsT1.ShouldBe(3);
    }
}
