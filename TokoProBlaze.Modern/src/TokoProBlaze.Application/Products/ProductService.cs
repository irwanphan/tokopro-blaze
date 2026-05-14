using TokoProBlaze.Domain.Products;

namespace TokoProBlaze.Application.Products;

public sealed record ProductDto(
    string Code,
    string Name,
    string Unit1,
    bool IsActive,
    string Unit2 = "",
    string Unit3 = "",
    string Barcode = "",
    string Tipe = "",
    string Divisi = "",
    string Merk = "",
    string Grup = "",
    string Ukuran = "");

public sealed record ProductCreateCommand(string Code, string Name, string Unit1, bool IsActive);

public sealed record ProductUpdateCommand(string Name, string Unit1, bool IsActive);

public enum ProductSaveError
{
    None,
    Validation,
    DuplicateCode,
    NotFound
}

public sealed record ProductSaveResult(bool Ok, ProductDto? Product, ProductSaveError Error, string? Message);

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Product?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<bool> CreateAsync(Product product, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(Product product, CancellationToken cancellationToken = default);
}

public interface IProductService
{
    Task<IReadOnlyList<ProductDto>> GetProductsAsync(string? keyword, CancellationToken cancellationToken = default);

    Task<ProductDto?> GetProductByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<ProductSaveResult> CreateProductAsync(ProductCreateCommand command, CancellationToken cancellationToken = default);

    Task<ProductSaveResult> UpdateProductAsync(string code, ProductUpdateCommand command, CancellationToken cancellationToken = default);
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
                p.Unit1.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                p.Unit2.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                p.Unit3.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                p.Barcode.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                p.Tipe.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                p.Divisi.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                p.Merk.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                p.Grup.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                p.Ukuran.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }

        return query
            .OrderBy(p => p.Name)
            .Select(MapDto)
            .ToArray();
    }

    public async Task<ProductDto?> GetProductByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return null;
        }

        var product = await productRepository.GetByCodeAsync(code.Trim(), cancellationToken);
        return product is null
            ? null
            : MapDto(product);
    }

    public async Task<ProductSaveResult> CreateProductAsync(ProductCreateCommand command, CancellationToken cancellationToken = default)
    {
        var validation = Validate(command.Code, command.Name, command.Unit1);
        if (validation is not null)
        {
            return new ProductSaveResult(false, null, ProductSaveError.Validation, validation);
        }

        var code = command.Code.Trim();
        var existing = await productRepository.GetByCodeAsync(code, cancellationToken);
        if (existing is not null)
        {
            return new ProductSaveResult(false, null, ProductSaveError.DuplicateCode, "Kode barang sudah digunakan.");
        }

        var product = new Product
        {
            Code = code,
            Name = command.Name.Trim(),
            Unit1 = command.Unit1.Trim(),
            IsActive = command.IsActive
        };

        var created = await productRepository.CreateAsync(product, cancellationToken);
        if (!created)
        {
            return new ProductSaveResult(false, null, ProductSaveError.DuplicateCode, "Gagal menyimpan: kode duplikat.");
        }

        var reloaded = await productRepository.GetByCodeAsync(code, cancellationToken);
        return reloaded is null
            ? new ProductSaveResult(false, null, ProductSaveError.DuplicateCode, "Gagal memuat barang baru.")
            : new ProductSaveResult(true, MapDto(reloaded), ProductSaveError.None, null);
    }

    public async Task<ProductSaveResult> UpdateProductAsync(string code, ProductUpdateCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return new ProductSaveResult(false, null, ProductSaveError.Validation, "Kode barang wajib diisi.");
        }

        var validation = Validate(code, command.Name, command.Unit1);
        if (validation is not null)
        {
            return new ProductSaveResult(false, null, ProductSaveError.Validation, validation);
        }

        var trimmedCode = code.Trim();
        var existing = await productRepository.GetByCodeAsync(trimmedCode, cancellationToken);
        if (existing is null)
        {
            return new ProductSaveResult(false, null, ProductSaveError.NotFound, "Barang tidak ditemukan.");
        }

        var product = new Product
        {
            Code = existing.Code,
            Name = command.Name.Trim(),
            Unit1 = command.Unit1.Trim(),
            IsActive = command.IsActive,
            Unit2 = existing.Unit2,
            Unit3 = existing.Unit3,
            Barcode = existing.Barcode,
            Tipe = existing.Tipe,
            Divisi = existing.Divisi,
            Merk = existing.Merk,
            Grup = existing.Grup,
            Ukuran = existing.Ukuran
        };

        var updated = await productRepository.UpdateAsync(product, cancellationToken);
        if (!updated)
        {
            return new ProductSaveResult(false, null, ProductSaveError.NotFound, "Barang tidak ditemukan.");
        }

        return new ProductSaveResult(true, MapDto(product), ProductSaveError.None, null);
    }

    private static string? Validate(string code, string name, string unit1)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return "Kode barang wajib diisi.";
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return "Nama barang wajib diisi.";
        }

        if (string.IsNullOrWhiteSpace(unit1))
        {
            return "Satuan wajib diisi.";
        }

        return null;
    }

    private static ProductDto MapDto(Product p) =>
        new(
            p.Code,
            p.Name,
            p.Unit1,
            p.IsActive,
            p.Unit2,
            p.Unit3,
            p.Barcode,
            p.Tipe,
            p.Divisi,
            p.Merk,
            p.Grup,
            p.Ukuran);
}
