using Shouldly;
using WackyRaces.Domain.Entities;
using WackyRaces.Domain.Types;

namespace WackyRaces.Domain.UnitTests.Demo;

[TestClass]
public class TypeBasedFormattingDemo
{
    [TestMethod]
    public void ShouldDemonstrate_TypeBasedFunctionFormatting()
    {
        // Arrange
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "Type Demo");

        table.SetCell(Coordinate.Parse("A1"), new DataValue(10));
        table.SetCell(Coordinate.Parse("A2"), new DataValue(20));

        // Test creating Function objects directly with Type parameters (no more format syntax parsing)
        var intFunction = new Function("SUM(A1:A2)", typeof(int));
        var decimalFunction = new Function("SUM(A1:A2)", typeof(decimal));
        var percentFunction = new Function("SUM(A1:A2)", typeof(Percentage));
        var dataValueFunction = new Function("SUM(A1:A2)", typeof(DataValue));

        table.SetCell(Coordinate.Parse("B1"), new DataValue(intFunction));
        table.SetCell(Coordinate.Parse("B2"), new DataValue(decimalFunction));
        table.SetCell(Coordinate.Parse("B3"), new DataValue(percentFunction));
        table.SetCell(Coordinate.Parse("B4"), new DataValue(dataValueFunction));

        // Act & Assert - GetValue() evaluates functions but ignores format type during evaluation
        // The format type is stored in the Function object but doesn't affect the evaluation result
        var intResult = table.GetValue(Coordinate.Parse("B1"));
        intResult.IsT1.ShouldBeTrue(); // SUM always returns int for integer inputs
        intResult.AsT1.ShouldBe(30); // SUM(A1:A2) = 10 + 20

        var decimalResult = table.GetValue(Coordinate.Parse("B2"));
        decimalResult.IsT1.ShouldBeTrue(); // SUM always returns int for integer inputs (format ignored)
        decimalResult.AsT1.ShouldBe(30); // SUM(A1:A2) = 10 + 20

        var percentageResult = table.GetValue(Coordinate.Parse("B3"));
        percentageResult.IsT1.ShouldBeTrue(); // SUM always returns int for integer inputs (format ignored)
        percentageResult.AsT1.ShouldBe(30); // SUM(A1:A2) = 10 + 20

        var dataValueResult = table.GetValue(Coordinate.Parse("B4"));
        dataValueResult.IsT1.ShouldBeTrue(); // SUM always returns int for integer inputs (format ignored)
        dataValueResult.AsT1.ShouldBe(30); // SUM(A1:A2) = 10 + 20

        // To access the raw function objects, use GetCell() instead
        var rawIntFunction = table.GetCell(Coordinate.Parse("B1"));
        rawIntFunction.IsT6.ShouldBeTrue(); // Raw cell should be Function
        rawIntFunction.AsT6.Format.ShouldBe(typeof(int));

        var rawDecimalFunction = table.GetCell(Coordinate.Parse("B2"));
        rawDecimalFunction.IsT6.ShouldBeTrue(); // Raw cell should be Function
        rawDecimalFunction.AsT6.Format.ShouldBe(typeof(decimal));

        var rawPercentFunction = table.GetCell(Coordinate.Parse("B3"));
        rawPercentFunction.IsT6.ShouldBeTrue(); // Raw cell should be Function
        rawPercentFunction.AsT6.Format.ShouldBe(typeof(Percentage));

        var rawDataValueFunction = table.GetCell(Coordinate.Parse("B4"));
        rawDataValueFunction.IsT6.ShouldBeTrue(); // Raw cell should be Function
        rawDataValueFunction.AsT6.Format.ShouldBe(typeof(DataValue));
    }

    [TestMethod]
    public void ShouldCreate_FunctionWithTypeParameters()
    {
        // Demonstrate creating Function objects with Type parameters
        var intFunction = new Function("SUM(A1:A10)", typeof(int));
        var decimalFunction = new Function("AVERAGE(A1:A10)", typeof(decimal));
        var percentFunction = new Function("COUNT(A1:A10)", typeof(Percentage));
        var dataValueFunction = new Function("SUM(A1:A10)", typeof(DataValue));

        // Assert
        intFunction.Format.ShouldBe(typeof(int));
        decimalFunction.Format.ShouldBe(typeof(decimal));
        percentFunction.Format.ShouldBe(typeof(Percentage));
        dataValueFunction.Format.ShouldBe(typeof(DataValue));
    }
}
