using System;
using System.Collections.Generic;

namespace DaffaCatering.API.Models;

public partial class HeaderPembelian
{
    public string IdPembelian { get; set; } = null!;

    public string IdPemasok { get; set; } = null!;

    public DateOnly TglPembelian { get; set; }

    public decimal Total { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<DetailPembelian> DetailPembelians { get; set; } = new List<DetailPembelian>();

    public virtual ICollection<HeaderPenerimaan> HeaderPenerimaans { get; set; } = new List<HeaderPenerimaan>();

    public virtual Pemasok IdPemasokNavigation { get; set; } = null!;
}
