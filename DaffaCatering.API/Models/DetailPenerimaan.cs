using System;
using System.Collections.Generic;

namespace DaffaCatering.API.Models;

public partial class DetailPenerimaan
{
    public string IdPenerimaan { get; set; } = null!;

    public string IdBahanBaku { get; set; } = null!;

    public string IdSatuan { get; set; } = null!;

    public decimal Jumlah { get; set; }

    public DateOnly TglKadaluwarsa { get; set; }

    public string StatusBahanBaku { get; set; } = null!;

    public virtual HeaderBahanBaku IdBahanBakuNavigation { get; set; } = null!;

    public virtual HeaderPenerimaan IdPenerimaanNavigation { get; set; } = null!;

    public virtual SatuanBahanBaku IdSatuanNavigation { get; set; } = null!;
}
