using DaffaCatering.Blazor.Models.MasterMakanan

namespace DaffaCatering.Blazor.Services.MasterMakanan
{
    public class ResepService : CrudServiceBase<ResepModels>
    {
        public ResepService(IHttpClientFactory factory, JwtAuthStateProvider auth)
            : base(factory, auth, "api/HeaderResep") { }
    {
}