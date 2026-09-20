using System;
using System.Collections.Generic;

namespace DaffaCatering.API.Models;

public partial class DetailPenggunaan
{
    public string IdPenggunaan { get; set; } = null!;

    public string IdBahanBaku { get; set; } = null!;

    public string IdSatuan { get; set; } = null!;

    public decimal JumlahPenggunaan { get; set; }

    public DateOnly TglKadaluwarsa { get; set; }

    public virtual HeaderBahanBaku IdBahanBakuNavigation { get; set; } = null!;

    public virtual HeaderPenggunaan IdPenggunaanNavigation { get; set; } = null!;

    public virtual SatuanBahanBaku IdSatuanNavigation { get; set; } = null!;
}
