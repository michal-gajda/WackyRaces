namespace WackyRaces.Domain.UnitTests.Types;

using WackyRaces.Domain.Types;

[TestClass]
public sealed class ConcatFunctionTests
{
    [TestMethod]
    public void ShouldCreate_ConcatFunctionWithStringFormat()
    {
        // Act
        var concatFunction = new Function("CONCAT(A1,B1)", typeof(string));

        // Assert
        concatFunction.Value.ShouldBe("CONCAT(A1,B1)");
        concatFunction.Format.ShouldBe(typeof(string));
    }

    [TestMethod]
    public void ShouldCreate_ConcatFunctionWithDefaultFormat()
    {
        // Act
        var concatFunction = new Function("CONCAT(A1,B1)");

        // Assert
        concatFunction.Value.ShouldBe("CONCAT(A1,B1)");
        concatFunction.Format.ShouldBe(typeof(decimal)); // Default format
    }

    [TestMethod]
    public void ShouldCreate_ConcatFunctionWithMultipleArguments()
    {
        // Act
        var concatFunction = new Function("CONCAT(A1,B1,C1,D1)", typeof(string));

        // Assert
        concatFunction.Value.ShouldBe("CONCAT(A1,B1,C1,D1)");
        concatFunction.Format.ShouldBe(typeof(string));
    }

    [TestMethod]
    public void ShouldCreate_ConcatFunctionWithLiteralStrings()
    {
        // Act
        var concatFunction = new Function("CONCAT(A1,\" - \",B1)", typeof(string));

        // Assert
        concatFunction.Value.ShouldBe("CONCAT(A1,\" - \",B1)");
        concatFunction.Format.ShouldBe(typeof(string));
    }

    [TestMethod]
    public void ShouldGetFunctionName_ForConcatFunction()
    {
        // Arrange
        var concatFunction = new Function("CONCAT(A1,B1)", typeof(string));

        // Act
        var functionName = concatFunction.GetFunctionName();

        // Assert
        functionName.ShouldBe("CONCAT");
    }

    [TestMethod]
    public void ShouldGetArguments_ForConcatFunction()
    {
        // Arrange
        var concatFunction = new Function("CONCAT(A1,B1,C1)", typeof(string));

        // Act
        var arguments = concatFunction.GetArguments();

        // Assert
        arguments.ShouldBe("A1,B1,C1");
    }

    [TestMethod]
    public void ShouldGetArgumentTokens_ForConcatFunction()
    {
        // Arrange
        var concatFunction = new Function("CONCAT(A1,B1,C1)", typeof(string));

        // Act
        var tokens = concatFunction.GetArgumentTokens();

        // Assert
        tokens.Count.ShouldBe(3);
        tokens[0].ShouldBe("A1");
        tokens[1].ShouldBe("B1");
        tokens[2].ShouldBe("C1");
    }

    [TestMethod]
    public void ShouldTryCreate_ConcatFunctionSuccessfully()
    {
        // Act
        var success = Function.TryCreate("CONCAT(A1,B1)", typeof(string), out var function);

        // Assert
        success.ShouldBeTrue();
        function.Value.ShouldBe("CONCAT(A1,B1)");
        function.Format.ShouldBe(typeof(string));
    }

    [TestMethod]
    public void ShouldValidate_ConcatFunctionAsValidExpression()
    {
        // This test verifies that CONCAT is now accepted as a valid function
        // by ensuring no exception is thrown during construction

        // Act & Assert - Should not throw
        var concatFunction = new Function("CONCAT(A1,B1)", typeof(string));
        concatFunction.Value.ShouldBe("CONCAT(A1,B1)");
    }

    [TestMethod]
    public void ShouldCreate_NestedConcatFunction()
    {
        // Act
        var nestedFunction = new Function("CONCAT(CONCAT(A1,B1),C1)", typeof(string));

        // Assert
        nestedFunction.Value.ShouldBe("CONCAT(CONCAT(A1,B1),C1)");
        nestedFunction.Format.ShouldBe(typeof(string));
        nestedFunction.IsNestedFunction().ShouldBeTrue();
    }
}
