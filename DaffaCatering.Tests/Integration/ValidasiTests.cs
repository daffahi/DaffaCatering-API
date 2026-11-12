using System.Net;
using System.Net.Http.Json;

namespace DaffaCatering.Tests.Integration;

public class ValidasiTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;
    private readonly HttpClient _admin;
    private static readonly DateOnly Hari = DateOnly.FromDateTime(DateTime.Today);

    public ValidasiTests(ApiFactory factory)
    {
        _factory = factory;
        _admin = factory.ClientDengan("R001");
    }

    private static string Id(string awalan) => awalan + Random.Shared.Next(0, 10_000_000).ToString("D7");

    [Fact]
    public async Task Login_tanpa_username_ditolak_400()
    {
        var res = await _factory.CreateClient().PostAsJsonAsync("/api/User/login", new { password = "x" });

        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
    }

    [Fact]
    public async Task User_baru_dengan_password_kurang_dari_6_karakter_ditolak()
    {
        var res = await _admin.PostAsJsonAsync("/api/User", new
        {
            idUser = Id("U"),
            idRole = "R002",
            namaUser = Id("usr"),
            password = "12345",
            status = true
        });

        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
    }

    [Fact]
    public async Task User_baru_dengan_role_tidak_ada_ditolak()
    {
        var res = await _admin.PostAsJsonAsync("/api/User", new
        {
            idUser = Id("U"),
            idRole = "R999",
            namaUser = Id("usr"),
            password = "Rahasia123",
            status = true
        });

        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
    }

    [Fact]
    public async Task Username_yang_sudah_dipakai_ditolak_409()
    {
        var res = await _admin.PostAsJsonAsync("/api/User", new
        {
            idUser = Id("U"),
            idRole = "R002",
            namaUser = "admin",
            password = "Rahasia123",
            status = true
        });

        Assert.Equal(HttpStatusCode.Conflict, res.StatusCode);
    }

    [Fact]
    public async Task Pemasok_tanpa_data_wajib_ditolak()
    {
        var res = await _admin.PostAsJsonAsync("/api/Pemasok", new { });

        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
    }

    [Fact]
    public async Task Pembelian_tanpa_item_ditolak()
    {
        var res = await _admin.PostAsJsonAsync("/api/HeaderPembelian", new
        {
            idPembelian = Id("PBL"),
            idPemasok = Id("PMS"),
            tglPembelian = Hari,
            status = "Pending",
            details = Array.Empty<object>()
        });

        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
    }

    [Fact]
    public async Task Penggunaan_dengan_jumlah_nol_ditolak()
    {
        var res = await _admin.PostAsJsonAsync("/api/HeaderPenggunaan", new
        {
            idPenggunaan = Id("PNG"),
            tglPenggunaan = Hari,
            details = new[] { new { idBahanBaku = Id("BHN"), tglKadaluwarsa = Hari.AddDays(5), jumlahPenggunaan = 0m } }
        });

        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
    }
}