using Shouldly;
using WackyRaces.Domain.Types;

namespace WackyRaces.Domain.UnitTests.Types;

[TestClass]
public sealed class FunctionFormatTests
{
    [TestMethod]
    public void ShouldCreate_FunctionWithDefaultFormat()
    {
        // Act
        var function = new Function("SUM(A1:A10)");

        // Assert
        function.Value.ShouldBe("SUM(A1:A10)");
        function.Format.ShouldBe(typeof(decimal));
    }

    [TestMethod]
    public void ShouldCreate_FunctionWithIntegerFormat()
    {
        // Act
        var function = new Function("AVERAGE(A1:A10)", typeof(int));

        // Assert
        function.Value.ShouldBe("AVERAGE(A1:A10)");
        function.Format.ShouldBe(typeof(int));
    }

    [TestMethod]
    public void ShouldCreate_FunctionWithPercentageFormat()
    {
        // Act
        var function = new Function("COUNT(A1:A10)", typeof(Percentage));

        // Assert
        function.Value.ShouldBe("COUNT(A1:A10)");
        function.Format.ShouldBe(typeof(Percentage));
    }

    [TestMethod]
    public void ShouldCreate_FunctionWithDataValueFormat()
    {
        // Act
        var function = new Function("SUM(A1:A10)", typeof(DataValue));

        // Assert
        function.Value.ShouldBe("SUM(A1:A10)");
        function.Format.ShouldBe(typeof(DataValue));
    }

    [TestMethod]
    public void ShouldTryCreate_FunctionWithFormat()
    {
        // Act
        var success = Function.TryCreate("SUM(A1:A10)", typeof(int), out var function);

        // Assert
        success.ShouldBeTrue();
        function.Value.ShouldBe("SUM(A1:A10)");
        function.Format.ShouldBe(typeof(int));
    }

    [TestMethod]
    public void ShouldTryCreate_FunctionWithDefaultFormat()
    {
        // Act
        var success = Function.TryCreate("AVG(A1:A10)", out var function);

        // Assert
        success.ShouldBeTrue();
        function.Value.ShouldBe("AVG(A1:A10)");
        function.Format.ShouldBe(typeof(decimal));
    }

    [TestMethod]
    public void ShouldPreserveFormat_InNestedFunctions()
    {
        // Arrange
        var parentFunction = new Function("SUM(AVERAGE(A1:A5), COUNT(B1:B5))", typeof(int));

        // Act
        var nestedFunctions = parentFunction.GetNestedFunctions();

        // Assert
        nestedFunctions.Count.ShouldBe(2);
        nestedFunctions[0].Format.ShouldBe(typeof(int));
        nestedFunctions[1].Format.ShouldBe(typeof(int));
    }
}
