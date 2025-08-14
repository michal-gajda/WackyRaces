namespace WackyRaces.Domain.UnitTests;

using WackyRaces.Domain.Exceptions;
using WackyRaces.Domain.Types;

[TestClass]
public sealed class DataValueMathOperatorsTests
{
    #region Addition Tests

    [TestMethod]
    public void Add_TwoIntegers_ReturnsCorrectSum()
    {
        // Arrange
        var left = new DataValue(5);
        var right = new DataValue(3);

        // Act
        var result = left + right;

        // Assert
        result.IsT1.ShouldBeTrue(); // int
        result.AsT1.ShouldBe(8);
    }

    [TestMethod]
    public void Add_IntegerAndDecimal_ReturnsDecimal()
    {
        // Arrange
        var left = new DataValue(5);
        var right = new DataValue(3.5m);

        // Act
        var result = left + right;

        // Assert
        result.IsT2.ShouldBeTrue(); // decimal
        result.AsT2.ShouldBe(8.5m);
    }

    [TestMethod]
    public void Add_DecimalAndInteger_ReturnsDecimal()
    {
        // Arrange
        var left = new DataValue(5.5m);
        var right = new DataValue(3);

        // Act
        var result = left + right;

        // Assert
        result.IsT2.ShouldBeTrue(); // decimal
        result.AsT2.ShouldBe(8.5m);
    }

    [TestMethod]
    public void Add_TwoDecimals_ReturnsDecimal()
    {
        // Arrange
        var left = new DataValue(5.5m);
        var right = new DataValue(3.2m);

        // Act
        var result = left + right;

        // Assert
        result.IsT2.ShouldBeTrue(); // decimal
        result.AsT2.ShouldBe(8.7m);
    }

    [TestMethod]
    public void Add_DataValueAndInt_ReturnsCorrectSum()
    {
        // Arrange
        var left = new DataValue(5);

        // Act
        var result = left + 3;

        // Assert
        result.IsT1.ShouldBeTrue(); // int
        result.AsT1.ShouldBe(8);
    }

    [TestMethod]
    public void Add_IntAndDataValue_ReturnsCorrectSum()
    {
        // Arrange
        var right = new DataValue(3);

        // Act
        var result = 5 + right;

        // Assert
        result.IsT1.ShouldBeTrue(); // int
        result.AsT1.ShouldBe(8);
    }

    [TestMethod]
    public void Add_DataValueAndDecimal_ReturnsDecimal()
    {
        // Arrange
        var left = new DataValue(5);

        // Act
        var result = left + 3.5m;

        // Assert
        result.IsT2.ShouldBeTrue(); // decimal
        result.AsT2.ShouldBe(8.5m);
    }

    [TestMethod]
    public void Add_DecimalAndDataValue_ReturnsDecimal()
    {
        // Arrange
        var right = new DataValue(3);

        // Act
        var result = 5.5m + right;

        // Assert
        result.IsT2.ShouldBeTrue(); // decimal
        result.AsT2.ShouldBe(8.5m);
    }

    [TestMethod]
    public void Add_StringValues_ThrowsUnsupportedDataValueOperationException()
    {
        // Arrange
        var left = new DataValue("hello");
        var right = new DataValue("world");

        // Act & Assert
        Should.Throw<UnsupportedDataValueOperationException>(() => left + right);
    }

    [TestMethod]
    public void Add_NumberAndString_ThrowsUnsupportedDataValueOperationException()
    {
        // Arrange
        var left = new DataValue(5);
        var right = new DataValue("test");

        // Act & Assert
        Should.Throw<UnsupportedDataValueOperationException>(() => left + right);
    }

    [TestMethod]
    public void Add_NumberAndPercentage_ThrowsUnsupportedDataValueOperationException()
    {
        // Arrange
        var left = new DataValue(5);
        var right = new DataValue(new Percentage(50m));

        // Act & Assert
        var exception = Should.Throw<UnsupportedDataValueOperationException>(() => left + right);
        exception.Message.ShouldContain("Adding Percentage to number is ambiguous");
    }

