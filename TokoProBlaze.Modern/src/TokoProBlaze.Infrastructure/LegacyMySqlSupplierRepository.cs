using MySqlConnector;
using TokoProBlaze.Application.Suppliers;
using TokoProBlaze.Domain.Suppliers;

namespace TokoProBlaze.Infrastructure;

public sealed class LegacyMySqlSupplierRepository(string connectionString) : ISupplierRepository
{
    private static readonly IReadOnlyList<Supplier> FallbackSeed =
    [
        new() { Code = "SUPP001", Name = "CV Sumber Rejeki", City = "Jakarta", IsActive = true },
        new() { Code = "SUPP002", Name = "PT Mitra Niaga", City = "Surabaya", IsActive = true },
        new() { Code = "SUPP003", Name = "UD Lama", City = "Bandung", IsActive = false }
    ];

    public Task<IReadOnlyList<Supplier>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            const string sql = """
                SELECT Kode, Nama, Kota, bNonAktif
                FROM tbpemasok
                ORDER BY Nama ASC
                """;

            var result = new List<Supplier>();

            try
            {
                using var connection = new MySqlConnection(connectionString);
                using var command = new MySqlCommand(sql, connection);
                connection.Open();

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var nonActiveValue = reader["bNonAktif"]?.ToString();
                    var isNonActive = nonActiveValue is "1" or "True" or "true";

                    result.Add(new Supplier
                    {
                        Code = reader["Kode"]?.ToString() ?? string.Empty,
                        Name = reader["Nama"]?.ToString() ?? string.Empty,
                        City = reader["Kota"]?.ToString() ?? string.Empty,
                        IsActive = !isNonActive
                    });
                }
            }
            catch
            {
                return FallbackSeed;
            }

            return (IReadOnlyList<Supplier>)result;
        }, cancellationToken);
    }
}

public sealed class FallbackSupplierRepository : ISupplierRepository
{
    private static readonly IReadOnlyList<Supplier> Seed =
    [
        new() { Code = "SUPP001", Name = "CV Sumber Rejeki", City = "Jakarta", IsActive = true },
        new() { Code = "SUPP002", Name = "PT Mitra Niaga", City = "Surabaya", IsActive = true },
        new() { Code = "SUPP003", Name = "UD Lama", City = "Bandung", IsActive = false }
    ];

    public Task<IReadOnlyList<Supplier>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Seed);
    }
}
