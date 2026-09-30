using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.Blazor.Models.MasterSatuanBahanBaku
{
    public class KonversiSatuanModels
    {
        [Required(ErrorMessage = "ID Konversi wajib diisi")]
        [StringLength(10, ErrorMessage = "ID Konversi maksimal 10 karakter")]
        public string IdKonversi { get; set; } = null!;

        [Required(ErrorMessage = "ID Satuan Besar wajib diisi")]
        [StringLength(10, ErrorMessage = "ID Satuan Besar maksimal 10 karakter")]
        public string IdSatuanBesar { get; set; } = null!;

        [Required(ErrorMessage = "ID Satuan Kecil wajib diisi")]
        [StringLength(10, ErrorMessage = "ID Satuan Kecil maksimal 10 karakter")]
        public string IdSatuanKecil { get; set; } = null!;

        public decimal NilaiKonversi { get; set; }
    }
}