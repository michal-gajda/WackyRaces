using Shouldly;
using WackyRaces.Domain.Types;
using WackyRaces.Domain.Exceptions;

namespace WackyRaces.Domain.UnitTests.Types;

[TestClass]
public class FunctionValidationTests
{
    [TestMethod]
    public void ShouldReject_FunctionValueWithEqualsPrefix()
    {
        // Direct constructor should throw
        var exception = Should.Throw<InvalidFunctionValueException>(() => new Function("=SUM(A1:A2)"));
        exception.Message.ShouldContain("Function value should not start with '='");
    }

    [TestMethod]
    public void ShouldReject_FunctionValueWithEqualsPrefixInTryCreate()
    {
        // TryCreate should return false for values starting with "="
        var success = Function.TryCreate("=SUM(A1:A2)", out var function);

        success.ShouldBeFalse();
        function.ShouldBe(default(Function));
    }

    [TestMethod]
    public void ShouldAccept_ValidFunctionValueWithoutEqualsPrefix()
    {
        // Valid function values should work fine
        var function = new Function("SUM(A1:A2)");
        function.Value.ShouldBe("SUM(A1:A2)");

        var tryCreateSuccess = Function.TryCreate("AVERAGE(B1:B5)", out var tryCreateFunction);
        tryCreateSuccess.ShouldBeTrue();
        tryCreateFunction.Value.ShouldBe("AVERAGE(B1:B5)");
    }
}
