using System.Net.Http.Json;

namespace TokoProBlaze.Blazor.Services;

public sealed record SupplierVm(string Code, string Name, string City, bool IsActive);
public sealed record SuppliersPageVm(int Page, int PageSize, int TotalItems, int TotalPages, IReadOnlyList<SupplierVm> Items);

public sealed class SupplierApiClient(HttpClient httpClient)
{
    public async Task<SuppliersPageVm> GetSuppliersPageAsync(int page, int pageSize, string? keyword, CancellationToken cancellationToken = default)
    {
        var endpoint = $"/api/suppliers/page?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            endpoint = $"{endpoint}&q={Uri.EscapeDataString(keyword)}";
        }

        var result = await httpClient.GetFromJsonAsync<SuppliersPageVm>(endpoint, cancellationToken);
        return result ?? new SuppliersPageVm(1, pageSize, 0, 1, []);
    }

    public async Task<SupplierVm?> GetSupplierByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<SupplierVm>($"/api/suppliers/detail?code={Uri.EscapeDataString(code)}", cancellationToken);
    }
}
