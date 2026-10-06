using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.API.DTOs.MasterBahanBaku
{
    public class DetailPenggunaanBahanBakuDto
    {
        [Required]
        [StringLength(10)]
        public string IdBahanBaku { get; set; } = null!;

        // Batch yang dipakai dipilih user (bahan baku + tanggal kadaluwarsa).
        // idSatuan tidak dimasukkan karena backend mengambilnya dari batch tersebut.
        public DateOnly TglKadaluwarsa { get; set; }

        [Range(0.01, 999999999, ErrorMessage = "Jumlah penggunaan harus lebih dari 0")]
        public decimal JumlahPenggunaan { get; set; }
    }
}