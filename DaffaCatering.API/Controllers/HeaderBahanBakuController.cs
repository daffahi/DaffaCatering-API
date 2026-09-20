using DaffaCatering.API.Data;
using DaffaCatering.API.Models;
using DaffaCatering.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DaffaCatering.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HeaderBahanBakuController : ControllerBase
    {
        private readonly DaffaCateringContext _context;

        public HeaderBahanBakuController(DaffaCateringContext context)
        {
            _context = context;
        }

        // GET semua data, dengan opsi filter ?status=true/false
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool? status)
        {
            var query = _context.HeaderBahanBakus.AsQueryable();

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            var data = await query.ToListAsync();
            return Ok(data);
        }

        // GET satu data spesifik berdasarkan ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var data = await _context.HeaderBahanBakus.FindAsync(id);
            if (data == null)
                return NotFound();
            return Ok(data);
        }

        // POST - Create data baru, dengan validasi otomatis dari DTO
        // dan error handling untuk duplicate ID
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] HeaderBahanBakuDto input)
        {
            try
            {
                var entity = new HeaderBahanBaku
                {
                    IdBahanBaku = input.IdBahanBaku,
                    NamaBahanBaku = input.NamaBahanBaku,
                    Jenis = input.Jenis,
                    Status = input.Status
                };

                _context.HeaderBahanBakus.Add(entity);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = entity.IdBahanBaku }, entity);
            }
            catch (DbUpdateException)
            {
                return Conflict("ID Bahan Baku sudah ada, gunakan ID yang berbeda");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Terjadi kesalahan: {ex.Message}");
            }
        }

        // PUT - Update data yang sudah ada, berdasarkan ID
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] HeaderBahanBakuDto input)
        {
            if (id != input.IdBahanBaku)
                return BadRequest("ID tidak sesuai");

            var existing = await _context.HeaderBahanBakus.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.NamaBahanBaku = input.NamaBahanBaku;
            existing.Jenis = input.Jenis;
            existing.Status = input.Status;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE - Hapus data berdasarkan ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _context.HeaderBahanBakus.FindAsync(id);
            if (existing == null)
                return NotFound();

            _context.HeaderBahanBakus.Remove(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}