using DaffaCatering.Blazor.Models.MasterUser;

namespace DaffaCatering.Blazor.Services.MasterUser
{
    public class RoleService : CrudServiceBase<RoleModels>
    {
        public RoleService(IHttpClientFactory factory, JwtAuthStateProvider auth)
            : base(factory, auth, "api/Role") { }
    }
}