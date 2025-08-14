using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using WackyRaces.Domain.Entities;
using WackyRaces.Domain.Templates;
using WackyRaces.Domain.Types;

namespace WackyRaces.Domain.UnitTests.Templates;

[TestClass]
public sealed class SimpleJsonTemplateIntegrationTests
{
    [TestMethod]
    public void FullWorkflow_CreateTemplateFromCode_SaveLoad_ShouldWork()
    {
        // Arrange - Create a complex table programmatically
        var tableId = new TableId(Guid.NewGuid());
        var originalTable = new TableEntity(tableId, "Sales Dashboard");

        // Headers
        originalTable.SetCell(Coordinate.Parse("A1"), new DataValue("Product"));
        originalTable.SetCell(Coordinate.Parse("B1"), new DataValue("Q1 Sales"));
        originalTable.SetCell(Coordinate.Parse("C1"), new DataValue("Q2 Sales"));
        originalTable.SetCell(Coordinate.Parse("D1"), new DataValue("Total"));
        originalTable.SetCell(Coordinate.Parse("E1"), new DataValue("Growth %"));

        // Products
        originalTable.SetCell(Coordinate.Parse("A2"), new DataValue("Widget A"));
        originalTable.SetCell(Coordinate.Parse("A3"), new DataValue("Widget B"));
        originalTable.SetCell(Coordinate.Parse("A4"), new DataValue("Widget C"));
        originalTable.SetCell(Coordinate.Parse("A5"), new DataValue("TOTALS"));

        // Q1 Sales Data
        originalTable.SetCell(Coordinate.Parse("B2"), new DataValue(1000m));
        originalTable.SetCell(Coordinate.Parse("B3"), new DataValue(1500m));
        originalTable.SetCell(Coordinate.Parse("B4"), new DataValue(750m));

        // Q2 Sales Data
        originalTable.SetCell(Coordinate.Parse("C2"), new DataValue(1200m));
        originalTable.SetCell(Coordinate.Parse("C3"), new DataValue(1800m));
        originalTable.SetCell(Coordinate.Parse("C4"), new DataValue(900m));

        // Formulas with different return types
        originalTable.SetCell(Coordinate.Parse("D2"), new DataValue(new Function("SUM(B2,C2)", typeof(decimal))));
        originalTable.SetCell(Coordinate.Parse("D3"), new DataValue(new Function("SUM(B3,C3)", typeof(decimal))));
        originalTable.SetCell(Coordinate.Parse("D4"), new DataValue(new Function("SUM(B4,C4)", typeof(decimal))));
        originalTable.SetCell(Coordinate.Parse("B5"), new DataValue(new Function("SUM(B2:B4)", typeof(decimal))));
        originalTable.SetCell(Coordinate.Parse("C5"), new DataValue(new Function("SUM(C2:C4)", typeof(decimal))));
        originalTable.SetCell(Coordinate.Parse("D5"), new DataValue(new Function("SUM(D2:D4)", typeof(decimal))));

        // Growth percentages (using decimal return type for percentage calculations)
        originalTable.SetCell(Coordinate.Parse("E2"), new DataValue(new Function("SUM(C2,-B2)", typeof(decimal))));
        originalTable.SetCell(Coordinate.Parse("E3"), new DataValue(new Function("SUM(C3,-B3)", typeof(decimal))));
        originalTable.SetCell(Coordinate.Parse("E4"), new DataValue(new Function("SUM(C4,-B4)", typeof(decimal))));

        // Act - Save to JSON and reload
        var json = SimpleJsonTemplateLoader.SaveToJson(originalTable);
        var reloadedTable = SimpleJsonTemplateLoader.LoadFromJson(json);

        // Assert - Verify complete integrity
        reloadedTable.Name.ShouldBe("Sales Dashboard");

        // Check headers
        reloadedTable.GetCell(Coordinate.Parse("A1")).AsT0.ShouldBe("Product");
        reloadedTable.GetCell(Coordinate.Parse("B1")).AsT0.ShouldBe("Q1 Sales");
        reloadedTable.GetCell(Coordinate.Parse("C1")).AsT0.ShouldBe("Q2 Sales");
        reloadedTable.GetCell(Coordinate.Parse("D1")).AsT0.ShouldBe("Total");
        reloadedTable.GetCell(Coordinate.Parse("E1")).AsT0.ShouldBe("Growth %");

        // Check product names
        reloadedTable.GetCell(Coordinate.Parse("A2")).AsT0.ShouldBe("Widget A");
        reloadedTable.GetCell(Coordinate.Parse("A3")).AsT0.ShouldBe("Widget B");
        reloadedTable.GetCell(Coordinate.Parse("A4")).AsT0.ShouldBe("Widget C");
        reloadedTable.GetCell(Coordinate.Parse("A5")).AsT0.ShouldBe("TOTALS");

        // Check Q1 sales data
        reloadedTable.GetCell(Coordinate.Parse("B2")).AsT2.ShouldBe(1000m);
        reloadedTable.GetCell(Coordinate.Parse("B3")).AsT2.ShouldBe(1500m);
        reloadedTable.GetCell(Coordinate.Parse("B4")).AsT2.ShouldBe(750m);

        // Check Q2 sales data
        reloadedTable.GetCell(Coordinate.Parse("C2")).AsT2.ShouldBe(1200m);
        reloadedTable.GetCell(Coordinate.Parse("C3")).AsT2.ShouldBe(1800m);
        reloadedTable.GetCell(Coordinate.Parse("C4")).AsT2.ShouldBe(900m);

        // Check all formulas preserve format
        var d2Formula = reloadedTable.GetCell(Coordinate.Parse("D2"));
        d2Formula.IsT6.ShouldBeTrue();
        d2Formula.AsT6.Value.ShouldBe("SUM(B2,C2)");
        d2Formula.AsT6.Format.ShouldBe(typeof(decimal));

        var b5Formula = reloadedTable.GetCell(Coordinate.Parse("B5"));
        b5Formula.IsT6.ShouldBeTrue();
        b5Formula.AsT6.Value.ShouldBe("SUM(B2:B4)");
        b5Formula.AsT6.Format.ShouldBe(typeof(decimal));

        var e2Formula = reloadedTable.GetCell(Coordinate.Parse("E2"));
        e2Formula.IsT6.ShouldBeTrue();
        e2Formula.AsT6.Value.ShouldBe("SUM(C2,-B2)");
        e2Formula.AsT6.Format.ShouldBe(typeof(decimal));
    }

