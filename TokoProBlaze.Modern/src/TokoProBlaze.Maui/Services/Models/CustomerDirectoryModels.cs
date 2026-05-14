namespace TokoProBlaze.Maui.Services.Models;

public sealed record CustomerRow(string Code, string Name, string City, bool IsActive);

public sealed record CustomersPage(int Page, int PageSize, int TotalItems, int TotalPages, IReadOnlyList<CustomerRow> Items);

public sealed record LegacyDbHealth(bool IsHealthy, string Mode, string Message);
