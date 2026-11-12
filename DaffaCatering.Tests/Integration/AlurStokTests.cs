using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace DaffaCatering.Tests.Integration;

public class AlurStokTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _admin;
    private static readonly DateOnly Hari = DateOnly.FromDateTime(DateTime.Today);

    private record Master(string IdPemasok, string IdSatuan, string IdBahan);

    public AlurStokTests(ApiFactory factory) => _admin = factory.ClientDengan("R001");

    // ---------- tes ----------

    [Fact]
    public async Task Pembelian_menghitung_total_otomatis_dari_jumlah_kali_harga()
    {
        var m = await SiapkanMaster();

        var idPembelian = await BuatPembelian(m, jumlah: 12.5m, harga: 4000m);

        var json = await _admin.GetFromJsonAsync<JsonElement>($"/api/HeaderPembelian/{idPembelian}");
        Assert.Equal(50000m, json.GetProperty("total").GetDecimal());
    }

    [Fact]
    public async Task Pembelian_dengan_pemasok_tidak_ada_ditolak()
    {
        var m = await SiapkanMaster();

        var res = await _admin.PostAsJsonAsync("/api/HeaderPembelian", new
        {
            idPembelian = Id("PBL"),
            idPemasok = Id("PMS"),
            tglPembelian = Hari,
            status = "Pending",
            details = new[] { new { idBahanBaku = m.IdBahan, idSatuan = m.IdSatuan, jumlah = 1m, harga = 1000m } }
        });

        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
    }

    [Fact]
    public async Task Penerimaan_dengan_pembelian_tidak_ada_ditolak()
    {
        var m = await SiapkanMaster();

        var res = await TerimaMentah(m, Id("PBL"), 5m, Hari.AddDays(10), "Dibuka");

        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
    }

    [Fact]
    public async Task Penerimaan_menambah_stok_per_batch_dan_bahan_ditutup_tidak_menambah()
    {
        var m = await SiapkanMaster();
        var idPembelian = await BuatPembelian(m);

        await Terima(m, idPembelian, 4m, Hari.AddDays(10));
        await Terima(m, idPembelian, 10m, Hari.AddDays(30));
        // batch yang sama (tanggal kadaluwarsa sama) hanya menambah sisa stok
        await Terima(m, idPembelian, 5m, Hari.AddDays(30));
        // bahan baku yang ditutup tidak menambah stok
        await Terima(m, idPembelian, 7m, Hari.AddDays(50), "Ditutup");

        var stok = await AmbilStok(m.IdBahan);

        Assert.Equal(2, stok.Count);
        Assert.Equal(4m, stok[Hari.AddDays(10)]);
        Assert.Equal(15m, stok[Hari.AddDays(30)]);
    }

    [Fact]
    public async Task Penggunaan_mengurangi_sisa_pada_batch_yang_dipilih_saja()
    {
        var m = await SiapkanMaster();
        var idPembelian = await BuatPembelian(m);
        await Terima(m, idPembelian, 4m, Hari.AddDays(10));
        await Terima(m, idPembelian, 10m, Hari.AddDays(30));

        await Sukses(await Gunakan(m, Hari.AddDays(30), 6m));

        var stok = await AmbilStok(m.IdBahan);
        Assert.Equal(4m, stok[Hari.AddDays(10)]);   // batch lain tidak berubah
        Assert.Equal(4m, stok[Hari.AddDays(30)]);
    }

    [Fact]
    public async Task Penggunaan_melebihi_stok_ditolak_dan_stok_tidak_berubah()
    {
        var m = await SiapkanMaster();
        var idPembelian = await BuatPembelian(m);
        await Terima(m, idPembelian, 4m, Hari.AddDays(10));

        var res = await Gunakan(m, Hari.AddDays(10), 100m);

        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
        Assert.Equal(4m, (await AmbilStok(m.IdBahan))[Hari.AddDays(10)]);
    }

    [Fact]
    public async Task Penggunaan_batch_yang_sudah_kadaluwarsa_ditolak()
    {
        var m = await SiapkanMaster();
        var idPembelian = await BuatPembelian(m);
        await Terima(m, idPembelian, 5m, Hari.AddDays(-1));

        var res = await Gunakan(m, Hari.AddDays(-1), 1m);

        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
        Assert.Equal(5m, (await AmbilStok(m.IdBahan))[Hari.AddDays(-1)]);
    }

    [Fact]
    public async Task Penggunaan_batch_yang_tidak_ada_ditolak()
    {
        var m = await SiapkanMaster();

        var res = await Gunakan(m, Hari.AddDays(10), 1m);

        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
    }

    // ---------- bantuan ----------

    private static string Id(string awalan) => awalan + Random.Shared.Next(0, 10_000_000).ToString("D7");

    private static async Task Sukses(HttpResponseMessage res)
    {
        if (!res.IsSuccessStatusCode)
            Assert.Fail($"{(int)res.StatusCode} {res.StatusCode}: {await res.Content.ReadAsStringAsync()}");
    }

    private async Task<Master> SiapkanMaster()
    {
        var m = new Master(Id("PMS"), Id("SAT"), Id("BHN"));

        await Sukses(await _admin.PostAsJsonAsync("/api/Pemasok", new
        {
            idPemasok = m.IdPemasok,
            namaPemasok = "Pemasok Uji",
            tglBergabung = Hari,
            status = true
        }));
        await Sukses(await _admin.PostAsJsonAsync("/api/SatuanBahanBaku", new
        {
            idSatuan = m.IdSatuan,
            namaSatuan = "Kg"
        }));
        await Sukses(await _admin.PostAsJsonAsync("/api/HeaderBahanBaku", new
        {
            idBahanBaku = m.IdBahan,
            namaBahanBaku = "Tepung Uji",
            jenis = true,
            status = true,
            details = Array.Empty<object>()
        }));

        return m;
    }

    private async Task<string> BuatPembelian(Master m, decimal jumlah = 50m, decimal harga = 5000m)
    {
        var idPembelian = Id("PBL");
        await Sukses(await _admin.PostAsJsonAsync("/api/HeaderPembelian", new
        {
            idPembelian,
            idPemasok = m.IdPemasok,
            tglPembelian = Hari,
            status = "Pending",
            details = new[] { new { idBahanBaku = m.IdBahan, idSatuan = m.IdSatuan, jumlah, harga } }
        }));
        return idPembelian;
    }

    // Satu bahan baku hanya boleh muncul satu kali per penerimaan (kunci tabel DetailPenerimaan),
    // jadi setiap batch diterima lewat penerimaan tersendiri.
    private Task<HttpResponseMessage> TerimaMentah(Master m, string idPembelian,
        decimal jumlah, DateOnly kadaluwarsa, string status) =>
        _admin.PostAsJsonAsync("/api/HeaderPenerimaan", new
        {
            idPenerimaan = Id("PNR"),
            idPemasok = m.IdPemasok,
            idPembelian,
            tglPenerimaan = Hari,
            status = "Selesai",
            details = new[]
            {
                new
                {
                    idBahanBaku = m.IdBahan, idSatuan = m.IdSatuan, jumlah,
                    tglKadaluwarsa = kadaluwarsa, statusBahanBaku = status
                }
            }
        });

    private async Task Terima(Master m, string idPembelian,
        decimal jumlah, DateOnly kadaluwarsa, string status = "Dibuka") =>
        await Sukses(await TerimaMentah(m, idPembelian, jumlah, kadaluwarsa, status));

    private Task<HttpResponseMessage> Gunakan(Master m, DateOnly kadaluwarsa, decimal jumlah) =>
        _admin.PostAsJsonAsync("/api/HeaderPenggunaan", new
        {
            idPenggunaan = Id("PNG"),
            tglPenggunaan = Hari,
            details = new[] { new { idBahanBaku = m.IdBahan, tglKadaluwarsa = kadaluwarsa, jumlahPenggunaan = jumlah } }
        });

    // Mengembalikan sisa stok per tanggal kadaluwarsa
    private async Task<Dictionary<DateOnly, decimal>> AmbilStok(string idBahan)
    {
        var json = await _admin.GetFromJsonAsync<JsonElement>($"/api/HeaderBahanBaku/{idBahan}");
        return json.GetProperty("details").EnumerateArray().ToDictionary(
            d => DateOnly.Parse(d.GetProperty("tglKadaluwarsa").GetString()!, CultureInfo.InvariantCulture),
            d => d.GetProperty("sisaStok").GetDecimal());
    }
}