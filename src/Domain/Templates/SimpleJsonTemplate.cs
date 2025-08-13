using System.Text.Json;
using System.Text.Json.Serialization;

namespace WackyRaces.Domain.Templates;

/// <summary>
/// Simplified JSON template definition with separate columns, rows, and cells
/// </summary>
public class SimpleJsonTemplate
{
    [JsonPropertyName("Name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("Description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("Columns")]
    public ColumnDefinition[] Columns { get; set; } = Array.Empty<ColumnDefinition>();

    [JsonPropertyName("Rows")]
    public RowDefinition[] Rows { get; set; } = Array.Empty<RowDefinition>();

    [JsonPropertyName("Cells")]
    public CellDefinition[] Cells { get; set; } = Array.Empty<CellDefinition>();
}

/// <summary>
/// Column header definition
/// </summary>
public class ColumnDefinition
{
    [JsonPropertyName("Coordinate")]
    public string Coordinate { get; set; } = string.Empty;

    [JsonPropertyName("Title")]
    public string Title { get; set; } = string.Empty;
}

/// <summary>
/// Row header definition
/// </summary>
public class RowDefinition
{
    [JsonPropertyName("Coordinate")]
    public string Coordinate { get; set; } = string.Empty;

    [JsonPropertyName("Title")]
    public string Title { get; set; } = string.Empty;
}

/// <summary>
/// Cell definition with data value
/// </summary>
public class CellDefinition
{
    [JsonPropertyName("Coordinate")]
    public string Coordinate { get; set; } = string.Empty;

    [JsonPropertyName("DataValue")]
    public DataValueDefinition DataValue { get; set; } = new();
}

/// <summary>
/// Data value definition for cells
/// </summary>
public class DataValueDefinition
{
    [JsonPropertyName("Value")]
    public string Value { get; set; } = string.Empty;

    [JsonPropertyName("Type")]
    public string Type { get; set; } = "text"; // text, decimal, int, percentage, function
}