    #endregion

    #region Subtraction Tests

    [TestMethod]
    public void Subtract_TwoIntegers_ReturnsCorrectDifference()
    {
        // Arrange
        var left = new DataValue(5);
        var right = new DataValue(3);

        // Act
        var result = left - right;

        // Assert
        result.IsT1.ShouldBeTrue(); // int
        result.AsT1.ShouldBe(2);
    }

    [TestMethod]
    public void Subtract_IntegerAndDecimal_ReturnsDecimal()
    {
        // Arrange
        var left = new DataValue(5);
        var right = new DataValue(3.5m);

        // Act
        var result = left - right;

        // Assert
        result.IsT2.ShouldBeTrue(); // decimal
        result.AsT2.ShouldBe(1.5m);
    }

    [TestMethod]
    public void Subtract_DataValueAndInt_ReturnsCorrectDifference()
    {
        // Arrange
        var left = new DataValue(5);

        // Act
        var result = left - 3;

        // Assert
        result.IsT1.ShouldBeTrue(); // int
        result.AsT1.ShouldBe(2);
    }

    [TestMethod]
    public void Subtract_IntAndDataValue_ReturnsCorrectDifference()
    {
        // Arrange
        var right = new DataValue(3);

        // Act
        var result = 5 - right;

        // Assert
        result.IsT1.ShouldBeTrue(); // int
        result.AsT1.ShouldBe(2);
    }

    [TestMethod]
    public void Subtract_NumberAndPercentage_ThrowsUnsupportedDataValueOperationException()
    {
        // Arrange
        var left = new DataValue(5);
        var right = new DataValue(new Percentage(50m));

        // Act & Assert
        var exception = Should.Throw<UnsupportedDataValueOperationException>(() => left - right);
        exception.Message.ShouldContain("Subtracting Percentage from number is ambiguous");
    }

    #endregion

    #region Multiplication Tests

    [TestMethod]
    public void Multiply_TwoIntegers_ReturnsCorrectProduct()
    {
        // Arrange
        var left = new DataValue(5);
        var right = new DataValue(3);

        // Act
        var result = left * right;

        // Assert
        result.IsT1.ShouldBeTrue(); // int
        result.AsT1.ShouldBe(15);
    }

    [TestMethod]
    public void Multiply_IntegerAndDecimal_ReturnsDecimal()
    {
        // Arrange
        var left = new DataValue(5);
        var right = new DataValue(3.5m);

        // Act
        var result = left * right;

        // Assert
        result.IsT2.ShouldBeTrue(); // decimal
        result.AsT2.ShouldBe(17.5m);
    }

    [TestMethod]
    public void Multiply_DataValueAndInt_ReturnsCorrectProduct()
    {
        // Arrange
        var left = new DataValue(5);

        // Act
        var result = left * 3;

        // Assert
        result.IsT1.ShouldBeTrue(); // int
        result.AsT1.ShouldBe(15);
    }

    [TestMethod]
    public void Multiply_IntAndDataValue_ReturnsCorrectProduct()
    {
        // Arrange
        var right = new DataValue(3);

        // Act
        var result = 5 * right;

        // Assert
        result.IsT1.ShouldBeTrue(); // int
        result.AsT1.ShouldBe(15);
    }

    [TestMethod]
    public void Multiply_IntegerAndPercentage_ReturnsDecimal()
    {
        // Arrange
        var left = new DataValue(100);
        var right = new DataValue(new Percentage(50m)); // 50%

        // Act
        var result = left * right;

        // Assert
        result.IsT2.ShouldBeTrue(); // decimal
        result.AsT2.ShouldBe(50m); // 100 * 0.5 = 50
    }

