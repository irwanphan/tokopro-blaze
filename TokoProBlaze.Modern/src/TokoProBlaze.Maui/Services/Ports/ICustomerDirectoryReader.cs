using TokoProBlaze.Maui.Services.Models;

namespace TokoProBlaze.Maui.Services.Ports;

/// <summary>
/// Abstraksi akses data pelanggan; implementasi memanggil API wrapper legacy (bukan DB langsung dari UI).
/// </summary>
public interface ICustomerDirectoryReader
{
    Task<IReadOnlyList<CustomerRow>> GetCustomersAsync(string? keyword, CancellationToken cancellationToken = default);

    Task<CustomersPage> GetCustomersPageAsync(int page, int pageSize, string? keyword, CancellationToken cancellationToken = default);

    Task<CustomerRow?> GetCustomerByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<LegacyDbHealth> GetLegacyDbHealthAsync(CancellationToken cancellationToken = default);
}
