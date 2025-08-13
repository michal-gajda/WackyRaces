using System.Text.Json;
using WackyRaces.Domain.Entities;
using WackyRaces.Domain.Types;

namespace WackyRaces.Domain.Templates;

/// <summary>
/// Loads and saves simple JSON templates with separated columns, rows, and cells
/// </summary>
public static class SimpleJsonTemplateLoader
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Loads a table from simple JSON template string
    /// </summary>
    public static TableEntity LoadFromJson(string jsonContent)
    {
        var template = JsonSerializer.Deserialize<SimpleJsonTemplate>(jsonContent, JsonOptions);
        if (template == null)
            throw new ArgumentException("Invalid JSON template format");

        return ConvertToTableEntity(template);
    }

    /// <summary>
    /// Loads a table from simple JSON template file
    /// </summary>
    public static TableEntity LoadFromJsonFile(string filePath)
    {
        var jsonContent = File.ReadAllText(filePath);
        return LoadFromJson(jsonContent);
    }

    /// <summary>
    /// Saves a table as simple JSON template string
    /// </summary>
    public static string SaveToJson(TableEntity table)
    {
        var template = ConvertToSimpleTemplate(table);
        return JsonSerializer.Serialize(template, JsonOptions);
    }

    /// <summary>
    /// Saves a table as simple JSON template file
    /// </summary>
    public static void SaveToJsonFile(TableEntity table, string filePath)
    {
        var jsonContent = SaveToJson(table);
        File.WriteAllText(filePath, jsonContent);
    }

    /// <summary>
    /// Converts simple JSON template to TableEntity
    /// </summary>
    private static TableEntity ConvertToTableEntity(SimpleJsonTemplate template)
    {
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, template.Name);

        // Add column headers
        foreach (var column in template.Columns)
        {
            var coordinate = Coordinate.Parse(column.Coordinate);
            table.SetCell(coordinate, column.Title);
        }

        // Add row headers
        foreach (var row in template.Rows)
        {
            var coordinate = Coordinate.Parse(row.Coordinate);
            table.SetCell(coordinate, row.Title);
        }

        // Add cells with data values
        foreach (var cell in template.Cells)
        {
            var coordinate = Coordinate.Parse(cell.Coordinate);
            var dataValue = ConvertToDataValue(cell.DataValue);
            table.SetCell(coordinate, dataValue);
        }

        return table;
    }

    /// <summary>
    /// Converts DataValueDefinition to DataValue
    /// </summary>
    private static DataValue ConvertToDataValue(DataValueDefinition definition)
    {
        // If the value starts with "=", it's a function regardless of the Type field
        if (definition.Value.StartsWith("="))
        {
            var format = GetFormatType(definition.Type); // Type indicates return type
            var functionValue = definition.Value.Substring(1); // Remove the "=" prefix
            var function = new Function(functionValue, format);
            return new DataValue(function);
        }

        // Otherwise, treat as literal value based on Type
        switch (definition.Type.ToLowerInvariant())
        {
            case "decimal":
                if (decimal.TryParse(definition.Value, out var decimalValue))
                    return new DataValue(decimalValue);
                return new DataValue(definition.Value);

            case "int":
            case "integer":
                if (int.TryParse(definition.Value, out var intValue))
                    return new DataValue(intValue);
                return new DataValue(definition.Value);

            case "percentage":
                if (decimal.TryParse(definition.Value, out var percentValue))
                    return new DataValue(new Percentage(percentValue));
                return new DataValue(definition.Value);

            case "text":
            default:
                return new DataValue(definition.Value);
        }
    }

    /// <summary>
    /// Converts TableEntity to simple JSON template
    /// </summary>
    private static SimpleJsonTemplate ConvertToSimpleTemplate(TableEntity table)
    {
        var columns = new List<ColumnDefinition>();
        var rows = new List<RowDefinition>();
        var cells = new List<CellDefinition>();

        foreach (var (coordinate, dataValue) in table.Cells)
        {
            var coordinateString = coordinate.ToString();

            // Check if this is a column header (row 1)
            if (coordinate.RowId.Value == 1)
            {
                columns.Add(new ColumnDefinition
                {
                    Coordinate = coordinateString,
                    Title = GetCellValueAsString(dataValue)
                });
            }
            // Check if this is a row header (column A)
            else if (coordinate.ColumnId.Value == 1)
            {
                rows.Add(new RowDefinition
                {
                    Coordinate = coordinateString,
                    Title = GetCellValueAsString(dataValue)
                });
            }
            // Otherwise, it's a data cell
            else
            {
                cells.Add(new CellDefinition
                {
                    Coordinate = coordinateString,
                    DataValue = ConvertToDataValueDefinition(dataValue)
                });
            }
        }

        return new SimpleJsonTemplate
        {
            Name = table.Name,
            Description = $"Template created from {table.Name}",
            Columns = columns.ToArray(),
            Rows = rows.ToArray(),
            Cells = cells.ToArray()
        };
    }

    /// <summary>
    /// Converts DataValue to DataValueDefinition
    /// </summary>
    private static DataValueDefinition ConvertToDataValueDefinition(DataValue dataValue)
    {
        if (dataValue.IsT0) // String
        {
            var value = dataValue.AsT0;
            return new DataValueDefinition
            {
                Value = value,
                Type = value.StartsWith("=") ? "function" : "text"
            };
        }
        else if (dataValue.IsT1) // Int
        {
            return new DataValueDefinition
            {
                Value = dataValue.AsT1.ToString(),
                Type = "int"
            };
        }
        else if (dataValue.IsT2) // Decimal
        {
            return new DataValueDefinition
            {
                Value = dataValue.AsT2.ToString(),
                Type = "decimal"
            };
        }
        else if (dataValue.IsT5) // Percentage
        {
            return new DataValueDefinition
            {
                Value = dataValue.AsT5.Value.ToString(),
                Type = "percentage"
            };
        }
        else if (dataValue.IsT6) // Function
        {
            return new DataValueDefinition
            {
                Value = "=" + dataValue.AsT6.Value, // Add = prefix for consistency
                Type = GetFormatName(dataValue.AsT6.Format) // Use return type, not "function"
            };
        }

        return new DataValueDefinition
        {
            Value = dataValue.ToString(),
            Type = "text"
        };
    }

    /// <summary>
    /// Gets cell value as string
    /// </summary>
    private static string GetCellValueAsString(DataValue dataValue)
    {
        if (dataValue.IsT0) return dataValue.AsT0;
        if (dataValue.IsT1) return dataValue.AsT1.ToString();
        if (dataValue.IsT2) return dataValue.AsT2.ToString();
        if (dataValue.IsT5) return dataValue.AsT5.Value.ToString();
        if (dataValue.IsT6) return dataValue.AsT6.Value;
        return dataValue.ToString();
    }

    /// <summary>
    /// Gets the Type from type string
    /// </summary>
    private static Type? GetFormatType(string? type)
    {
        return type?.ToLowerInvariant() switch
        {
            "int" or "integer" => typeof(int),
            "decimal" => typeof(decimal),
            "percentage" => typeof(decimal),
            "text" => typeof(string),
            _ => typeof(decimal) // Default format
        };
    }

    /// <summary>
    /// Gets format name from Type
    /// </summary>
    private static string GetFormatName(Type? format)
    {
        if (format == typeof(int)) return "int";
        if (format == typeof(decimal)) return "decimal";
        if (format == typeof(string)) return "text";
        return "decimal";
    }
}
