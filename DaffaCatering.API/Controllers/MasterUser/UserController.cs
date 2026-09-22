using DaffaCatering.API.Data;
using DaffaCatering.API.DTOs.MasterUser;
using DaffaCatering.API.DTOs.MasterUser;
using DaffaCatering.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DaffaCatering.API.Controllers.User
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly DaffaCateringContext _context;

        public UserController(DaffaCateringContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool? status)
        {
            var query = _context.Users.AsQueryable();
            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);
            var data = await query.ToListAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var data = await _context.Users.FindAsync(id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserDto input)
        {
            try
            {
                var entity = new Models.User
                {
                    IdUser = input.IdUser,
                    IdRole = input.IdRole,
                    NamaUser = input.NamaUser,
                    Password = BCrypt.Net.BCrypt.HashPassword(input.Password),  // ⬅ HASH, bukan plain
                    Status = input.Status
                };

                _context.Users.Add(entity);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = entity.IdUser }, entity);
            }
            catch (DbUpdateException)
            {
                return Conflict("ID User sudah ada, gunakan ID yang berbeda");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UserDto input)
        {
            if (id != input.IdUser) return BadRequest("ID tidak sesuai");

            var existing = await _context.Users.FindAsync(id);
            if (existing == null) return NotFound();

            existing.IdRole = input.IdRole;
            existing.NamaUser = input.NamaUser;
            existing.Password = BCrypt.Net.BCrypt.HashPassword(input.Password);  // ⬅ HASH juga di update
            existing.Status = input.Status;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _context.Users.FindAsync(id);
            if (existing == null) return NotFound();
            _context.Users.Remove(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}