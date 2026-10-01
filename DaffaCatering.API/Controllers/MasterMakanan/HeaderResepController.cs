using DaffaCatering.API.Data;
using DaffaCatering.API.DTOs.MasterMakanan;
using DaffaCatering.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DaffaCatering.API.Controllers.Resep
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "R001,R002")]
    public class HeaderResepController : ControllerBase
    {
        private readonly DaffaCateringContext _context;

        public HeaderResepController(DaffaCateringContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool? status)
        {
            var query = _context.HeaderReseps.AsQueryable();
            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);
            var data = await query.ToListAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var data = await _context.HeaderReseps
                .Where(h => h.IdResep == id)
                .Select(h => new
                {
                    idResep = h.IdResep,
                    idMenu = h.IdMenu,
                    status = h.Status,
                    details = h.DetailReseps.Select(d => new
                    {
                        idBahanBaku = d.IdBahanBaku,
                        idSatuan = d.IdSatuan,
                        jumlah = d.Jumlah
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] HeaderResepDto input)
        {
            // VALIDASI DULU, sebelum transaksi dibuka
            if (input.Details == null || input.Details.Count == 0)
                return BadRequest("Resep harus memiliki minimal 1 bahan baku");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (!await _context.Menus.AnyAsync(m => m.IdMenu == input.IdMenu))
                    return BadRequest($"Menu '{input.IdMenu}' tidak ditemukan");

                var entity = new Models.HeaderResep
                {
                    IdResep = input.IdResep,
                    IdMenu = input.IdMenu,
                    Status = input.Status
                };

                foreach (var detailInput in input.Details)
                {
                    entity.DetailReseps.Add(new Models.DetailResep
                    {
                        IdResep = input.IdResep,
                        IdBahanBaku = detailInput.IdBahanBaku,
                        IdSatuan = detailInput.IdSatuan,
                        Jumlah = detailInput.Jumlah
                    });
                }

                _context.HeaderReseps.Add(entity);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetById), new { id = entity.IdResep }, null);
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                return Conflict($"Gagal menyimpan — cek ID atau referensi. Detail: {ex.InnerException?.Message}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Terjadi kesalahan: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] HeaderResepDto input)
        {
            // VALIDASI DULU, sebelum transaksi dibuka
            if (id != input.IdResep)
                return BadRequest("ID tidak sesuai");

            if (input.Details == null || input.Details.Count == 0)
                return BadRequest("Resep harus memiliki minimal 1 bahan baku");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existing = await _context.HeaderReseps
                    .Include(h => h.DetailReseps)
                    .FirstOrDefaultAsync(h => h.IdResep == id);

                if (existing == null) return NotFound();

                if (!await _context.Menus.AnyAsync(m => m.IdMenu == input.IdMenu))
                    return BadRequest($"Menu '{input.IdMenu}' tidak ditemukan");

                existing.IdMenu = input.IdMenu;
                existing.Status = input.Status;

                var idBaru = input.Details.Select(d => d.IdBahanBaku).ToHashSet();

                var dihapus = existing.DetailReseps
                    .Where(d => !idBaru.Contains(d.IdBahanBaku))
                    .ToList();
                foreach (var d in dihapus)
                    _context.DetailReseps.Remove(d);

                foreach (var detailInput in input.Details)
                {
                    var existingDetail = existing.DetailReseps
                        .FirstOrDefault(d => d.IdBahanBaku == detailInput.IdBahanBaku);

                    if (existingDetail != null)
                    {
                        existingDetail.IdSatuan = detailInput.IdSatuan;
                        existingDetail.Jumlah = detailInput.Jumlah;
                    }
                    else
                    {
                        existing.DetailReseps.Add(new Models.DetailResep
                        {
                            IdResep = id,
                            IdBahanBaku = detailInput.IdBahanBaku,
                            IdSatuan = detailInput.IdSatuan,
                            Jumlah = detailInput.Jumlah
                        });
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Terjadi kesalahan: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _context.HeaderReseps.FindAsync(id);
            if (existing == null) return NotFound();
            _context.HeaderReseps.Remove(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}