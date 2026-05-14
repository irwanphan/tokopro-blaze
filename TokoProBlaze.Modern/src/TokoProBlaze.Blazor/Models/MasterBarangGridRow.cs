using TokoProBlaze.Blazor.Services;

namespace TokoProBlaze.Blazor.Models;

/// <summary>
/// Baris grid Master Barang &amp; Jasa. Kolom diperluas mengikuti legacy; nilai non-API ditampilkan kosong sampai mapping DB ditambahkan.
/// </summary>
public sealed class MasterBarangGridRow
{
    public int No { get; init; }

    public string Kode { get; init; } = "";

    public string Nama { get; init; } = "";

    public string Satuan1 { get; init; } = "";

    public string Satuan2 { get; init; } = "";

    public string Satuan3 { get; init; } = "";

    public string Barcode { get; init; } = "";

    public string Tipe { get; init; } = "";

    public string Divisi { get; init; } = "";

    public string Merk { get; init; } = "";

    public string Grup { get; init; } = "";

    public string Ukuran { get; init; } = "";

    public bool Aktif { get; init; }

    public static MasterBarangGridRow FromProduct(ProductVm p, int rowNumber) => new()
    {
        No = rowNumber,
        Kode = p.Code,
        Nama = p.Name,
        Satuan1 = p.Unit1,
        Aktif = p.IsActive
    };
}
