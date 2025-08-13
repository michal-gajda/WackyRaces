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

        // Act & Assert
        var intResult = table.GetValue(Coordinate.Parse("B1"));
        intResult.IsT6.ShouldBeTrue(); // Should be Function
        intResult.AsT6.Format.ShouldBe(typeof(int));

        var decimalResult = table.GetValue(Coordinate.Parse("B2"));
        decimalResult.IsT6.ShouldBeTrue(); // Should be Function
        decimalResult.AsT6.Format.ShouldBe(typeof(decimal));

        var percentResult = table.GetValue(Coordinate.Parse("B3"));
        percentResult.IsT6.ShouldBeTrue(); // Should be Function
        percentResult.AsT6.Format.ShouldBe(typeof(Percentage));

        var dataValueResult = table.GetValue(Coordinate.Parse("B4"));
        dataValueResult.IsT6.ShouldBeTrue(); // Should be Function
        dataValueResult.AsT6.Format.ShouldBe(typeof(DataValue));
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
