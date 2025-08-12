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
        => this.Match(
            str => str?.GetHashCode() ?? 0,
            i => i.GetHashCode(),
            d => d.GetHashCode(),
            b => b.GetHashCode(),
            dt => dt.GetHashCode(),
            p => p.GetHashCode(),
            f => f.GetHashCode()
        );

    private bool ValueEquals(DataValue other)
        => this.Match(
            str => other.TryPickT0(out var oStr, out _) && str == oStr,
            i => other.TryPickT1(out var oInt, out _) && i == oInt,
            d => other.TryPickT2(out var oDec, out _) && d == oDec,
            b => other.TryPickT3(out var oBool, out _) && b == oBool,
            dt => other.TryPickT4(out var oDt, out _) && dt == oDt,
            p => other.TryPickT5(out var oP, out _) && p.Equals(oP),
            f => other.TryPickT6(out var oF, out _) && f.Equals(oF)
        );
}