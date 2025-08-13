using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using WackyRaces.Domain.Templates;
using WackyRaces.Domain.Types;

namespace WackyRaces.Domain.UnitTests.Templates;

[TestClass]
public class SimpleJsonTemplateDefinitionTests
{
    [TestMethod]
    public void SimpleJsonTemplate_ShouldSerializeAndDeserialize()
    {
        // Arrange
        var template = new SimpleJsonTemplate
        {
            Name = "Test Template",
            Description = "A test template for validation",
            Columns = new ColumnDefinition[]
            {
                new() { Coordinate = "A1", Title = "Category" },
                new() { Coordinate = "B1", Title = "Amount" }
            },
            Rows = new RowDefinition[]
            {
                new() { Coordinate = "A2", Title = "Income" },
                new() { Coordinate = "A3", Title = "Expenses" }
            },
            Cells = new CellDefinition[]
            {
                new()
                {
                    Coordinate = "B4",
                    DataValue = new DataValueDefinition
                    {
                        Value = "=SUM(B2:B3)",
                        Type = "decimal"
                    }
                }
            }
        };

        // Act
        var json = System.Text.Json.JsonSerializer.Serialize(template, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        var deserialized = System.Text.Json.JsonSerializer.Deserialize<SimpleJsonTemplate>(json);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized.Name.ShouldBe("Test Template");
        deserialized.Description.ShouldBe("A test template for validation");
        deserialized.Columns.Length.ShouldBe(2);
        deserialized.Rows.Length.ShouldBe(2);
        deserialized.Cells.Length.ShouldBe(1);

        deserialized.Columns[0].Coordinate.ShouldBe("A1");
        deserialized.Columns[0].Title.ShouldBe("Category");
        deserialized.Columns[1].Coordinate.ShouldBe("B1");
        deserialized.Columns[1].Title.ShouldBe("Amount");

        deserialized.Rows[0].Coordinate.ShouldBe("A2");
        deserialized.Rows[0].Title.ShouldBe("Income");
        deserialized.Rows[1].Coordinate.ShouldBe("A3");
        deserialized.Rows[1].Title.ShouldBe("Expenses");

        deserialized.Cells[0].Coordinate.ShouldBe("B4");
        deserialized.Cells[0].DataValue.Value.ShouldBe("=SUM(B2:B3)");
        deserialized.Cells[0].DataValue.Type.ShouldBe("decimal");
    }

    [TestMethod]
    public void ColumnDefinition_ShouldHaveCorrectProperties()
    {
        // Arrange & Act
        var column = new ColumnDefinition
        {
            Coordinate = "A1",
            Title = "Test Column"
        };

        // Assert
        column.Coordinate.ShouldBe("A1");
        column.Title.ShouldBe("Test Column");
    }

    [TestMethod]
    public void RowDefinition_ShouldHaveCorrectProperties()
    {
        // Arrange & Act
        var row = new RowDefinition
        {
            Coordinate = "A2",
            Title = "Test Row"
        };

        // Assert
        row.Coordinate.ShouldBe("A2");
        row.Title.ShouldBe("Test Row");
    }

    [TestMethod]
    public void CellDefinition_ShouldHaveCorrectProperties()
    {
        // Arrange & Act
        var cell = new CellDefinition
        {
            Coordinate = "B2",
            DataValue = new DataValueDefinition
            {
                Value = "Test Value",
                Type = "text"
            }
        };

        // Assert
        cell.Coordinate.ShouldBe("B2");
        cell.DataValue.ShouldNotBeNull();
        cell.DataValue.Value.ShouldBe("Test Value");
        cell.DataValue.Type.ShouldBe("text");
    }

    [TestMethod]
    public void DataValueDefinition_ShouldHaveCorrectProperties()
    {
        // Arrange & Act
        var dataValue = new DataValueDefinition
        {
            Value = "42",
            Type = "int"
        };

        // Assert
        dataValue.Value.ShouldBe("42");
        dataValue.Type.ShouldBe("int");
    }

    [TestMethod]
    public void SimpleJsonTemplate_EmptyLists_ShouldBeValid()
    {
        // Arrange & Act
        var template = new SimpleJsonTemplate
        {
            Name = "Empty Template",
            Description = "",
            Columns = Array.Empty<ColumnDefinition>(),
            Rows = Array.Empty<RowDefinition>(),
            Cells = Array.Empty<CellDefinition>()
        };

        // Assert
        template.Name.ShouldBe("Empty Template");
        template.Description.ShouldBe("");
        template.Columns.ShouldNotBeNull();
        template.Columns.Length.ShouldBe(0);
        template.Rows.ShouldNotBeNull();
        template.Rows.Length.ShouldBe(0);
        template.Cells.ShouldNotBeNull();
        template.Cells.Length.ShouldBe(0);
    }

    [TestMethod]
    public void SimpleJsonTemplate_LargeTemplate_ShouldHandleMultipleItems()
    {
        // Arrange
        var template = new SimpleJsonTemplate
        {
            Name = "Large Template",
            Description = "A template with many items",
            Columns = Array.Empty<ColumnDefinition>(),
            Rows = Array.Empty<RowDefinition>(),
            Cells = Array.Empty<CellDefinition>()
        };

        // Build columns (A1 through J1)
        var columns = new List<ColumnDefinition>();
        for (char col = 'A'; col <= 'J'; col++)
        {
            columns.Add(new ColumnDefinition
            {
                Coordinate = $"{col}1",
                Title = $"Column {col}"
            });
        }
        template.Columns = columns.ToArray();

        // Build rows (A2 through A11)
        var rows = new List<RowDefinition>();
        for (int row = 2; row <= 11; row++)
        {
            rows.Add(new RowDefinition
            {
                Coordinate = $"A{row}",
                Title = $"Row {row}"
            });
        }
        template.Rows = rows.ToArray();

        // Build data cells with various types
        var cells = new List<CellDefinition>();
        var types = new[] { "text", "int", "decimal", "percentage" };
        for (char col = 'B'; col <= 'E'; col++)
        {
            for (int row = 2; row <= 5; row++)
            {
                var typeIndex = ((col - 'B') + (row - 2)) % types.Length;
                cells.Add(new CellDefinition
                {
                    Coordinate = $"{col}{row}",
                    DataValue = new DataValueDefinition
                    {
                        Value = typeIndex == 0 ? "Text Value" :
                               typeIndex == 1 ? "42" :
                               typeIndex == 2 ? "3.14" : "0.25",
                        Type = types[typeIndex]
                    }
                });
            }
        }
        template.Cells = cells.ToArray();

        // Act & Assert
        template.Columns.Length.ShouldBe(10);
        template.Rows.Length.ShouldBe(10);
        template.Cells.Length.ShouldBe(16); // 4 columns × 4 rows

        // Verify first and last columns
        template.Columns[0].Coordinate.ShouldBe("A1");
        template.Columns[0].Title.ShouldBe("Column A");
        template.Columns[9].Coordinate.ShouldBe("J1");
        template.Columns[9].Title.ShouldBe("Column J");

        // Verify first and last rows
        template.Rows[0].Coordinate.ShouldBe("A2");
        template.Rows[0].Title.ShouldBe("Row 2");
        template.Rows[9].Coordinate.ShouldBe("A11");
        template.Rows[9].Title.ShouldBe("Row 11");

        // Verify some cells
        template.Cells[0].Coordinate.ShouldBe("B2");
        template.Cells[15].Coordinate.ShouldBe("E5");
    }

    [TestMethod]
    public void DataValueDefinition_AllTypes_ShouldBeSupported()
    {
        // Arrange & Act
        var textValue = new DataValueDefinition { Value = "Text", Type = "text" };
        var intValue = new DataValueDefinition { Value = "42", Type = "int" };
        var decimalValue = new DataValueDefinition { Value = "3.14", Type = "decimal" };
        var percentageValue = new DataValueDefinition { Value = "0.25", Type = "percentage" };
        var functionValue = new DataValueDefinition { Value = "=SUM(A1:A10)", Type = "decimal" };

        // Assert
        textValue.Type.ShouldBe("text");
        intValue.Type.ShouldBe("int");
        decimalValue.Type.ShouldBe("decimal");
        percentageValue.Type.ShouldBe("percentage");
        functionValue.Type.ShouldBe("decimal");
        functionValue.Value.ShouldStartWith("=");
    }

    [TestMethod]
    public void SimpleJsonTemplate_JsonSerialization_ShouldProduceReadableOutput()
    {
        // Arrange
        var template = new SimpleJsonTemplate
        {
            Name = "Readable Template",
            Description = "Testing JSON output readability",
            Columns = new ColumnDefinition[]
            {
                new() { Coordinate = "A1", Title = "Name" },
                new() { Coordinate = "B1", Title = "Value" }
            },
            Rows = new RowDefinition[]
            {
                new() { Coordinate = "A2", Title = "Item 1" }
            },
            Cells = new CellDefinition[]
            {
                new()
                {
                    Coordinate = "B2",
                    DataValue = new DataValueDefinition
                    {
                        Value = "=SUM(A1:A1)",
                        Type = "decimal"
                    }
                }
            }
        };

        // Act
        var json = System.Text.Json.JsonSerializer.Serialize(template, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        });

        // Assert
        json.ShouldContain("\"Name\": \"Readable Template\"");
        json.ShouldContain("\"Description\": \"Testing JSON output readability\"");
        json.ShouldContain("\"Columns\":");
        json.ShouldContain("\"Rows\":");
        json.ShouldContain("\"Cells\":");
        json.ShouldContain("\"Coordinate\": \"A1\"");
        json.ShouldContain("\"Title\": \"Name\"");
        json.ShouldContain("\"Value\": \"=SUM(A1:A1)\"");
        json.ShouldContain("\"Type\": \"decimal\"");
    }
}
