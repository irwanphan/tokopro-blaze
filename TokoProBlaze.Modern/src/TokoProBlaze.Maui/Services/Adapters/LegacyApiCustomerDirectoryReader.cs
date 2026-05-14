using System.Net.Http.Json;
using TokoProBlaze.Maui.Services.Models;
using TokoProBlaze.Maui.Services.Ports;

namespace TokoProBlaze.Maui.Services.Adapters;

/// <summary>
/// Adapter HTTP ke <see cref="TokoProBlaze.Api"/> — UI hanya bergantung pada <see cref="ICustomerDirectoryReader"/>.
/// </summary>
public sealed class LegacyApiCustomerDirectoryReader(HttpClient httpClient) : ICustomerDirectoryReader
{
    public async Task<IReadOnlyList<CustomerRow>> GetCustomersAsync(string? keyword, CancellationToken cancellationToken = default)
    {
        var endpoint = string.IsNullOrWhiteSpace(keyword)
            ? "api/customers"
            : $"api/customers?q={Uri.EscapeDataString(keyword)}";

        var result = await httpClient.GetFromJsonAsync<IReadOnlyList<CustomerRow>>(endpoint, cancellationToken);
        return result ?? [];
    }

    public async Task<CustomersPage> GetCustomersPageAsync(int page, int pageSize, string? keyword, CancellationToken cancellationToken = default)
    {
        var endpoint = $"api/customers/page?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            endpoint = $"{endpoint}&q={Uri.EscapeDataString(keyword)}";
        }

        var result = await httpClient.GetFromJsonAsync<CustomersPage>(endpoint, cancellationToken);
        return result ?? new CustomersPage(1, pageSize, 0, 1, []);
    }

    public async Task<CustomerRow?> GetCustomerByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<CustomerRow>($"api/customers/detail?code={Uri.EscapeDataString(code)}", cancellationToken);
    }

    public async Task<LegacyDbHealth> GetLegacyDbHealthAsync(CancellationToken cancellationToken = default)
    {
        var result = await httpClient.GetFromJsonAsync<LegacyDbHealth>("api/health/legacy-db", cancellationToken);
        return result ?? new LegacyDbHealth(false, "fallback", "Status koneksi tidak tersedia.");
    }
}
