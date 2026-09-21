using DaffaCatering.API.Data;
using DaffaCatering.API.Models;
using DaffaCatering.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DaffaCatering.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class SatuanBahanBakuController : ControllerBase
    {
        private readonly DaffaCateringContext _context;

        public SatuanBahanBakuController(DaffaCateringContext context)
        {
            _context = context;
        }

        // GET semua satuan bahan baku
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.SatuanBahanBakus.ToListAsync();
            return Ok(data);
        }

        // GET satu satuan bahan baku berdasarkan ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var data = await _context.SatuanBahanBakus.FindAsync(id);
            if (data == null)
                return NotFound();
            return Ok(data);
        }

        // POST - Create satuan bahan baku baru
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SatuanBahanBakuDto input)
        {
            try
            {
                var entity = new SatuanBahanBaku
                {
                    IdSatuan = input.IdSatuanBahanBaku,
                    NamaSatuan = input.NamaSatuanBahanBaku
                };

                _context.SatuanBahanBakus.Add(entity);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = entity.IdSatuan }, entity);
            }
            catch (DbUpdateException)
            {
                return Conflict("ID Satuan Bahan Baku sudah ada, gunakan ID yang berbeda");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Terjadi kesalahan: {ex.Message}");
            }
        }

        // PUT - Update satuan bahan baku berdasarkan ID
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] SatuanBahanBakuDto input)
        {
            if (id != input.IdSatuanBahanBaku)
                return BadRequest("ID tidak sesuai");

            var existing = await _context.SatuanBahanBakus.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.NamaSatuan = input.NamaSatuanBahanBaku;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE - Hapus satuan bahan baku berdasarkan ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _context.SatuanBahanBakus.FindAsync(id);
            if (existing == null)
                return NotFound();

            _context.SatuanBahanBakus.Remove(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}