using TokoProBlaze.Domain.Suppliers;

namespace TokoProBlaze.Application.Suppliers;

public sealed record SupplierDto(string Code, string Name, string City, bool IsActive);

public interface ISupplierRepository
{
    Task<IReadOnlyList<Supplier>> GetAllAsync(CancellationToken cancellationToken = default);
}

public interface ISupplierService
{
    Task<IReadOnlyList<SupplierDto>> GetSuppliersAsync(string? keyword, CancellationToken cancellationToken = default);
    Task<SupplierDto?> GetSupplierByCodeAsync(string code, CancellationToken cancellationToken = default);
}

public sealed class SupplierService(ISupplierRepository supplierRepository) : ISupplierService
{
    public async Task<IReadOnlyList<SupplierDto>> GetSuppliersAsync(string? keyword, CancellationToken cancellationToken = default)
    {
        var suppliers = await supplierRepository.GetAllAsync(cancellationToken);
        IEnumerable<Supplier> query = suppliers;

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(s =>
                s.Code.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                s.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                s.City.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }

        return query
            .OrderBy(s => s.Name)
            .Select(s => new SupplierDto(s.Code, s.Name, s.City, s.IsActive))
            .ToArray();
    }

    public async Task<SupplierDto?> GetSupplierByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return null;
        }

        var suppliers = await supplierRepository.GetAllAsync(cancellationToken);
        var supplier = suppliers.FirstOrDefault(s => s.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
        return supplier is null
            ? null
            : new SupplierDto(supplier.Code, supplier.Name, supplier.City, supplier.IsActive);
    }
}
