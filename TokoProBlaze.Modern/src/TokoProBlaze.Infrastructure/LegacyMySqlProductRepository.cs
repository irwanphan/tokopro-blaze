using MySqlConnector;
using TokoProBlaze.Application.Products;
using TokoProBlaze.Domain.Products;

namespace TokoProBlaze.Infrastructure;

public sealed class LegacyMySqlProductRepository(string connectionString) : IProductRepository
{
    private static readonly IReadOnlyList<Product> FallbackSeed =
    [
        new()
        {
            Code = "BRG001",
            Name = "Contoh Barang A",
            Unit1 = "PCS",
            Unit2 = "BOX",
            Unit3 = "",
            Barcode = "899001",
            Tipe = "J",
            Divisi = "D01",
            Merk = "M01",
            Grup = "G01",
            Ukuran = "-",
            IsActive = true
        },
        new()
        {
            Code = "BRG002",
            Name = "Contoh Barang B",
            Unit1 = "BOX",
            Unit2 = "",
            Unit3 = "",
            Barcode = "",
            Tipe = "J",
            Divisi = "D01",
            Merk = "M02",
            Grup = "G02",
            Ukuran = "L",
            IsActive = true
        },
        new()
        {
            Code = "BRG003",
            Name = "Contoh Barang C",
            Unit1 = "PCS",
            Unit2 = "",
            Unit3 = "",
            Barcode = "",
            Tipe = "J",
            Divisi = "",
            Merk = "",
            Grup = "",
            Ukuran = "",
            IsActive = false
        }
    ];

