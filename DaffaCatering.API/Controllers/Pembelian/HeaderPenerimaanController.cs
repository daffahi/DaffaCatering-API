using DaffaCatering.API.Data;
using DaffaCatering.API.DTOs.Pembelian;
using DaffaCatering.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DaffaCatering.API.Controllers.Pembelian
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "R001,R003")]

    public class HeaderPenerimaanController : ControllerBase
    {
        private readonly DaffaCateringContext _context;

        public HeaderPenerimaanController(DaffaCateringContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.HeaderPenerimaans.ToListAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var data = await _context.HeaderPenerimaans
                .Include(h => h.DetailPenerimaans)
                .FirstOrDefaultAsync(h => h.IdPenerimaan == id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] HeaderPenerimaanDto input)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validasi FK: Pemasok dan HeaderPembelian harus ada
                if (!await _context.Pemasoks.AnyAsync(p => p.IdPemasok == input.IdPemasok))
                    return BadRequest($"Pemasok '{input.IdPemasok}' tidak ditemukan");

                if (!await _context.HeaderPembelians.AnyAsync(p => p.IdPembelian == input.IdPembelian))
                    return BadRequest($"Pembelian '{input.IdPembelian}' tidak ditemukan");

                var entity = new Models.HeaderPenerimaan
                {
                    IdPenerimaan = input.IdPenerimaan,
                    IdPemasok = input.IdPemasok,
                    IdPembelian = input.IdPembelian,
                    TglPenerimaan = input.TglPenerimaan,
                    Status = input.Status
                };

                foreach (var detailInput in input.Details)
                {
                    // 1. Catat detail penerimaan
                    entity.DetailPenerimaans.Add(new Models.DetailPenerimaan
                    {
                        IdPenerimaan = input.IdPenerimaan,
                        IdBahanBaku = detailInput.IdBahanBaku,
                        IdSatuan = detailInput.IdSatuan,
                        Jumlah = detailInput.Jumlah,
                        TglKadaluwarsa = detailInput.TglKadaluwarsa,
                        StatusBahanBaku = detailInput.StatusBahanBaku
                    });

                    // 2. AUTO-UPDATE stok: cek apakah batch (bahan baku + tgl kadaluwarsa) ini sudah ada
                    var existingStok = await _context.DetailBahanBakus.FirstOrDefaultAsync(
                        d => d.IdBahanBaku == detailInput.IdBahanBaku
                          && d.TglKadaluwarsa == detailInput.TglKadaluwarsa);

                    if (existingStok != null)
                    {
                        // Batch dengan tanggal kadaluwarsa yang sama sudah ada -> tambah stoknya
                        existingStok.StokAwal += detailInput.Jumlah;
                        existingStok.SisaStok += detailInput.Jumlah;
                    }
                    else
                    {
                        // Batch baru -> buat entry DetailBahanBaku baru
                        _context.DetailBahanBakus.Add(new Models.DetailBahanBaku
                        {
                            IdBahanBaku = detailInput.IdBahanBaku,
                            TglKadaluwarsa = detailInput.TglKadaluwarsa,
                            IdSatuan = detailInput.IdSatuan,
                            StokAwal = detailInput.Jumlah,
                            SisaStok = detailInput.Jumlah
                        });
                    }
                }

                _context.HeaderPenerimaans.Add(entity);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetById), new { id = entity.IdPenerimaan }, entity);
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _context.HeaderPenerimaans.FindAsync(id);
            if (existing == null) return NotFound();
            _context.HeaderPenerimaans.Remove(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
