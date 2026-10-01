using DaffaCatering.Blazor.Models;

namespace DaffaCatering.Blazor.Services
{
    public class ResepService : CrudServiceBase<ResepModel>
    {
        public ResepService(IHttpClientFactory factory, JwtAuthStateProvider auth)
            : base(factory, auth, "api/HeaderResep") { }
    }
}