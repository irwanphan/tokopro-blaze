using TokoProBlaze.Blazor.Services;

namespace TokoProBlaze.Blazor.Models;

/// <summary>
/// Baris grid Master Barang &amp; Jasa — kolom diselaraskan dengan <c>tbbarang</c> (API).
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
        Satuan2 = p.Unit2,
        Satuan3 = p.Unit3,
        Barcode = p.Barcode,
        Tipe = p.Tipe,
        Divisi = p.Divisi,
        Merk = p.Merk,
        Grup = p.Grup,
        Ukuran = p.Ukuran,
        Aktif = p.IsActive
    };
}
