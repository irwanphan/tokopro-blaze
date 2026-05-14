using TokoProBlaze.Application.Customers;
using TokoProBlaze.Domain.Customers;
using MySqlConnector;

namespace TokoProBlaze.Infrastructure;

public sealed class LegacyMySqlCustomerRepository(string connectionString) : ICustomerRepository
{
    private static readonly IReadOnlyList<Customer> FallbackSeed =
    [
        new() { Code = "CUST001", Name = "PT Maju Bersama", City = "Surabaya", IsActive = true },
        new() { Code = "CUST002", Name = "CV Sentosa Jaya", City = "Sidoarjo", IsActive = true },
        new() { Code = "CUST003", Name = "UD Bintang Timur", City = "Malang", IsActive = false }
    ];

    public Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            // Query is aligned with legacy VB.NET module (tbpelanggan).
            const string sql = """
                SELECT Kode, Nama, Kota, bNonAktif
                FROM tbpelanggan
                ORDER BY Nama ASC
                """;

            var result = new List<Customer>();

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

                    result.Add(new Customer
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

            return (IReadOnlyList<Customer>)result;
        }, cancellationToken);
    }
}

public sealed class FallbackCustomerRepository : ICustomerRepository
{
    private static readonly IReadOnlyList<Customer> Seed =
    [
        new() { Code = "CUST001", Name = "PT Maju Bersama", City = "Surabaya", IsActive = true },
        new() { Code = "CUST002", Name = "CV Sentosa Jaya", City = "Sidoarjo", IsActive = true },
        new() { Code = "CUST003", Name = "UD Bintang Timur", City = "Malang", IsActive = false }
    ];

    public Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Seed);
    }
}
