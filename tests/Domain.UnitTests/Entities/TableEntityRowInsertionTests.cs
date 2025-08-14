namespace WackyRaces.Domain.UnitTests.Entities;

using WackyRaces.Domain.Entities;
using WackyRaces.Domain.Types;

[TestClass]
public sealed class TableEntityRowInsertionTests
{
    private TableEntity CreateTestTable()
    {
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "Test Table");
        return table;
    }

    [TestMethod]
    public void InsertRowAfter_ValidRowNumber_InsertsRowAndMovesOthersDown()
    {
        // Arrange
        var table = CreateTestTable();

        // Set up initial data
        table.SetCell(Coordinate.Parse("A1"), new DataValue("Header"));
        table.SetCell(Coordinate.Parse("A2"), new DataValue("Row 1"));
        table.SetCell(Coordinate.Parse("A3"), new DataValue("Row 2"));
        table.SetCell(Coordinate.Parse("A4"), new DataValue("Row 3"));
        table.SetCell(Coordinate.Parse("B2"), new DataValue(100));
        table.SetCell(Coordinate.Parse("B3"), new DataValue(200));
        table.SetCell(Coordinate.Parse("B4"), new DataValue(300));

        // Act
        table.InsertRowAfter(2); // Insert after row 2

        // Assert
        table.GetCell(Coordinate.Parse("A1")).AsT0.ShouldBe("Header"); // Header unchanged
        table.GetCell(Coordinate.Parse("A2")).AsT0.ShouldBe("Row 1"); // Row 1 unchanged
        table.GetCell(Coordinate.Parse("A3")).AsT0.ShouldBe(string.Empty); // New empty row inserted
        table.GetCell(Coordinate.Parse("A4")).AsT0.ShouldBe("Row 2"); // Row 2 moved down
        table.GetCell(Coordinate.Parse("A5")).AsT0.ShouldBe("Row 3"); // Row 3 moved down

        table.GetCell(Coordinate.Parse("B2")).AsT1.ShouldBe(100); // B2 unchanged
        table.GetCell(Coordinate.Parse("B3")).AsT0.ShouldBe(string.Empty); // New empty row (default is string)
        table.GetCell(Coordinate.Parse("B4")).AsT1.ShouldBe(200); // B3 data moved to B4
        table.GetCell(Coordinate.Parse("B5")).AsT1.ShouldBe(300); // B4 data moved to B5
    }

    [TestMethod]
    public void InsertRowAfter_AtEnd_InsertsRowWithoutMovingOthers()
    {
        // Arrange
        var table = CreateTestTable();

        table.SetCell(Coordinate.Parse("A1"), new DataValue("Row 1"));
        table.SetCell(Coordinate.Parse("A2"), new DataValue("Row 2"));

        // Act
        table.InsertRowAfter(2); // Insert after last row

        // Assert
        table.GetCell(Coordinate.Parse("A1")).AsT0.ShouldBe("Row 1");
        table.GetCell(Coordinate.Parse("A2")).AsT0.ShouldBe("Row 2");
        table.GetCell(Coordinate.Parse("A3")).AsT0.ShouldBe(string.Empty); // New empty row
    }

    [TestMethod]
    public void InsertRowAfter_WithFormulas_UpdatesReferences()
    {
        // Arrange
        var table = CreateTestTable();

        table.SetCell(Coordinate.Parse("A1"), new DataValue(10));
        table.SetCell(Coordinate.Parse("A2"), new DataValue(20));
        table.SetCell(Coordinate.Parse("A3"), new DataValue(30));
        table.SetCell(Coordinate.Parse("B3"), new DataValue("=SUM(A1:A3)")); // Formula referencing A1:A3
        table.SetCell(Coordinate.Parse("B4"), new DataValue("=A3*2")); // Formula referencing A3

        // Act
        table.InsertRowAfter(2); // Insert after row 2

        // Assert - formulas should be updated
        // Original B3 moves to B4, A3 in the range becomes A4 (since A3 shifts down)
        table.GetCell(Coordinate.Parse("B4")).AsT0.ShouldBe("=SUM(A1:A4)"); // B3 moved to B4, A3 range end becomes A4
        table.GetCell(Coordinate.Parse("B5")).AsT0.ShouldBe("=A4*2"); // B4 moved to B5, A3 becomes A4
    }

    [TestMethod]
    public void InsertRowAfter_WithComplexFormulas_UpdatesMultipleReferences()
    {
        // Arrange
        var table = CreateTestTable();

        table.SetCell(Coordinate.Parse("A1"), new DataValue(10));
        table.SetCell(Coordinate.Parse("A2"), new DataValue(20));
        table.SetCell(Coordinate.Parse("A3"), new DataValue(30));
        table.SetCell(Coordinate.Parse("A4"), new DataValue(40));
        table.SetCell(Coordinate.Parse("B5"), new DataValue("=SUM(A2,A4)+A3*2")); // Multiple references

        // Act
        table.InsertRowAfter(1); // Insert after row 1

        // Assert - formula should update A2->A3, A4->A5, A3->A4
        table.GetCell(Coordinate.Parse("B6")).AsT0.ShouldBe("=SUM(A3,A5)+A4*2");
    }

    [TestMethod]
    public void InsertRowAfter_WithReferencesToUnaffectedRows_DoesNotUpdateThoseReferences()
    {
        // Arrange
        var table = CreateTestTable();

        table.SetCell(Coordinate.Parse("A1"), new DataValue(10));
        table.SetCell(Coordinate.Parse("A2"), new DataValue(20));
        table.SetCell(Coordinate.Parse("A5"), new DataValue(50));
        table.SetCell(Coordinate.Parse("B1"), new DataValue("=A1*2")); // Reference to A1 (should not change)
        table.SetCell(Coordinate.Parse("B6"), new DataValue("=A5+10")); // Reference to A5 (should change)

        // Act
        table.InsertRowAfter(3); // Insert after row 3

        // Assert
        table.GetCell(Coordinate.Parse("B1")).AsT0.ShouldBe("=A1*2"); // A1 reference unchanged
        table.GetCell(Coordinate.Parse("B7")).AsT0.ShouldBe("=A6+10"); // A5 became A6, B6 became B7
    }

    [TestMethod]
    public void InsertRowAfter_NonFormulaValues_RemainsUnchanged()
    {
        // Arrange
        var table = CreateTestTable();

        table.SetCell(Coordinate.Parse("A1"), new DataValue("Text Value"));
        table.SetCell(Coordinate.Parse("A2"), new DataValue(42));
        table.SetCell(Coordinate.Parse("A3"), new DataValue(3.14m));

        // Act
        table.InsertRowAfter(1);

        // Assert - non-formula values should remain the same type and value
        table.GetCell(Coordinate.Parse("A1")).AsT0.ShouldBe("Text Value");
        table.GetCell(Coordinate.Parse("A3")).AsT1.ShouldBe(42); // A2 moved to A3
        table.GetCell(Coordinate.Parse("A4")).AsT2.ShouldBe(3.14m); // A3 moved to A4
    }

    [TestMethod]
    public void InsertRowAfter_InvalidRowNumber_ThrowsException()
    {
        // Arrange
        var table = CreateTestTable();

        // Act & Assert
        Should.Throw<ArgumentOutOfRangeException>(() => table.InsertRowAfter(0)); // Should throw ArgumentOutOfRangeException
    }

    [TestMethod]
    public void InsertRowAfter_NegativeRowNumber_ThrowsException()
    {
        // Arrange
        var table = CreateTestTable();

        // Act & Assert
        Should.Throw<ArgumentOutOfRangeException>(() => table.InsertRowAfter(-1)); // Should throw ArgumentOutOfRangeException
    }

    [TestMethod]
    public void InsertRowAfter_EmptyTable_CanInsertAfterRow1()
    {
        // Arrange
        var table = CreateTestTable();

        // Act
        table.InsertRowAfter(1); // Insert after row 1 in empty table

        // Assert - Should not throw, table remains empty
        table.GetCell(Coordinate.Parse("A2")).AsT0.ShouldBe(string.Empty);
    }

    [TestMethod]
    public void InsertRowAfter_MultipleInsertions_WorksCorrectly()
    {
        // Arrange
        var table = CreateTestTable();

        table.SetCell(Coordinate.Parse("A1"), new DataValue("Row 1"));
        table.SetCell(Coordinate.Parse("A2"), new DataValue("Row 2"));
        table.SetCell(Coordinate.Parse("A3"), new DataValue("Row 3"));

        // Act
        table.InsertRowAfter(1); // Insert after row 1
        table.InsertRowAfter(2); // Insert after row 2 (which is now empty)

        // Assert
        table.GetCell(Coordinate.Parse("A1")).AsT0.ShouldBe("Row 1");
        table.GetCell(Coordinate.Parse("A2")).AsT0.ShouldBe(string.Empty); // First insertion
        table.GetCell(Coordinate.Parse("A3")).AsT0.ShouldBe(string.Empty); // Second insertion
        table.GetCell(Coordinate.Parse("A4")).AsT0.ShouldBe("Row 2"); // Original row 2
        table.GetCell(Coordinate.Parse("A5")).AsT0.ShouldBe("Row 3"); // Original row 3
    }
}
