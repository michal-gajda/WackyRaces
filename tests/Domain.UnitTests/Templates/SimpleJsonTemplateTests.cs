namespace WackyRaces.Domain.UnitTests.Templates;

using WackyRaces.Domain.Entities;
using WackyRaces.Domain.Templates;
using WackyRaces.Domain.Types;

[TestClass]
public sealed class SimpleJsonTemplateTests
{
    [TestMethod]
    public void LoadFromJson_SimpleTemplate_ShouldCreateCorrectTable()
    {
        // Arrange
        var jsonTemplate = """
        {
          "Name": "Test Template",
          "Description": "A test template",
          "Columns": [
            { "Coordinate": "A1", "Title": "Category" },
            { "Coordinate": "B1", "Title": "Amount" }
          ],
          "Rows": [
            { "Coordinate": "A2", "Title": "Income" },
            { "Coordinate": "A3", "Title": "Expenses" }
          ],
          "Cells": [
            { "Coordinate": "B4", "DataValue": { "Value": "=SUM(B2:B3)", "Type": "function" } }
          ]
        }
        """;

        // Act
        var table = SimpleJsonTemplateLoader.LoadFromJson(jsonTemplate);

        // Assert
        table.ShouldNotBeNull();
        table.Name.ShouldBe("Test Template");

        // Check column headers
        var categoryHeader = table.GetCell(new Coordinate(new RowId(1), new ColumnId(1)));
        categoryHeader.AsT0.ShouldBe("Category");

        var amountHeader = table.GetCell(new Coordinate(new RowId(1), new ColumnId(2)));
        amountHeader.AsT0.ShouldBe("Amount");

        // Check row headers
        var incomeRow = table.GetCell(new Coordinate(new RowId(2), new ColumnId(1)));
        incomeRow.AsT0.ShouldBe("Income");

        var expensesRow = table.GetCell(new Coordinate(new RowId(3), new ColumnId(1)));
        expensesRow.AsT0.ShouldBe("Expenses");

        // Check formula cell
        var formulaCell = table.GetCell(new Coordinate(new RowId(4), new ColumnId(2)));
        formulaCell.IsT6.ShouldBeTrue(); // Should be Function
        formulaCell.AsT6.Value.ShouldBe("SUM(B2:B3)"); // Function stores without =
    }

    [TestMethod]
    public void LoadFromJson_WithDifferentDataTypes_ShouldCreateCorrectValues()
    {
        // Arrange
        var jsonTemplate = """
        {
          "Name": "Data Types Test",
          "Columns": [],
          "Rows": [],
          "Cells": [
            { "Coordinate": "A1", "DataValue": { "Value": "Text Value", "Type": "text" } },
            { "Coordinate": "A2", "DataValue": { "Value": "42", "Type": "int" } },
            { "Coordinate": "A3", "DataValue": { "Value": "3.14", "Type": "decimal" } },
            { "Coordinate": "A4", "DataValue": { "Value": "0.25", "Type": "percentage" } },
            { "Coordinate": "A5", "DataValue": { "Value": "=SUM(A2,A3)", "Type": "function" } }
          ]
        }
        """;

        // Act
        var table = SimpleJsonTemplateLoader.LoadFromJson(jsonTemplate);

        // Assert
        // Text value
        var textCell = table.GetCell(new Coordinate(new RowId(1), new ColumnId(1)));
        Assert.IsTrue(textCell.IsT0);
        Assert.AreEqual("Text Value", textCell.AsT0);

        // Integer value
        var intCell = table.GetCell(new Coordinate(new RowId(2), new ColumnId(1)));
        Assert.IsTrue(intCell.IsT1);
        Assert.AreEqual(42, intCell.AsT1);

        // Decimal value
        var decimalCell = table.GetCell(new Coordinate(new RowId(3), new ColumnId(1)));
        Assert.IsTrue(decimalCell.IsT2);
        Assert.AreEqual(3.14m, decimalCell.AsT2);

        // Percentage value
        var percentageCell = table.GetCell(new Coordinate(new RowId(4), new ColumnId(1)));
        Assert.IsTrue(percentageCell.IsT5);
        Assert.AreEqual(0.25m, percentageCell.AsT5.Value);

        // Function value
        var functionCell = table.GetCell(new Coordinate(new RowId(5), new ColumnId(1)));
        Assert.IsTrue(functionCell.IsT6);
        Assert.AreEqual("SUM(A2,A3)", functionCell.AsT6.Value); // Function stores without =
    }

    [TestMethod]
    public void SaveToJson_TableWithMixedContent_ShouldCreateValidJson()
    {
        // Arrange
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "Test Table");

        // Add column headers (row 1)
        table.SetCell(new Coordinate(new RowId(1), new ColumnId(1)), new DataValue("Category"));
        table.SetCell(new Coordinate(new RowId(1), new ColumnId(2)), new DataValue("Amount"));

