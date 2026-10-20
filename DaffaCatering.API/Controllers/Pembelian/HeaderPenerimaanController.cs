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

        // PERUBAHAN: detail ikut dimuat. Tanpa ini "Jumlah Terima" di form selalu 0.
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.HeaderPenerimaans
                .Include(h => h.DetailPenerimaans)
                .ToListAsync();
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

                var pembelian = await _context.HeaderPembelians
                    .FirstOrDefaultAsync(p => p.IdPembelian == input.IdPembelian);
                if (pembelian == null)
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

                    // 2. Stok (tambahBB() di WinForms)
                    // WinForms: bahan baku yang ditutup TIDAK menambah stok.
                    // Hapus 2 baris di bawah ini jika perilaku itu ternyata tidak diinginkan.
                    if (detailInput.StatusBahanBaku == "Ditutup")
                        continue;

                    var existingStok = await _context.DetailBahanBakus.FirstOrDefaultAsync(
                        d => d.IdBahanBaku == detailInput.IdBahanBaku
                          && d.TglKadaluwarsa == detailInput.TglKadaluwarsa);

                    if (existingStok != null)
                    {
                        // Batch (bahan baku + tgl kadaluwarsa) sudah ada -> hanya SisaStok yang bertambah
                        existingStok.SisaStok += detailInput.Jumlah;
                    }
                    else
                    {
                        // Batch baru -> WinForms mengisi StokAwal = 0, SisaStok = jumlah
                        _context.DetailBahanBakus.Add(new Models.DetailBahanBaku
                        {
                            IdBahanBaku = detailInput.IdBahanBaku,
                            TglKadaluwarsa = detailInput.TglKadaluwarsa,
                            IdSatuan = detailInput.IdSatuan,
                            StokAwal = 0,
                            SisaStok = detailInput.Jumlah
                        });
                    }
                }

                // 3. Update status HeaderPembelian (sama seperti WinForms)
                pembelian.Status = input.Status;

                // 4. Update status SEMUA penerimaan dengan No. Pembelian yang sama (sama seperti WinForms)
                var penerimaanTerkait = await _context.HeaderPenerimaans
                    .Where(h => h.IdPembelian == input.IdPembelian)
                    .ToListAsync();
                foreach (var h in penerimaanTerkait)
                    h.Status = input.Status;

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