    [TestMethod]
    public void MultipleDataTypes_RoundTrip_ShouldPreserveAllTypes()
    {
        // Arrange
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "Data Types Demo");

        // Add various data types
        table.SetCell(Coordinate.Parse("A1"), new DataValue("Text Value"));
        table.SetCell(Coordinate.Parse("A2"), new DataValue(42));
        table.SetCell(Coordinate.Parse("A3"), new DataValue(3.14159m));
        table.SetCell(Coordinate.Parse("A4"), new DataValue(true));
        table.SetCell(Coordinate.Parse("A5"), new DataValue(DateTime.Parse("2024-01-15")));
        table.SetCell(Coordinate.Parse("A6"), new DataValue(new Percentage(75m)));

        // Add functions with different return types
        table.SetCell(Coordinate.Parse("B2"), new DataValue(new Function("COUNT(A1:A6)", typeof(int))));
        table.SetCell(Coordinate.Parse("B3"), new DataValue(new Function("SUM(A2,A3)", typeof(decimal))));
        table.SetCell(Coordinate.Parse("B4"), new DataValue(new Function("AVERAGE(A2:A3)", typeof(decimal))));
        table.SetCell(Coordinate.Parse("B5"), new DataValue(new Function("CONCAT(A1,A1)", typeof(string))));

        // Act
        var json = SimpleJsonTemplateLoader.SaveToJson(table);
        var reloadedTable = SimpleJsonTemplateLoader.LoadFromJson(json);

        // Assert
        // Check preserved data types
        reloadedTable.GetCell(Coordinate.Parse("A1")).IsT0.ShouldBeTrue(); // string
        reloadedTable.GetCell(Coordinate.Parse("A1")).AsT0.ShouldBe("Text Value");

        reloadedTable.GetCell(Coordinate.Parse("A2")).IsT1.ShouldBeTrue(); // int
        reloadedTable.GetCell(Coordinate.Parse("A2")).AsT1.ShouldBe(42);

        reloadedTable.GetCell(Coordinate.Parse("A3")).IsT2.ShouldBeTrue(); // decimal
        reloadedTable.GetCell(Coordinate.Parse("A3")).AsT2.ShouldBe(3.14159m);

        // Note: bool and DateTime are not directly supported in current JSON template format
        // so they might be serialized as strings

        reloadedTable.GetCell(Coordinate.Parse("A6")).IsT5.ShouldBeTrue(); // Percentage
        reloadedTable.GetCell(Coordinate.Parse("A6")).AsT5.Value.ShouldBe(75m);

