using DaffaCatering.API.Data;
using DaffaCatering.API.Models;
using DaffaCatering.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DaffaCatering.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class MenuController : ControllerBase
    {
        private readonly DaffaCateringContext _context;

        public MenuController(DaffaCateringContext context)
        {
            _context = context;
        }

        // GET all data, dengan opsi filter ?status=true/false
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool? status)
        {
            var query = _context.Menus.AsQueryable();

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            var data = await query.ToListAsync();
            return Ok(data);
        }

        // GET satu data spesifik berdasarkan ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var data = await _context.Menus.FindAsync(id);
            if (data == null)
                return NotFound();
            return Ok(data);
        }

        // CREATE & error handling untuk duplicate ID
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MenuDto input)
        {
            try
            {
                var entity = new Menu
                {
                    IdMenu = input.IdMenu,
                    IdSatuan = input.IdSatuan,
                    NamaMenu = input.NamaMenu,
                    Jumlah = input.Jumlah,
                    Harga = input.Harga,
                    Status = input.Status
                };

                _context.Menus.Add(entity);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = entity.IdMenu }, entity);
            }
            catch (DbUpdateException)
            {
                return Conflict("ID Menu Makanan sudah ada, gunakan ID yang berbeda");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Terjadi kesalahan: {ex.Message}");
            }
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] MenuDto input)
        {
            if (id != input.IdMenu)
                return BadRequest("ID tidak sesuai");

            var existing = await _context.Menus.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.NamaMenu = input.NamaMenu;
            existing.Jumlah = input.Jumlah;
            existing.Harga = input.Harga;
            existing.Status = input.Status;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _context.Menus.FindAsync(id);
            if (existing == null)
                return NotFound();

            _context.Menus.Remove(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}