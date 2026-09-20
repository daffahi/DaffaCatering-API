using System;
using System.Collections.Generic;

namespace DaffaCatering.API.Models;

public partial class DetailResep
{
    public string IdResep { get; set; } = null!;

    public string IdBahanBaku { get; set; } = null!;

    public string IdSatuan { get; set; } = null!;

    public decimal Jumlah { get; set; }

    public virtual HeaderBahanBaku IdBahanBakuNavigation { get; set; } = null!;

    public virtual HeaderResep IdResepNavigation { get; set; } = null!;

    public virtual SatuanBahanBaku IdSatuanNavigation { get; set; } = null!;
}