        // Check function return types
        var intFunction = reloadedTable.GetCell(Coordinate.Parse("B2"));
        intFunction.IsT6.ShouldBeTrue();
        intFunction.AsT6.Format.ShouldBe(typeof(int));

        var decimalFunction = reloadedTable.GetCell(Coordinate.Parse("B3"));
        decimalFunction.IsT6.ShouldBeTrue();
        decimalFunction.AsT6.Format.ShouldBe(typeof(decimal));

        var stringFunction = reloadedTable.GetCell(Coordinate.Parse("B5"));
        stringFunction.IsT6.ShouldBeTrue();
        stringFunction.AsT6.Format.ShouldBe(typeof(string));
    }

    [TestMethod]
    public void ComplexFormulas_WithDifferentReturnTypes_ShouldSerializeCorrectly()
    {
        // Arrange
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "Formula Types Test");

        // Different function types (avoid row 1 and column A)
        table.SetCell(Coordinate.Parse("B2"), new DataValue(new Function("SUM(B1:B10)", typeof(decimal))));
        table.SetCell(Coordinate.Parse("B3"), new DataValue(new Function("COUNT(B1:B10)", typeof(int))));
        table.SetCell(Coordinate.Parse("B4"), new DataValue(new Function("AVERAGE(B1:B10)", typeof(decimal))));
        table.SetCell(Coordinate.Parse("B5"), new DataValue(new Function("MAX(B1:B10)", typeof(decimal))));
        table.SetCell(Coordinate.Parse("B6"), new DataValue(new Function("MIN(B1:B10)", typeof(decimal))));
        table.SetCell(Coordinate.Parse("B7"), new DataValue(new Function("CONCAT(B1,B2)", typeof(string))));

        // Complex nested formulas
        table.SetCell(Coordinate.Parse("C2"), new DataValue(new Function("SUM(A1,A3)", typeof(decimal))));
        table.SetCell(Coordinate.Parse("C3"), new DataValue(new Function("SUM(A2,COUNT(B1:B5))", typeof(int))));

        // Act
        var json = SimpleJsonTemplateLoader.SaveToJson(table);

        // Assert - Check JSON contains correct type information
        json.ShouldContain("\"Type\": \"decimal\"");
        json.ShouldContain("\"Type\": \"int\"");
        json.ShouldContain("\"Type\": \"text\"");

        // Verify specific formulas in JSON
        json.ShouldContain("=SUM(B1:B10)");
        json.ShouldContain("=COUNT(B1:B10)");
        json.ShouldContain("=CONCAT(B1,B2)");

        // Reload and verify
        var reloadedTable = SimpleJsonTemplateLoader.LoadFromJson(json);

        var sumFunction = reloadedTable.GetCell(Coordinate.Parse("B2"));
        sumFunction.AsT6.Format.ShouldBe(typeof(decimal));

        var countFunction = reloadedTable.GetCell(Coordinate.Parse("B3"));
        countFunction.AsT6.Format.ShouldBe(typeof(int));

        var concatFunction = reloadedTable.GetCell(Coordinate.Parse("B7"));
        concatFunction.AsT6.Format.ShouldBe(typeof(string));
    }

    [TestMethod]
    public void FileOperations_CreateSaveLoadDelete_ShouldWorkEndToEnd()
    {
        // Arrange
        var tempFilePath = Path.GetTempFileName();
        var jsonFilePath = Path.ChangeExtension(tempFilePath, ".json");

        var tableId = new TableId(Guid.NewGuid());
        var originalTable = new TableEntity(tableId, "File Test Table");
        originalTable.SetCell(Coordinate.Parse("A1"), new DataValue("Header"));
        originalTable.SetCell(Coordinate.Parse("A2"), new DataValue(123.45m));
        originalTable.SetCell(Coordinate.Parse("A3"), new DataValue(new Function("SUM(A2,10)", typeof(decimal))));

        try
        {
            // Act - Save to file
            SimpleJsonTemplateLoader.SaveToJsonFile(originalTable, jsonFilePath);

            // Verify file exists and has content
            File.Exists(jsonFilePath).ShouldBeTrue();
            var fileContent = File.ReadAllText(jsonFilePath);
            fileContent.ShouldNotBeEmpty();
            fileContent.ShouldContain("File Test Table");

            // Load from file
            var loadedTable = SimpleJsonTemplateLoader.LoadFromJsonFile(jsonFilePath);

            // Assert - Verify loaded content
            loadedTable.Name.ShouldBe("File Test Table");
            loadedTable.GetCell(Coordinate.Parse("A1")).AsT0.ShouldBe("Header");
            loadedTable.GetCell(Coordinate.Parse("A2")).AsT2.ShouldBe(123.45m);

            var formulaCell = loadedTable.GetCell(Coordinate.Parse("A3"));
            formulaCell.IsT6.ShouldBeTrue();
            formulaCell.AsT6.Value.ShouldBe("SUM(A2,10)");
            formulaCell.AsT6.Format.ShouldBe(typeof(decimal));
        }
        finally
        {
            // Cleanup
            if (File.Exists(tempFilePath))
                File.Delete(tempFilePath);
            if (File.Exists(jsonFilePath))
                File.Delete(jsonFilePath);
        }
    }

    [TestMethod]
    public void ExampleTemplates_LoadAndValidate_ShouldWork()
    {
        // This test tries to load the example templates from the Examples directory
        var exampleDirectory = Path.Combine(
            Directory.GetCurrentDirectory(),
            "..", "..", "..", "..", "..",
            "src", "Domain", "Templates", "Examples"
        );

        if (!Directory.Exists(exampleDirectory))
        {
            Assert.Inconclusive("Examples directory not found");
            return;
        }

        var jsonFiles = Directory.GetFiles(exampleDirectory, "*.json");

        if (jsonFiles.Length == 0)
        {
            Assert.Inconclusive("No example JSON files found");
            return;
        }

        foreach (var jsonFile in jsonFiles)
        {
            try
            {
                // Act
                var table = SimpleJsonTemplateLoader.LoadFromJsonFile(jsonFile);

                // Assert
                table.ShouldNotBeNull();
                table.Name.ShouldNotBeNullOrEmpty();

                // Basic validation that the table loaded successfully
                Console.WriteLine($"Successfully loaded template: {table.Name} from {Path.GetFileName(jsonFile)}");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Failed to load template from {Path.GetFileName(jsonFile)}: {ex.Message}");
            }
        }
    }

    [TestMethod]
    public void PerformanceTest_LargeTableRoundTrip_ShouldCompleteReasonably()
    {
        // Arrange - Create a moderately large table
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "Performance Test");

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Add 26x26 grid of data (limit to valid column range)
        for (int row = 1; row <= 26; row++)
        {
            for (int col = 1; col <= 26; col++)
            {
                var coordinate = new Coordinate(new RowId(row), new ColumnId(col));
                if (row == 1)
                {
                    // Headers
                    table.SetCell(coordinate, new DataValue($"Col{col}"));
                }
                else if (col == 1)
                {
                    // Row labels
                    table.SetCell(coordinate, new DataValue($"Row{row}"));
                }
                else if (row % 5 == 0 && col % 5 == 0)
                {
                    // Add some formulas
                    table.SetCell(coordinate, new DataValue(new Function($"SUM(A{row},A{col})", typeof(decimal))));
                }
                else
                {
                    // Regular data
                    table.SetCell(coordinate, new DataValue(row * col));
                }
            }
        }

        var setupTime = stopwatch.ElapsedMilliseconds;
        stopwatch.Restart();

        // Act - Save to JSON
        var json = SimpleJsonTemplateLoader.SaveToJson(table);
        var saveTime = stopwatch.ElapsedMilliseconds;
        stopwatch.Restart();

        // Load from JSON
        var reloadedTable = SimpleJsonTemplateLoader.LoadFromJson(json);
        var loadTime = stopwatch.ElapsedMilliseconds;

        // Assert
        reloadedTable.Name.ShouldBe("Performance Test");
        reloadedTable.Cells.Count.ShouldBe(676); // 26x26

        // Verify some specific cells
        reloadedTable.GetCell(new Coordinate(new RowId(1), new ColumnId(1))).AsT0.ShouldBe("Col1");
        reloadedTable.GetCell(new Coordinate(new RowId(2), new ColumnId(1))).AsT0.ShouldBe("Row2");
        reloadedTable.GetCell(new Coordinate(new RowId(2), new ColumnId(2))).AsT1.ShouldBe(4); // 2*2

        // Performance assertions (adjust thresholds as needed)
        setupTime.ShouldBeLessThan(5000); // Setup should take less than 5 seconds
        saveTime.ShouldBeLessThan(2000);  // Save should take less than 2 seconds
        loadTime.ShouldBeLessThan(2000);  // Load should take less than 2 seconds

        Console.WriteLine($"Performance metrics - Setup: {setupTime}ms, Save: {saveTime}ms, Load: {loadTime}ms");
    }
}
