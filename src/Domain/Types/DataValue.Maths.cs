namespace WackyRaces.Domain.Types;

public sealed partial class DataValue
{
    public static DataValue operator +(DataValue left, DataValue right) => Add(left, right);
    public static DataValue operator -(DataValue left, DataValue right) => Sub(left, right);
    public static DataValue operator *(DataValue left, DataValue right) => Mul(left, right);
    public static DataValue operator /(DataValue left, DataValue right) => Div(left, right);

    public static DataValue operator +(DataValue left, int right) => Add(left, new DataValue(right));
    public static DataValue operator +(int left, DataValue right) => Add(new DataValue(left), right);
    public static DataValue operator +(DataValue left, decimal right) => Add(left, new DataValue(right));
    public static DataValue operator +(decimal left, DataValue right) => Add(new DataValue(left), right);

    public static DataValue operator -(DataValue left, int right) => Sub(left, new DataValue(right));
    public static DataValue operator -(int left, DataValue right) => Sub(new DataValue(left), right);
    public static DataValue operator -(DataValue left, decimal right) => Sub(left, new DataValue(right));
    public static DataValue operator -(decimal left, DataValue right) => Sub(new DataValue(left), right);

    public static DataValue operator *(DataValue left, int right) => Mul(left, new DataValue(right));
    public static DataValue operator *(int left, DataValue right) => Mul(new DataValue(left), right);
    public static DataValue operator *(DataValue left, decimal right) => Mul(left, new DataValue(right));
    public static DataValue operator *(decimal left, DataValue right) => Mul(new DataValue(left), right);

    public static DataValue operator /(DataValue left, int right) => Div(left, new DataValue(right));
    public static DataValue operator /(int left, DataValue right) => Div(new DataValue(left), right);
    public static DataValue operator /(DataValue left, decimal right) => Div(left, new DataValue(right));
    public static DataValue operator /(decimal left, DataValue right) => Div(new DataValue(left), right);

    public static DataValue operator +(DataValue value) => value.Match(
        s => throw new InvalidOperationException("Unary + not supported for string."),
        i => new DataValue(+i),
        d => new DataValue(+d),
        b => throw new InvalidOperationException("Unary + not supported for bool."),
        dt => throw new InvalidOperationException("Unary + not supported for DateTime."),
        p => throw new InvalidOperationException("Unary + not supported for Percentage.")
    );

    public static DataValue operator -(DataValue value) => value.Match(
        s => throw new InvalidOperationException("Unary - not supported for string."),
        i => new DataValue(-i),
        d => new DataValue(-d),
        b => throw new InvalidOperationException("Unary - not supported for bool."),
        dt => throw new InvalidOperationException("Unary - not supported for DateTime."),
        p => throw new InvalidOperationException("Unary - not supported for Percentage.")
    );

    private static DataValue Add(DataValue left, DataValue right)
    {
        return left.Match<DataValue>(
            s => Throw<string>(),
            i => right.Match<DataValue>(
                _s => Throw<string>(),
                i2 => new DataValue(checked(i + i2)),
                d2 => new DataValue(i + d2),
                _b => Throw<bool>(),
                _dt => Throw<DateTime>(),
                _p => Throw<Percentage>("Adding Percentage to number is ambiguous.")
            ),
            d => right.Match<DataValue>(
                _s => Throw<string>(),
                i2 => new DataValue(d + i2),
                d2 => new DataValue(d + d2),
                _b => Throw<bool>(),
                _dt => Throw<DateTime>(),
                _p => Throw<Percentage>("Adding Percentage to number is ambiguous.")
            ),
            _b => Throw<bool>(),
            _dt => Throw<DateTime>(),
            _p => Throw<Percentage>("Adding Percentage to number is ambiguous.")
        );
    }

    private static DataValue Sub(DataValue left, DataValue right)
    {
        return left.Match<DataValue>(
            s => Throw<string>(),
            i => right.Match<DataValue>(
                _s => Throw<string>(),
                i2 => new DataValue(checked(i - i2)),
                d2 => new DataValue(i - d2),
                _b => Throw<bool>(),
                _dt => Throw<DateTime>(),
                _p => Throw<Percentage>("Subtracting Percentage from number is ambiguous.")
            ),
            d => right.Match<DataValue>(
                _s => Throw<string>(),
                i2 => new DataValue(d - i2),
                d2 => new DataValue(d - d2),
                _b => Throw<bool>(),
                _dt => Throw<DateTime>(),
                _p => Throw<Percentage>("Subtracting Percentage from number is ambiguous.")
            ),
            _b => Throw<bool>(),
            _dt => Throw<DateTime>(),
            _p => Throw<Percentage>("Subtracting Percentage from number is ambiguous.")
        );
    }

    private static DataValue Mul(DataValue left, DataValue right)
    {
        return left.Match<DataValue>(
            s => Throw<string>(),
            i => right.Match<DataValue>(
                _s => Throw<string>(),
                i2 => new DataValue(checked(i * i2)),
                d2 => new DataValue(i * d2),
                _b => Throw<bool>(),
                _dt => Throw<DateTime>(),
                p2 => new DataValue(i * ToRatio(p2))
            ),
            d => right.Match<DataValue>(
                _s => Throw<string>(),
                i2 => new DataValue(d * i2),
                d2 => new DataValue(d * d2),
                _b => Throw<bool>(),
                _dt => Throw<DateTime>(),
                p2 => new DataValue(d * ToRatio(p2))
            ),
            _b => Throw<bool>(),
            _dt => Throw<DateTime>(),
            p => right.Match<DataValue>(
                _s => Throw<string>(),
                i2 => new DataValue(ToRatio(p) * i2),
                d2 => new DataValue(ToRatio(p) * d2),
                _b2 => Throw<bool>(),
                _dt2 => Throw<DateTime>(),
                p2 => new DataValue(ToRatio(p) * ToRatio(p2))
            )
        );
    }

    private static DataValue Div(DataValue left, DataValue right)
    {
        return left.Match<DataValue>(
            s => Throw<string>(),
            i => right.Match<DataValue>(
                _s => Throw<string>(),
                i2 => new DataValue((decimal)i / i2),
                d2 => new DataValue(i / d2),
                _b => Throw<bool>(),
                _dt => Throw<DateTime>(),
                p2 => new DataValue(i / ToRatio(p2))
            ),
            d => right.Match<DataValue>(
                _s => Throw<string>(),
                i2 => new DataValue(d / i2),
                d2 => new DataValue(d / d2),
                _b => Throw<bool>(),
                _dt => Throw<DateTime>(),
                p2 => new DataValue(d / ToRatio(p2))
            ),
            _b => Throw<bool>(),
            _dt => Throw<DateTime>(),
            p => right.Match<DataValue>(
                _s => Throw<string>(),
                i2 => new DataValue(ToRatio(p) / i2),
                d2 => new DataValue(ToRatio(p) / d2),
                _b2 => Throw<bool>(),
                _dt2 => Throw<DateTime>(),
                p2 => new DataValue(ToRatio(p) / ToRatio(p2))
            )
        );
    }

    private static DataValue Throw<T>(string? msg = null) => throw new InvalidOperationException(msg ?? $"Operation not supported for {typeof(T).Name}.");

    private static decimal ToRatio(Percentage p)
    {
        return p.Value / 100m;
    }
}
