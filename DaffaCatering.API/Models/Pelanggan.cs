using System;
using System.Collections.Generic;

namespace DaffaCatering.API.Models;

public partial class Pelanggan
{
    public string IdPelanggan { get; set; } = null!;

    public string NamaPelanggan { get; set; } = null!;

    public string? NoKontak { get; set; }

    public string? Alamat { get; set; }

    public DateOnly TglBergabung { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<HeaderPenjualan> HeaderPenjualans { get; set; } = new List<HeaderPenjualan>();

    public virtual ICollection<HeaderPesanan> HeaderPesanans { get; set; } = new List<HeaderPesanan>();
}
