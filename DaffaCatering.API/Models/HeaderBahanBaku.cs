using System;
using System.Collections.Generic;

namespace DaffaCatering.API.Models;

public partial class HeaderBahanBaku
{
    public string IdBahanBaku { get; set; } = null!;

    public string NamaBahanBaku { get; set; } = null!;

    public bool Jenis { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<DetailBahanBaku> DetailBahanBakus { get; set; } = new List<DetailBahanBaku>();

    public virtual ICollection<DetailPembelian> DetailPembelians { get; set; } = new List<DetailPembelian>();

    public virtual ICollection<DetailPenerimaan> DetailPenerimaans { get; set; } = new List<DetailPenerimaan>();

    public virtual ICollection<DetailPenggunaan> DetailPenggunaans { get; set; } = new List<DetailPenggunaan>();

    public virtual ICollection<DetailPenyesuaian> DetailPenyesuaians { get; set; } = new List<DetailPenyesuaian>();

    public virtual ICollection<DetailResep> DetailReseps { get; set; } = new List<DetailResep>();
}
