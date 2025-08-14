namespace WackyRaces.Domain.UnitTests.Demo;

using WackyRaces.Domain.Types;
using WackyRaces.Domain.Entities;

[TestClass]
public class FormulaSyntaxDemo
{
    [TestMethod]
    public void ShouldDemonstrate_BothSyntaxesAreCorrectInTheirContext()
    {
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "Syntax Demo");

        table.SetCell(Coordinate.Parse("A1"), new DataValue(10));
        table.SetCell(Coordinate.Parse("A2"), new DataValue(20));

        // CORRECT: Function constructor uses expression WITHOUT "="
        var functionObject = new Function("SUM(A1:A2)", typeof(int));
        functionObject.Value.ShouldBe("SUM(A1:A2)"); // No "=" in Function.Value

        // CORRECT: TableEntity formula uses string WITH "="
        table.SetCell(Coordinate.Parse("B1"), new DataValue("=SUM(A1:A2)"));

        // When TableEntity processes "=SUM(A1:A2)", it:
        // 1. Strips the "=" prefix
        // 2. Creates a Function internally with "SUM(A1:A2)"
        // 3. Evaluates the function

        var result = table.GetValue(Coordinate.Parse("B1"));
        result.IsT1.ShouldBeTrue(); // Result is integer
        result.AsT1.ShouldBe(30);
    }

    [TestMethod]
    public void ShouldShow_WhatIsIncorrect()
    {
        // INCORRECT: Function constructor should NOT have "=" prefix
        // This should throw an exception (we added validation for this)
        Should.Throw<WackyRaces.Domain.Exceptions.InvalidFunctionValueException>(() =>
            new Function("=SUM(A1:A2)", typeof(int)));

        // CORRECT: Function constructor without "=" prefix
        var correctFunction = new Function("SUM(A1:A2)", typeof(int));
        correctFunction.Value.ShouldBe("SUM(A1:A2)");
    }
}
