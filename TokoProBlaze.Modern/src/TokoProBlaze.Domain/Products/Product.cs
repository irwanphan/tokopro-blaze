namespace TokoProBlaze.Domain.Products;

public sealed class Product
{
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required string Unit1 { get; init; }
    public string Unit2 { get; init; } = "";
    public string Unit3 { get; init; } = "";
    public string Barcode { get; init; } = "";
    public string Tipe { get; init; } = "";
    public string Divisi { get; init; } = "";
    public string Merk { get; init; } = "";
    public string Grup { get; init; } = "";
    public string Ukuran { get; init; } = "";
    public bool IsActive { get; init; }
}
