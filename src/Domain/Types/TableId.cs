namespace WackyRaces.Domain.Types;

public readonly record struct TableId
{
    public Guid Value { get; private init; }

    public TableId(Guid value)
    {
        this.Value = value;
    }
}
