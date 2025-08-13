using Shouldly;
using WackyRaces.Domain.Entities;
using WackyRaces.Domain.Types;

namespace WackyRaces.Domain.UnitTests.Entities;

[TestClass]
public class UserExampleTest
{
    [TestMethod]
    public void UserExample_InsertRowAfter2_ShouldWork()
    {
        // User's updated example
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "Sample");
        
        table.SetCell(Coordinate.Parse("A1"), new DataValue(1)); // ✅ Fixed: wrapped in DataValue
        table.SetCell(Coordinate.Parse("A2"), new DataValue(1)); // ✅ Fixed: wrapped in DataValue
        table.SetCell(Coordinate.Parse("A3"), new DataValue(1)); // ✅ Fixed: wrapped in DataValue
        table.SetCell(Coordinate.Parse("B3"), new DataValue(new Function("SUM(A1:A3)", typeof(int))));   
        
        Console.WriteLine("Before InsertRowAfter(2):");
        Console.WriteLine($"A1: {table.GetCell(Coordinate.Parse("A1"))}");
        Console.WriteLine($"A2: {table.GetCell(Coordinate.Parse("A2"))}");
        Console.WriteLine($"A3: {table.GetCell(Coordinate.Parse("A3"))}");
        Console.WriteLine($"B3: {table.GetCell(Coordinate.Parse("B3"))}");
        Console.WriteLine($"B3 evaluated: {table.GetValue(Coordinate.Parse("B3"))}");
        
        table.InsertRowAfter(2); // Insert new row at position 3, moving A3->A4, B3->B4

        Console.WriteLine("\nAfter InsertRowAfter(2):");
        Console.WriteLine($"A1: {table.GetCell(Coordinate.Parse("A1"))}");
        Console.WriteLine($"A2: {table.GetCell(Coordinate.Parse("A2"))}");
        Console.WriteLine($"A3: {table.GetCell(Coordinate.Parse("A3"))} (new empty row)");
        Console.WriteLine($"A4: {table.GetCell(Coordinate.Parse("A4"))} (moved from A3)");
        Console.WriteLine($"B3: {table.GetCell(Coordinate.Parse("B3"))} (should be empty)");
        Console.WriteLine($"B4: {table.GetCell(Coordinate.Parse("B4"))} (moved from B3)");

        var value = table.GetValue(Coordinate.Parse("B4"));
        Console.WriteLine($"B4 evaluated: {value}");

        // Verify the behavior
        // A1 and A2 should stay in place (row <= 2)
        table.GetCell(Coordinate.Parse("A1")).AsT1.ShouldBe(1);
        table.GetCell(Coordinate.Parse("A2")).AsT1.ShouldBe(1);
        
        // A3 should be empty (new inserted row)
        table.GetCell(Coordinate.Parse("A3")).AsT0.ShouldBe(string.Empty);
        
        // A4 should have the old A3 value (moved)
        table.GetCell(Coordinate.Parse("A4")).AsT1.ShouldBe(1);
        
        // B3 should be empty (new inserted row)
        table.GetCell(Coordinate.Parse("B3")).AsT0.ShouldBe(string.Empty);
        
        // B4 should have the function (moved from B3)
        table.GetCell(Coordinate.Parse("B4")).IsT6.ShouldBeTrue();
        var functionAtB4 = table.GetCell(Coordinate.Parse("B4")).AsT6;
        functionAtB4.Value.ShouldBe("SUM(A1:A4)"); // Should be updated to include A4
        
        // The evaluated value at B4 should be 3 (1+1+0+1 = 3, where A3 is empty=0)
        value.IsT1.ShouldBeTrue();
        value.AsT1.ShouldBe(3);
    }
}
