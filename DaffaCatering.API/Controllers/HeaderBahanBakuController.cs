using DaffaCatering.API.Data;
using DaffaCatering.API.Models;
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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.HeaderBahanBakus.ToListAsync();
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] HeaderBahanBaku input)
        {
            _context.HeaderBahanBakus.Add(input);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), new { id = input.IdBahanBaku }, input);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var data = await _context.HeaderBahanBakus.FindAsync(id);
            if (data == null)
                return NotFound();
            return Ok(data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] HeaderBahanBaku input)
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