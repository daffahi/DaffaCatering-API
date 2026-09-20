using System;
using System.Collections.Generic;

namespace DaffaCatering.API.Models;

public partial class SatuanBahanBaku
{
    public string IdSatuan { get; set; } = null!;

    public string NamaSatuan { get; set; } = null!;

    public virtual ICollection<DetailBahanBaku> DetailBahanBakus { get; set; } = new List<DetailBahanBaku>();

    public virtual ICollection<DetailPembelian> DetailPembelians { get; set; } = new List<DetailPembelian>();

    public virtual ICollection<DetailPenerimaan> DetailPenerimaans { get; set; } = new List<DetailPenerimaan>();

    public virtual ICollection<DetailPenggunaan> DetailPenggunaans { get; set; } = new List<DetailPenggunaan>();

    public virtual ICollection<DetailPenjualan> DetailPenjualans { get; set; } = new List<DetailPenjualan>();

    public virtual ICollection<DetailPenyesuaian> DetailPenyesuaians { get; set; } = new List<DetailPenyesuaian>();

    public virtual ICollection<DetailPesanan> DetailPesanans { get; set; } = new List<DetailPesanan>();

    public virtual ICollection<DetailResep> DetailReseps { get; set; } = new List<DetailResep>();

    public virtual ICollection<KonversiSatuan> KonversiSatuanIdSatuanBesarNavigations { get; set; } = new List<KonversiSatuan>();

    public virtual ICollection<KonversiSatuan> KonversiSatuanIdSatuanKecilNavigations { get; set; } = new List<KonversiSatuan>();

    public virtual ICollection<Menu> Menus { get; set; } = new List<Menu>();
}
