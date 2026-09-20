using System;
using System.Collections.Generic;

namespace DaffaCatering.API.Models;

public partial class DetailBahanBaku
{
    public string IdBahanBaku { get; set; } = null!;

    public string IdSatuan { get; set; } = null!;

    public decimal? StokAwal { get; set; }

    public DateOnly TglKadaluwarsa { get; set; }

    public decimal? SisaStok { get; set; }

    public virtual HeaderBahanBaku IdBahanBakuNavigation { get; set; } = null!;

    public virtual SatuanBahanBaku IdSatuanNavigation { get; set; } = null!;
}
