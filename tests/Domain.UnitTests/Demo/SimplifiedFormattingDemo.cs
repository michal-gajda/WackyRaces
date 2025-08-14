namespace WackyRaces.Domain.UnitTests.Demo;

using WackyRaces.Domain.Types;
using WackyRaces.Domain.Exceptions;
using WackyRaces.Domain.Entities;

[TestClass]
public sealed class SimplifiedFormattingDemo
{
    [TestMethod]
    public void ShouldCreate_FunctionWithDifferentTypes()
    {
        // Demonstrate that Function constructor with Type parameters works
        var intFunction = new Function("SUM(A1:A5)", typeof(int));
        var decimalFunction = new Function("AVERAGE(B1:B10)", typeof(decimal));
        var percentFunction = new Function("COUNT(C1:C3)", typeof(Percentage));
        var defaultFunction = new Function("MAX(D1:D2)"); // Should default to typeof(decimal)

        // Assert Format property is correctly set
        intFunction.Format.ShouldBe(typeof(int));
        decimalFunction.Format.ShouldBe(typeof(decimal));
        percentFunction.Format.ShouldBe(typeof(Percentage));
        defaultFunction.Format.ShouldBe(typeof(decimal)); // Default format

        // Assert Value property contains the formula
        intFunction.Value.ShouldBe("SUM(A1:A5)");
        decimalFunction.Value.ShouldBe("AVERAGE(B1:B10)");
        percentFunction.Value.ShouldBe("COUNT(C1:C3)");
        defaultFunction.Value.ShouldBe("MAX(D1:D2)");
    }

    [TestMethod]
    public void ShouldDemonstrate_NoMoreFormatSyntaxParsing()
    {
        // This demonstrates that the parser no longer recognizes format syntax
        // Previously, "=SUM(A1:A2):INT" would have been parsed as a function with integer format
        // Now it would fail because ":INT" is treated as an invalid token

        var functionWithoutSyntax = new Function("SUM(A1:A2)", typeof(int));

        // The format is specified via constructor, not syntax
        functionWithoutSyntax.Format.ShouldBe(typeof(int));
        functionWithoutSyntax.Value.ShouldBe("SUM(A1:A2)"); // No format syntax in the formula
    }

    [TestMethod]
    public void ShouldThrow_WhenFunctionValueStartsWithEquals()
    {
        // Function constructor should reject values that start with "="
        // The correct form is to use "=SUM(A1:A2)" in TableEntity formulas,
        // but Function constructor should only accept "SUM(A1:A2)"

        var exception = Should.Throw<InvalidFunctionValueException>(() => new Function("=SUM(A1:A2)", typeof(int)));
        exception.Message.ShouldContain("Function value should not start with '='");
        exception.Message.ShouldContain("Use 'SUM(A1:A2)' instead of '=SUM(A1:A2)'");
    }

    [TestMethod]
    public void ShouldDemonstrate_CorrectUsagePattern()
    {
        // Demonstrate the correct usage pattern:
        // 1. Use "=SUM(A1:A2)" in TableEntity formulas (with "=" prefix)
        // 2. Use "SUM(A1:A2)" in Function constructor (without "=" prefix)

        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "Usage Demo");

        table.SetCell(Coordinate.Parse("A1"), new DataValue(10));
        table.SetCell(Coordinate.Parse("A2"), new DataValue(20));

        // Correct: Formula string with "=" prefix for TableEntity
        table.SetCell(Coordinate.Parse("B1"), new DataValue("=SUM(A1:A2)"));

        // Correct: Function constructor without "=" prefix
        var directFunction = new Function("SUM(A1:A2)", typeof(int));
        table.SetCell(Coordinate.Parse("B2"), new DataValue(directFunction));

        // Both should work correctly and evaluate to the same result
        var formulaResult = table.GetValue(Coordinate.Parse("B1"));
        var functionResult = table.GetValue(Coordinate.Parse("B2"));

        formulaResult.IsT1.ShouldBeTrue();
        formulaResult.AsT1.ShouldBe(30);

        // GetValue() should now evaluate functions and return the result
        functionResult.IsT1.ShouldBeTrue(); // Should be the evaluated result
        functionResult.AsT1.ShouldBe(30); // Same result as formula

        // To access the raw function object, use GetCell() instead
        var rawFunction = table.GetCell(Coordinate.Parse("B2"));
        rawFunction.IsT6.ShouldBeTrue(); // Raw cell should be Function type
        rawFunction.AsT6.Value.ShouldBe("SUM(A1:A2)");
        rawFunction.AsT6.Format.ShouldBe(typeof(int));
    }
}
