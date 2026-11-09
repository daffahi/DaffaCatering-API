using DaffaCatering.API.Constants;
using DaffaCatering.API.Data;
using DaffaCatering.API.DTOs.MasterBahanBaku;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DaffaCatering.API.Controllers.MasterBahanBaku
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.AdminPersediaan)]
    public class LaporanDaftarBahanBakuController : ControllerBase
    {
        private readonly DaffaCateringContext _context;

        public LaporanDaftarBahanBakuController(DaffaCateringContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var bahanBaku = await _context.HeaderBahanBakus.AsNoTracking()
                .OrderBy(h => h.Jenis)
                .ThenBy(h => h.IdBahanBaku)
                .Select(h => new { h.IdBahanBaku, h.NamaBahanBaku, h.Jenis })
                .ToListAsync();

            // Satuan diambil dari batch bahan baku (satu bahan baku = satu satuan)
            var satuanBahan = await _context.DetailBahanBakus.AsNoTracking()
                .GroupBy(d => d.IdBahanBaku)
                .Select(g => new { IdBahanBaku = g.Key, IdSatuan = g.Min(d => d.IdSatuan) })
                .ToDictionaryAsync(x => x.IdBahanBaku, x => x.IdSatuan);

            var namaSatuan = await _context.SatuanBahanBakus.AsNoTracking()
                .ToDictionaryAsync(s => s.IdSatuan, s => s.NamaSatuan);

            var hasil = bahanBaku.Select(b => new LaporanDaftarBahanBakuDto
            {
                IdBahanBaku = b.IdBahanBaku,
                NamaBahanBaku = b.NamaBahanBaku,
                // bit jenis: false = Persediaan, true = Bahan Habis Pakai (tukar kalau kebalikan)
                Jenis = b.Jenis ? "Bahan Habis Pakai" : "Persediaan",
                NamaSatuan = satuanBahan.TryGetValue(b.IdBahanBaku, out var idSatuan)
                             && namaSatuan.TryGetValue(idSatuan, out var nama)
                    ? nama
                    : "-"
            }).ToList();

            return Ok(hasil);
        }
    }
}