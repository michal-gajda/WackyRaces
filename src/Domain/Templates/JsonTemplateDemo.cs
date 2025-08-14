namespace WackyRaces.Domain.Templates.Demo;

using WackyRaces.Domain.Entities;
using WackyRaces.Domain.Types;

/// <summary>
/// Demonstrates how to use the SimpleJsonTemplate system
/// </summary>
public static class JsonTemplateDemo
{
    /// <summary>
    /// Creates a simple table from JSON and populates it with data
    /// </summary>
    public static void CreateAndPopulateFromJson()
    {
        // Load template from JSON
        var jsonTemplate = """
        {
          "Name": "Monthly Budget",
          "Columns": [
            { "Coordinate": "A1", "Title": "Category" },
            { "Coordinate": "B1", "Title": "Budget" },
            { "Coordinate": "C1", "Title": "Actual" },
            { "Coordinate": "D1", "Title": "Difference" }
          ],
          "Rows": [
            { "Coordinate": "A2", "Title": "Income" },
            { "Coordinate": "A3", "Title": "Expenses" }
          ],
          "Cells": [
            { "Coordinate": "D2", "DataValue": { "Value": "=SUM(C2,-B2)", "Type": "decimal" } },
            { "Coordinate": "D3", "DataValue": { "Value": "=SUM(C3,-B3)", "Type": "decimal" } },
            { "Coordinate": "A5", "DataValue": { "Value": "Net Income", "Type": "text" } },
            { "Coordinate": "D5", "DataValue": { "Value": "=SUM(D2,D3)", "Type": "decimal" } }
          ]
        }
        """;

        // Create table from template
        var table = SimpleJsonTemplateLoader.LoadFromJson(jsonTemplate);

        Console.WriteLine($"Created table: {table.Name}");

        // Add some sample data
        table.SetCell(Coordinate.Parse("B2"), new DataValue(5000m)); // Income budget
        table.SetCell(Coordinate.Parse("C2"), new DataValue(4800m)); // Income actual
        table.SetCell(Coordinate.Parse("B3"), new DataValue(3000m)); // Expenses budget
        table.SetCell(Coordinate.Parse("C3"), new DataValue(3200m)); // Expenses actual

        // Show some calculated results
        var incomeDiff = table.GetValue(Coordinate.Parse("D2"));
        var expensesDiff = table.GetValue(Coordinate.Parse("D3"));
        var netIncome = table.GetValue(Coordinate.Parse("D5"));

        Console.WriteLine($"Income difference: {incomeDiff}");
        Console.WriteLine($"Expenses difference: {expensesDiff}");
        Console.WriteLine($"Net income: {netIncome}");
    }

    /// <summary>
    /// Demonstrates using CONCAT function for string operations
    /// </summary>
    public static void DemonstrateConcatFunction()
    {
        // Load template with CONCAT functions
        var jsonTemplate = """
        {
          "Name": "Employee Report",
          "Columns": [
            { "Coordinate": "A1", "Title": "First Name" },
            { "Coordinate": "B1", "Title": "Last Name" },
            { "Coordinate": "C1", "Title": "Department" },
            { "Coordinate": "D1", "Title": "Full Name" },
            { "Coordinate": "E1", "Title": "Display Name" }
          ],
          "Rows": [
            { "Coordinate": "A2", "Title": "John" },
            { "Coordinate": "A3", "Title": "Jane" }
          ],
          "Cells": [
            { "Coordinate": "D2", "DataValue": { "Value": "=CONCAT(A2,\" \",B2)", "Type": "text" } },
            { "Coordinate": "D3", "DataValue": { "Value": "=CONCAT(A3,\" \",B3)", "Type": "text" } },
            { "Coordinate": "E2", "DataValue": { "Value": "=CONCAT(A2,\" (\",C2,\")\")", "Type": "text" } },
            { "Coordinate": "E3", "DataValue": { "Value": "=CONCAT(A3,\" (\",C3,\")\")", "Type": "text" } }
          ]
        }
        """;

        // Create table from template
        var table = SimpleJsonTemplateLoader.LoadFromJson(jsonTemplate);

        Console.WriteLine($"Created table: {table.Name}");

        // Add sample data
        table.SetCell(Coordinate.Parse("B2"), new DataValue("Doe"));
        table.SetCell(Coordinate.Parse("C2"), new DataValue("Engineering"));
        table.SetCell(Coordinate.Parse("B3"), new DataValue("Smith"));
        table.SetCell(Coordinate.Parse("C3"), new DataValue("Marketing"));

        // Show CONCAT function results
        var fullName1 = table.GetValue(Coordinate.Parse("D2"));
        var fullName2 = table.GetValue(Coordinate.Parse("D3"));
        var displayName1 = table.GetValue(Coordinate.Parse("E2"));
        var displayName2 = table.GetValue(Coordinate.Parse("E3"));

        Console.WriteLine($"Full Name 1: {fullName1}"); // Should show: John Doe
        Console.WriteLine($"Full Name 2: {fullName2}"); // Should show: Jane Smith
        Console.WriteLine($"Display Name 1: {displayName1}"); // Should show: John (Engineering)
        Console.WriteLine($"Display Name 2: {displayName2}"); // Should show: Jane (Marketing)
    }

