using System;
using System.Collections.Generic;

namespace DaffaCatering.API.Models;

public partial class Menu
{
    public string IdMenu { get; set; } = null!;

    public string IdSatuan { get; set; } = null!;

    public string NamaMenu { get; set; } = null!;

    public int Jumlah { get; set; }

    public decimal Harga { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<DetailPenjualan> DetailPenjualans { get; set; } = new List<DetailPenjualan>();

    public virtual ICollection<DetailPesanan> DetailPesanans { get; set; } = new List<DetailPesanan>();

    public virtual ICollection<HeaderResep> HeaderReseps { get; set; } = new List<HeaderResep>();

    public virtual SatuanBahanBaku IdSatuanNavigation { get; set; } = null!;
}
