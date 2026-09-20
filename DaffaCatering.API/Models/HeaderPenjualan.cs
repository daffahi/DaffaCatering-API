using System;
using System.Collections.Generic;

namespace DaffaCatering.API.Models;

public partial class HeaderPenjualan
{
    public string IdPenjualan { get; set; } = null!;

    public string? IdPesanan { get; set; }

    public string IdPelanggan { get; set; } = null!;

    public string MetodePembayaran { get; set; } = null!;

    public DateOnly TglPenjualan { get; set; }

    public decimal? UangMuka { get; set; }

    public decimal Total { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<DetailPenjualan> DetailPenjualans { get; set; } = new List<DetailPenjualan>();

    public virtual Pelanggan IdPelangganNavigation { get; set; } = null!;

    public virtual HeaderPesanan? IdPesananNavigation { get; set; }
}
