using WackyRaces.Domain.Entities;
using WackyRaces.Domain.Types;

namespace WackyRaces.Domain.Templates.Demo;

/// <summary>
/// Demonstrates the InsertRowAfter functionality
/// </summary>
public static class RowInsertionDemo
{
    /// <summary>
    /// Shows how to insert rows and how formulas are automatically updated
    /// </summary>
    public static void DemonstrateRowInsertion()
    {
        Console.WriteLine("=== Row Insertion Demo ===");

        // Create a simple table
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "Sales Data");

        // Set up headers
        table.SetCell(Coordinate.Parse("A1"), new DataValue("Product"));
        table.SetCell(Coordinate.Parse("B1"), new DataValue("Q1 Sales"));
        table.SetCell(Coordinate.Parse("C1"), new DataValue("Q2 Sales"));
        table.SetCell(Coordinate.Parse("D1"), new DataValue("Total"));

        // Add initial data
        table.SetCell(Coordinate.Parse("A2"), new DataValue("Widget A"));
        table.SetCell(Coordinate.Parse("B2"), new DataValue(1000));
        table.SetCell(Coordinate.Parse("C2"), new DataValue(1200));
        table.SetCell(Coordinate.Parse("D2"), new DataValue("=SUM(B2,C2)"));

        table.SetCell(Coordinate.Parse("A3"), new DataValue("Widget B"));
        table.SetCell(Coordinate.Parse("B3"), new DataValue(800));
        table.SetCell(Coordinate.Parse("C3"), new DataValue(900));
        table.SetCell(Coordinate.Parse("D3"), new DataValue("=SUM(B3,C3)"));

        // Add total row
        table.SetCell(Coordinate.Parse("A5"), new DataValue("TOTAL"));
        table.SetCell(Coordinate.Parse("B5"), new DataValue("=SUM(B2:B3)"));
        table.SetCell(Coordinate.Parse("C5"), new DataValue("=SUM(C2:C3)"));
        table.SetCell(Coordinate.Parse("D5"), new DataValue("=SUM(D2:D3)"));

        Console.WriteLine("Before row insertion:");
        DisplayTable(table);

        // Insert a new row after row 2 for a new product
        Console.WriteLine("\nInserting row after row 2...\n");
        table.InsertRowAfter(2);

        // Add data for the new product
        table.SetCell(Coordinate.Parse("A3"), new DataValue("Widget C"));
        table.SetCell(Coordinate.Parse("B3"), new DataValue(600));
        table.SetCell(Coordinate.Parse("C3"), new DataValue(750));
        table.SetCell(Coordinate.Parse("D3"), new DataValue("=SUM(B3,C3)"));

        Console.WriteLine("After row insertion and adding new product:");
        DisplayTable(table);

        Console.WriteLine("\nNotice how:");
        Console.WriteLine("1. Widget B moved from row 3 to row 4");
        Console.WriteLine("2. The total formulas automatically updated:");
        Console.WriteLine($"   - B6 formula: {table.GetCell(Coordinate.Parse("B6")).AsT0} (was =SUM(B2:B3))");
        Console.WriteLine($"   - C6 formula: {table.GetCell(Coordinate.Parse("C6")).AsT0} (was =SUM(C2:C3))");
        Console.WriteLine($"   - D6 formula: {table.GetCell(Coordinate.Parse("D6")).AsT0} (was =SUM(D2:D3))");
        Console.WriteLine("3. Individual product formulas in column D were also updated correctly");
    }

    private static void DisplayTable(TableEntity table)
    {
        Console.WriteLine($"Table: {table.Name}");
        Console.WriteLine("Row | A        | B        | C        | D");
        Console.WriteLine("----|----------|----------|----------|----------");

        for (int row = 1; row <= 6; row++)
        {
            var a = GetCellDisplay(table, $"A{row}");
            var b = GetCellDisplay(table, $"B{row}");
            var c = GetCellDisplay(table, $"C{row}");
            var d = GetCellDisplay(table, $"D{row}");

            Console.WriteLine($" {row}  | {a,-8} | {b,-8} | {c,-8} | {d,-8}");
        }
        Console.WriteLine();
    }

    private static string GetCellDisplay(TableEntity table, string coordinate)
    {
        var cell = table.GetCell(Coordinate.Parse(coordinate));

        if (cell.IsT0 && !string.IsNullOrEmpty(cell.AsT0))
        {
            return cell.AsT0.Length > 8 ? cell.AsT0.Substring(0, 8) : cell.AsT0;
        }
        else if (cell.IsT1)
        {
            return cell.AsT1.ToString();
        }
        else if (cell.IsT2)
        {
            return cell.AsT2.ToString();
        }

        return "";
    }
}
