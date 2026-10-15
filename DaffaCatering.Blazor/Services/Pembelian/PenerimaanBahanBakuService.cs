using DaffaCatering.Blazor.Models.Pembelian;

namespace DaffaCatering.Blazor.Services.Pembelian
{
    public class PenerimaanBahanBakuService : CrudServiceBase<PenerimaanBahanBakuModels>
    {
        public PenerimaanBahanBakuService(IHttpClientFactory factory, JwtAuthStateProvider auth)
            : base(factory, auth, "api/HeaderPenerimaan") { }
    }
}