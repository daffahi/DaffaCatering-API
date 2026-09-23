using DaffaCatering.API.Data;
using DaffaCatering.API.DTOs.MasterPemasok;
using DaffaCatering.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DaffaCatering.API.Controllers.MasterPemasok
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]

    public class PemasokController : ControllerBase
    {
        private readonly DaffaCateringContext _context;

        public PemasokController(DaffaCateringContext context)
        {
            _context = context;
        }

        // GET semua pemasok, dengan opsi filter ?status=true/false
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool? status)
        {
            var query = _context.Pemasoks.AsQueryable();

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            var data = await query.ToListAsync();
            return Ok(data);
        }

        // GET satu data spesifik berdasarkan ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var data = await _context.Pemasoks.FindAsync(id);
            if (data == null)
                return NotFound();
            return Ok(data);
        }

        // POST - Create data baru, dengan validasi otomatis dari DTO
        // dan error handling untuk duplicate ID
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PemasokDto input)
        {
            try
            {
                var entity = new Pemasok
                {
                    IdPemasok = input.IdPemasok,
                    NamaPemasok = input.NamaPemasok,
                    NoKontak = input.NoKontak,
                    Alamat = input.Alamat,
                    TglBergabung = input.TglBergabung,
                    Status = input.Status
                };

                _context.Pemasoks.Add(entity);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = entity.IdPemasok }, entity);
            }
            catch (DbUpdateException)
            {
                return Conflict("ID Pemasok sudah ada, gunakan ID yang berbeda");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Terjadi kesalahan: {ex.Message}");
            }
        }

        // PUT - Update Pemasok, berdasarkan ID
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] PemasokDto input)
        {
            if (id != input.IdPemasok)
                return BadRequest("ID tidak sesuai");

            var existing = await _context.Pemasoks.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.NamaPemasok = input.NamaPemasok;
            existing.NoKontak = input.NoKontak;
            existing.Alamat = input.Alamat;
            existing.TglBergabung = input.TglBergabung;
            existing.Status = input.Status;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE - Hapus pemasok
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _context.Pemasoks.FindAsync(id);
            if (existing == null)
                return NotFound();

            _context.Pemasoks.Remove(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}