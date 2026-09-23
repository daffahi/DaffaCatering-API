using DaffaCatering.API.Data;
using DaffaCatering.API.DTOs.MasterPelanggan;
using DaffaCatering.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DaffaCatering.API.Controllers.MasterPelanggan
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]

    public class PelangganController : ControllerBase
    {
        private readonly DaffaCateringContext _context;

        public PelangganController(DaffaCateringContext context)
        {
            _context = context;
        }

        // GET semua pelanggan, dengan opsi filter ?status=true/false
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool? status)
        {
            var query = _context.Pelanggans.AsQueryable();

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            var data = await query.ToListAsync();
            return Ok(data);
        }

        // GET satu pelanggan berdasarkan ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var data = await _context.Pelanggans.FindAsync(id);
            if (data == null)
                return NotFound();
            return Ok(data);
        }

        // POST - Create pelanggan baru
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PelangganDto input)
        {
            try
            {
                var entity = new Pelanggan
                {
                    IdPelanggan = input.IdPelanggan,
                    NamaPelanggan = input.NamaPelanggan,
                    Alamat = input.Alamat,
                    NoKontak = input.NoKontak,
                    Status = input.Status,
                    TglBergabung = input.TglBergabung
                };

                _context.Pelanggans.Add(entity);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = entity.IdPelanggan }, entity);
            }
            catch (DbUpdateException)
            {
                return Conflict("ID Pelanggan sudah ada, gunakan ID yang berbeda");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Terjadi kesalahan: {ex.Message}");
            }
        }

        // PUT - Update pelanggan
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] PelangganDto input)
        {
            if (id != input.IdPelanggan)
                return BadRequest("ID tidak sesuai");

            var existing = await _context.Pelanggans.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.NamaPelanggan = input.NamaPelanggan;
            existing.Alamat = input.Alamat;
            existing.NoKontak = input.NoKontak;
            existing.Status = input.Status;
            existing.TglBergabung = input.TglBergabung;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE - Hapus pelanggan
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _context.Pelanggans.FindAsync(id);
            if (existing == null)
                return NotFound();

            _context.Pelanggans.Remove(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}