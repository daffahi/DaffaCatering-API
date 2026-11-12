using System.Net;
using System.Net.Http.Json;

namespace DaffaCatering.Tests.Integration;

public class OtorisasiRoleTests : IClassFixture<ApiFactory>
{
    private const string Admin = "R001", Penjualan = "R002", Pembelian = "R003", Persediaan = "R004";
    private static readonly string[] SemuaRole = { Admin, Penjualan, Pembelian, Persediaan };

    private readonly ApiFactory _factory;
    public OtorisasiRoleTests(ApiFactory factory) => _factory = factory;

    // Endpoint GET dan role yang boleh mengaksesnya
    private static readonly Dictionary<string, string[]> Peta = new()
    {
        ["/api/Pelanggan"] = new[] { Admin, Penjualan },
        ["/api/MenuMakanan"] = new[] { Admin, Penjualan },
        ["/api/HeaderResep"] = new[] { Admin, Penjualan },
        ["/api/HeaderPesanan"] = new[] { Admin, Penjualan },
        ["/api/HeaderPenjualan"] = new[] { Admin, Penjualan },
        ["/api/Pemasok"] = new[] { Admin, Pembelian },
        ["/api/HeaderPembelian"] = new[] { Admin, Pembelian },
        ["/api/HeaderPenerimaan"] = new[] { Admin, Pembelian },
        ["/api/KonversiSatuan"] = new[] { Admin, Persediaan },
        ["/api/HeaderPenggunaan"] = new[] { Admin, Persediaan },
        ["/api/HeaderPenyesuaianBahanBaku"] = new[] { Admin, Persediaan },
        ["/api/HeaderBahanBaku"] = SemuaRole,
        ["/api/SatuanBahanBaku"] = SemuaRole,
        ["/api/User"] = new[] { Admin },
        ["/api/Role"] = new[] { Admin },
    };

    public static IEnumerable<object[]> Matriks()
    {
        foreach (var (url, boleh) in Peta)
            foreach (var role in SemuaRole)
                yield return new object[] { url, role, boleh.Contains(role) };
    }

    public static IEnumerable<object[]> UrlSaja() => Peta.Keys.Select(u => new object[] { u });

    [Theory]
    [MemberData(nameof(Matriks))]
    public async Task Get_mengikuti_hak_akses_role(string url, string role, bool diizinkan)
    {
        var client = _factory.ClientDengan(role);

        var res = await client.GetAsync(url);

        Assert.Equal(diizinkan ? HttpStatusCode.OK : HttpStatusCode.Forbidden, res.StatusCode);
    }

    [Theory]
    [MemberData(nameof(UrlSaja))]
    public async Task Tanpa_token_ditolak_401(string url)
    {
        var res = await _factory.CreateClient().GetAsync(url);

        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }

    // Endpoint yang boleh dibaca banyak role (untuk dropdown), tetapi tulisnya hanya untuk role tertentu
    [Theory]
    [InlineData("/api/HeaderBahanBaku", Penjualan)]
    [InlineData("/api/HeaderBahanBaku", Pembelian)]
    [InlineData("/api/SatuanBahanBaku", Penjualan)]
    [InlineData("/api/SatuanBahanBaku", Pembelian)]
    [InlineData("/api/Pelanggan", Pembelian)]
    [InlineData("/api/Pelanggan", Persediaan)]
    public async Task Post_ditolak_untuk_role_yang_tidak_berhak(string url, string role)
    {
        var client = _factory.ClientDengan(role);

        var res = await client.PostAsJsonAsync(url, new { });

        Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);
    }
}