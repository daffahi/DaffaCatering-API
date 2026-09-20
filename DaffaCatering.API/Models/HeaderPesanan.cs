using System;
using System.Collections.Generic;

namespace DaffaCatering.API.Models;

public partial class HeaderPesanan
{
    public string IdPesanan { get; set; } = null!;

    public string IdPelanggan { get; set; } = null!;

    public string MetodePembayaran { get; set; } = null!;

    public DateOnly TglPesanan { get; set; }

    public DateOnly TglPengambilan { get; set; }

    public decimal UangMuka { get; set; }

    public decimal Total { get; set; }

    public string? Keterangan { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<DetailPesanan> DetailPesanans { get; set; } = new List<DetailPesanan>();

    public virtual ICollection<HeaderPenjualan> HeaderPenjualans { get; set; } = new List<HeaderPenjualan>();

    public virtual Pelanggan IdPelangganNavigation { get; set; } = null!;
}
