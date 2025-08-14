namespace WackyRaces.Domain.UnitTests.Templates;

using WackyRaces.Domain.Entities;
using WackyRaces.Domain.Templates;
using WackyRaces.Domain.Types;

[TestClass]
public sealed class SimpleJsonTemplateLoaderTests
{
    [TestMethod]
    public void LoadFromJson_WithStringFunction_ShouldCreateTextReturnType()
    {
        // Arrange
        var jsonTemplate = """
        {
          "Name": "Text Function Test",
          "Columns": [],
          "Rows": [],
          "Cells": [
            { "Coordinate": "A1", "DataValue": { "Value": "=CONCAT(B1,C1)", "Type": "text" } }
          ]
        }
        """;

        // Act
        var table = SimpleJsonTemplateLoader.LoadFromJson(jsonTemplate);

        // Assert
        var cell = table.GetCell(Coordinate.Parse("A1"));
        cell.IsT6.ShouldBeTrue(); // Should be Function
        cell.AsT6.Value.ShouldBe("CONCAT(B1,C1)");
        cell.AsT6.Format.ShouldBe(typeof(string));
    }

    [TestMethod]
    public void LoadFromJson_WithIntegerFunction_ShouldCreateIntReturnType()
    {
        // Arrange
        var jsonTemplate = """
        {
          "Name": "Integer Function Test",
          "Columns": [],
          "Rows": [],
          "Cells": [
            { "Coordinate": "A1", "DataValue": { "Value": "=COUNT(B1:B10)", "Type": "int" } }
          ]
        }
        """;

        // Act
        var table = SimpleJsonTemplateLoader.LoadFromJson(jsonTemplate);

        // Assert
        var cell = table.GetCell(Coordinate.Parse("A1"));
        cell.IsT6.ShouldBeTrue(); // Should be Function
        cell.AsT6.Value.ShouldBe("COUNT(B1:B10)");
        cell.AsT6.Format.ShouldBe(typeof(int));
    }

    [TestMethod]
    public void LoadFromJson_WithPercentageFunction_ShouldCreateDecimalReturnType()
    {
        // Arrange
        var jsonTemplate = """
        {
          "Name": "Percentage Function Test",
          "Columns": [],
          "Rows": [],
          "Cells": [
            { "Coordinate": "A1", "DataValue": { "Value": "=SUM(B1:B10)", "Type": "percentage" } }
          ]
        }
        """;

        // Act
        var table = SimpleJsonTemplateLoader.LoadFromJson(jsonTemplate);

        // Assert
        var cell = table.GetCell(Coordinate.Parse("A1"));
        cell.IsT6.ShouldBeTrue(); // Should be Function
        cell.AsT6.Value.ShouldBe("SUM(B1:B10)");
        cell.AsT6.Format.ShouldBe(typeof(decimal)); // Percentage maps to decimal for function format
    }

    [TestMethod]
    public void LoadFromJson_WithUnknownType_ShouldDefaultToDecimal()
    {
        // Arrange
        var jsonTemplate = """
        {
          "Name": "Unknown Type Test",
          "Columns": [],
          "Rows": [],
          "Cells": [
            { "Coordinate": "A1", "DataValue": { "Value": "=SUM(B1:B10)", "Type": "unknown" } }
          ]
        }
        """;

        // Act
        var table = SimpleJsonTemplateLoader.LoadFromJson(jsonTemplate);

        // Assert
        var cell = table.GetCell(Coordinate.Parse("A1"));
        cell.IsT6.ShouldBeTrue(); // Should be Function
        cell.AsT6.Value.ShouldBe("SUM(B1:B10)");
        cell.AsT6.Format.ShouldBe(typeof(decimal)); // Default format
    }

    [TestMethod]
    public void SaveToJson_WithStringFunction_ShouldSerializeAsText()
    {
        // Arrange
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "String Function Test");
        var stringFunction = new Function("CONCAT(A1,B1)", typeof(string));
        table.SetCell(Coordinate.Parse("C2"), new DataValue(stringFunction)); // Use C2 instead of C1

        // Act
        var json = SimpleJsonTemplateLoader.SaveToJson(table);

