using System;
using System.Collections.Generic;

namespace DaffaCatering.API.Models;

public partial class HeaderPenggunaan
{
    public string IdPenggunaan { get; set; } = null!;

    public DateOnly TglPenggunaan { get; set; }

    public virtual ICollection<DetailPenggunaan> DetailPenggunaans { get; set; } = new List<DetailPenggunaan>();
}
