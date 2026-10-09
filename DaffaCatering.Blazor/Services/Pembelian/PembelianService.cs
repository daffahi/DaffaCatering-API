using DaffaCatering.Blazor.Models.Pembelian;

namespace DaffaCatering.Blazor.Services.Pembelian
{
    public class PembelianService : CrudServiceBase<PembelianModels>
    {
        public PembelianService(IHttpClientFactory factory, JwtAuthStateProvider auth)
            : base(factory, auth, "api/HeaderPembelian") { }
    }
}