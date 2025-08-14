namespace WackyRaces.Domain.UnitTests;

using WackyRaces.Domain.Types;
using WackyRaces.Domain.Exceptions;
using Shouldly;

[TestClass]
public sealed class CustomExceptionsTests
{
    [TestMethod]
    public void UnsupportedDataValueOperation_ShouldThrowCustomException()
    {
        // Arrange
        var left = new DataValue("text");
        var right = new DataValue(42);

        // Act & Assert
        var exception = Should.Throw<UnsupportedDataValueOperationException>(() => left + right);
        exception.ShouldBeOfType<UnsupportedDataValueOperationException>();
        exception.Message.ShouldContain("operation not supported");
    }

    [TestMethod]
    public void InvalidPercentageFormat_ShouldThrowCustomException()
    {
        // Act & Assert
        var exception = Should.Throw<InvalidPercentageFormatException>(() => Percentage.Parse("not a percentage"));
        exception.ShouldBeOfType<InvalidPercentageFormatException>();
        exception.Message.ShouldContain("Invalid percentage format");
    }

    [TestMethod]
    public void EmptyPercentageText_ShouldThrowCustomException()
    {
        // Act & Assert
        var exception = Should.Throw<EmptyPercentageTextException>(() => Percentage.Parse(string.Empty));
        exception.ShouldBeOfType<EmptyPercentageTextException>();
        exception.Message.ShouldContain("Percentage text cannot be null, empty, or contain only whitespace characters");
    }

    [TestMethod]
    public void AllCustomExceptions_ShouldInheritFromDomainException()
    {
        // Arrange & Act & Assert
        var unsupportedOp = new UnsupportedDataValueOperationException("test", "String");
        var invalidPercentage = new InvalidPercentageFormatException("test");
        var emptyPercentage = new EmptyPercentageTextException();
        var invalidFunction = new InvalidFunctionSyntaxException("test");
        var unknownFunction = new UnknownFunctionException("test");
        var invalidRange = new InvalidRangeFormatException("test");
        var unsupportedRange = new UnsupportedComplexRangeException("test");
        var invalidNumber = new InvalidNumberTokenException("test");
        var insufficientOperands = new InsufficientOperandsException("test");
        var unknownOperator = new UnknownOperatorException("test");
        var invalidExpression = new InvalidExpressionException();

        // All should inherit from DomainException
        unsupportedOp.ShouldBeAssignableTo<DomainException>();
        invalidPercentage.ShouldBeAssignableTo<DomainException>();
        emptyPercentage.ShouldBeAssignableTo<DomainException>();
        invalidFunction.ShouldBeAssignableTo<DomainException>();
        unknownFunction.ShouldBeAssignableTo<DomainException>();
        invalidRange.ShouldBeAssignableTo<DomainException>();
        unsupportedRange.ShouldBeAssignableTo<DomainException>();
        invalidNumber.ShouldBeAssignableTo<DomainException>();
        insufficientOperands.ShouldBeAssignableTo<DomainException>();
        unknownOperator.ShouldBeAssignableTo<DomainException>();
        invalidExpression.ShouldBeAssignableTo<DomainException>();
    }

    [TestMethod]
    public void PercentageToDecimal_ShouldWorkWithCustomExceptions()
    {
        // Test that percentage parsing and conversion work with custom exceptions
        var percentage = Percentage.Parse("15%");
        var decimal_value = percentage.ToDecimal();

        decimal_value.ShouldBe(0.15m);
    }
}
