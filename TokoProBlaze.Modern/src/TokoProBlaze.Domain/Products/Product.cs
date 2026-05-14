namespace TokoProBlaze.Domain.Products;

public sealed class Product
{
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required string Unit1 { get; init; }
    public bool IsActive { get; init; }
}
