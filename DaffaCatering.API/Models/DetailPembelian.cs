using System;
using System.Collections.Generic;

namespace DaffaCatering.API.Models;

public partial class DetailPembelian
{
    public string IdPembelian { get; set; } = null!;

    public string IdBahanBaku { get; set; } = null!;

    public string IdSatuan { get; set; } = null!;

    public decimal Jumlah { get; set; }

    public decimal Harga { get; set; }

    public virtual HeaderBahanBaku IdBahanBakuNavigation { get; set; } = null!;

    public virtual HeaderPembelian IdPembelianNavigation { get; set; } = null!;

    public virtual SatuanBahanBaku IdSatuanNavigation { get; set; } = null!;
}
