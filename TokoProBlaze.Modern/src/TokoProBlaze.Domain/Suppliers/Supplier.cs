namespace TokoProBlaze.Domain.Suppliers;

public sealed class Supplier
{
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required string City { get; init; }
    public bool IsActive { get; init; }
}
