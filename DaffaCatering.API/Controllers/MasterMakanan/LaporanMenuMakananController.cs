using DaffaCatering.API.Constants;
using DaffaCatering.API.Data;
using DaffaCatering.API.DTOs.MasterMakanan;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DaffaCatering.API.Controllers.MasterMakanan
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.AdminPenjualan)]
    public class LaporanMenuMakananController : ControllerBase
    {
        private readonly DaffaCateringContext _context;

        public LaporanMenuMakananController(DaffaCateringContext context)
        {
            _context = context;
        }

        // Daftar semua menu beserta nama satuannya (sama seperti query loadDataAll di WinForms)
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var hasil = await (
                from m in _context.Menus.AsNoTracking()
                join s in _context.SatuanBahanBakus.AsNoTracking() on m.IdSatuan equals s.IdSatuan
                orderby m.IdMenu
                select new LaporanMenuMakananDto
                {
                    IdMenu = m.IdMenu,
                    NamaMenu = m.NamaMenu,
                    Jumlah = m.Jumlah,
                    NamaSatuan = s.NamaSatuan,
                    Harga = m.Harga
                }).ToListAsync();

            return Ok(hasil);
        }
    }
}