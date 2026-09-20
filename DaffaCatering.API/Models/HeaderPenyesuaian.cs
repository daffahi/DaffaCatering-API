using System;
using System.Collections.Generic;

namespace DaffaCatering.API.Models;

public partial class HeaderPenyesuaian
{
    public string IdPenyesuaian { get; set; } = null!;

    public DateOnly TglPenyesuaian { get; set; }

    public virtual ICollection<DetailPenyesuaian> DetailPenyesuaians { get; set; } = new List<DetailPenyesuaian>();
}
