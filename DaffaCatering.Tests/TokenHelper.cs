using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace DaffaCatering.Tests;

public static class TokenHelper
{
    public static string BuatToken(string role, string idUser = "U0000001")
    {
        var kunci = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ApiFactory.Key));
        var jwt = new JwtSecurityToken(
            issuer: ApiFactory.Issuer,
            audience: ApiFactory.Audience,
            claims: new[]
            {
                new Claim(ClaimTypes.NameIdentifier, idUser),
                new Claim(ClaimTypes.Name, "tester"),
                new Claim(ClaimTypes.Role, role)
            },
            expires: DateTime.UtcNow.AddMinutes(10),
            signingCredentials: new SigningCredentials(kunci, SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    public static HttpClient ClientDengan(this ApiFactory factory, string role)
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", BuatToken(role));
        return client;
    }
}