    /// <summary>
    /// Demonstrates creating a template from an existing table
    /// </summary>
    public static void SaveTableAsJson()
    {
        // Create a table programmatically
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "Sales Report");

        // Set up headers
        table.SetCell(Coordinate.Parse("A1"), new DataValue("Product"));
        table.SetCell(Coordinate.Parse("B1"), new DataValue("Units Sold"));
        table.SetCell(Coordinate.Parse("C1"), new DataValue("Unit Price"));
        table.SetCell(Coordinate.Parse("D1"), new DataValue("Total Revenue"));

        // Add sample data
        table.SetCell(Coordinate.Parse("A2"), new DataValue("Widget A"));
        table.SetCell(Coordinate.Parse("B2"), new DataValue(100));
        table.SetCell(Coordinate.Parse("C2"), new DataValue(25.50m));
        table.SetCell(Coordinate.Parse("D2"), new DataValue(new Function("SUM(B2,C2)", typeof(decimal))));

        table.SetCell(Coordinate.Parse("A3"), new DataValue("Widget B"));
        table.SetCell(Coordinate.Parse("B3"), new DataValue(75));
        table.SetCell(Coordinate.Parse("C3"), new DataValue(30.00m));
        table.SetCell(Coordinate.Parse("D3"), new DataValue(new Function("SUM(B3,C3)", typeof(decimal))));

        // Add totals
        table.SetCell(Coordinate.Parse("A5"), new DataValue("TOTAL"));
        table.SetCell(Coordinate.Parse("B5"), new DataValue(new Function("SUM(B2:B3)", typeof(int))));
        table.SetCell(Coordinate.Parse("D5"), new DataValue(new Function("SUM(D2:D3)", typeof(decimal))));

        // Save as JSON template
        var json = SimpleJsonTemplateLoader.SaveToJson(table);

        Console.WriteLine("Generated JSON Template:");
        Console.WriteLine(json);
    }

    /// <summary>
    /// Shows the structure of the preferred JSON format
    /// </summary>
    public static void ShowJsonStructure()
    {
        Console.WriteLine("Your preferred JSON template structure:");
        Console.WriteLine("""
        {
          "Name": "Template Name",
          "Description": "Template description",
          "Columns": [
            { "Coordinate": "A1", "Title": "Column1/RowTitle"},
            { "Coordinate": "B1", "Title": "Column2"},
            { "Coordinate": "C1", "Title": "Column3"}
          ],
          "Rows": [
            { "Coordinate": "A2", "Title": "Row1"},
            { "Coordinate": "A3", "Title": "Row2"}
          ],
          "Cells": [
            { "Coordinate": "B3", "DataValue": { "Value": "=SUM(A2:A3)", "Type": "decimal" } }
          ]
        }
        """);

        Console.WriteLine("\nKey points:");
        Console.WriteLine("- Columns: Headers in row 1");
        Console.WriteLine("- Rows: Headers in column A");
        Console.WriteLine("- Cells: Data and formulas with calculations");
        Console.WriteLine("- Type: Indicates return value type (decimal, int, percentage, text)");
        Console.WriteLine("- Formulas: Start with '=' and Type indicates what the formula returns");
        Console.WriteLine("- Supported functions: SUM, AVG/AVERAGE, COUNT, MIN, MAX, CONCAT");
        Console.WriteLine("- CONCAT example: =CONCAT(A1,\" \",B1) for string concatenation");
    }
}
