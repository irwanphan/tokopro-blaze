namespace TokoProBlaze.Domain.Customers;

public sealed class Customer
{
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required string City { get; init; }
    public bool IsActive { get; init; }
}
