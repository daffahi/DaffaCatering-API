using DaffaCatering.API.Constants;
using DaffaCatering.API.Data;
using DaffaCatering.API.DTOs.Pembelian;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DaffaCatering.API.Controllers.Pembelian
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.AdminPembelian)]
    public class LaporanPembelianController : ControllerBase
    {
        private readonly DaffaCateringContext _context;

        public LaporanPembelianController(DaffaCateringContext context)
        {
            _context = context;
        }

        // jenis: pembelian | penerimaan
        // kelompok (hanya untuk pembelian): pemasok | bahanbaku
        // Penerimaan selalu dikelompokkan per nomor penerimaan
        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] string jenis,
            [FromQuery] string? kelompok,
            [FromQuery] DateOnly dari,
            [FromQuery] DateOnly sampai)
        {
            if (jenis != "pembelian" && jenis != "penerimaan")
                return BadRequest("Jenis laporan tidak valid");

            if (jenis == "pembelian" && kelompok != "pemasok" && kelompok != "bahanbaku")
                return BadRequest("Pengelompokan laporan tidak valid");

            if (dari > sampai)
                return BadRequest("Tanggal awal tidak boleh lebih besar dari tanggal akhir");

            // Selalu per item detail: layar mengelompokkan ulang per transaksi,
            // sedangkan dokumen cetak membutuhkan baris bahan baku
            var query = jenis == "penerimaan"
                ? DetailPenerimaan(dari, sampai)
                : DetailPembelian(dari, sampai);

            var baris = (await query.ToListAsync())
                .OrderBy(b => b.Tanggal)
                .ThenBy(b => b.IdTransaksi)
                .ThenBy(b => b.IdBahanBaku)
                .ToList();

            string Kunci(LaporanPembelianBarisDto b) => jenis == "penerimaan"
                ? b.IdTransaksi
                : (kelompok == "pemasok" ? b.IdPemasok : b.IdBahanBaku) ?? "-";

            var hasil = baris
                .GroupBy(Kunci)
                .Select(g => new LaporanPembelianKelompokDto
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
        // QUERY PEMBELIAN
        // =====================================================
        private IQueryable<LaporanPembelianBarisDto> DetailPembelian(DateOnly dari, DateOnly sampai)
        {
            return _context.HeaderPembelians.AsNoTracking()
                .Where(h => h.TglPembelian >= dari && h.TglPembelian <= sampai)
                .SelectMany(h => h.DetailPembelians.Select(d => new LaporanPembelianBarisDto
                {
                    IdTransaksi = h.IdPembelian,
                    Tanggal = h.TglPembelian,
                    IdPemasok = h.IdPemasok,
                    IdBahanBaku = d.IdBahanBaku,
                    IdSatuan = d.IdSatuan,
                    Jumlah = d.Jumlah,
                    Harga = d.Harga,
                    Subtotal = d.Jumlah * d.Harga,
                    TotalTransaksi = h.Total
                }));
        }

        // =====================================================
        // QUERY PENERIMAAN BAHAN BAKU
        // =====================================================
        private IQueryable<LaporanPembelianBarisDto> DetailPenerimaan(DateOnly dari, DateOnly sampai)
        {
            return _context.HeaderPenerimaans.AsNoTracking()
                .Where(h => h.TglPenerimaan >= dari && h.TglPenerimaan <= sampai)
                .SelectMany(h => h.DetailPenerimaans.Select(d => new LaporanPembelianBarisDto
                {
                    IdTransaksi = h.IdPenerimaan,
                    Tanggal = h.TglPenerimaan,
                    IdPembelian = h.IdPembelian,
                    IdPemasok = _context.HeaderPembelians
                        .Where(p => p.IdPembelian == h.IdPembelian)
                        .Select(p => p.IdPemasok)
                        .FirstOrDefault(),
                    IdBahanBaku = d.IdBahanBaku,
                    IdSatuan = d.IdSatuan,
                    TglKadaluwarsa = d.TglKadaluwarsa,
                    Jumlah = d.Jumlah,
                    Status = d.StatusBahanBaku
                }));
        }
    }
}