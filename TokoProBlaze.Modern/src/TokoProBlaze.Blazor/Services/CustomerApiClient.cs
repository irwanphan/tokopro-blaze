using System.Net.Http.Json;

namespace TokoProBlaze.Blazor.Services;

public sealed record CustomerVm(string Code, string Name, string City, bool IsActive);
public sealed record LegacyDbHealthVm(bool IsHealthy, string Mode, string Message);
public sealed record CustomersPageVm(int Page, int PageSize, int TotalItems, int TotalPages, IReadOnlyList<CustomerVm> Items);

public sealed class CustomerApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<CustomerVm>> GetCustomersAsync(string? keyword, CancellationToken cancellationToken = default)
    {
        var endpoint = string.IsNullOrWhiteSpace(keyword)
            ? "/api/customers"
            : $"/api/customers?q={Uri.EscapeDataString(keyword)}";

        var result = await httpClient.GetFromJsonAsync<IReadOnlyList<CustomerVm>>(endpoint, cancellationToken);
        return result ?? [];
    }

    public async Task<CustomersPageVm> GetCustomersPageAsync(int page, int pageSize, string? keyword, CancellationToken cancellationToken = default)
    {
        var endpoint = $"/api/customers/page?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            endpoint = $"{endpoint}&q={Uri.EscapeDataString(keyword)}";
        }

        var result = await httpClient.GetFromJsonAsync<CustomersPageVm>(endpoint, cancellationToken);
        return result ?? new CustomersPageVm(1, pageSize, 0, 1, []);
    }

    public async Task<CustomerVm?> GetCustomerByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<CustomerVm>($"/api/customers/detail?code={Uri.EscapeDataString(code)}", cancellationToken);
    }

    public async Task<LegacyDbHealthVm> GetLegacyDbHealthAsync(CancellationToken cancellationToken = default)
    {
        var result = await httpClient.GetFromJsonAsync<LegacyDbHealthVm>("/api/health/legacy-db", cancellationToken);
        return result ?? new LegacyDbHealthVm(false, "fallback", "Status koneksi tidak tersedia.");
    }
}
