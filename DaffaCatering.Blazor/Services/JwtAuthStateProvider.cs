using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace DaffaCatering.Blazor.Services
{
    public class JwtAuthStateProvider : AuthenticationStateProvider
    {
        private const string KunciToken = "authToken";
        private readonly ProtectedSessionStorage _storage;
        private readonly ClaimsPrincipal _anonim = new ClaimsPrincipal(new ClaimsIdentity());

        public JwtAuthStateProvider(ProtectedSessionStorage storage)
        {
            _storage = storage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var hasil = await _storage.GetAsync<string>(KunciToken);
                if (hasil.Success && !string.IsNullOrEmpty(hasil.Value))
                {
                    return new AuthenticationState(BuatPrincipal(hasil.Value));
                }
            }
            catch
            {
                // Storage belum bisa diakses atau token rusak, anggap belum login
            }
            return new AuthenticationState(_anonim);
        }

        public async Task<string> AmbilTokenAsync()
        {
            try
            {
                var hasil = await _storage.GetAsync<string>(KunciToken);
                if (hasil.Success && !string.IsNullOrEmpty(hasil.Value))
                {
                    return hasil.Value;
                }
            }
            catch
            {
                // Interop belum siap, anggap belum ada token
            }
            return "";
        }

        public async Task MasukAsync(string token)
        {
            await _storage.SetAsync(KunciToken, token);
            var principal = BuatPrincipal(token);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
        }

        public async Task KeluarAsync()
        {
            await _storage.DeleteAsync(KunciToken);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonim)));
        }

        private ClaimsPrincipal BuatPrincipal(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwt = handler.ReadJwtToken(token);

            // Token sudah kedaluwarsa
            if (jwt.ValidTo < DateTime.UtcNow)
            {
                return _anonim;
            }

            var claims = new List<Claim>();
            foreach (var c in jwt.Claims)
            {
                if (c.Type == "role" || c.Type == ClaimTypes.Role)
                {
                    claims.Add(new Claim(ClaimTypes.Role, c.Value));
                }
                else if (c.Type == "unique_name" || c.Type == ClaimTypes.Name)
                {
                    claims.Add(new Claim(ClaimTypes.Name, c.Value));
                }
                else if (c.Type == "nameid" || c.Type == ClaimTypes.NameIdentifier)
                {
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, c.Value));
                }
                else
                {
                    claims.Add(new Claim(c.Type, c.Value));
                }
            }

            return new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
        }
    }
}