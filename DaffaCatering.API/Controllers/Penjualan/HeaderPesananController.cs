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

    public class HeaderPesananController : ControllerBase
    {
        private readonly DaffaCateringContext _context;

        public HeaderPesananController(DaffaCateringContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? status)
        {
            var query = _context.HeaderPesanans.AsQueryable();
            if (!string.IsNullOrEmpty(status))
                query = query.Where(x => x.Status == status);
            var data = await query.ToListAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var data = await _context.HeaderPesanans
                .Include(h => h.DetailPesanans)
                .FirstOrDefaultAsync(h => h.IdPesanan == id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] HeaderPesananDto input)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (!await _context.Pelanggans.AnyAsync(p => p.IdPelanggan == input.IdPelanggan))
                    return BadRequest($"Pelanggan '{input.IdPelanggan}' tidak ditemukan");

                decimal total = input.Details.Sum(d => d.Jumlah * d.Harga);

                var entity = new Models.HeaderPesanan
                {
                    IdPesanan = input.IdPesanan,
                    IdPelanggan = input.IdPelanggan,
                    TglPesanan = input.TglPesanan,
                    TglPengambilan = input.TglPengambilan,
                    MetodePembayaran = input.MetodePembayaran,
                    UangMuka = input.UangMuka,
                    Keterangan = input.Keterangan,
                    Status = input.Status,
                    Total = total
                };

                foreach (var detailInput in input.Details)
                {
                    entity.DetailPesanans.Add(new Models.DetailPesanan
                    {
                        IdPesanan = input.IdPesanan,
                        IdMenu = detailInput.IdMenu,
                        IdSatuan = detailInput.IdSatuan,
                        Jumlah = detailInput.Jumlah,
                        Harga = detailInput.Harga
                    });
                }

                _context.HeaderPesanans.Add(entity);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetById), new { id = entity.IdPesanan }, entity);
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                return Conflict($"Gagal menyimpan — cek ID atau referensi. Detail: {ex.InnerException?.Message}");
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
            var existing = await _context.HeaderPesanans.FindAsync(id);
            if (existing == null) return NotFound();
            _context.HeaderPesanans.Remove(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}