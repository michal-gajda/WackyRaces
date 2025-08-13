public readonly record struct Function
{
    public string Value { get; private init; }

    public Function(string value)
    {
        this.Value = value;
    }
}
