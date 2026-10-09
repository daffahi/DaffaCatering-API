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

        // Dikembalikan dengan properti "Details" supaya cocok dengan PenyesuaianBahanBakuModels di Blazor
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.HeaderPenyesuaians
                .AsNoTracking()
                .OrderByDescending(h => h.IdPenyesuaian)
                .Select(h => new
                {
                    h.IdPenyesuaian,
                    h.TglPenyesuaian,
                    Details = h.DetailPenyesuaians.Select(d => new
                    {
                        d.IdBahanBaku,
                        d.IdSatuan,
                        d.StokFisik,
                        d.SelisihStok,
                        d.TglKadaluwarsa,
                        d.Keterangan
                    }).ToList()
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var data = await _context.HeaderPenyesuaians
                .AsNoTracking()
                .Where(h => h.IdPenyesuaian == id)
                .Select(h => new
                {
                    h.IdPenyesuaian,
                    h.TglPenyesuaian,
                    Details = h.DetailPenyesuaians.Select(d => new
                    {
                        d.IdBahanBaku,
                        d.IdSatuan,
                        d.StokFisik,
                        d.SelisihStok,
                        d.TglKadaluwarsa,
                        d.Keterangan
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] HeaderPenyesuaianBahanBakuDto input)
        {
            // Bahan baku + tanggal kadaluwarsa yang sama tidak boleh muncul dua kali
            var adaDuplikat = input.Details
                .GroupBy(d => new { d.IdBahanBaku, d.TglKadaluwarsa })
                .Any(g => g.Count() > 1);

            if (adaDuplikat)
            {
                return BadRequest("Ada bahan baku dengan tanggal kadaluwarsa yang sama dalam satu penyesuaian");
            }

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

                    decimal selisih;

                    if (existingBatch != null)
                    {
                        // SisaStok bertipe decimal? sehingga harus diubah ke decimal dulu
                        decimal stokSebelumnya = 0;
                        if (existingBatch.SisaStok != null)
                        {
                            stokSebelumnya = existingBatch.SisaStok.Value;
                        }

                        // Batch sudah ada -> koreksi stoknya jadi sesuai stok fisik
                        selisih = detailInput.StokFisik - stokSebelumnya;
                        existingBatch.SisaStok = detailInput.StokFisik;
                    }
                    else
                    {
                        // Batch baru (hasil pecah dari batch lama ke tanggal baru).
                        // Selisih 0 karena stoknya dipindahkan dari batch lain (sama seperti WinForms)
                        selisih = 0;

                        _context.DetailBahanBakus.Add(new Models.DetailBahanBaku
                        {
                            IdBahanBaku = detailInput.IdBahanBaku,
                            TglKadaluwarsa = detailInput.TglKadaluwarsa,
                            IdSatuan = detailInput.IdSatuan,
                            StokAwal = detailInput.StokFisik,
                            SisaStok = detailInput.StokFisik
                        });

                        // FEFO: kurangi stok dari batch dengan tanggal kadaluwarsa terdekat
                        decimal sisaKurang = detailInput.StokFisik;

                        var batchLain = await _context.DetailBahanBakus
                            .Where(d => d.IdBahanBaku == detailInput.IdBahanBaku
                                     && d.TglKadaluwarsa != detailInput.TglKadaluwarsa
                                     && d.SisaStok > 0)
                            .OrderBy(d => d.TglKadaluwarsa)
                            .ToListAsync();

                        foreach (var batch in batchLain)
                        {
                            if (sisaKurang <= 0) break;

                            decimal stokBatch = 0;
                            if (batch.SisaStok != null)
                            {
                                stokBatch = batch.SisaStok.Value;
                            }

                            decimal dikurangi = Math.Min(stokBatch, sisaKurang);
                            batch.SisaStok = stokBatch - dikurangi;
                            sisaKurang -= dikurangi;
                        }
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

                // Jangan kembalikan entity utuh (ada navigation property yang bisa berputar saat diserialisasi)
                return CreatedAtAction(nameof(GetById),
                    new { id = entity.IdPenyesuaian },
                    new { entity.IdPenyesuaian, entity.TglPenyesuaian });
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
