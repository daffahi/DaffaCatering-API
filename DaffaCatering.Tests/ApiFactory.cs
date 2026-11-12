using DaffaCatering.API.Data;
using DaffaCatering.API.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DaffaCatering.Tests;

public class ApiFactory : WebApplicationFactory<Program>
{
    public const string Key = "kunci-rahasia-khusus-pengujian-minimal-32-karakter!";
    public const string Issuer = "DaffaCateringAPI";
    public const string Audience = "DaffaCateringUsers";
    public const string PasswordUji = "Rahasia123";

    // Di laptop: SQL Server lokal dengan Windows Authentication.
    // Di GitHub Actions: diganti lewat variabel lingkungan.
    private static readonly string KoneksiServer =
        Environment.GetEnvironmentVariable("TEST_DB_SERVER_CONNECTION")
        ?? "Server=.;Trusted_Connection=True;TrustServerCertificate=True;";

    // Satu database per kelas tes, supaya kelas tes yang jalan bersamaan tidak saling mengganggu
    private readonly string _namaDb = "DaffaCateringDB_Test_" + Guid.NewGuid().ToString("N");
    private bool _dbDihapus;

    public ApiFactory()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DaffaCateringContext>();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
        IsiDataDasar(db);
    }

    // EnsureCreated tidak membawa data seed dari script SQL, jadi role dan user diisi di sini
    private static void IsiDataDasar(DaffaCateringContext db)
    {
        db.Roles.AddRange(
            new Role { IdRole = "R001", NamaRole = "Admin" },
            new Role { IdRole = "R002", NamaRole = "Penjualan" },
            new Role { IdRole = "R003", NamaRole = "Pembelian" },
            new Role { IdRole = "R004", NamaRole = "Persediaan" });

        db.Users.AddRange(
            new User
            {
                IdUser = "U0000001",
                IdRole = "R001",
                NamaUser = "admin",
                Password = BCrypt.Net.BCrypt.HashPassword(PasswordUji),
                Status = true
            },
            new User
            {
                IdUser = "U0000002",
                IdRole = "R002",
                NamaUser = "nonaktif",
                Password = BCrypt.Net.BCrypt.HashPassword(PasswordUji),
                Status = false
            });

        db.SaveChanges();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("Jwt:Key", Key);
        builder.UseSetting("Jwt:Issuer", Issuer);
        builder.UseSetting("Jwt:Audience", Audience);
        builder.UseSetting("Jwt:ExpiresInMinutes", "10");
        builder.UseSetting("ConnectionStrings:DefaultConnection",
            $"{KoneksiServer.TrimEnd(';')};Database={_namaDb};");
    }

    private void HapusDb()
    {
        if (_dbDihapus) return;
        _dbDihapus = true;

        using var scope = Services.CreateScope();
        scope.ServiceProvider.GetRequiredService<DaffaCateringContext>().Database.EnsureDeleted();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) HapusDb();
        base.Dispose(disposing);
    }

    public override async ValueTask DisposeAsync()
    {
        HapusDb();
        await base.DisposeAsync();
    }
}