    [TestMethod]
    public void Multiply_DecimalAndPercentage_ReturnsDecimal()
    {
        // Arrange
        var left = new DataValue(200.0m);
        var right = new DataValue(new Percentage(25m)); // 25%

        // Act
        var result = left * right;

        // Assert
        result.IsT2.ShouldBeTrue(); // decimal
        result.AsT2.ShouldBe(50m); // 200 * 0.25 = 50
    }

    [TestMethod]
    public void Multiply_PercentageAndInteger_ReturnsDecimal()
    {
        // Arrange
        var left = new DataValue(new Percentage(75m)); // 75%
        var right = new DataValue(40);

        // Act
        var result = left * right;

        // Assert
        result.IsT2.ShouldBeTrue(); // decimal
        result.AsT2.ShouldBe(30m); // 0.75 * 40 = 30
    }

    [TestMethod]
    public void Multiply_TwoPercentages_ReturnsDecimal()
    {
        // Arrange
        var left = new DataValue(new Percentage(50m)); // 50%
        var right = new DataValue(new Percentage(20m)); // 20%

        // Act
        var result = left * right;

        // Assert
        result.IsT2.ShouldBeTrue(); // decimal
        result.AsT2.ShouldBe(0.1m); // 0.5 * 0.2 = 0.1
    }

    #endregion

    #region Division Tests

    [TestMethod]
    public void Divide_TwoIntegers_ReturnsDecimal()
    {
        // Arrange
        var left = new DataValue(10);
        var right = new DataValue(4);

        // Act
        var result = left / right;

        // Assert
        result.IsT2.ShouldBeTrue(); // decimal
        result.AsT2.ShouldBe(2.5m);
    }

    [TestMethod]
    public void Divide_IntegerAndDecimal_ReturnsDecimal()
    {
        // Arrange
        var left = new DataValue(10);
        var right = new DataValue(4.0m);

        // Act
        var result = left / right;

        // Assert
        result.IsT2.ShouldBeTrue(); // decimal
        result.AsT2.ShouldBe(2.5m);
    }

    [TestMethod]
    public void Divide_DataValueAndInt_ReturnsDecimal()
    {
        // Arrange
        var left = new DataValue(10);

        // Act
        var result = left / 4;

        // Assert
        result.IsT2.ShouldBeTrue(); // decimal
        result.AsT2.ShouldBe(2.5m);
    }

    [TestMethod]
    public void Divide_IntAndDataValue_ReturnsDecimal()
    {
        // Arrange
        var right = new DataValue(4);

        // Act
        var result = 10 / right;

        // Assert
        result.IsT2.ShouldBeTrue(); // decimal
        result.AsT2.ShouldBe(2.5m);
    }

    [TestMethod]
    public void Divide_IntegerByPercentage_ReturnsDecimal()
    {
        // Arrange
        var left = new DataValue(50);
        var right = new DataValue(new Percentage(25m)); // 25%

        // Act
        var result = left / right;

        // Assert
        result.IsT2.ShouldBeTrue(); // decimal
        result.AsT2.ShouldBe(200m); // 50 / 0.25 = 200
    }

    [TestMethod]
    public void Divide_PercentageByInteger_ReturnsDecimal()
    {
        // Arrange
        var left = new DataValue(new Percentage(75m)); // 75%
        var right = new DataValue(3);

        // Act
        var result = left / right;

        // Assert
        result.IsT2.ShouldBeTrue(); // decimal
        result.AsT2.ShouldBe(0.25m); // 0.75 / 3 = 0.25
    }

    #endregion

    #region Unary Operators Tests

    [TestMethod]
    public void UnaryPlus_Integer_ReturnsPositiveInteger()
    {
        // Arrange
        var value = new DataValue(-5);

        // Act
        var result = +value;

        // Assert
        result.IsT1.ShouldBeTrue(); // int
        result.AsT1.ShouldBe(-5);
    }

