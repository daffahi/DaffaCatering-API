using DaffaCatering.API.Data;
using DaffaCatering.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DaffaCatering.API.DTOs.MasterUser;

namespace DaffaCatering.API.Controllers.MasterUser
{
    [ApiController]
    [Route("api/[controller]")]

    public class RoleController : ControllerBase
    {
        private readonly DaffaCateringContext _context;

        public RoleController(DaffaCateringContext context)
        {
            _context = context;
        }

        // GET semua role
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.Roles.ToListAsync();
            return Ok(data);
        }

        // GET satu role berdasarkan ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var data = await _context.Roles.FindAsync(id);
            if (data == null)
                return NotFound();
            return Ok(data);
        }

        // POST - Create Role
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RoleDto input)
        {
            try
            {
                var entity = new Role
                {
                    IdRole = input.IdRole,
                    NamaRole = input.NamaRole
                };

                _context.Roles.Add(entity);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = entity.IdRole }, entity);
            }
            catch (DbUpdateException)
            {
                return Conflict("ID Role sudah ada, gunakan ID yang berbeda");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Terjadi kesalahan: {ex.Message}");
            }
        }

        // PUT - Update role berdasarkan ID
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] RoleDto input)
        {
            if (id != input.IdRole)
                return BadRequest("ID tidak sesuai");

            var existing = await _context.Roles.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.NamaRole = input.NamaRole;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE - Hapus role berdasarkan ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _context.Roles.FindAsync(id);
            if (existing == null)
                return NotFound();

            _context.Roles.Remove(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}