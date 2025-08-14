namespace WackyRaces.Domain.UnitTests.Entities;

using WackyRaces.Domain.Entities;
using WackyRaces.Domain.Types;

[TestClass]
public sealed class TableEntityTests
{
    [TestMethod]
    public void ShouldRecalculateFunction()
    {
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "sample");

        table.SetCell(Coordinate.Parse("A1"), new DataValue(1));
        table.SetCell(Coordinate.Parse("A2"), new DataValue(1));
        table.SetCell(Coordinate.Parse("B2"), new DataValue("=A1+A2"));

        var value = table.GetValue(Coordinate.Parse("B2"));

        int sut = (int)value;

        sut.ShouldBe(2);
    }

    [TestMethod]
    public void ShouldRecalculateComplexFunction()
    {
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "sample");

        table.SetCell(Coordinate.Parse("A1"), new DataValue(2));
        table.SetCell(Coordinate.Parse("A2"), new DataValue(2));
        table.SetCell(Coordinate.Parse("B2"), new DataValue("=(A1+A2)*2"));

        var value = table.GetValue(Coordinate.Parse("B2"));

        int sut = (int)value;

        sut.ShouldBe(8);
    }

    [TestMethod]
    public void ShouldHandleOperatorPrecedence()
    {
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "sample");

        table.SetCell(Coordinate.Parse("A1"), new DataValue(10));
        table.SetCell(Coordinate.Parse("A2"), new DataValue(5));
        table.SetCell(Coordinate.Parse("A3"), new DataValue(2));
        table.SetCell(Coordinate.Parse("B2"), new DataValue("=A1+A2*A3")); // Should be 10 + (5*2) = 20

        var value = table.GetValue(Coordinate.Parse("B2"));

        int sut = (int)value;

        sut.ShouldBe(20);
    }

    [TestMethod]
    public void ShouldHandleParenthesesOverride()
    {
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "sample");

        table.SetCell(Coordinate.Parse("A1"), new DataValue(10));
        table.SetCell(Coordinate.Parse("A2"), new DataValue(5));
        table.SetCell(Coordinate.Parse("A3"), new DataValue(2));
        table.SetCell(Coordinate.Parse("B2"), new DataValue("=(A1+A2)*A3")); // Should be (10+5)*2 = 30

        var value = table.GetValue(Coordinate.Parse("B2"));

        int sut = (int)value;

        sut.ShouldBe(30);
    }

    [TestMethod]
    public void ShouldHandleNumbersInFormula()
    {
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "sample");

        table.SetCell(Coordinate.Parse("A1"), new DataValue(5));
        table.SetCell(Coordinate.Parse("B2"), new DataValue("=A1*3+10")); // Should be 5*3+10 = 25

        var value = table.GetValue(Coordinate.Parse("B2"));

        int sut = (int)value;

        sut.ShouldBe(25);
    }

    [TestMethod]
    public void ShouldHandleSumFunction()
    {
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "sample");

        table.SetCell(Coordinate.Parse("A1"), new DataValue(10));
        table.SetCell(Coordinate.Parse("A2"), new DataValue(20));
        table.SetCell(Coordinate.Parse("A3"), new DataValue(30));
        table.SetCell(Coordinate.Parse("B1"), new DataValue("=SUM(A1:A3)")); // Should be 10+20+30 = 60

        var value = table.GetValue(Coordinate.Parse("B1"));

        int sut = (int)value;

        sut.ShouldBe(60);
    }

    [TestMethod]
    public void ShouldHandleAvgFunction()
    {
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "sample");

        table.SetCell(Coordinate.Parse("A1"), new DataValue(10));
        table.SetCell(Coordinate.Parse("A2"), new DataValue(20));
        table.SetCell(Coordinate.Parse("A3"), new DataValue(30));
        table.SetCell(Coordinate.Parse("B1"), new DataValue("=AVG(A1:A3)")); // Should be (10+20+30)/3 = 20

        var value = table.GetValue(Coordinate.Parse("B1"));

        decimal sut = (decimal)value;

        sut.ShouldBe(20m);
    }

    [TestMethod]
    public void ShouldHandleCountFunction()
    {
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "sample");

        table.SetCell(Coordinate.Parse("A1"), new DataValue(10));
        table.SetCell(Coordinate.Parse("A2"), new DataValue(20));
        table.SetCell(Coordinate.Parse("A3"), new DataValue("text"));
        table.SetCell(Coordinate.Parse("A4"), new DataValue(string.Empty));
        table.SetCell(Coordinate.Parse("B1"), new DataValue("=COUNT(A1:A4)")); // Should count non-empty cells = 3

        var value = table.GetValue(Coordinate.Parse("B1"));

        int sut = (int)value;

        sut.ShouldBe(3);
    }

    [TestMethod]
    public void ShouldCombineFunctionWithArithmetic()
    {
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "sample");

        table.SetCell(Coordinate.Parse("A1"), new DataValue(10));
        table.SetCell(Coordinate.Parse("A2"), new DataValue(20));
        table.SetCell(Coordinate.Parse("A3"), new DataValue(30));
        table.SetCell(Coordinate.Parse("B1"), new DataValue("=SUM(A1:A3)"));
        table.SetCell(Coordinate.Parse("B2"), new DataValue("=B1*2")); // Should be 60*2 = 120

        var value = table.GetValue(Coordinate.Parse("B2"));

        int sut = (int)value;

        sut.ShouldBe(120);
    }

    [TestMethod]
    public void ShouldHandleDecimalLiteralsInFormula()
    {
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "sample");

        table.SetCell(Coordinate.Parse("A1"), new DataValue(5));
        table.SetCell(Coordinate.Parse("B1"), new DataValue("=A1+2.5")); // Should be 5+2.5 = 7.5

        var value = table.GetValue(Coordinate.Parse("B1"));

        decimal sut = (decimal)value;

        sut.ShouldBe(7.5m);
    }

    [TestMethod]
    public void ShouldHandleDecimalArithmetic()
    {
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "sample");

        table.SetCell(Coordinate.Parse("A1"), new DataValue(3.14m));
        table.SetCell(Coordinate.Parse("A2"), new DataValue(2.0m));
        table.SetCell(Coordinate.Parse("B1"), new DataValue("=A1*A2")); // Should be 3.14*2.0 = 6.28

        var value = table.GetValue(Coordinate.Parse("B1"));

        decimal sut = (decimal)value;

        sut.ShouldBe(6.28m);
    }

    [TestMethod]
    public void ShouldHandleComplexDecimalFormula()
    {
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "sample");

        table.SetCell(Coordinate.Parse("A1"), new DataValue(10.5m));
        table.SetCell(Coordinate.Parse("A2"), new DataValue(2.5m));
        table.SetCell(Coordinate.Parse("B1"), new DataValue("=(A1+A2)*1.5")); // Should be (10.5+2.5)*1.5 = 19.5

        var value = table.GetValue(Coordinate.Parse("B1"));

        decimal sut = (decimal)value;

        sut.ShouldBe(19.5m);
    }

    [TestMethod]
    public void ShouldHandleDecimalPrecision()
    {
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "sample");

        table.SetCell(Coordinate.Parse("A1"), new DataValue(0.1m));
        table.SetCell(Coordinate.Parse("A2"), new DataValue(0.2m));
        table.SetCell(Coordinate.Parse("B1"), new DataValue("=A1+A2")); // Should be 0.1+0.2 = 0.3

        var value = table.GetValue(Coordinate.Parse("B1"));

        decimal sut = (decimal)value;

        sut.ShouldBe(0.3m);
    }

    [TestMethod]
    public void ShouldHandleSumFunctionWithDecimals()
    {
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "sample");

        table.SetCell(Coordinate.Parse("A1"), new DataValue(10.5m));
        table.SetCell(Coordinate.Parse("A2"), new DataValue(20.25m));
        table.SetCell(Coordinate.Parse("A3"), new DataValue(30.75m));
        table.SetCell(Coordinate.Parse("B1"), new DataValue("=SUM(A1:A3)")); // Should be 10.5+20.25+30.75 = 61.5

        var value = table.GetValue(Coordinate.Parse("B1"));

        decimal sut = (decimal)value;

        sut.ShouldBe(61.5m);
    }

    [TestMethod]
    public void ShouldHandleAvgFunctionWithDecimals()
    {
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "sample");

        table.SetCell(Coordinate.Parse("A1"), new DataValue(10.5m));
        table.SetCell(Coordinate.Parse("A2"), new DataValue(20.25m));
        table.SetCell(Coordinate.Parse("A3"), new DataValue(30.75m));
        table.SetCell(Coordinate.Parse("B1"), new DataValue("=AVG(A1:A3)")); // Should be (10.5+20.25+30.75)/3 = 20.5

        var value = table.GetValue(Coordinate.Parse("B1"));

        decimal sut = (decimal)value;

        sut.ShouldBe(20.5m);
    }

    [TestMethod]
    public void ShouldHandleMixedIntegerAndDecimalInFormula()
    {
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "sample");

        table.SetCell(Coordinate.Parse("A1"), new DataValue(10)); // integer
        table.SetCell(Coordinate.Parse("A2"), new DataValue(2.5m)); // decimal
        table.SetCell(Coordinate.Parse("B1"), new DataValue("=A1*A2+1.5")); // Should be 10*2.5+1.5 = 26.5

        var value = table.GetValue(Coordinate.Parse("B1"));

        decimal sut = (decimal)value;

        sut.ShouldBe(26.5m);
    }

    [TestMethod]
    public void ShouldHandleDecimalDivision()
    {
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "sample");

        table.SetCell(Coordinate.Parse("A1"), new DataValue(22.5m));
        table.SetCell(Coordinate.Parse("A2"), new DataValue(4.5m));
        table.SetCell(Coordinate.Parse("B1"), new DataValue("=A1/A2")); // Should be 22.5/4.5 = 5.0

        var value = table.GetValue(Coordinate.Parse("B1"));

        decimal sut = (decimal)value;

        sut.ShouldBe(5.0m);
    }

    [TestMethod]
    public void ShouldHandlePercentageLiteralInFormula()
    {
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "sample");

        table.SetCell(Coordinate.Parse("A1"), new DataValue(100));
        table.SetCell(Coordinate.Parse("B1"), new DataValue("=A1*50%")); // Should be 100*0.5 = 50

        var value = table.GetValue(Coordinate.Parse("B1"));

        decimal sut = (decimal)value;

        sut.ShouldBe(50m);
    }

    [TestMethod]
    public void ShouldHandlePercentageLiteralWithDecimal()
    {
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "sample");

        table.SetCell(Coordinate.Parse("A1"), new DataValue(200m));
        table.SetCell(Coordinate.Parse("B1"), new DataValue("=A1*12.5%")); // Should be 200*0.125 = 25

        var value = table.GetValue(Coordinate.Parse("B1"));

        decimal sut = (decimal)value;

        sut.ShouldBe(25m);
    }

    [TestMethod]
    public void ShouldHandleComplexFormulaWithPercentage()
    {
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "sample");

        table.SetCell(Coordinate.Parse("A1"), new DataValue(1000m));
        table.SetCell(Coordinate.Parse("A2"), new DataValue(20m));
        table.SetCell(Coordinate.Parse("B1"), new DataValue("=(A1+A2)*15%")); // Should be (1000+20)*0.15 = 153

        var value = table.GetValue(Coordinate.Parse("B1"));

        decimal sut = (decimal)value;

        sut.ShouldBe(153m);
    }

    [TestMethod]
    public void ShouldHandlePercentageWithCellReference()
    {
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "sample");

        table.SetCell(Coordinate.Parse("A1"), new DataValue(80m));
        table.SetCell(Coordinate.Parse("A2"), new DataValue(new Percentage(25m))); // 25%
        table.SetCell(Coordinate.Parse("B1"), new DataValue("=A1*A2")); // Should be 80*0.25 = 20

        var value = table.GetValue(Coordinate.Parse("B1"));

        decimal sut = (decimal)value;

        sut.ShouldBe(20m);
    }

    [TestMethod]
    public void ShouldHandleMultiplePercentagesInFormula()
    {
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "sample");

        table.SetCell(Coordinate.Parse("A1"), new DataValue(100m));
        table.SetCell(Coordinate.Parse("B1"), new DataValue("=A1*50%*20%")); // Should be 100*0.5*0.2 = 10

        var value = table.GetValue(Coordinate.Parse("B1"));

        decimal sut = (decimal)value;

        sut.ShouldBe(10m);
    }

    [TestMethod]
    public void ShouldHandleRequestedExample_A2Times50Percent()
    {
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "sample");

        // Example: =A2*50%
        table.SetCell(Coordinate.Parse("A2"), new DataValue(200m));
        table.SetCell(Coordinate.Parse("B1"), new DataValue("=A2*50%")); // Should be 200*0.5 = 100

        var value = table.GetValue(Coordinate.Parse("B1"));

        decimal sut = (decimal)value;

        sut.ShouldBe(100m);
    }

    [TestMethod]
    public void ShouldDemonstrate15PercentConversion()
    {
        var tableId = new TableId(Guid.NewGuid());
        var table = new TableEntity(tableId, "sample");

        // Demonstrating 15% converts to 0.15 decimal in formulas
        table.SetCell(Coordinate.Parse("A1"), new DataValue(100m));
        table.SetCell(Coordinate.Parse("B1"), new DataValue("=A1*15%")); // Should be 100*0.15 = 15

        var value = table.GetValue(Coordinate.Parse("B1"));

        decimal sut = (decimal)value;

        sut.ShouldBe(15m); // 100 * 0.15 = 15
    }
}
