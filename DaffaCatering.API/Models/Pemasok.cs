using System;
using System.Collections.Generic;

namespace DaffaCatering.API.Models;

public partial class Pemasok
{
    public string IdPemasok { get; set; } = null!;

    public string NamaPemasok { get; set; } = null!;

    public string? NoKontak { get; set; }

    public string? Alamat { get; set; }

    public DateOnly TglBergabung { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<HeaderPembelian> HeaderPembelians { get; set; } = new List<HeaderPembelian>();

    public virtual ICollection<HeaderPenerimaan> HeaderPenerimaans { get; set; } = new List<HeaderPenerimaan>();
}
