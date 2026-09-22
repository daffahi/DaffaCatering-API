using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.API.DTOs.MasterBahanBaku
{
    public class DetailPenggunaanBahanBakuDto
    {
        [Required]
        [StringLength(10)]
        public string IdBahanBaku { get; set; } = null!;

        // idSatuan tidak dimasukkan karena backend akan ambil otomatis dari batch yang dipilih

        [Required]
        public decimal JumlahPenggunaan { get; set; }

        // TglKadaluwarsa TIDAK diminta dari user — backend yang pilih otomatis (FEFO)
    }
}