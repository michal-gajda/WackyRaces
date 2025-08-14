using Microsoft.VisualStudio.TestTools.UnitTesting;
using WackyRaces.Domain.Entities;
using WackyRaces.Domain.Types;

namespace WackyRaces.Domain.UnitTests.Entities;

[TestClass]
public sealed class DebugRowInsertionTest
{
    [TestMethod]
    public void Debug_InsertRowAfter_TraceExecution()
    {
        // Arrange
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "Debug Table");

        table.SetCell(Coordinate.Parse("A1"), new DataValue(10));
        table.SetCell(Coordinate.Parse("A2"), new DataValue(20));
        table.SetCell(Coordinate.Parse("A3"), new DataValue(30));
        table.SetCell(Coordinate.Parse("B3"), new DataValue("=SUM(A1:A3)"));

        Console.WriteLine("Before insertion:");
        Console.WriteLine($"B3: {table.GetCell(Coordinate.Parse("B3")).AsT0}");

        // Act
        table.InsertRowAfter(2); // Insert after row 2

        // Debug output
        Console.WriteLine("After insertion:");
        Console.WriteLine($"B3: {table.GetCell(Coordinate.Parse("B3")).AsT0}");
        Console.WriteLine($"B4: {table.GetCell(Coordinate.Parse("B4")).AsT0}");

        // The expected behavior:
        // - Original B3 (=SUM(A1:A3)) should move to B4
        // - A3 in the formula should become A4 since row 3 data moved to row 4
        // - So we expect B4 to contain "=SUM(A1:A4)"

        Assert.AreEqual("=SUM(A1:A4)", table.GetCell(Coordinate.Parse("B4")).AsT0);
    }
}
