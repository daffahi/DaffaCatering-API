using DaffaCatering.API.Constants;
using DaffaCatering.API.Data;
using DaffaCatering.API.DTOs.MasterPemasok;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DaffaCatering.API.Controllers.MasterPemasok
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.AdminPembelian)]

    public class DaftarPemasokController : ControllerBase
    {
        private readonly DaffaCateringContext _context;

        public DaftarPemasokController(DaffaCateringContext context)
        {
            _context = context;
        }

        // status: semua | aktif | nonaktif
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string status = "semua")
        {
            if (status != "semua" && status != "aktif" && status != "nonaktif")
                return BadRequest("Filter status tidak valid");

            var query = _context.Pemasoks.AsNoTracking();

            if (status == "aktif")
                query = query.Where(p => p.Status);
            else if (status == "nonaktif")
                query = query.Where(p => !p.Status);

            var hasil = await query
                .OrderBy(p => p.IdPemasok)
                .Select(p => new DaftarPemasokDto
                {
                    IdPemasok = p.IdPemasok,
                    NamaPemasok = p.NamaPemasok,
                    NoKontak = p.NoKontak,
                    Alamat = p.Alamat,
                    TglBergabung = p.TglBergabung,
                    Status = p.Status
                })
                .ToListAsync();

            return Ok(hasil);
        }
    }
}