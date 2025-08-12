using WackyRaces.Domain.Exceptions;

public readonly record struct Percentage
{
    public decimal Value { get; private init; }

    public Percentage(decimal value)
    {
        this.Value = value;
    }

    public static Percentage Parse(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new EmptyPercentageTextException();
        }

        var trimmed = text.Trim();

        if (trimmed.EndsWith('%') is false)
        {
            throw new InvalidPercentageFormatException(trimmed);
        }

        var numberPart = trimmed.Substring(0, trimmed.Length - 1);

        if (decimal.TryParse(numberPart, out var value) is false)
        {
            throw new InvalidPercentageFormatException(trimmed);
        }

        return new Percentage(value);
    }

    public static bool TryParse(string text, out Percentage percentage)
    {
        try
        {
            percentage = Parse(text);
            return true;
        }
        catch
        {
            percentage = default;
            return false;
        }
    }

    public override string ToString()
    {
        return $"{this.Value}%";
    }

    /// <summary>
    /// Converts the percentage to its decimal ratio representation.
    /// For example, 15% becomes 0.15, 50% becomes 0.5, 100% becomes 1.0
    /// </summary>
    public decimal ToDecimal()
    {
        return this.Value / 100m;
    }
}
