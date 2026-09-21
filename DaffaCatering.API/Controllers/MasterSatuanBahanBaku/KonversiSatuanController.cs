using DaffaCatering.API.Data;
using DaffaCatering.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DaffaCatering.API.DTOs.MasterSatuanBahanBaku;

namespace DaffaCatering.API.Controllers.MasterSatuanBahanBaku
{
    [ApiController]
    [Route("api/[controller]")]

    public class KonversiSatuanController : ControllerBase
    {
        private readonly DaffaCateringContext _context;

        public KonversiSatuanController(DaffaCateringContext context)
        {
            _context = context;
        }

        // GET all data
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.KonversiSatuans.ToListAsync();
            return Ok(data);
        }

        // GET satu data berdasarkan ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var data = await _context.KonversiSatuans.FindAsync(id);
            if (data == null)
                return NotFound();
            return Ok(data);
        }

        // CREATE & error handling untuk duplicate ID
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] KonversiSatuanDto input)
        {
            try
            {
                var entity = new KonversiSatuan
                {
                    IdKonversi = input.IdKonversi,
                    IdSatuanBesar = input.IdSatuanBesar,
                    IdSatuanKecil = input.IdSatuanKecil,
                    NilaiKonversi = input.NilaiKonversi
                };

                _context.KonversiSatuans.Add(entity);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = entity.IdKonversi }, entity);
            }
            catch (DbUpdateException)
            {
                return Conflict("ID Konversi sudah ada, gunakan ID yang berbeda");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Terjadi kesalahan: {ex.Message}");
            }
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] KonversiSatuanDto input)
        {
            if (id != input.IdKonversi)
                return BadRequest("ID tidak sesuai");

            var existing = await _context.KonversiSatuans.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.IdSatuanBesar = input.IdSatuanBesar;
            existing.IdSatuanKecil = input.IdSatuanKecil;
            existing.NilaiKonversi = input.NilaiKonversi;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _context.KonversiSatuans.FindAsync(id);
            if (existing == null)
                return NotFound();

            _context.KonversiSatuans.Remove(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}