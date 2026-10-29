using DaffaCatering.API.Constants;
using DaffaCatering.API.Data;
using DaffaCatering.API.DTOs.Penjualan;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DaffaCatering.API.Controllers.Penjualan
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.AdminPenjualan)]
    public class LaporanPenjualanController : ControllerBase
    {
        private readonly DaffaCateringContext _context;

        public LaporanPenjualanController(DaffaCateringContext context)
        {
            _context = context;
        }

        // jenis: penjualan | pesanan, kelompok: pelanggan | menu
        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] string jenis,
            [FromQuery] string kelompok,
            [FromQuery] DateOnly dari,
            [FromQuery] DateOnly sampai,
            [FromQuery] string? metode,
            [FromQuery] string? status)
        {
            if (jenis != "penjualan" && jenis != "pesanan")
                return BadRequest("Jenis laporan tidak valid");

            if (kelompok != "pelanggan" && kelompok != "menu")
                return BadRequest("Pengelompokan laporan tidak valid");

            if (dari > sampai)
                return BadRequest("Tanggal awal tidak boleh lebih besar dari tanggal akhir");

            var perPelanggan = kelompok == "pelanggan";

            // Selalu per item detail: layar mengelompokkan ulang per transaksi,
            // sedangkan dokumen cetak membutuhkan baris menu
            var query = jenis == "pesanan"
                ? DetailPesanan(dari, sampai, metode, status)
                : DetailPenjualan(dari, sampai, metode);

            var baris = (await query.ToListAsync())
                .OrderBy(b => b.Tanggal)
                .ThenBy(b => b.IdTransaksi)
                .ThenBy(b => b.IdMenu)
                .ToList();

            Func<LaporanBarisDto, string> kunci = b => (perPelanggan ? b.IdPelanggan : b.IdMenu) ?? "-";

            var hasil = baris
                .GroupBy(kunci)
                .Select(g => new LaporanKelompokDto
                {
                    Id = g.Key,
                    JumlahTransaksi = g.Select(b => b.IdTransaksi).Distinct().Count(),
                    TotalJumlah = g.Sum(b => b.Jumlah),
                    Subtotal = g.Sum(b => b.Subtotal),
                    Baris = g.ToList()
                })
                .ToList();

            return Ok(hasil);
        }

        // =====================================================
        // QUERY PENJUALAN
        // =====================================================
        private IQueryable<LaporanBarisDto> DetailPenjualan(DateOnly dari, DateOnly sampai, string? metode)
        {
            var q = _context.HeaderPenjualans.AsNoTracking()
                .Where(h => h.TglPenjualan >= dari && h.TglPenjualan <= sampai);

            if (!string.IsNullOrEmpty(metode))
                q = q.Where(h => h.MetodePembayaran == metode);

            return q.SelectMany(h => h.DetailPenjualans.Select(d => new LaporanBarisDto
            {
                IdTransaksi = h.IdPenjualan,
                Tanggal = h.TglPenjualan,
                IdPelanggan = h.IdPelanggan,
                IdMenu = d.IdMenu,
                IdSatuan = d.IdSatuan,
                Metode = h.MetodePembayaran,
                Status = h.Status,
                Jumlah = d.Jumlah,
                Harga = d.Harga,
                Subtotal = d.Jumlah * d.Harga,
                UangMuka = h.UangMuka,
                TotalTransaksi = h.Total
            }));
        }

        // =====================================================
        // QUERY PESANAN PENJUALAN
        // =====================================================
        private IQueryable<LaporanBarisDto> DetailPesanan(DateOnly dari, DateOnly sampai, string? metode, string? status)
        {
            var q = _context.HeaderPesanans.AsNoTracking()
                .Where(h => h.TglPesanan >= dari && h.TglPesanan <= sampai);

            if (!string.IsNullOrEmpty(metode))
                q = q.Where(h => h.MetodePembayaran == metode);

            if (!string.IsNullOrEmpty(status))
                q = q.Where(h => h.Status == status);

            return q.SelectMany(h => h.DetailPesanans.Select(d => new LaporanBarisDto
            {
                IdTransaksi = h.IdPesanan,
                Tanggal = h.TglPesanan,
                IdPelanggan = h.IdPelanggan,
                IdMenu = d.IdMenu,
                IdSatuan = d.IdSatuan,
                Metode = h.MetodePembayaran,
                Status = h.Status,
                Jumlah = d.Jumlah,
                Harga = d.Harga,
                Subtotal = d.Jumlah * d.Harga,
                UangMuka = h.UangMuka,
                TotalTransaksi = h.Total
            }));
        }
    }
}