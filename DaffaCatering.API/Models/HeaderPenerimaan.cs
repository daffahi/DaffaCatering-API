using System;
using System.Collections.Generic;

namespace DaffaCatering.API.Models;

public partial class HeaderPenerimaan
{
    public string IdPenerimaan { get; set; } = null!;

    public string IdPemasok { get; set; } = null!;

    public string IdPembelian { get; set; } = null!;

    public DateOnly TglPenerimaan { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<DetailPenerimaan> DetailPenerimaans { get; set; } = new List<DetailPenerimaan>();

    public virtual Pemasok IdPemasokNavigation { get; set; } = null!;

    public virtual HeaderPembelian IdPembelianNavigation { get; set; } = null!;
}
