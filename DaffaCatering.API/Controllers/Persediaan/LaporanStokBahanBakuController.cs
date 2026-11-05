using DaffaCatering.API.Constants;
using DaffaCatering.API.Data;
using DaffaCatering.API.DTOs.Persediaan;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static DaffaCatering.API.DTOs.Persediaan.LaporanStokBahanBakuDto;

namespace DaffaCatering.API.Controllers.Persediaan
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.AdminPersediaan)]
    public class LaporanStokBahanBakuController : ControllerBase
    {
        private readonly DaffaCateringContext _context;

        // hanya penerimaan dengan statusBahanBaku = 'False' yang menambah stok
        private const string StatusPenerimaanMasuk = "False";

        public LaporanStokBahanBakuController(DaffaCateringContext context)
        {
            _context = context;
        }

        // jenis: stok | kadaluwarsa | penggunaan | penyesuaian
        // dari & sampai kosong = semua periode
        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] string jenis,
            [FromQuery] DateOnly? dari,
            [FromQuery] DateOnly? sampai)
        {
            if (dari.HasValue != sampai.HasValue)
                return BadRequest("Tanggal awal dan akhir harus diisi bersamaan");

            if (dari > sampai)
                return BadRequest("Tanggal awal tidak boleh lebih besar dari tanggal akhir");

            switch (jenis)
            {
                case "stok":
                    return Ok(await LaporanStok(dari, sampai));

                case "kadaluwarsa":
                case "penggunaan":
                case "penyesuaian":
                    return StatusCode(StatusCodes.Status501NotImplemented, "Laporan ini belum tersedia");

                default:
                    return BadRequest("Jenis laporan tidak valid");
            }
        }

        // =====================================================
        // LAPORAN STOK BAHAN BAKU (kartu stok per bahan baku)
        // =====================================================
        private sealed class Gerak
        {
            public string IdBahanBaku { get; set; } = "";
            public DateOnly Tanggal { get; set; }
            public string IdUrut { get; set; } = "";
            public string Keterangan { get; set; } = "";
            public decimal Masuk { get; set; }
            public decimal Keluar { get; set; }
        }

        private async Task<List<LaporanStokBahanBakuKelompokDto>> LaporanStok(DateOnly? dari, DateOnly? sampai)
        {
            // 1. Stok awal & satuan per bahan baku (jumlah stokAwal seluruh batch)
            var bahanBaku = await _context.DetailBahanBakus.AsNoTracking()
                .GroupBy(d => d.IdBahanBaku)
                .Select(g => new
                {
                    IdBahanBaku = g.Key,
                    IdSatuan = g.Min(d => d.IdSatuan),
                    StokAwal = g.Sum(d => d.StokAwal ?? 0m)
                })
                .ToListAsync();

            var satuanBahan = bahanBaku.ToDictionary(b => b.IdBahanBaku, b => b.IdSatuan);

            // 2. Penerimaan (masuk)
            var penerimaan = await (
                from d in _context.DetailPenerimaans.AsNoTracking()
                join h in _context.HeaderPenerimaans.AsNoTracking() on d.IdPenerimaan equals h.IdPenerimaan
                where d.StatusBahanBaku == StatusPenerimaanMasuk
                select new Gerak
                {
                    IdBahanBaku = d.IdBahanBaku,
                    Tanggal = h.TglPenerimaan,
                    IdUrut = h.IdPenerimaan,
                    Keterangan = "Penerimaan",
                    Masuk = d.Jumlah,
                    Keluar = 0m
                }).ToListAsync();

            // 3. Penyesuaian (+ masuk, - keluar)
            var penyesuaianRaw = await (
                from d in _context.DetailPenyesuaians.AsNoTracking()
                join h in _context.HeaderPenyesuaians.AsNoTracking() on d.IdPenyesuaian equals h.IdPenyesuaian
                where d.SelisihStok != 0
                select new { d.IdBahanBaku, h.TglPenyesuaian, h.IdPenyesuaian, d.SelisihStok }).ToListAsync();

            var penyesuaian = penyesuaianRaw.Select(x => new Gerak
            {
                IdBahanBaku = x.IdBahanBaku,
                Tanggal = x.TglPenyesuaian,
                IdUrut = x.IdPenyesuaian,
                Keterangan = x.SelisihStok > 0 ? "Penyesuaian (+)" : "Penyesuaian (-)",
                Masuk = x.SelisihStok > 0 ? x.SelisihStok : 0m,
                Keluar = x.SelisihStok < 0 ? Math.Abs(x.SelisihStok) : 0m
            });

            // 4. Penggunaan (keluar)
            var penggunaan = await (
                from d in _context.DetailPenggunaans.AsNoTracking()
                join h in _context.HeaderPenggunaans.AsNoTracking() on d.IdPenggunaan equals h.IdPenggunaan
                select new Gerak
                {
                    IdBahanBaku = d.IdBahanBaku,
                    Tanggal = h.TglPenggunaan,
                    IdUrut = h.IdPenggunaan,
                    Keterangan = "Penggunaan Bahan Baku",
                    Masuk = 0m,
                    Keluar = d.JumlahPenggunaan
                }).ToListAsync();

            // 5. Penjualan (keluar): resep x jumlah menu, dikonversi ke satuan bahan baku
            var penjualanRaw = await (
                from dp in _context.DetailPenjualans.AsNoTracking()
                join hp in _context.HeaderPenjualans.AsNoTracking() on dp.IdPenjualan equals hp.IdPenjualan
                join hr in _context.HeaderReseps.AsNoTracking() on dp.IdMenu equals hr.IdMenu
                join dr in _context.DetailReseps.AsNoTracking() on hr.IdResep equals dr.IdResep
                select new
                {
                    dr.IdBahanBaku,
                    SatuanResep = dr.IdSatuan,
                    JumlahResep = dr.Jumlah,
                    JumlahJual = dp.Jumlah,
                    hp.TglPenjualan,
                    hp.IdPenjualan
                }).ToListAsync();

            var konversi = await _context.KonversiSatuans.AsNoTracking().ToListAsync();

            decimal KeSatuanBahan(string satuanResep, string satuanBahan, decimal qty)
            {
                if (satuanResep == satuanBahan) return qty;

                var k = konversi.FirstOrDefault(x => x.IdSatuanKecil == satuanResep && x.IdSatuanBesar == satuanBahan);
                if (k is not null && k.NilaiKonversi != 0) return qty / k.NilaiKonversi;

                k = konversi.FirstOrDefault(x => x.IdSatuanBesar == satuanResep && x.IdSatuanKecil == satuanBahan);
                if (k is not null) return qty * k.NilaiKonversi;

                return 0m; // tidak ada konversi: di WinForms hasilnya NULL (tidak terhitung)
            }

            var penjualan = penjualanRaw
                .Where(x => satuanBahan.ContainsKey(x.IdBahanBaku))
                .Select(x => new Gerak
                {
                    IdBahanBaku = x.IdBahanBaku,
                    Tanggal = x.TglPenjualan,
                    IdUrut = x.IdPenjualan,
                    Keterangan = "Penjualan",
                    Masuk = 0m,
                    Keluar = KeSatuanBahan(x.SatuanResep, satuanBahan[x.IdBahanBaku], x.JumlahResep * x.JumlahJual)
                })
                .Where(g => g.Keluar > 0);

            var semua = penerimaan.Concat(penyesuaian).Concat(penggunaan).Concat(penjualan)
                .GroupBy(g => g.IdBahanBaku)
                .ToDictionary(g => g.Key, g => g.ToList());

            // 6. Susun kartu stok per bahan baku
            var hasil = new List<LaporanStokBahanBakuKelompokDto>();

            foreach (var b in bahanBaku.OrderBy(b => b.IdBahanBaku, StringComparer.Ordinal))
            {
                var gerak = semua.TryGetValue(b.IdBahanBaku, out var list) ? list : new List<Gerak>();

                // Urutan: tanggal, lalu yang masuk dulu, lalu nomor transaksi (sama seperti WinForms)
                gerak = gerak
                    .OrderBy(g => g.Tanggal)
                    .ThenBy(g => g.Masuk > 0 ? 1 : 2)
                    .ThenBy(g => g.IdUrut, StringComparer.Ordinal)
                    .ToList();

                // Saldo awal periode = stok awal + semua mutasi sebelum tanggal "dari"
                var saldo = b.StokAwal;
                if (dari.HasValue)
                    saldo += gerak.Where(g => g.Tanggal < dari.Value).Sum(g => g.Masuk - g.Keluar);

                var dalamPeriode = gerak
                    .Where(g => (!dari.HasValue || g.Tanggal >= dari.Value)
                             && (!sampai.HasValue || g.Tanggal <= sampai.Value))
                    .ToList();

                // Dengan filter periode, hanya bahan baku yang bergerak yang ditampilkan
                if (dari.HasValue && dalamPeriode.Count == 0) continue;

                var kelompok = new LaporanStokBahanBakuKelompokDto
                {
                    Id = b.IdBahanBaku,
                    IdSatuan = b.IdSatuan,
                    StokAwal = saldo,
                    JumlahTransaksi = dalamPeriode.Count
                };

                foreach (var g in dalamPeriode)
                {
                    saldo += g.Masuk - g.Keluar;

                    kelompok.Baris.Add(new LaporanStokBahanBakuBarisDto
                    {
                        IdTransaksi = g.IdUrut,
                        Tanggal = g.Tanggal,
                        Keterangan = g.Keterangan,
                        QtyMasuk = g.Masuk,
                        QtyKeluar = g.Keluar,
                        SisaStok = saldo
                    });
                }

                kelompok.TotalMasuk = dalamPeriode.Sum(g => g.Masuk);
                kelompok.TotalKeluar = dalamPeriode.Sum(g => g.Keluar);
                kelompok.StokAkhir = saldo;

                hasil.Add(kelompok);
            }

            return hasil;
        }
    }
}