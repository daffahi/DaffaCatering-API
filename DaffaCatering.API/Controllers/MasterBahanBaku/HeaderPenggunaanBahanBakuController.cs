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
    [Authorize(Roles = "R001,R004")]

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
            // Satu baris per kombinasi bahan baku + tanggal kadaluwarsa (aturan yang sama dengan WinForms)
            var adaKembar = input.Details
                .GroupBy(d => new { d.IdBahanBaku, d.TglKadaluwarsa })
                .Any(g => g.Count() > 1);
            if (adaKembar)
                return BadRequest("Bahan baku dengan tanggal kadaluwarsa yang sama tidak boleh muncul lebih dari satu kali.");

            var hariIni = DateOnly.FromDateTime(DateTime.Today);

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
                    // Batch yang dipilih user: bahan baku + tanggal kadaluwarsa
                    var batch = await _context.DetailBahanBakus.FirstOrDefaultAsync(d =>
                        d.IdBahanBaku == detailInput.IdBahanBaku &&
                        d.TglKadaluwarsa == detailInput.TglKadaluwarsa);

                    if (batch == null)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest($"Batch '{detailInput.IdBahanBaku}' dengan tanggal kadaluwarsa {detailInput.TglKadaluwarsa:dd MMM yyyy} tidak ditemukan.");
                    }

                    if (batch.TglKadaluwarsa < hariIni)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest($"Bahan baku '{detailInput.IdBahanBaku}' dengan tanggal kadaluwarsa {detailInput.TglKadaluwarsa:dd MMM yyyy} sudah kadaluwarsa.");
                    }

                    decimal stok = batch.SisaStok ?? 0;
                    if (stok < detailInput.JumlahPenggunaan)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest(
                            $"Stok '{detailInput.IdBahanBaku}' (kadaluwarsa {detailInput.TglKadaluwarsa:dd MMM yyyy}) tidak cukup. " +
                            $"Dibutuhkan {detailInput.JumlahPenggunaan}, tersedia {stok}");
                    }

                    batch.SisaStok = stok - detailInput.JumlahPenggunaan;

                    entity.DetailPenggunaans.Add(new Models.DetailPenggunaan
                    {
                        IdPenggunaan = input.IdPenggunaan,
                        IdBahanBaku = detailInput.IdBahanBaku,
                        TglKadaluwarsa = batch.TglKadaluwarsa,
                        IdSatuan = batch.IdSatuan,
                        JumlahPenggunaan = detailInput.JumlahPenggunaan
                    });
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