    [TestMethod]
    public void UnaryPlus_Decimal_ReturnsPositiveDecimal()
    {
        // Arrange
        var value = new DataValue(-5.5m);

        // Act
        var result = +value;

        // Assert
        result.IsT2.ShouldBeTrue(); // decimal
        result.AsT2.ShouldBe(-5.5m);
    }

    [TestMethod]
    public void UnaryPlus_String_ThrowsUnsupportedDataValueOperationException()
    {
        // Arrange
        var value = new DataValue("test");

        // Act & Assert
        var exception = Should.Throw<UnsupportedDataValueOperationException>(() => +value);
        exception.Message.ShouldContain("Unary + operation not supported for string type");
    }

    [TestMethod]
    public void UnaryPlus_Bool_ThrowsUnsupportedDataValueOperationException()
    {
        // Arrange
        var value = new DataValue(true);

        // Act & Assert
        var exception = Should.Throw<UnsupportedDataValueOperationException>(() => +value);
        exception.Message.ShouldContain("Unary + operation not supported for bool type");
    }

    [TestMethod]
    public void UnaryPlus_DateTime_ThrowsUnsupportedDataValueOperationException()
    {
        // Arrange
        var value = new DataValue(DateTime.Now);

        // Act & Assert
        var exception = Should.Throw<UnsupportedDataValueOperationException>(() => +value);
        exception.Message.ShouldContain("Unary + operation not supported for DateTime type");
    }

    [TestMethod]
    public void UnaryPlus_Percentage_ThrowsUnsupportedDataValueOperationException()
    {
        // Arrange
        var value = new DataValue(new Percentage(50m));

        // Act & Assert
        var exception = Should.Throw<UnsupportedDataValueOperationException>(() => +value);
        exception.Message.ShouldContain("Unary + operation not supported for Percentage type");
    }

    [TestMethod]
    public void UnaryMinus_Integer_ReturnsNegatedInteger()
    {
        // Arrange
        var value = new DataValue(5);

        // Act
        var result = -value;

        // Assert
        result.IsT1.ShouldBeTrue(); // int
        result.AsT1.ShouldBe(-5);
    }

    [TestMethod]
    public void UnaryMinus_Decimal_ReturnsNegatedDecimal()
    {
        // Arrange
        var value = new DataValue(5.5m);

        // Act
        var result = -value;

        // Assert
        result.IsT2.ShouldBeTrue(); // decimal
        result.AsT2.ShouldBe(-5.5m);
    }

    [TestMethod]
    public void UnaryMinus_String_ThrowsUnsupportedDataValueOperationException()
    {
        // Arrange
        var value = new DataValue("test");

        // Act & Assert
        var exception = Should.Throw<UnsupportedDataValueOperationException>(() => -value);
        exception.Message.ShouldContain("Unary - operation not supported for string type");
    }

    [TestMethod]
    public void UnaryMinus_Bool_ThrowsUnsupportedDataValueOperationException()
    {
        // Arrange
        var value = new DataValue(true);

        // Act & Assert
        var exception = Should.Throw<UnsupportedDataValueOperationException>(() => -value);
        exception.Message.ShouldContain("Unary - operation not supported for bool type");
    }

    [TestMethod]
    public void UnaryMinus_DateTime_ThrowsUnsupportedDataValueOperationException()
    {
        // Arrange
        var value = new DataValue(DateTime.Now);

        // Act & Assert
        var exception = Should.Throw<UnsupportedDataValueOperationException>(() => -value);
        exception.Message.ShouldContain("Unary - operation not supported for DateTime type");
    }

    [TestMethod]
    public void UnaryMinus_Percentage_ThrowsUnsupportedDataValueOperationException()
    {
        // Arrange
        var value = new DataValue(new Percentage(50m));

        // Act & Assert
        var exception = Should.Throw<UnsupportedDataValueOperationException>(() => -value);
        exception.Message.ShouldContain("Unary - operation not supported for Percentage type");
    }