    public Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default) =>
        Task.Run(() =>
        {
            const string sql = """
                SELECT
                    Kode,
                    Nama,
                    Satuan1,
                    IFNULL(Satuan2, '') AS Satuan2,
                    IFNULL(Satuan3, '') AS Satuan3,
                    IFNULL(Barcode, '') AS Barcode,
                    IFNULL(Tipe, '') AS Tipe,
                    IFNULL(KodeDivisi, '') AS KodeDivisi,
                    IFNULL(KodeMerk, '') AS KodeMerk,
                    IFNULL(KodeGrup, '') AS KodeGrup,
                    IFNULL(Ukuran, '') AS Ukuran,
                    bNonAktif
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
                    result.Add(ReadProduct(reader));
                }
            }
            catch
            {
                return FallbackSeed;
            }

            return (IReadOnlyList<Product>)result;
        }, cancellationToken);

    public Task<Product?> GetByCodeAsync(string code, CancellationToken cancellationToken = default) =>
        Task.Run(() =>
        {
            const string sql = """
                SELECT
                    Kode,
                    Nama,
                    Satuan1,
                    IFNULL(Satuan2, '') AS Satuan2,
                    IFNULL(Satuan3, '') AS Satuan3,
                    IFNULL(Barcode, '') AS Barcode,
                    IFNULL(Tipe, '') AS Tipe,
                    IFNULL(KodeDivisi, '') AS KodeDivisi,
                    IFNULL(KodeMerk, '') AS KodeMerk,
                    IFNULL(KodeGrup, '') AS KodeGrup,
                    IFNULL(Ukuran, '') AS Ukuran,
                    bNonAktif
                FROM tbbarang
                WHERE Kode = @Kode
                LIMIT 1
                """;

            try
            {
                using var connection = new MySqlConnection(connectionString);
                using var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Kode", code);
                connection.Open();

                using var reader = command.ExecuteReader();
                return reader.Read() ? ReadProduct(reader) : null;
            }
            catch
            {
                return null;
            }
        }, cancellationToken);

    public Task<bool> CreateAsync(Product product, CancellationToken cancellationToken = default) =>
        Task.Run(() =>
        {
            const string sql = """
                INSERT INTO tbbarang (Kode, Nama, Satuan1, bNonAktif)
                VALUES (@Kode, @Nama, @Satuan1, @bNonAktif)
                """;

            try
            {
                using var connection = new MySqlConnection(connectionString);
                using var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Kode", product.Code);
                command.Parameters.AddWithValue("@Nama", product.Name);
                command.Parameters.AddWithValue("@Satuan1", product.Unit1);
                command.Parameters.AddWithValue("@bNonAktif", product.IsActive ? 0 : 1);
                connection.Open();
                command.ExecuteNonQuery();
                return true;
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                return false;
            }
            catch
            {
                return false;
            }
        }, cancellationToken);

    public Task<bool> UpdateAsync(Product product, CancellationToken cancellationToken = default) =>
        Task.Run(() =>
        {
            const string sql = """
                UPDATE tbbarang
                SET Nama = @Nama,
                    Satuan1 = @Satuan1,
                    bNonAktif = @bNonAktif
                WHERE Kode = @Kode
                """;

            try
            {
                using var connection = new MySqlConnection(connectionString);
                using var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Kode", product.Code);
                command.Parameters.AddWithValue("@Nama", product.Name);
                command.Parameters.AddWithValue("@Satuan1", product.Unit1);
                command.Parameters.AddWithValue("@bNonAktif", product.IsActive ? 0 : 1);
                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch
            {
                return false;
            }
        }, cancellationToken);

    private static Product ReadProduct(MySqlDataReader reader)
    {
        var nonActiveValue = reader["bNonAktif"]?.ToString();
        var isNonActive = nonActiveValue is "1" or "True" or "true";

        return new Product
        {
            Code = reader["Kode"]?.ToString() ?? string.Empty,
            Name = reader["Nama"]?.ToString() ?? string.Empty,
            Unit1 = reader["Satuan1"]?.ToString() ?? string.Empty,
            Unit2 = reader["Satuan2"]?.ToString() ?? string.Empty,
            Unit3 = reader["Satuan3"]?.ToString() ?? string.Empty,
            Barcode = reader["Barcode"]?.ToString() ?? string.Empty,
            Tipe = reader["Tipe"]?.ToString() ?? string.Empty,
            Divisi = reader["KodeDivisi"]?.ToString() ?? string.Empty,
            Merk = reader["KodeMerk"]?.ToString() ?? string.Empty,
            Grup = reader["KodeGrup"]?.ToString() ?? string.Empty,
            Ukuran = reader["Ukuran"]?.ToString() ?? string.Empty,
            IsActive = !isNonActive
        };
    }
}

public sealed class FallbackProductRepository : IProductRepository
{
    private static readonly object Gate = new();
    private static readonly List<Product> Items =
    [
        new()
        {
            Code = "BRG001",
            Name = "Contoh Barang A",
            Unit1 = "PCS",
            Unit2 = "BOX",
            Unit3 = "",
            Barcode = "899001",
            Tipe = "J",
            Divisi = "D01",
            Merk = "M01",
            Grup = "G01",
            Ukuran = "-",
            IsActive = true
        },
        new()
        {
            Code = "BRG002",
            Name = "Contoh Barang B",
            Unit1 = "BOX",
            Unit2 = "",
            Unit3 = "",
            Barcode = "",
            Tipe = "J",
            Divisi = "D01",
            Merk = "M02",
            Grup = "G02",
            Ukuran = "L",
            IsActive = true
        },
        new()
        {
            Code = "BRG003",
            Name = "Contoh Barang C",
            Unit1 = "PCS",
            Unit2 = "",
            Unit3 = "",
            Barcode = "",
            Tipe = "J",
            Divisi = "",
            Merk = "",
            Grup = "",
            Ukuran = "",
            IsActive = false
        }
    ];

    public Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        lock (Gate)
        {
            return Task.FromResult((IReadOnlyList<Product>)Items.OrderBy(p => p.Name).ToList());
        }
    }

    public Task<Product?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        lock (Gate)
        {
            var match = Items.FirstOrDefault(p => p.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(match);
        }
    }

    public Task<bool> CreateAsync(Product product, CancellationToken cancellationToken = default)
    {
        lock (Gate)
        {
            if (Items.Any(p => p.Code.Equals(product.Code, StringComparison.OrdinalIgnoreCase)))
            {
                return Task.FromResult(false);
            }

            Items.Add(product);
            return Task.FromResult(true);
        }
    }

    public Task<bool> UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        lock (Gate)
        {
            var index = Items.FindIndex(p => p.Code.Equals(product.Code, StringComparison.OrdinalIgnoreCase));
            if (index < 0)
            {
                return Task.FromResult(false);
            }

            Items[index] = product;
            return Task.FromResult(true);
        }
    }
}
