using DaffaCatering.Blazor.Models;
using DaffaCatering.Blazor.Services.Pembelian;
using DaffaCatering.Blazor.Services.Penjualan;
using System.Net.Http.Json;
using System.Security.Claims;

namespace DaffaCatering.Blazor.Services
{
    public record NotifikasiItem(string Tingkat, int Jumlah, string Teks, string Tujuan, string Tautan);

    public class NotifikasiService : ApiServiceBase
    {
        // Ambang yang sama dengan dashboard
        public const decimal BatasStok = 10;
        public const int HariKadaluwarsa = 7;

        public const string Mendesak = "mendesak";
        public const string Perhatian = "peringatan";

        private const int RentangPesanan = 30;      // hari ke belakang dan ke depan
        private const int RentangPembelian = 90;    // hari ke belakang
        private const string StatusMenunggu = "Pending";

        private readonly JwtAuthStateProvider _auth;
        private readonly LaporanPenjualanService _penjualan;
        private readonly LaporanPembelianService _pembelian;

        public NotifikasiService(
            IHttpClientFactory factory,
            JwtAuthStateProvider auth,
            LaporanPenjualanService penjualan,
            LaporanPembelianService pembelian)
            : base(factory, auth)
        {
            _auth = auth;
            _penjualan = penjualan;
            _pembelian = pembelian;
        }

        // Gagal = true jika ada bagian yang tidak bisa dimuat
        public async Task<(List<NotifikasiItem> Item, bool Gagal)> AmbilAsync()
        {
            var item = new List<NotifikasiItem>();

            var user = (await _auth.GetAuthenticationStateAsync()).User;
            if (user.Identity?.IsAuthenticated != true) return (item, false);

            var hariIni = DateOnly.FromDateTime(DateTime.Today);
            var gagal = false;

            if (PunyaPeran(user, AppRoles.AdminPenjualan) && !await CekPesanan(item, hariIni)) gagal = true;
            if (PunyaPeran(user, AppRoles.AdminPembelian) && !await CekPembelian(item, hariIni)) gagal = true;
            if (PunyaPeran(user, AppRoles.AdminPersediaan) && !await CekStok(item, hariIni)) gagal = true;

            // yang mendesak tampil lebih dulu
            var urut = item.OrderBy(n => n.Tingkat == Mendesak ? 0 : 1).ToList();
            return (urut, gagal);
        }

        // =====================================================
        // PESANAN PENJUALAN
        // =====================================================
        private async Task<bool> CekPesanan(List<NotifikasiItem> item, DateOnly hariIni)
        {
            try
            {
                var (data, _) = await _penjualan.AmbilAsync("pesanan", "pelanggan",
                    hariIni.AddDays(-RentangPesanan), hariIni.AddDays(RentangPesanan), "", StatusMenunggu);

                if (data is null) return false;

                var pending = data
                    .SelectMany(g => g.Baris)
                    .GroupBy(b => b.IdTransaksi)
                    .Select(t => t.First())
                    .ToList();

                var telat = pending.Count(t => (t.TglPengambilan ?? t.Tanggal) < hariIni);
                var hariIniJml = pending.Count(t => (t.TglPengambilan ?? t.Tanggal) == hariIni);

                if (telat > 0)
                    item.Add(new(Mendesak, telat, "pesanan sudah lewat tanggalnya",
                        "Pesanan Penjualan", "/pesanan-penjualan"));

                if (hariIniJml > 0)
                    item.Add(new(Perhatian, hariIniJml, "pesanan jatuh tempo hari ini",
                        "Pesanan Penjualan", "/pesanan-penjualan"));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // =====================================================
        // PEMBELIAN
        // =====================================================
        private async Task<bool> CekPembelian(List<NotifikasiItem> item, DateOnly hariIni)
        {
            try
            {
                var (data, _) = await _pembelian.AmbilAsync("pembelian", "pemasok",
                    hariIni.AddDays(-RentangPembelian), hariIni);

                if (data is null) return false;

                var menunggu = data
                    .SelectMany(g => g.Baris.GroupBy(b => b.IdTransaksi).Select(t => t.First()))
                    .Where(t => string.Equals(t.Status, StatusMenunggu, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (menunggu.Count > 0)
                {
                    var hari = hariIni.DayNumber - menunggu.Min(t => t.Tanggal).DayNumber;
                    var teks = hari <= 0
                        ? "pembelian belum diterima"
                        : $"pembelian belum diterima, terlama {hari} hari lalu";

                    item.Add(new(Perhatian, menunggu.Count, teks,
                        "Penerimaan Bahan Baku", "/penerimaan-bahan-baku"));
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // =====================================================
        // STOK BAHAN BAKU
        // =====================================================
        private async Task<bool> CekStok(List<NotifikasiItem> item, DateOnly hariIni)
        {
            try
            {
                await SiapkanTokenAsync();
                var bahanBaku = await Http.GetFromJsonAsync<List<BahanBakuRingkas>>("api/HeaderBahanBaku")
                                ?? new();

                var menipis = bahanBaku
                    .Where(x => x.Status && x.Jenis)
                    .Count(x => x.Details.Sum(d => d.SisaStok ?? 0) < BatasStok);

                // batch yang masih ada isinya dan kadaluwarsa dalam HariKadaluwarsa hari (termasuk yang sudah lewat)
                var batas = hariIni.AddDays(HariKadaluwarsa);
                var tglBatch = bahanBaku
                    .Where(x => x.Status)
                    .SelectMany(x => x.Details)
                    .Where(d => d.TglKadaluwarsa is not null && (d.SisaStok ?? 0) > 0)
                    .Select(d => d.TglKadaluwarsa!.Value)
                    .Where(t => t <= batas)
                    .ToList();

                var sudah = tglBatch.Count(t => t < hariIni);
                var segera = tglBatch.Count - sudah;

                if (sudah > 0)
                    item.Add(new(Mendesak, sudah, "batch sudah kadaluwarsa dan masih tercatat berstok",
                        "Bahan Baku", "/master-bahan-baku"));

                if (segera > 0)
                    item.Add(new(Perhatian, segera, $"batch kadaluwarsa dalam {HariKadaluwarsa} hari",
                        "Bahan Baku", "/master-bahan-baku"));

                if (menipis > 0)
                    item.Add(new(Perhatian, menipis, $"bahan baku stoknya di bawah {BatasStok:0.##}",
                        "Bahan Baku", "/master-bahan-baku"));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // =====================================================
        // HELPER
        // =====================================================
        // AppRoles.* berisi daftar role dipisah koma (format atribut [Authorize(Roles = ...)])
        private static bool PunyaPeran(ClaimsPrincipal user, string daftarPeran)
            => daftarPeran
                .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Any(user.IsInRole);

        public class BahanBakuRingkas
        {
            public bool Jenis { get; set; }
            public bool Status { get; set; }
            public List<DetailRingkas> Details { get; set; } = new();
        }

        public class DetailRingkas
        {
            public decimal? SisaStok { get; set; }
            public DateOnly? TglKadaluwarsa { get; set; }
        }
    }
}