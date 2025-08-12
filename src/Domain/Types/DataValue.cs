namespace WackyRaces.Domain.Types;

using OneOf;

[GenerateOneOf]
public sealed partial class DataValue : OneOfBase<string, int, decimal, bool, DateTime, Percentage, Function>
{
}
