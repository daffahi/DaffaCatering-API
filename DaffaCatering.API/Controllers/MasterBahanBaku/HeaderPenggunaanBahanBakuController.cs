using DaffaCatering.API.Data;
using DaffaCatering.API.DTOs.MasterBahanBaku;
using DaffaCatering.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DaffaCatering.API.Controllers.MasterBahanBaku
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]

    public class HeaderPenggunaanController : ControllerBase
    {
        private readonly DaffaCateringContext _context;

        public HeaderPenggunaanController(DaffaCateringContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.HeaderPenggunaans.ToListAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var data = await _context.HeaderPenggunaans
                .Include(h => h.DetailPenggunaans)
                .FirstOrDefaultAsync(h => h.IdPenggunaan == id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] HeaderPenggunaanBahanBakuDto input)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var entity = new Models.HeaderPenggunaan
                {
                    IdPenggunaan = input.IdPenggunaan,
                    TglPenggunaan = input.TglPenggunaan
                };

                foreach (var detailInput in input.Details)
                {
                    decimal sisaDibutuhkan = detailInput.JumlahPenggunaan;

                    // Ambil semua batch bahan baku ini yang masih ada stok,
                    // urutkan dari tanggal kadaluwarsa PALING DEKAT (FEFO)
                    var batches = await _context.DetailBahanBakus
                        .Where(d => d.IdBahanBaku == detailInput.IdBahanBaku && d.SisaStok > 0)
                        .OrderBy(d => d.TglKadaluwarsa)
                        .ToListAsync();

                    decimal totalTersedia = batches.Sum(b => b.SisaStok ?? 0);
                    if (totalTersedia < sisaDibutuhkan)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest(
                            $"Stok '{detailInput.IdBahanBaku}' tidak cukup. " +
                            $"Dibutuhkan {sisaDibutuhkan}, tersedia {totalTersedia}");
                    }

                    // Ambil dari batch dengan expired paling dekat dulu (FEFO),
                    // pecah ke beberapa batch kalau 1 batch nggak cukup
                    foreach (var batch in batches)
                    {
                        if (sisaDibutuhkan <= 0) break;

                        decimal ambilDariBatchIni = Math.Min(batch.SisaStok ?? 0, sisaDibutuhkan);

                        // Kurangi stok di batch ini
                        batch.SisaStok -= ambilDariBatchIni;
                        sisaDibutuhkan -= ambilDariBatchIni;

                        // Catat detail penggunaan untuk batch spesifik ini
                        entity.DetailPenggunaans.Add(new Models.DetailPenggunaan
                        {
                            IdPenggunaan = input.IdPenggunaan,
                            IdBahanBaku = detailInput.IdBahanBaku,
                            TglKadaluwarsa = batch.TglKadaluwarsa,
                            IdSatuan = batch.IdSatuan,
                            JumlahPenggunaan = ambilDariBatchIni
                        });
                    }
                }

                _context.HeaderPenggunaans.Add(entity);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetById), new { id = entity.IdPenggunaan }, entity);
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
            var existing = await _context.HeaderPenggunaans.FindAsync(id);
            if (existing == null) return NotFound();
            _context.HeaderPenggunaans.Remove(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}