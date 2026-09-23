using DaffaCatering.API.Data;
using DaffaCatering.API.DTOs.Pembelian;
using DaffaCatering.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DaffaCatering.API.Controllers.Pembelian
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "R001,R003")]

    public class HeaderPembelianController : ControllerBase
    {
        private readonly DaffaCateringContext _context;

        public HeaderPembelianController(DaffaCateringContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? status)
        {
            var query = _context.HeaderPembelians.AsQueryable();
            if (!string.IsNullOrEmpty(status))
                query = query.Where(x => x.Status == status);
            var data = await query.ToListAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var data = await _context.HeaderPembelians
                .Include(h => h.DetailPembelians)
                .FirstOrDefaultAsync(h => h.IdPembelian == id);

            if (data == null)
                return NotFound();
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] HeaderPembelianDto input)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validasi: pastikan IdPemasok yang dikirim benar-benar ada
                var pemasokExists = await _context.Pemasoks.AnyAsync(p => p.IdPemasok == input.IdPemasok);
                if (!pemasokExists)
                    return BadRequest($"Pemasok dengan ID '{input.IdPemasok}' tidak ditemukan");

                // Hitung total otomatis dari semua detail (jumlah x harga)
                decimal total = input.Details.Sum(d => d.Jumlah * d.Harga);

                var entity = new Models.HeaderPembelian
                {
                    IdPembelian = input.IdPembelian,
                    IdPemasok = input.IdPemasok,
                    TglPembelian = input.TglPembelian,
                    Status = input.Status,
                    Total = total
                };

                foreach (var detailInput in input.Details)
                {
                    entity.DetailPembelians.Add(new Models.DetailPembelian
                    {
                        IdPembelian = input.IdPembelian,
                        IdBahanBaku = detailInput.IdBahanBaku,
                        IdSatuan = detailInput.IdSatuan,
                        Jumlah = detailInput.Jumlah,
                        Harga = detailInput.Harga
                    });
                }

                _context.HeaderPembelians.Add(entity);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetById), new { id = entity.IdPembelian }, entity);
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                return Conflict($"Gagal menyimpan — ID sudah ada atau ID referensi tidak valid. Detail: {ex.InnerException?.Message}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Terjadi kesalahan: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] HeaderPembelianDto input)
        {
            if (id != input.IdPembelian)
                return BadRequest("ID tidak sesuai");

            var existing = await _context.HeaderPembelians.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.IdPemasok = input.IdPemasok;
            existing.TglPembelian = input.TglPembelian;
            existing.Status = input.Status;
            existing.Total = input.Details.Sum(d => d.Jumlah * d.Harga);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _context.HeaderPembelians.FindAsync(id);
            if (existing == null)
                return NotFound();

            _context.HeaderPembelians.Remove(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}