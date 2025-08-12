public readonly record struct Percentage
{
    public decimal Value { get; private init; }
    public Percentage(decimal value)
    {
        this.Value = value;
    }
}
