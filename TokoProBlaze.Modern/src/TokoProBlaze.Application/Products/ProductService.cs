using TokoProBlaze.Domain.Products;

namespace TokoProBlaze.Application.Products;

public sealed record ProductDto(string Code, string Name, string Unit1, bool IsActive);

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default);
}

public interface IProductService
{
    Task<IReadOnlyList<ProductDto>> GetProductsAsync(string? keyword, CancellationToken cancellationToken = default);
    Task<ProductDto?> GetProductByCodeAsync(string code, CancellationToken cancellationToken = default);
}

public sealed class ProductService(IProductRepository productRepository) : IProductService
{
    public async Task<IReadOnlyList<ProductDto>> GetProductsAsync(string? keyword, CancellationToken cancellationToken = default)
    {
        var products = await productRepository.GetAllAsync(cancellationToken);
        IEnumerable<Product> query = products;

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(p =>
                p.Code.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                p.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                p.Unit1.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }

        return query
            .OrderBy(p => p.Name)
            .Select(p => new ProductDto(p.Code, p.Name, p.Unit1, p.IsActive))
            .ToArray();
    }

    public async Task<ProductDto?> GetProductByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return null;
        }

        var products = await productRepository.GetAllAsync(cancellationToken);
        var product = products.FirstOrDefault(p => p.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
        return product is null
            ? null
            : new ProductDto(product.Code, product.Name, product.Unit1, product.IsActive);
    }
}
