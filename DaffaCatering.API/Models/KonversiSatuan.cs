using System;
using System.Collections.Generic;

namespace DaffaCatering.API.Models;

public partial class KonversiSatuan
{
    public string IdKonversi { get; set; } = null!;

    public string IdSatuanBesar { get; set; } = null!;

    public string IdSatuanKecil { get; set; } = null!;

    public decimal NilaiKonversi { get; set; }

    public virtual SatuanBahanBaku IdSatuanBesarNavigation { get; set; } = null!;

    public virtual SatuanBahanBaku IdSatuanKecilNavigation { get; set; } = null!;
}
