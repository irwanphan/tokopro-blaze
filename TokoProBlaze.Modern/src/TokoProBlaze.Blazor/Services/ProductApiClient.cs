using System.Net.Http.Json;

namespace TokoProBlaze.Blazor.Services;

public sealed record ProductVm(string Code, string Name, string Unit1, bool IsActive);
public sealed record ProductsPageVm(int Page, int PageSize, int TotalItems, int TotalPages, IReadOnlyList<ProductVm> Items);

public sealed class ProductApiClient(HttpClient httpClient)
{
    public async Task<ProductsPageVm> GetProductsPageAsync(int page, int pageSize, string? keyword, CancellationToken cancellationToken = default)
    {
        var endpoint = $"/api/products/page?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            endpoint = $"{endpoint}&q={Uri.EscapeDataString(keyword)}";
        }

        var result = await httpClient.GetFromJsonAsync<ProductsPageVm>(endpoint, cancellationToken);
        return result ?? new ProductsPageVm(1, pageSize, 0, 1, []);
    }

    public async Task<ProductVm?> GetProductByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<ProductVm>($"/api/products/detail?code={Uri.EscapeDataString(code)}", cancellationToken);
    }
}
