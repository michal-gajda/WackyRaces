namespace WackyRaces.Domain.UnitTests.Entities;

using WackyRaces.Domain.Entities;
using WackyRaces.Domain.Types;

[TestClass]
public sealed class ExpressionParsingTests
{
    [TestMethod]
    public void ShouldEvaluate_SumDividedBySum()
    {
        // Arrange
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "Test Table");
        table.SetCell(Coordinate.Parse("A1"), new DataValue(10));
        table.SetCell(Coordinate.Parse("A2"), new DataValue(20));
        table.SetCell(Coordinate.Parse("B1"), new DataValue(5));
        table.SetCell(Coordinate.Parse("B2"), new DataValue(5));

        // =SUM(A1:A2)/SUM(B1:B2) = 30/10 = 3
        table.SetCell(Coordinate.Parse("C1"), new DataValue("=SUM(A1:A2)/SUM(B1:B2)"));

        // Act
        var result = table.GetValue(Coordinate.Parse("C1"));

        // Assert
        result.IsT2.ShouldBeTrue(); // Should be decimal
        result.AsT2.ShouldBe(3.0m);
    }

    [TestMethod]
    public void ShouldEvaluate_SumPlusAverageTimesNumber()
    {
        // Arrange
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "Test Table");
        table.SetCell(Coordinate.Parse("A1"), new DataValue(10));
        table.SetCell(Coordinate.Parse("A2"), new DataValue(20));
        table.SetCell(Coordinate.Parse("B1"), new DataValue(4));
        table.SetCell(Coordinate.Parse("B2"), new DataValue(6));

        // =SUM(A1:A2)+AVERAGE(B1:B2)*2 = 30 + 5*2 = 40
        table.SetCell(Coordinate.Parse("C1"), new DataValue("=SUM(A1:A2)+AVERAGE(B1:B2)*2"));

        // Act
        var result = table.GetValue(Coordinate.Parse("C1"));

        // Assert
        result.IsT2.ShouldBeTrue(); // Should be decimal
        result.AsT2.ShouldBe(40.0m);
    }

    [TestMethod]
    public void ShouldEvaluate_ParenthesesMultiplication()
    {
        // Arrange
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "Test Table");

        // =(2+2)*2 = 4*2 = 8
        table.SetCell(Coordinate.Parse("A1"), new DataValue("=(2+2)*2"));

        // Act
        var result = table.GetValue(Coordinate.Parse("A1"));

        // Assert
        result.IsT1.ShouldBeTrue(); // Should be integer
        result.AsT1.ShouldBe(8);
    }

    [TestMethod]
    public void ShouldEvaluate_NestedParenthesesWithExponentiation()
    {
        // Arrange
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "Test Table");

        // =((2+2)*(2+2))^2 = (4*4)^2 = 16^2 = 256
        table.SetCell(Coordinate.Parse("A1"), new DataValue("=((2+2)*(2+2))^2"));

        // Act
        var result = table.GetValue(Coordinate.Parse("A1"));

        // Assert
        result.IsT2.ShouldBeTrue(); // Should be decimal (due to Power operation)
        result.AsT2.ShouldBe(256.0m);
    }
}
