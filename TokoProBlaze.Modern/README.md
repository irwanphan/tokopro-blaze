# TokoProBlaze Modern - Sprint 1–3

Baseline migrasi bertahap dari legacy VB.NET ke Blazor dengan pola wrapper API.

## Struktur

- `TokoProBlaze.Domain`: entitas inti domain.
- `TokoProBlaze.Application`: use case/service + kontrak repository.
- `TokoProBlaze.Infrastructure`: implementasi adapter ke data legacy (sementara mock).
- `TokoProBlaze.Api`: API wrapper agar UI tidak akses DB langsung.
- `TokoProBlaze.Blazor`: UI Blazor untuk modul awal.

## Modul Sprint 3

- Master Pemasok (read-only, pola sama dengan pelanggan).
- Endpoint API:
  - `GET /api/suppliers?q=...`
  - `GET /api/suppliers/page?page=1&pageSize=20&q=...`
  - `GET /api/suppliers/detail?code=...`
- Halaman Blazor: `/master-pemasok`.
- Data dari tabel legacy `tbpemasok (Kode, Nama, Kota, bNonAktif)`.

## Modul Sprint 2

- Master Barang (read-only, pola sama dengan pelanggan).
- Endpoint API:
  - `GET /api/products?q=...`
  - `GET /api/products/page?page=1&pageSize=20&q=...`
  - `GET /api/products/detail?code=...`
- Halaman Blazor: `/master-barang`.
- Data dari tabel legacy `tbbarang (Kode, Nama, Satuan1, bNonAktif)`.

## Modul Sprint 1

- Master Pelanggan (end-to-end).
- Endpoint API:
  - `GET /api/customers?q=...`
  - `GET /api/customers/page?page=1&pageSize=20&q=...`
  - `GET /api/customers/detail?code=...`
  - `GET /api/health/legacy-db`
- Halaman Blazor: `/master-pelanggan`.
- Repository pelanggan sudah membaca data nyata dari tabel legacy `tbpelanggan` (query yang sama dengan VB.NET), memakai `MySqlConnector`.
- Jika koneksi belum diisi/invalid, sistem fallback ke data sample agar UI tetap bisa dites.

## Konfigurasi DB Legacy

Isi konfigurasi koneksi legacy dengan environment variable:

- `TOKOPRO_LEGACY_MYSQL`
- contoh:
  - `Server=127.0.0.1;Port=3306;User Id=root;Password=...;Database=tokopro_nja;Allow User Variables=True;Allow Zero DateTime=No;SslMode=None`

Catatan:

- `appsettings.json` sengaja tidak menyimpan password database.
- Struktur tabel pelanggan: `tbpelanggan (Kode, Nama, Kota, bNonAktif)`.
- Struktur tabel barang: `tbbarang (Kode, Nama, Satuan1, bNonAktif)`.
- Struktur tabel pemasok: `tbpemasok (Kode, Nama, Kota, bNonAktif)`.

## Kebijakan skema database (legacy)

- **Struktur database** (tabel, kolom, index, constraint, migrasi) **diurus sepenuhnya oleh tim dev legacy** di luar repo modern ini.
- Proyek **TokoProBlaze.Modern** saat ini hanya melakukan **baca** (`SELECT`) ke tabel yang sudah ada; **tidak** ada Entity Framework migrations, `EnsureCreated`, skrip `CREATE`/`ALTER`/`DROP`, atau alat lain yang mengubah skema lewat kode di repo ini.
- Perubahan kebutuhan kolom/tabel baru: **koordinasi dengan tim legacy** dulu; di sisi modern cukup menyesuaikan query/DTO **setelah** skema resmi diubah di DB.

## Menjalankan

1. Jalankan API:
   - `dotnet run --project src/TokoProBlaze.Api`
   - atau `dotnet run --project src/TokoProBlaze.Api --launch-profile http`

2. Jalankan Blazor:
   - `dotnet run --project src/TokoProBlaze.Blazor`
   - atau `dotnet run --project src/TokoProBlaze.Blazor --launch-profile http`

Pastikan `ApiBaseUrl` di `src/TokoProBlaze.Blazor/appsettings.json` mengarah ke alamat API yang aktif (default profil `http` API di repo ini: `http://localhost:5252`).

## TokoProBlaze.Maui (.NET MAUI + Blazor Hybrid)

Shell desktop/mobile memakai `BlazorWebView`; modul contoh **Master Pelanggan** memanggil `TokoProBlaze.Api` lewat `ICustomerDirectoryReader` (adapter HTTP), sejalan dengan pola wrapper API di atas.

1. Jalankan API (profil `http` di `TokoProBlaze.Api` memakai `http://localhost:5252` — sesuaikan `ApiBaseUrl` di `src/TokoProBlaze.Maui/appsettings.json` bila port Anda beda).
2. Jalankan MAUI: `dotnet build -t:Run -f net10.0-maccatalyst --project src/TokoProBlaze.Maui/TokoProBlaze.Maui.csproj` (atau buka solusi `TokoProBlaze.Modern.slnx` di IDE dan pilih target Android/iOS/Windows).

**Android emulator:** `localhost` di `appsettings.json` otomatis dipetakan ke `10.0.2.2` pada build Android. Perangkat fisik membutuhkan IP mesin dev Anda di `ApiBaseUrl`. Untuk HTTP dev, manifest memakai `usesCleartextTraffic`; sesuaikan keamanan sebelum rilis produksi.

**Catatan:** DevExpress / `.repx` belum di-wire di proyek ini; langkah berikutnya bisa menambah viewer laporan sesuai lisensi stack Anda.

**Prasyarat build:** Workload .NET MAUI + SDK platform (Android API / Xcode untuk MacCatalyst/iOS) harus cocok dengan versi .NET Anda. Jika restore gagal karena izin cache NuGet, coba `NUGET_HTTP_CACHE_PATH=/path/yang/dapat-ditulis dotnet restore`.
