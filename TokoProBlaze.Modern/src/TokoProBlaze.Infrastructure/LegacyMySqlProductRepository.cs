using MySqlConnector;
using TokoProBlaze.Application.Products;
using TokoProBlaze.Domain.Products;

namespace TokoProBlaze.Infrastructure;

public sealed class LegacyMySqlProductRepository(string connectionString) : IProductRepository
{
    private static readonly IReadOnlyList<Product> FallbackSeed =
    [
        new() { Code = "BRG001", Name = "Contoh Barang A", Unit1 = "PCS", IsActive = true },
        new() { Code = "BRG002", Name = "Contoh Barang B", Unit1 = "BOX", IsActive = true },
        new() { Code = "BRG003", Name = "Contoh Barang C", Unit1 = "PCS", IsActive = false }
    ];

    public Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            const string sql = """
                SELECT Kode, Nama, Satuan1, bNonAktif
                FROM tbbarang
                ORDER BY Nama ASC
                """;

            var result = new List<Product>();

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

                    result.Add(new Product
                    {
                        Code = reader["Kode"]?.ToString() ?? string.Empty,
                        Name = reader["Nama"]?.ToString() ?? string.Empty,
                        Unit1 = reader["Satuan1"]?.ToString() ?? string.Empty,
                        IsActive = !isNonActive
                    });
                }
            }
            catch
            {
                return FallbackSeed;
            }

            return (IReadOnlyList<Product>)result;
        }, cancellationToken);
    }
}

public sealed class FallbackProductRepository : IProductRepository
{
    private static readonly IReadOnlyList<Product> Seed =
    [
        new() { Code = "BRG001", Name = "Contoh Barang A", Unit1 = "PCS", IsActive = true },
        new() { Code = "BRG002", Name = "Contoh Barang B", Unit1 = "BOX", IsActive = true },
        new() { Code = "BRG003", Name = "Contoh Barang C", Unit1 = "PCS", IsActive = false }
    ];

    public Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Seed);
    }
}
