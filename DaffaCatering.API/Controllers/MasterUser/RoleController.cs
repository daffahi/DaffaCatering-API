using DaffaCatering.API.Constants;
using DaffaCatering.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DaffaCatering.API.Controllers.MasterUser
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.Admin)]
    public class RoleController : ControllerBase
    {
        private readonly DaffaCateringContext _context;

        public RoleController(DaffaCateringContext context)
        {
            _context = context;
        }

        // Daftar role untuk dropdown di halaman Pengaturan.
        // Role bersifat tetap (hak akses diatur di kode lewat AppRoles), jadi tidak ada tambah/ubah/hapus.
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.Roles.AsNoTracking()
                .OrderBy(r => r.IdRole)
                .ToListAsync();

            return Ok(data);
        }
    }
}