        // Assert
        json.ShouldContain("CONCAT(A1,B1)");
        json.ShouldContain("\"Type\": \"text\"");
        json.ShouldContain("\"Value\": \"=CONCAT(A1,B1)\""); // Should include = prefix
    }

    [TestMethod]
    public void SaveToJson_WithIntFunction_ShouldSerializeAsInt()
    {
        // Arrange
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "Int Function Test");
        var intFunction = new Function("COUNT(A1:A10)", typeof(int));
        table.SetCell(Coordinate.Parse("B2"), new DataValue(intFunction)); // Use B2 instead of B1

        // Act
        var json = SimpleJsonTemplateLoader.SaveToJson(table);

        // Assert
        json.ShouldContain("COUNT(A1:A10)");
        json.ShouldContain("\"Type\": \"int\"");
        json.ShouldContain("\"Value\": \"=COUNT(A1:A10)\""); // Should include = prefix
    }

    [TestMethod]
    public void LoadFromJson_EmptyTemplate_ShouldCreateEmptyTable()
    {
        // Arrange
        var jsonTemplate = """
        {
          "Name": "Empty Template",
          "Columns": [],
          "Rows": [],
          "Cells": []
        }
        """;

        // Act
        var table = SimpleJsonTemplateLoader.LoadFromJson(jsonTemplate);

        // Assert
        table.ShouldNotBeNull();
        table.Name.ShouldBe("Empty Template");
        table.Cells.ShouldBeEmpty();
    }

    [TestMethod]
    public void LoadFromJson_OnlyColumnsTemplate_ShouldCreateTableWithColumns()
    {
        // Arrange
        var jsonTemplate = """
        {
          "Name": "Columns Only",
          "Columns": [
            { "Coordinate": "A1", "Title": "Col1" },
            { "Coordinate": "B1", "Title": "Col2" },
            { "Coordinate": "C1", "Title": "Col3" }
          ],
          "Rows": [],
          "Cells": []
        }
        """;

        // Act
        var table = SimpleJsonTemplateLoader.LoadFromJson(jsonTemplate);

        // Assert
        table.ShouldNotBeNull();
        table.Name.ShouldBe("Columns Only");
        table.GetCell(Coordinate.Parse("A1")).AsT0.ShouldBe("Col1");
        table.GetCell(Coordinate.Parse("B1")).AsT0.ShouldBe("Col2");
        table.GetCell(Coordinate.Parse("C1")).AsT0.ShouldBe("Col3");
    }

    [TestMethod]
    public void LoadFromJson_OnlyRowsTemplate_ShouldCreateTableWithRows()
    {
        // Arrange
        var jsonTemplate = """
        {
          "Name": "Rows Only",
          "Columns": [],
          "Rows": [
            { "Coordinate": "A1", "Title": "Row1" },
            { "Coordinate": "A2", "Title": "Row2" },
            { "Coordinate": "A3", "Title": "Row3" }
          ],
          "Cells": []
        }
        """;

        // Act
        var table = SimpleJsonTemplateLoader.LoadFromJson(jsonTemplate);

        // Assert
        table.ShouldNotBeNull();
        table.Name.ShouldBe("Rows Only");
        table.GetCell(Coordinate.Parse("A1")).AsT0.ShouldBe("Row1");
        table.GetCell(Coordinate.Parse("A2")).AsT0.ShouldBe("Row2");
        table.GetCell(Coordinate.Parse("A3")).AsT0.ShouldBe("Row3");
    }

    [TestMethod]
    public void RoundTrip_ComplexTemplate_ShouldPreserveAllData()
    {
        // Arrange
        var tableId = new TableId(Guid.NewGuid());
        var originalTable = new TableEntity(tableId, "Complex Test");

        // Column headers
        originalTable.SetCell(Coordinate.Parse("A1"), new DataValue("Item"));
        originalTable.SetCell(Coordinate.Parse("B1"), new DataValue("Price"));
        originalTable.SetCell(Coordinate.Parse("C1"), new DataValue("Quantity"));
        originalTable.SetCell(Coordinate.Parse("D1"), new DataValue("Total"));

        // Row headers
        originalTable.SetCell(Coordinate.Parse("A2"), new DataValue("Widget A"));
        originalTable.SetCell(Coordinate.Parse("A3"), new DataValue("Widget B"));
        originalTable.SetCell(Coordinate.Parse("A4"), new DataValue("TOTALS"));

        // Data values
        originalTable.SetCell(Coordinate.Parse("B2"), new DataValue(10.50m));
        originalTable.SetCell(Coordinate.Parse("C2"), new DataValue(5));
        originalTable.SetCell(Coordinate.Parse("B3"), new DataValue(25.00m));
        originalTable.SetCell(Coordinate.Parse("C3"), new DataValue(3));

        // Functions with different return types
        originalTable.SetCell(Coordinate.Parse("D2"), new DataValue(new Function("SUM(B2,C2)", typeof(decimal))));
        originalTable.SetCell(Coordinate.Parse("D3"), new DataValue(new Function("SUM(B3,C3)", typeof(decimal))));
        originalTable.SetCell(Coordinate.Parse("B4"), new DataValue(new Function("SUM(B2:B3)", typeof(decimal))));
        originalTable.SetCell(Coordinate.Parse("C4"), new DataValue(new Function("SUM(C2:C3)", typeof(int))));
        originalTable.SetCell(Coordinate.Parse("D4"), new DataValue(new Function("SUM(D2:D3)", typeof(decimal))));

        // Percentage and text functions
        originalTable.SetCell(Coordinate.Parse("E1"), new DataValue("Status"));
        originalTable.SetCell(Coordinate.Parse("E2"), new DataValue(new Function("CONCAT(A2,\" - Complete\")", typeof(string))));
        originalTable.SetCell(Coordinate.Parse("E4"), new DataValue(new Percentage(100m)));

        // Act
        var json = SimpleJsonTemplateLoader.SaveToJson(originalTable);
        var loadedTable = SimpleJsonTemplateLoader.LoadFromJson(json);

        // Assert
        loadedTable.Name.ShouldBe(originalTable.Name);

        // Check headers
        loadedTable.GetCell(Coordinate.Parse("A1")).AsT0.ShouldBe("Item");
        loadedTable.GetCell(Coordinate.Parse("B1")).AsT0.ShouldBe("Price");
        loadedTable.GetCell(Coordinate.Parse("C1")).AsT0.ShouldBe("Quantity");
        loadedTable.GetCell(Coordinate.Parse("D1")).AsT0.ShouldBe("Total");
        loadedTable.GetCell(Coordinate.Parse("E1")).AsT0.ShouldBe("Status");

        // Check data values
        loadedTable.GetCell(Coordinate.Parse("B2")).AsT2.ShouldBe(10.50m);
        loadedTable.GetCell(Coordinate.Parse("C2")).AsT1.ShouldBe(5);
        loadedTable.GetCell(Coordinate.Parse("B3")).AsT2.ShouldBe(25.00m);
        loadedTable.GetCell(Coordinate.Parse("C3")).AsT1.ShouldBe(3);

        // Check functions with correct return types
        var d2Cell = loadedTable.GetCell(Coordinate.Parse("D2"));
        d2Cell.IsT6.ShouldBeTrue();
        d2Cell.AsT6.Value.ShouldBe("SUM(B2,C2)");
        d2Cell.AsT6.Format.ShouldBe(typeof(decimal));

        var c4Cell = loadedTable.GetCell(Coordinate.Parse("C4"));
        c4Cell.IsT6.ShouldBeTrue();
        c4Cell.AsT6.Value.ShouldBe("SUM(C2:C3)");
        c4Cell.AsT6.Format.ShouldBe(typeof(int));

        var e2Cell = loadedTable.GetCell(Coordinate.Parse("E2"));
        e2Cell.IsT6.ShouldBeTrue();
        e2Cell.AsT6.Value.ShouldBe("CONCAT(A2,\" - Complete\")");
        e2Cell.AsT6.Format.ShouldBe(typeof(string));

        // Check percentage
        var e4Cell = loadedTable.GetCell(Coordinate.Parse("E4"));
        e4Cell.IsT5.ShouldBeTrue();
        e4Cell.AsT5.Value.ShouldBe(100m);
    }

    [TestMethod]
    public void LoadFromJson_InvalidJson_ShouldThrowException()
    {
        // Arrange
        var invalidJson = "{ invalid json }";

        // Act & Assert
        Should.Throw<Exception>(() => SimpleJsonTemplateLoader.LoadFromJson(invalidJson));
    }

    [TestMethod]
    public void LoadFromJson_MissingName_ShouldThrowException()
    {
        // Arrange
        var jsonTemplate = """
        {
          "Columns": [],
          "Rows": [],
          "Cells": []
        }
        """;

        // Act & Assert
        Should.Throw<Exception>(() => SimpleJsonTemplateLoader.LoadFromJson(jsonTemplate));
    }

    [TestMethod]
    public void SaveToJsonFile_ShouldCreateFileSuccessfully()
    {
        // Arrange
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "File Test");
        table.SetCell(Coordinate.Parse("A1"), new DataValue("Test Value"));

        var tempFilePath = Path.GetTempFileName();

        try
        {
            // Act
            SimpleJsonTemplateLoader.SaveToJsonFile(table, tempFilePath);

            // Assert
            File.Exists(tempFilePath).ShouldBeTrue();
            var content = File.ReadAllText(tempFilePath);
            content.ShouldContain("File Test");
            content.ShouldContain("Test Value");
        }
        finally
        {
            // Cleanup
            if (File.Exists(tempFilePath))
                File.Delete(tempFilePath);
        }
    }

    [TestMethod]
    public void LoadFromJsonFile_NonExistentFile_ShouldThrowException()
    {
        // Arrange
        var nonExistentPath = Path.Combine(Path.GetTempPath(), "nonexistent.json");

        // Act & Assert
        Should.Throw<FileNotFoundException>(() => SimpleJsonTemplateLoader.LoadFromJsonFile(nonExistentPath));
    }
}
