namespace WackyRaces.Domain.Types;

public sealed partial class DataValue
{
    public static bool operator ==(DataValue? left, DataValue? right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return left.Equals(right);
    }

    public static bool operator !=(DataValue? left, DataValue? right) => !(left == right);

    public override bool Equals(object? obj)
    {
        if (obj is DataValue dataValue)
        {
            return this.ValueEquals(dataValue);
        }

        return false;
    }

    public override int GetHashCode()
    {
        return this.Match(
            stringValue => stringValue?.GetHashCode() ?? 0,
            integerValue => integerValue.GetHashCode(),
            decimalValue => decimalValue.GetHashCode(),
            booleanValue => booleanValue.GetHashCode(),
            dateTimeValue => dateTimeValue.GetHashCode(),
            percentageValue => percentageValue.GetHashCode(),
            formulaValue => formulaValue.GetHashCode()
        );
    }

    private bool ValueEquals(DataValue other)
    {
        return this.Match(
            stringValue => other.TryPickT0(out var otherStringValue, out _) && stringValue == otherStringValue,
            integerValue => other.TryPickT1(out var otherIntegerValue, out _) && integerValue == otherIntegerValue,
            decimalValue => other.TryPickT2(out var otherDecimalValue, out _) && decimalValue == otherDecimalValue,
            booleanValue => other.TryPickT3(out var otherBooleanValue, out _) && booleanValue == otherBooleanValue,
            dateTimeValue => other.TryPickT4(out var otherDateTimeValue, out _) && dateTimeValue == otherDateTimeValue,
            percentageValue => other.TryPickT5(out var otherPercentageValue, out _) && percentageValue.Equals(otherPercentageValue),
            formulaValue => other.TryPickT6(out var otherFormulaValue, out _) && formulaValue.Equals(otherFormulaValue)
        );
    }
}