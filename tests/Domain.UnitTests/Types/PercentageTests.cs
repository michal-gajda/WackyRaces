namespace WackyRaces.Domain.UnitTests.Types;

using Shouldly;
using WackyRaces.Domain.Types;
using WackyRaces.Domain.Exceptions;

[TestClass]
public sealed class PercentageTests
{
    [TestMethod]
    public void Parse_ValidPercentageString_ReturnsCorrectValue()
    {
        var percentage = Percentage.Parse("50%");

        percentage.Value.ShouldBe(50m);
    }

    [TestMethod]
    public void Parse_DecimalPercentageString_ReturnsCorrectValue()
    {
        var percentage = Percentage.Parse("12.5%");

        percentage.Value.ShouldBe(12.5m);
    }

    [TestMethod]
    public void Parse_ZeroPercentage_ReturnsZero()
    {
        var percentage = Percentage.Parse("0%");

        percentage.Value.ShouldBe(0m);
    }

    [TestMethod]
    public void Parse_NegativePercentage_ReturnsNegativeValue()
    {
        var percentage = Percentage.Parse("-10%");

        percentage.Value.ShouldBe(-10m);
    }

    [TestMethod]
    public void Parse_WithWhitespace_TrimsAndReturnsCorrectValue()
    {
        var percentage = Percentage.Parse("  25%  ");

        percentage.Value.ShouldBe(25m);
    }

    [TestMethod]
    public void Parse_InvalidFormat_ThrowsFormatException()
    {
        Should.Throw<InvalidPercentageFormatException>(() => Percentage.Parse("50"));
        Should.Throw<InvalidPercentageFormatException>(() => Percentage.Parse("abc%"));
        Should.Throw<InvalidPercentageFormatException>(() => Percentage.Parse("%"));
    }

    [TestMethod]
    public void Parse_NullOrEmpty_ThrowsArgumentException()
    {
        Should.Throw<EmptyPercentageTextException>(() => Percentage.Parse(null!));
        Should.Throw<EmptyPercentageTextException>(() => Percentage.Parse(string.Empty));
        Should.Throw<EmptyPercentageTextException>(() => Percentage.Parse("   "));
    }

    [TestMethod]
    public void TryParse_ValidPercentage_ReturnsTrue()
    {
        var success = Percentage.TryParse("75%", out var percentage);

        success.ShouldBeTrue();
        percentage.Value.ShouldBe(75m);
    }

    [TestMethod]
    public void TryParse_InvalidPercentage_ReturnsFalse()
    {
        var success = Percentage.TryParse("invalid", out var percentage);

        success.ShouldBeFalse();
        percentage.ShouldBe(default(Percentage));
    }

    [TestMethod]
    public void ToString_ReturnsFormattedString()
    {
        var percentage = new Percentage(42.5m);

        percentage.ToString().ShouldBe("42.5%");
    }

    [TestMethod]
    public void Constructor_SetsValueCorrectly()
    {
        var percentage = new Percentage(33.33m);

        percentage.Value.ShouldBe(33.33m);
    }

    [TestMethod]
    public void ToDecimal_FifteenPercent_ReturnsPointOneFive()
    {
        var percentage = new Percentage(15m);

        percentage.ToDecimal().ShouldBe(0.15m);
    }

    [TestMethod]
    public void ToDecimal_FiftyPercent_ReturnsPointFive()
    {
        var percentage = new Percentage(50m);

        percentage.ToDecimal().ShouldBe(0.5m);
    }

    [TestMethod]
    public void ToDecimal_HundredPercent_ReturnsOne()
    {
        var percentage = new Percentage(100m);

        percentage.ToDecimal().ShouldBe(1.0m);
    }

    [TestMethod]
    public void ToDecimal_ZeroPercent_ReturnsZero()
    {
        var percentage = new Percentage(0m);

        percentage.ToDecimal().ShouldBe(0.0m);
    }

    [TestMethod]
    public void ToDecimal_TwoHundredPercent_ReturnsTwo()
    {
        var percentage = new Percentage(200m);

        percentage.ToDecimal().ShouldBe(2.0m);
    }

    [TestMethod]
    public void ToDecimal_DecimalPercentage_ReturnsCorrectDecimal()
    {
        var percentage = new Percentage(12.5m);

        percentage.ToDecimal().ShouldBe(0.125m);
    }

    [TestMethod]
    public void ToDecimal_NegativePercentage_ReturnsNegativeDecimal()
    {
        var percentage = new Percentage(-25m);

        percentage.ToDecimal().ShouldBe(-0.25m);
    }
}