        // Add row headers (column A)
        table.SetCell(new Coordinate(new RowId(2), new ColumnId(1)), new DataValue("Income"));
        table.SetCell(new Coordinate(new RowId(3), new ColumnId(1)), new DataValue("Expenses"));

        // Add data cells
        table.SetCell(new Coordinate(new RowId(2), new ColumnId(2)), new DataValue(1000m));
        table.SetCell(new Coordinate(new RowId(3), new ColumnId(2)), new DataValue(new Function("SUM(B2)")));

        // Act
        var json = SimpleJsonTemplateLoader.SaveToJson(table);

        // Assert
        Assert.IsNotNull(json);
        Assert.IsTrue(json.Contains("Test Table"));
        Assert.IsTrue(json.Contains("Category"));
        Assert.IsTrue(json.Contains("Amount"));
        Assert.IsTrue(json.Contains("Income"));
        Assert.IsTrue(json.Contains("Expenses"));
        Assert.IsTrue(json.Contains("SUM(B2)"));
    }

    [TestMethod]
    public void RoundTrip_SaveAndLoad_ShouldPreserveStructure()
    {
        // Arrange
        var tableId = new TableId(Guid.NewGuid());
        var originalTable = new TableEntity(tableId, "Round Trip Test");

        // Set up table structure
        originalTable.SetCell(new Coordinate(new RowId(1), new ColumnId(1)), new DataValue("Header1"));
        originalTable.SetCell(new Coordinate(new RowId(1), new ColumnId(2)), new DataValue("Header2"));
        originalTable.SetCell(new Coordinate(new RowId(2), new ColumnId(1)), new DataValue("Row1"));
        originalTable.SetCell(new Coordinate(new RowId(3), new ColumnId(1)), new DataValue("Row2"));
        originalTable.SetCell(new Coordinate(new RowId(2), new ColumnId(2)), new DataValue(100m));
        originalTable.SetCell(new Coordinate(new RowId(3), new ColumnId(2)), new DataValue(new Function("SUM(B2)")));

        // Act
        var json = SimpleJsonTemplateLoader.SaveToJson(originalTable);
        var loadedTable = SimpleJsonTemplateLoader.LoadFromJson(json);

        // Assert
        Assert.AreEqual(originalTable.Name, loadedTable.Name);

        // Check headers
        Assert.AreEqual("Header1", loadedTable.GetCell(new Coordinate(new RowId(1), new ColumnId(1))).AsT0);
        Assert.AreEqual("Header2", loadedTable.GetCell(new Coordinate(new RowId(1), new ColumnId(2))).AsT0);
        Assert.AreEqual("Row1", loadedTable.GetCell(new Coordinate(new RowId(2), new ColumnId(1))).AsT0);
        Assert.AreEqual("Row2", loadedTable.GetCell(new Coordinate(new RowId(3), new ColumnId(1))).AsT0);

        // Check data
        Assert.AreEqual(100m, loadedTable.GetCell(new Coordinate(new RowId(2), new ColumnId(2))).AsT2);

        var functionCell = loadedTable.GetCell(new Coordinate(new RowId(3), new ColumnId(2)));
        Assert.IsTrue(functionCell.IsT6);
        Assert.AreEqual("SUM(B2)", functionCell.AsT6.Value); // Function stores without =
    }

    [TestMethod]
    public void LoadFromJsonFile_BudgetTemplate_ShouldWork()
    {
        // Arrange
        var templatePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "..", "..", "..", "..", "..",
            "src", "Domain", "Templates", "Examples", "simple-budget.json"
        );

        // Skip test if file doesn't exist
        if (!File.Exists(templatePath))
        {
            Assert.Inconclusive("Template file not found");
            return;
        }

        // Act
        var table = SimpleJsonTemplateLoader.LoadFromJsonFile(templatePath);

        // Assert
        Assert.IsNotNull(table);
        Assert.AreEqual("Simple Budget Calculator", table.Name);

        // Check column headers
        Assert.AreEqual("Category", table.GetCell(new Coordinate(new RowId(1), new ColumnId(1))).AsT0);
        Assert.AreEqual("Budget", table.GetCell(new Coordinate(new RowId(1), new ColumnId(2))).AsT0);
        Assert.AreEqual("Actual", table.GetCell(new Coordinate(new RowId(1), new ColumnId(3))).AsT0);
        Assert.AreEqual("Difference", table.GetCell(new Coordinate(new RowId(1), new ColumnId(4))).AsT0);

        // Check row headers
        Assert.AreEqual("Income", table.GetCell(new Coordinate(new RowId(2), new ColumnId(1))).AsT0);
        Assert.AreEqual("Housing", table.GetCell(new Coordinate(new RowId(3), new ColumnId(1))).AsT0);

        // Check formulas
        var diffFormula = table.GetCell(new Coordinate(new RowId(2), new ColumnId(4)));
        Assert.IsTrue(diffFormula.IsT6);
        Assert.AreEqual("SUM(C2,-B2)", diffFormula.AsT6.Value); // Function stores without =
    }
}
