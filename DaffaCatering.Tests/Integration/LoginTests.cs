using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace DaffaCatering.Tests.Integration;

public class LoginTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;
    public LoginTests(ApiFactory factory) => _factory = factory;

    private Task<HttpResponseMessage> Login(string username, string password) =>
        _factory.CreateClient().PostAsJsonAsync("/api/User/login", new { username, password });

    [Fact]
    public async Task Login_benar_mengembalikan_token()
    {
        var res = await Login("admin", ApiFactory.PasswordUji);

        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        var json = await res.Content.ReadFromJsonAsync<JsonElement>();
        Assert.False(string.IsNullOrWhiteSpace(json.GetProperty("token").GetString()));
    }

    [Fact]
    public async Task Username_dengan_spasi_di_ujung_tetap_diterima()
    {
        var res = await Login("  admin  ", ApiFactory.PasswordUji);

        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }

    [Fact]
    public async Task Password_salah_ditolak()
    {
        var res = await Login("admin", "password-salah");

        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }

    [Fact]
    public async Task Username_tidak_ada_ditolak()
    {
        var res = await Login("tidak-ada", ApiFactory.PasswordUji);

        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }

    [Fact]
    public async Task Akun_nonaktif_ditolak()
    {
        var res = await Login("nonaktif", ApiFactory.PasswordUji);

        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
        Assert.Contains("tidak aktif", await res.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Token_hasil_login_bisa_dipakai_mengakses_endpoint_admin()
    {
        var login = await Login("admin", ApiFactory.PasswordUji);
        var token = (await login.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("token").GetString();

        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new("Bearer", token);
        var res = await client.GetAsync("/api/User");

        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }
}