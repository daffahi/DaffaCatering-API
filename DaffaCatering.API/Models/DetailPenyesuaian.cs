using System;
using System.Collections.Generic;

namespace DaffaCatering.API.Models;

public partial class DetailPenyesuaian
{
    public string IdPenyesuaian { get; set; } = null!;

    public string IdBahanBaku { get; set; } = null!;

    public string IdSatuan { get; set; } = null!;

    public decimal StokFisik { get; set; }

    public decimal SelisihStok { get; set; }

    public DateOnly TglKadaluwarsa { get; set; }

    public string Keterangan { get; set; } = null!;

    public virtual HeaderBahanBaku IdBahanBakuNavigation { get; set; } = null!;

    public virtual HeaderPenyesuaian IdPenyesuaianNavigation { get; set; } = null!;

    public virtual SatuanBahanBaku IdSatuanNavigation { get; set; } = null!;
}
