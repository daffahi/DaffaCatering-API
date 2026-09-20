using System;
using System.Collections.Generic;

namespace DaffaCatering.API.Models;

public partial class HeaderResep
{
    public string IdResep { get; set; } = null!;

    public string IdMenu { get; set; } = null!;

    public bool Status { get; set; }

    public virtual ICollection<DetailResep> DetailReseps { get; set; } = new List<DetailResep>();

    public virtual Menu IdMenuNavigation { get; set; } = null!;
}
