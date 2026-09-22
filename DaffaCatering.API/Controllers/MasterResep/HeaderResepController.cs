using DaffaCatering.API.Data;
using DaffaCatering.API.Models;
using DaffaCatering.API.DTOs.MasterResep;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DaffaCatering.API.DTOs.Resep;

namespace DaffaCatering.API.Controllers.MasterResep
{
    [ApiController]
    [Route("api/[controller]")]
    public class HeaderResepController : ControllerBase
    {
        private readonly DaffaCateringContext _context;

        public HeaderResepController(DaffaCateringContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool? status)
        {
            var query = _context.HeaderReseps.AsQueryable();
            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);
            var data = await query.ToListAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var data = await _context.HeaderReseps
                .Include(h => h.DetailReseps)
                .FirstOrDefaultAsync(h => h.IdResep == id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] HeaderResepDto input)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validasi FK: Menu harus ada
                if (!await _context.Menus.AnyAsync(m => m.IdMenu == input.IdMenu))
                    return BadRequest($"Menu '{input.IdMenu}' tidak ditemukan");

                var entity = new Models.HeaderResep
                {
                    IdResep = input.IdResep,
                    IdMenu = input.IdMenu,
                    Status = input.Status
                };

                foreach (var detailInput in input.Details)
                {
                    entity.DetailReseps.Add(new Models.DetailResep
                    {
                        IdResep = input.IdResep,
                        IdBahanBaku = detailInput.IdBahanBaku,
                        IdSatuan = detailInput.IdSatuan,
                        Jumlah = detailInput.Jumlah
                    });
                }

                _context.HeaderReseps.Add(entity);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetById), new { id = entity.IdResep }, entity);
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] HeaderResepDto input)
        {
            if (id != input.IdResep)
                return BadRequest("ID tidak sesuai");

            var existing = await _context.HeaderReseps.FindAsync(id);
            if (existing == null) return NotFound();

            existing.IdMenu = input.IdMenu;
            existing.Status = input.Status;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _context.HeaderReseps.FindAsync(id);
            if (existing == null) return NotFound();
            _context.HeaderReseps.Remove(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}