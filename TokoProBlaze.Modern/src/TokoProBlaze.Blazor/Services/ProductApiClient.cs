using System.Net.Http.Json;
using System.Text.Json;

namespace TokoProBlaze.Blazor.Services;

public sealed record ProductVm(string Code, string Name, string Unit1, bool IsActive);
public sealed record ProductsPageVm(int Page, int PageSize, int TotalItems, int TotalPages, IReadOnlyList<ProductVm> Items);

public sealed record ProductCreateRequest(string Code, string Name, string Unit1, bool IsActive);

public sealed record ProductUpdateRequest(string Name, string Unit1, bool IsActive);

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

    public async Task<(bool Ok, string? ErrorMessage, ProductVm? Product)> CreateProductAsync(ProductCreateRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("/api/products", request, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            var product = await response.Content.ReadFromJsonAsync<ProductVm>(cancellationToken: cancellationToken);
            return (true, null, product);
        }

        var message = await TryReadErrorAsync(response, cancellationToken);
        return (false, message, null);
    }

    public async Task<(bool Ok, string? ErrorMessage, ProductVm? Product)> UpdateProductAsync(string code, ProductUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PutAsJsonAsync($"/api/products?code={Uri.EscapeDataString(code)}", request, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            var product = await response.Content.ReadFromJsonAsync<ProductVm>(cancellationToken: cancellationToken);
            return (true, null, product);
        }

        var message = await TryReadErrorAsync(response, cancellationToken);
        return (false, message, null);
    }

    private static async Task<string?> TryReadErrorAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        try
        {
            var json = await response.Content.ReadFromJsonAsync<Dictionary<string, JsonElement>>(cancellationToken: cancellationToken);
            if (json is not null && json.TryGetValue("message", out var el))
            {
                return el.GetString();
            }
        }
        catch
        {
            // ignored
        }

        return $"HTTP {(int)response.StatusCode}";
    }
}
