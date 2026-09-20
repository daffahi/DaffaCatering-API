using System;
using System.Collections.Generic;

namespace DaffaCatering.API.Models;

public partial class DetailPenjualan
{
    public string IdPenjualan { get; set; } = null!;

    public string IdMenu { get; set; } = null!;

    public string IdSatuan { get; set; } = null!;

    public int Jumlah { get; set; }

    public decimal Harga { get; set; }

    public virtual Menu IdMenuNavigation { get; set; } = null!;

    public virtual HeaderPenjualan IdPenjualanNavigation { get; set; } = null!;

    public virtual SatuanBahanBaku IdSatuanNavigation { get; set; } = null!;
}
