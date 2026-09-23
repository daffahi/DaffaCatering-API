using DaffaCatering.API.Data;
using DaffaCatering.API.DTOs.Penjualan;
using DaffaCatering.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DaffaCatering.API.Controllers.Penjualan
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]

    public class HeaderPenjualanController : ControllerBase
    {
        private readonly DaffaCateringContext _context;

        public HeaderPenjualanController(DaffaCateringContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? status)
        {
            var query = _context.HeaderPenjualans.AsQueryable();
            if (!string.IsNullOrEmpty(status))
                query = query.Where(x => x.Status == status);
            var data = await query.ToListAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var data = await _context.HeaderPenjualans
                .Include(h => h.DetailPenjualans)
                .FirstOrDefaultAsync(h => h.IdPenjualan == id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] HeaderPenjualanDto input)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (!await _context.Pelanggans.AnyAsync(p => p.IdPelanggan == input.IdPelanggan))
                    return BadRequest($"Pelanggan '{input.IdPelanggan}' tidak ditemukan");

                if (!string.IsNullOrEmpty(input.IdPesanan) &&
                    !await _context.HeaderPesanans.AnyAsync(p => p.IdPesanan == input.IdPesanan))
                    return BadRequest($"Pesanan '{input.IdPesanan}' tidak ditemukan");

                decimal total = input.Details.Sum(d => d.Jumlah * d.Harga);

                var entity = new Models.HeaderPenjualan
                {
                    IdPenjualan = input.IdPenjualan,
                    IdPelanggan = input.IdPelanggan,
                    IdPesanan = input.IdPesanan,
                    TglPenjualan = input.TglPenjualan,
                    MetodePembayaran = input.MetodePembayaran,
                    UangMuka = input.UangMuka,
                    Status = input.Status,
                    Total = total
                };

                foreach (var detailInput in input.Details)
                {
                    entity.DetailPenjualans.Add(new Models.DetailPenjualan
                    {
                        IdPenjualan = input.IdPenjualan,
                        IdMenu = detailInput.IdMenu,
                        IdSatuan = detailInput.IdSatuan,
                        Jumlah = detailInput.Jumlah,
                        Harga = detailInput.Harga
                    });

                    // === Kurangi stok bahan baku sesuai resep menu ini ===
                    var resep = await _context.HeaderReseps
                        .Include(r => r.DetailReseps)
                        .FirstOrDefaultAsync(r => r.IdMenu == detailInput.IdMenu && r.Status == true);

                    if (resep == null)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest($"Menu '{detailInput.IdMenu}' belum punya resep aktif");
                    }

                    foreach (var komposisi in resep.DetailReseps)
                    {
                        // Total bahan baku dibutuhkan = jumlah per resep x jumlah menu terjual
                        decimal totalDibutuhkan = (komposisi.Jumlah) * detailInput.Jumlah;
                        decimal sisaDibutuhkan = totalDibutuhkan;

                        // FEFO: ambil batch dengan tanggal kadaluwarsa paling dekat dulu
                        var batches = await _context.DetailBahanBakus
                            .Where(d => d.IdBahanBaku == komposisi.IdBahanBaku && d.SisaStok > 0)
                            .OrderBy(d => d.TglKadaluwarsa)
                            .ToListAsync();

                        decimal totalTersedia = batches.Sum(b => b.SisaStok ?? 0);
                        if (totalTersedia < sisaDibutuhkan)
                        {
                            await transaction.RollbackAsync();
                            return BadRequest(
                                $"Stok '{komposisi.IdBahanBaku}' tidak cukup untuk menu '{detailInput.IdMenu}'. " +
                                $"Dibutuhkan {sisaDibutuhkan}, tersedia {totalTersedia}");
                        }

                        foreach (var batch in batches)
                        {
                            if (sisaDibutuhkan <= 0) break;

                            decimal ambilDariBatchIni = Math.Min(batch.SisaStok ?? 0, sisaDibutuhkan);
                            batch.SisaStok -= ambilDariBatchIni;
                            sisaDibutuhkan -= ambilDariBatchIni;
                        }
                    }
                }

                _context.HeaderPenjualans.Add(entity);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetById), new { id = entity.IdPenjualan }, entity);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Terjadi kesalahan: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _context.HeaderPenjualans.FindAsync(id);
            if (existing == null) return NotFound();
            _context.HeaderPenjualans.Remove(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}