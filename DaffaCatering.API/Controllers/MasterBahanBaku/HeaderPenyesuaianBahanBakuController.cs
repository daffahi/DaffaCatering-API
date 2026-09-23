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

    public class HeaderPenyesuaianBahanBakuController : ControllerBase
    {
        private readonly DaffaCateringContext _context;

        public HeaderPenyesuaianBahanBakuController(DaffaCateringContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.HeaderPenyesuaians.ToListAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var data = await _context.HeaderPenyesuaians
                .Include(h => h.DetailPenyesuaians)
                .FirstOrDefaultAsync(h => h.IdPenyesuaian == id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] HeaderPenyesuaianBahanBakuDto input)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var entity = new Models.HeaderPenyesuaian
                {
                    IdPenyesuaian = input.IdPenyesuaian,
                    TglPenyesuaian = input.TglPenyesuaian
                };

                foreach (var detailInput in input.Details)
                {
                    // Cari batch yang sudah ada (idBahanBaku + tglKadaluwarsa)
                    var existingBatch = await _context.DetailBahanBakus.FirstOrDefaultAsync(
                        d => d.IdBahanBaku == detailInput.IdBahanBaku
                          && d.TglKadaluwarsa == detailInput.TglKadaluwarsa);

                    decimal stokSistemSebelumnya = existingBatch?.SisaStok ?? 0;
                    decimal selisih = detailInput.StokFisik - stokSistemSebelumnya;

                    if (existingBatch != null)
                    {
                        // Batch sudah ada -> koreksi stoknya jadi sesuai stok fisik
                        existingBatch.SisaStok = detailInput.StokFisik;
                    }
                    else
                    {
                        // Batch baru (misal hasil pecah dari batch lama ke tanggal baru)
                        _context.DetailBahanBakus.Add(new Models.DetailBahanBaku
                        {
                            IdBahanBaku = detailInput.IdBahanBaku,
                            TglKadaluwarsa = detailInput.TglKadaluwarsa,
                            IdSatuan = detailInput.IdSatuan,
                            StokAwal = detailInput.StokFisik,
                            SisaStok = detailInput.StokFisik
                        });
                    }

                    // Catat histori penyesuaian, termasuk selisihnya
                    entity.DetailPenyesuaians.Add(new Models.DetailPenyesuaian
                    {
                        IdPenyesuaian = input.IdPenyesuaian,
                        IdBahanBaku = detailInput.IdBahanBaku,
                        TglKadaluwarsa = detailInput.TglKadaluwarsa,
                        IdSatuan = detailInput.IdSatuan,
                        StokFisik = detailInput.StokFisik,
                        SelisihStok = selisih,
                        Keterangan = detailInput.Keterangan
                    });
                }

                _context.HeaderPenyesuaians.Add(entity);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetById), new { id = entity.IdPenyesuaian }, entity);
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
            var existing = await _context.HeaderPenyesuaians.FindAsync(id);
            if (existing == null) return NotFound();
            _context.HeaderPenyesuaians.Remove(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}