    #endregion

    #region Edge Cases and Error Conditions

    [TestMethod]
    public void Add_IntegerOverflow_ThrowsOverflowException()
    {
        // Arrange
        var left = new DataValue(int.MaxValue);
        var right = new DataValue(1);

        // Act & Assert
        Should.Throw<OverflowException>(() => left + right);
    }

    [TestMethod]
    public void Subtract_IntegerUnderflow_ThrowsOverflowException()
    {
        // Arrange
        var left = new DataValue(int.MinValue);
        var right = new DataValue(1);

        // Act & Assert
        Should.Throw<OverflowException>(() => left - right);
    }

    [TestMethod]
    public void Multiply_IntegerOverflow_ThrowsOverflowException()
    {
        // Arrange
        var left = new DataValue(int.MaxValue);
        var right = new DataValue(2);

        // Act & Assert
        Should.Throw<OverflowException>(() => left * right);
    }

    [TestMethod]
    public void Divide_ByZero_ThrowsDivideByZeroException()
    {
        // Arrange
        var left = new DataValue(10);
        var right = new DataValue(0);

        // Act & Assert
        Should.Throw<DivideByZeroException>(() => left / right);
    }

    [TestMethod]
    public void Operations_OnBool_ThrowUnsupportedDataValueOperationException()
    {
        // Arrange
        var boolValue = new DataValue(true);
        var intValue = new DataValue(5);

        // Act & Assert
        Should.Throw<UnsupportedDataValueOperationException>(() => boolValue + intValue);
        Should.Throw<UnsupportedDataValueOperationException>(() => boolValue - intValue);
        Should.Throw<UnsupportedDataValueOperationException>(() => boolValue * intValue);
        Should.Throw<UnsupportedDataValueOperationException>(() => boolValue / intValue);
    }

    [TestMethod]
    public void Operations_OnDateTime_ThrowUnsupportedDataValueOperationException()
    {
        // Arrange
        var dateValue = new DataValue(DateTime.Now);
        var intValue = new DataValue(5);

        // Act & Assert
        Should.Throw<UnsupportedDataValueOperationException>(() => dateValue + intValue);
        Should.Throw<UnsupportedDataValueOperationException>(() => dateValue - intValue);
        Should.Throw<UnsupportedDataValueOperationException>(() => dateValue * intValue);
        Should.Throw<UnsupportedDataValueOperationException>(() => dateValue / intValue);
    }

    [TestMethod]
    public void Operations_OnFunction_ThrowUnsupportedDataValueOperationException()
    {
        // Arrange
        var functionValue = new DataValue(new Function("SUM(A1:A10)"));
        var intValue = new DataValue(5);

        // Act & Assert
        Should.Throw<UnsupportedDataValueOperationException>(() => functionValue + intValue);
        Should.Throw<UnsupportedDataValueOperationException>(() => functionValue - intValue);
        Should.Throw<UnsupportedDataValueOperationException>(() => functionValue * intValue);
        Should.Throw<UnsupportedDataValueOperationException>(() => functionValue / intValue);
    }

    [TestMethod]
    public void UnaryPlus_Function_ThrowsUnsupportedDataValueOperationException()
    {
        // Arrange
        var value = new DataValue(new Function("AVERAGE(B1:B5)"));

        // Act & Assert
        var exception = Should.Throw<UnsupportedDataValueOperationException>(() => +value);
        exception.Message.ShouldContain("Unary + operation not supported for Function type");
    }

    [TestMethod]
    public void UnaryMinus_Function_ThrowsUnsupportedDataValueOperationException()
    {
        // Arrange
        var value = new DataValue(new Function("COUNT(C1:C20)"));

        // Act & Assert
        var exception = Should.Throw<UnsupportedDataValueOperationException>(() => -value);
        exception.Message.ShouldContain("Unary - operation not supported for Function type");
    }

    #endregion
}
