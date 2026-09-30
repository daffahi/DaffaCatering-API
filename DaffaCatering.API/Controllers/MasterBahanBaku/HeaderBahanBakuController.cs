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

    public class HeaderBahanBakuController : ControllerBase
    {
        private readonly DaffaCateringContext _context;

        public HeaderBahanBakuController(DaffaCateringContext context)
        {
            _context = context;
        }

        // GET semua data, dengan opsi filter ?status=true/false
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool? status)
        {
            var query = _context.HeaderBahanBakus.AsNoTracking().AsQueryable();

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            var data = await query
                .OrderBy(x => x.IdBahanBaku)
                .Select(x => new HeaderBahanBakuDto
                {
                    IdBahanBaku = x.IdBahanBaku,
                    NamaBahanBaku = x.NamaBahanBaku,
                    Jenis = x.Jenis,
                    Status = x.Status,
                    Details = x.DetailBahanBakus
                        .OrderBy(d => d.TglKadaluwarsa)
                        .Select(d => new DetailBahanBakuDto
                        {
                            IdSatuan = d.IdSatuan,
                            TglKadaluwarsa = d.TglKadaluwarsa,
                            StokAwal = d.StokAwal ?? 0,
                            SisaStok = d.SisaStok ?? 0
                        })
                        .ToList()
                })
                .ToListAsync();

            return Ok(data);
        }

        // GET satu data spesifik berdasarkan ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var data = await _context.HeaderBahanBakus
                .AsNoTracking()
                .Where(h => h.IdBahanBaku == id)
                .Select(h => new HeaderBahanBakuDto
                {
                    IdBahanBaku = h.IdBahanBaku,
                    NamaBahanBaku = h.NamaBahanBaku,
                    Jenis = h.Jenis,
                    Status = h.Status,
                    Details = h.DetailBahanBakus
                        .OrderBy(d => d.TglKadaluwarsa)
                        .Select(d => new DetailBahanBakuDto
                        {
                            IdSatuan = d.IdSatuan,
                            TglKadaluwarsa = d.TglKadaluwarsa,
                            StokAwal = d.StokAwal ?? 0,
                            SisaStok = d.SisaStok ?? 0
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (data == null)
                return NotFound();
            return Ok(data);
        }

        // POST - Create data baru, dengan validasi otomatis dari DTO
        // dan error handling untuk duplicate ID
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] HeaderBahanBakuDto input)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var entity = new Models.HeaderBahanBaku
                {
                    IdBahanBaku = input.IdBahanBaku,
                    NamaBahanBaku = input.NamaBahanBaku,
                    Jenis = input.Jenis,
                    Status = input.Status
                };

                foreach (var detailInput in input.Details)
                {
                    entity.DetailBahanBakus.Add(new Models.DetailBahanBaku
                    {
                        IdBahanBaku = input.IdBahanBaku,
                        TglKadaluwarsa = detailInput.TglKadaluwarsa,
                        IdSatuan = detailInput.IdSatuan,
                        StokAwal = detailInput.StokAwal,
                        SisaStok = detailInput.SisaStok
                    });
                }

                _context.HeaderBahanBakus.Add(entity);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetById), new { id = entity.IdBahanBaku }, entity);
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                return Conflict($"Gagal menyimpan — ID sudah ada atau ID Satuan tidak valid. Detail: {ex.InnerException?.Message}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Terjadi kesalahan: {ex.Message}");
            }
        }

        // PUT - Update header + detail (update yang ada, tambah yang baru, tidak menghapus)
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] HeaderBahanBakuDto input)
        {
            if (id != input.IdBahanBaku)
                return BadRequest("ID tidak sesuai");

            if (input.Details.GroupBy(d => d.TglKadaluwarsa).Any(g => g.Count() > 1))
                return BadRequest("Tanggal kadaluwarsa tidak boleh sama dalam satu bahan baku");

            var existing = await _context.HeaderBahanBakus
                .Include(h => h.DetailBahanBakus)
                .FirstOrDefaultAsync(h => h.IdBahanBaku == id);

            if (existing == null)
                return NotFound();

            existing.NamaBahanBaku = input.NamaBahanBaku;
            existing.Jenis = input.Jenis;
            existing.Status = input.Status;

            foreach (var d in input.Details)
            {
                var row = existing.DetailBahanBakus
                    .FirstOrDefault(x => x.TglKadaluwarsa == d.TglKadaluwarsa);

                if (row != null) // UPDATE
                {
                    row.IdSatuan = d.IdSatuan;
                    row.StokAwal = d.StokAwal;
                    row.SisaStok = d.SisaStok;
                }
                else // INSERT
                {
                    existing.DetailBahanBakus.Add(new Models.DetailBahanBaku
                    {
                        IdBahanBaku = id,
                        TglKadaluwarsa = d.TglKadaluwarsa,
                        IdSatuan = d.IdSatuan,
                        StokAwal = d.StokAwal,
                        SisaStok = d.SisaStok
                    });
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return Conflict($"Gagal menyimpan — ID Satuan tidak valid atau data bentrok. Detail: {ex.InnerException?.Message}");
            }

            return NoContent();
        }

        // DELETE - Hapus data berdasarkan ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _context.HeaderBahanBakus.FindAsync(id);
            if (existing == null)
                return NotFound();

            _context.HeaderBahanBakus.Remove(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}