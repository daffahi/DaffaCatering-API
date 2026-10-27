using DaffaCatering.API.Data;
using DaffaCatering.API.DTOs.Penjualan;
using DaffaCatering.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DaffaCatering.API.Controllers.Penjualan
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "R001,R002")]

    public class HeaderPenjualanController : ControllerBase
    {
        private readonly DaffaCateringContext _context;

        private static readonly string[] MetodeValid = { "Tunai", "Cash", "Debit" };

        public HeaderPenjualanController(DaffaCateringContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? status)
        {
            var query = _context.HeaderPenjualans.AsNoTracking().AsQueryable();
            if (!string.IsNullOrEmpty(status))
                query = query.Where(x => x.Status == status);

            var data = await query
                .Select(h => new
                {
                    h.IdPenjualan,
                    h.IdPesanan,
                    h.IdPelanggan,
                    h.MetodePembayaran,
                    h.TglPenjualan,
                    h.UangMuka,
                    h.Total,
                    h.Status,
                    Details = h.DetailPenjualans.Select(d => new
                    {
                        d.IdPenjualan,
                        d.IdMenu,
                        d.IdSatuan,
                        d.Jumlah,
                        d.Harga
                    })
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var data = await _context.HeaderPenjualans
                .AsNoTracking()
                .Where(h => h.IdPenjualan == id)
                .Select(h => new
                {
                    h.IdPenjualan,
                    h.IdPesanan,
                    h.IdPelanggan,
                    h.MetodePembayaran,
                    h.TglPenjualan,
                    h.UangMuka,
                    h.Total,
                    h.Status,
                    Details = h.DetailPenjualans.Select(d => new
                    {
                        d.IdPenjualan,
                        d.IdMenu,
                        d.IdSatuan,
                        d.Jumlah,
                        d.Harga
                    })
                })
                .FirstOrDefaultAsync();

            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] HeaderPenjualanDto input)
        {
            // ---------- VALIDASI (tanpa menyentuh database transaksi) ----------
            if (string.IsNullOrWhiteSpace(input.IdPenjualan))
                return BadRequest("No. Penjualan belum ter-generate, coba buka form kembali");

            if (string.IsNullOrWhiteSpace(input.IdPelanggan))
                return BadRequest("Harap pilih pelanggan terlebih dahulu");

            var galatHeader = ValidasiMetode(input.MetodePembayaran);
            if (galatHeader != null) return BadRequest(galatHeader);

            var galatDetail = ValidasiDetail(input);
            if (galatDetail != null) return BadRequest(galatDetail);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (await _context.HeaderPenjualans.AnyAsync(h => h.IdPenjualan == input.IdPenjualan))
                    return Conflict($"No. Penjualan '{input.IdPenjualan}' sudah ada, coba buka form kembali");

                if (!await _context.Pelanggans.AnyAsync(p => p.IdPelanggan == input.IdPelanggan))
                    return BadRequest($"Pelanggan '{input.IdPelanggan}' tidak ditemukan");

                // Pesanan opsional; string kosong dianggap tanpa pesanan
                var idPesanan = string.IsNullOrWhiteSpace(input.IdPesanan) ? null : input.IdPesanan;
                Models.HeaderPesanan? pesanan = null;

                if (idPesanan != null)
                {
                    pesanan = await _context.HeaderPesanans.FirstOrDefaultAsync(p => p.IdPesanan == idPesanan);

                    if (pesanan == null)
                        return BadRequest($"Pesanan '{idPesanan}' tidak ditemukan");

                    if (!string.Equals(pesanan.Status, "Pending", StringComparison.OrdinalIgnoreCase))
                        return BadRequest($"Pesanan '{idPesanan}' sudah ditutup");

                    if (pesanan.IdPelanggan != input.IdPelanggan)
                        return BadRequest($"Pelanggan tidak sesuai dengan pesanan '{idPesanan}'");
                }

                decimal total = input.Details.Sum(d => d.Jumlah * d.Harga);

                var entity = new Models.HeaderPenjualan
                {
                    IdPenjualan = input.IdPenjualan,
                    IdPelanggan = input.IdPelanggan,
                    IdPesanan = idPesanan,
                    TglPenjualan = input.TglPenjualan,
                    MetodePembayaran = input.MetodePembayaran,
                    UangMuka = pesanan?.UangMuka,   // NULL jika tanpa pesanan (sama seperti WinForms)
                    Status = "Close",
                    Total = total
                };

                foreach (var detailInput in input.Details)
                {
                    entity.DetailPenjualans.Add(new Models.DetailPenjualan
                    {
                        IdPenjualan = input.IdPenjualan,
                        IdMenu = detailInput.IdMenu,
                        IdSatuan = detailInput.IdSatuan,
                        Jumlah = (int)detailInput.Jumlah,
                        Harga = detailInput.Harga
                    });
                }

                // Cek + kurangi stok bahan baku (FEFO); perubahan baru tersimpan di SaveChanges di bawah
                var galatStok = await KurangiStok(input);
                if (galatStok != null)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(galatStok);
                }

                // Pesanan asal ditutup
                if (pesanan != null)
                    pesanan.Status = "Close";

                _context.HeaderPenjualans.Add(entity);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetById), new { id = entity.IdPenjualan }, entity);
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

        // Edit: hanya metode pembayaran dan detail menu; stok tidak disentuh (sama seperti WinForms)
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] HeaderPenjualanDto input)
        {
            if (id != input.IdPenjualan)
                return BadRequest("ID penjualan tidak sesuai");

            var galatHeader = ValidasiMetode(input.MetodePembayaran);
            if (galatHeader != null) return BadRequest(galatHeader);

            var galatDetail = ValidasiDetail(input);
            if (galatDetail != null) return BadRequest(galatDetail);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var entity = await _context.HeaderPenjualans
                    .Include(h => h.DetailPenjualans)
                    .FirstOrDefaultAsync(h => h.IdPenjualan == id);

                if (entity == null) return NotFound();

                // HEADER
                entity.MetodePembayaran = input.MetodePembayaran;
                entity.Status = "Close";
                entity.Total = input.Details.Sum(d => d.Jumlah * d.Harga);

                // DETAIL: hapus yang lama, lalu simpan yang baru
                _context.DetailPenjualans.RemoveRange(entity.DetailPenjualans);
                await _context.SaveChangesAsync();

                foreach (var detailInput in input.Details)
                {
                    _context.DetailPenjualans.Add(new Models.DetailPenjualan
                    {
                        IdPenjualan = id,
                        IdMenu = detailInput.IdMenu,
                        IdSatuan = detailInput.IdSatuan,
                        Jumlah = (int)detailInput.Jumlah,
                        Harga = detailInput.Harga
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                return Conflict($"Gagal memperbarui, cek referensi data. Detail: {ex.InnerException?.Message}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Terjadi kesalahan: {ex.Message}");
            }
        }

        // =====================================================
        // VALIDASI (padanan btnTambah_Click / btnSimpan_Click WinForms)
        // =====================================================
        private static string? ValidasiMetode(string? metode)
        {
            if (string.IsNullOrWhiteSpace(metode))
                return "Harap pilih metode pembayaran";

            if (!MetodeValid.Contains(metode))
                return "Metode pembayaran tidak valid";

            return null;
        }

        private static string? ValidasiDetail(HeaderPenjualanDto input)
        {
            if (input.Details == null || !input.Details.Any())
                return "Harap tambahkan minimal satu menu";

            foreach (var d in input.Details)
            {
                if (string.IsNullOrWhiteSpace(d.IdMenu))
                    return "Harap pilih menu terlebih dahulu";

                if (d.Jumlah <= 0 || d.Jumlah != decimal.Truncate(d.Jumlah))
                    return $"Jumlah menu '{d.IdMenu}' harus berupa angka bulat lebih dari 0";

                if (d.Jumlah > 999)
                    return $"Jumlah menu '{d.IdMenu}' maksimal 3 digit";

                if (string.IsNullOrWhiteSpace(d.IdSatuan))
                    return $"Menu '{d.IdMenu}' belum memiliki satuan";

                if (d.Harga <= 0)
                    return $"Harga menu '{d.IdMenu}' tidak valid";
            }

            // Duplikat = menu yang sama
            var kembar = input.Details.GroupBy(d => d.IdMenu).FirstOrDefault(g => g.Count() > 1);
            if (kembar != null)
                return $"Menu '{kembar.Key}' sudah ada di daftar penjualan";

            return null;
        }

        // =====================================================
        // HELPER STOK (padanan cekStok / kurangiStok / hitungKonversi)
        // =====================================================
        private async Task<string?> KurangiStok(HeaderPenjualanDto input)
        {
            const string galatKonversi = "Konversi satuan tidak ditemukan! Silahkan input konversi terlebih dahulu.";

            // TAHAP 1: akumulasi kebutuhan bahan baku dari semua menu
            var kebutuhan = new Dictionary<string, decimal>();
            var satuanAcuan = new Dictionary<string, string>();

            foreach (var detailInput in input.Details)
            {
                var resep = await _context.HeaderReseps
                    .Include(r => r.DetailReseps)
                    .FirstOrDefaultAsync(r => r.IdMenu == detailInput.IdMenu && r.Status == true);

                if (resep == null)
                    return $"Menu '{detailInput.IdMenu}' belum punya resep aktif";

                foreach (var komposisi in resep.DetailReseps)
                {
                    decimal jumlahResep = komposisi.Jumlah * detailInput.Jumlah;

                    if (kebutuhan.ContainsKey(komposisi.IdBahanBaku))
                    {
                        var k = await Konversi(komposisi.IdSatuan, satuanAcuan[komposisi.IdBahanBaku], jumlahResep);
                        if (k == null) return galatKonversi;
                        kebutuhan[komposisi.IdBahanBaku] += k.Value;
                    }
                    else
                    {
                        kebutuhan[komposisi.IdBahanBaku] = jumlahResep;
                        satuanAcuan[komposisi.IdBahanBaku] = komposisi.IdSatuan;
                    }
                }
            }

            // TAHAP 2: cek stok lalu kurangi FEFO, sekali per bahan baku
            var today = DateOnly.FromDateTime(DateTime.Today); // jika TglKadaluwarsa bertipe DateOnly: DateOnly.FromDateTime(DateTime.Today)

            foreach (var (idBahan, totalButuh) in kebutuhan)
            {
                var bahan = await _context.HeaderBahanBakus.FirstOrDefaultAsync(h => h.IdBahanBaku == idBahan);
                var nama = bahan?.NamaBahanBaku ?? idBahan;
                var acuan = satuanAcuan[idBahan];

                var q = _context.DetailBahanBakus.Where(d => d.IdBahanBaku == idBahan && d.SisaStok > 0);
                q = bahan?.Jenis == true
                    ? q.Where(d => d.TglKadaluwarsa > today)
                    : q.Where(d => d.TglKadaluwarsa >= today);

                var batches = await q.OrderBy(d => d.TglKadaluwarsa).ToListAsync();

                if (batches.Count == 0)
                    return $"Stok {nama} tidak ditemukan atau sudah kadaluwarsa";

                decimal tersedia = 0;
                foreach (var b in batches)
                {
                    var s = await Konversi(b.IdSatuan, acuan, b.SisaStok ?? 0);
                    if (s == null) return galatKonversi;
                    tersedia += s.Value;
                }

                if (tersedia < totalButuh)
                    return $"Stok {nama} tidak mencukupi. Dibutuhkan {totalButuh}, tersedia {tersedia}";

                decimal sisa = totalButuh;
                foreach (var b in batches)
                {
                    if (sisa <= 0) break;

                    var stokAcuan = (await Konversi(b.IdSatuan, acuan, b.SisaStok ?? 0))!.Value;
                    var ambilAcuan = Math.Min(sisa, stokAcuan);
                    var ambilStok = (await Konversi(acuan, b.IdSatuan, ambilAcuan))!.Value;

                    b.SisaStok -= ambilStok;
                    sisa -= ambilAcuan;
                }
            }

            return null;
        }

        // null = konversi satuan tidak ditemukan
        private async Task<decimal?> Konversi(string dari, string ke, decimal jumlah)
        {
            if (dari == ke) return jumlah;

            var k = await _context.KonversiSatuans.FirstOrDefaultAsync(x =>
                (x.IdSatuanBesar == dari && x.IdSatuanKecil == ke) ||
                (x.IdSatuanBesar == ke && x.IdSatuanKecil == dari));

            if (k == null) return null;

            return dari == k.IdSatuanBesar ? jumlah * k.NilaiKonversi : jumlah / k.NilaiKonversi;
        }
    }
}