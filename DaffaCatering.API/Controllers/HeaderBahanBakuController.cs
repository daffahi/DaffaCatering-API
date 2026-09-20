using DaffaCatering.API.Data;
using DaffaCatering.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DaffaCatering.API.Models;

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
    }
}