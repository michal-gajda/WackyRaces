using Shouldly;
using WackyRaces.Domain.Types;
using WackyRaces.Domain.Entities;
using WackyRaces.Domain.Exceptions;

namespace WackyRaces.Domain.UnitTests.Demo;

[TestClass]
public class InvalidSyntaxDemo
{
    [TestMethod]
    public void ShouldConfirm_SumWithoutParenthesesIsIncorrect()
    {
        // "=SUM" without parentheses is INCORRECT
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "Invalid Syntax Demo");

        table.SetCell(Coordinate.Parse("A1"), new DataValue("=SUM"));

        // This should throw InvalidFunctionSyntaxException
        var exception = Should.Throw<InvalidFunctionSyntaxException>(() => table.GetValue(Coordinate.Parse("A1")));
        exception.Message.ShouldContain("Invalid function syntax");
    }

    [TestMethod]
    public void ShouldConfirm_SumWithMultipleColonsIsIncorrect()
    {
        // "=SUM(A1:A2:A3)" with multiple colons is INCORRECT
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "Invalid Syntax Demo");

        table.SetCell(Coordinate.Parse("A1"), new DataValue("=SUM(A1:A2:A3)"));

        // This should throw InvalidRangeFormatException
        var exception = Should.Throw<InvalidRangeFormatException>(() => table.GetValue(Coordinate.Parse("A1")));
        exception.Message.ShouldContain("Invalid range format");
    }

    [TestMethod]
    public void ShouldShow_CorrectSyntaxExamples()
    {
        // These are the CORRECT syntaxes
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "Correct Syntax Demo");

        table.SetCell(Coordinate.Parse("A1"), new DataValue(10));
        table.SetCell(Coordinate.Parse("A2"), new DataValue(20));
        table.SetCell(Coordinate.Parse("A3"), new DataValue(30));

        // CORRECT: =SUM(A1:A3) - proper function with parentheses and valid range
        table.SetCell(Coordinate.Parse("B1"), new DataValue("=SUM(A1:A3)"));

        // The result should work without throwing exceptions
        var result1 = table.GetValue(Coordinate.Parse("B1"));

        result1.IsT1.ShouldBeTrue();
        result1.AsT1.ShouldBe(60); // 10 + 20 + 30
    }

    [TestMethod]
    public void ShouldConfirm_FunctionConstructorBehavior()
    {
        // Function constructor actually accepts both forms:

        // Valid: Function constructor accepts "SUM" without parentheses
        var functionWithoutParens = new Function("SUM");
        functionWithoutParens.Value.ShouldBe("SUM");

        // Valid: Function constructor accepts "SUM(A1:A3)" with parentheses
        var functionWithParens = new Function("SUM(A1:A3)");
        functionWithParens.Value.ShouldBe("SUM(A1:A3)");
    }
}
