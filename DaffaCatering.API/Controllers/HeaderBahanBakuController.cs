using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DaffaCatering.API.Data;

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
    }
}