namespace WackyRaces.Domain.UnitTests.Entities;

using Shouldly;
using WackyRaces.Domain.Entities;
using WackyRaces.Domain.Types;
using WackyRaces.Domain.Exceptions;

[TestClass]
public sealed class TableEntityExceptionTests
{
    [TestMethod]
    public void ShouldThrowInvalidFunctionSyntaxException_WhenMissingParentheses()
    {
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "sample");

        table.SetCell(Coordinate.Parse("A1"), new DataValue("=SUM"));

        Should.Throw<InvalidFunctionSyntaxException>(() => table.GetValue(Coordinate.Parse("A1")));
    }

    [TestMethod]
    public void ShouldThrowUnknownFunctionException_WhenUsingUnsupportedFunction()
    {
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "sample");

        table.SetCell(Coordinate.Parse("A1"), new DataValue("=INVALID(B1)"));

        Should.Throw<UnknownFunctionException>(() => table.GetValue(Coordinate.Parse("A1")));
    }

    [TestMethod]
    public void ShouldThrowInvalidRangeFormatException_WhenUsingInvalidRange()
    {
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "sample");

        table.SetCell(Coordinate.Parse("A1"), new DataValue("=SUM(A1:B1:C1)"));

        Should.Throw<InvalidRangeFormatException>(() => table.GetValue(Coordinate.Parse("A1")));
    }

    [TestMethod]
    public void ShouldThrowUnsupportedComplexRangeException_WhenUsingComplexRange()
    {
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "sample");

        table.SetCell(Coordinate.Parse("A1"), new DataValue("=SUM(A1:B2)"));

        Should.Throw<UnsupportedComplexRangeException>(() => table.GetValue(Coordinate.Parse("A1")));
    }

    [TestMethod]
    public void ShouldThrowInvalidNumberTokenException_WhenUsingInvalidNumber()
    {
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "sample");

        table.SetCell(Coordinate.Parse("A1"), new DataValue("=abc+1"));

        Should.Throw<InvalidNumberTokenException>(() => table.GetValue(Coordinate.Parse("A1")));
    }

    [TestMethod]
    public void ShouldThrowInsufficientOperandsException_WhenMissingOperand()
    {
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "sample");

        table.SetCell(Coordinate.Parse("A1"), new DataValue("=1+"));

        Should.Throw<InsufficientOperandsException>(() => table.GetValue(Coordinate.Parse("A1")));
    }

    [TestMethod]
    public void ShouldThrowUnknownOperatorException_WhenUsingUnsupportedOperator()
    {
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "sample");

        table.SetCell(Coordinate.Parse("A1"), new DataValue("=1^2"));

        Should.Throw<UnknownOperatorException>(() => table.GetValue(Coordinate.Parse("A1")));
    }

    [TestMethod]
    public void ShouldThrowInvalidExpressionException_WhenExpressionIsIncomplete()
    {
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "sample");

        table.SetCell(Coordinate.Parse("A1"), new DataValue("=((1+2)"));

        Should.Throw<InvalidExpressionException>(() => table.GetValue(Coordinate.Parse("A1")));
    }

    [TestMethod]
    public void ShouldThrowUnsupportedDataValueOperationException_WhenPerformingInvalidOperation()
    {
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "sample");

        table.SetCell(Coordinate.Parse("A1"), new DataValue("hello"));
        table.SetCell(Coordinate.Parse("A2"), new DataValue(1));
        table.SetCell(Coordinate.Parse("B1"), new DataValue("=A1+A2"));

        Should.Throw<UnsupportedDataValueOperationException>(() => table.GetValue(Coordinate.Parse("B1")));
    }

    [TestMethod]
    public void ShouldThrowCircularReferenceException_WhenCellReferencesItself()
    {
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "sample");

        table.SetCell(Coordinate.Parse("A1"), new DataValue("=A1"));

        Should.Throw<CircularReferenceException>(() => table.GetValue(Coordinate.Parse("A1")));
    }

    [TestMethod]
    public void ShouldThrowCircularReferenceException_WhenFunctionReferencesItself()
    {
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "sample");

        table.SetCell(Coordinate.Parse("A1"), new DataValue("=SUM(A1)"));

        Should.Throw<CircularReferenceException>(() => table.GetValue(Coordinate.Parse("A1")));
    }
}
