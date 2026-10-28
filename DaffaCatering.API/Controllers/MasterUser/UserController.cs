using DaffaCatering.API.Constants;
using DaffaCatering.API.Data;
using DaffaCatering.API.DTOs.MasterUser;
using DaffaCatering.API.DTOs.User;
using DaffaCatering.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DaffaCatering.API.Controllers.User
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.Admin)]
    public class UserController : ControllerBase
    {
        private readonly DaffaCateringContext _context;
        private readonly IConfiguration _config;

        public UserController(DaffaCateringContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // ID user yang sedang login, diambil dari token
        private string? IdSaya => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // User digabung dengan nama role, password tidak ikut
        private IQueryable<UserResponseDto> QueryUser() =>
            from u in _context.Users
            join r in _context.Roles on u.IdRole equals r.IdRole
            select new UserResponseDto
            {
                IdUser = u.IdUser,
                IdRole = u.IdRole,
                NamaRole = r.NamaRole,
                NamaUser = u.NamaUser,
                Status = u.Status
            };

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool? status)
        {
            var query = QueryUser();
            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            var data = await query.OrderBy(x => x.IdUser).ToListAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var data = await QueryUser().FirstOrDefaultAsync(x => x.IdUser == id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserDto input)
        {
            if (!await _context.Roles.AnyAsync(r => r.IdRole == input.IdRole))
                return BadRequest("Role tidak ditemukan");

            try
            {
                var entity = new Models.User
                {
                    IdUser = input.IdUser,
                    IdRole = input.IdRole,
                    NamaUser = input.NamaUser.Trim(),
                    Password = BCrypt.Net.BCrypt.HashPassword(input.Password),
                    Status = input.Status
                };

                _context.Users.Add(entity);
                await _context.SaveChangesAsync();

                var hasil = await QueryUser().FirstAsync(x => x.IdUser == entity.IdUser);
                return CreatedAtAction(nameof(GetById), new { id = entity.IdUser }, hasil);
            }
            catch (DbUpdateException)
            {
                return Conflict("ID user tersebut sudah ada. Silakan gunakan ID yang lain");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UserUpdateDto input)
        {
            var existing = await _context.Users.FindAsync(id);
            if (existing == null) return NotFound();

            if (!await _context.Roles.AnyAsync(r => r.IdRole == input.IdRole))
                return BadRequest("Role tidak ditemukan");

            // Admin tidak boleh mengunci dirinya sendiri
            if (id == IdSaya && (!input.Status || input.IdRole != existing.IdRole))
                return BadRequest("Akun yang sedang dipakai tidak bisa dinonaktifkan atau diubah rolenya");

            existing.IdRole = input.IdRole;
            existing.NamaUser = input.NamaUser.Trim();
            existing.Status = input.Status;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}/password")]
        public async Task<IActionResult> ResetPassword(string id, [FromBody] ResetPasswordDto input)
        {
            var existing = await _context.Users.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Password = BCrypt.Net.BCrypt.HashPassword(input.PasswordBaru);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _context.Users.FindAsync(id);
            if (existing == null) return NotFound();
            _context.Users.Remove(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto input)
        {
            var user = await _context.Users.FindAsync(input.IdUser);

            if (user == null || !BCrypt.Net.BCrypt.Verify(input.Password, user.Password))
                return Unauthorized("ID User atau Password salah");

            if (!user.Status)
                return Unauthorized("Akun ini tidak aktif");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.IdUser),
                new Claim(ClaimTypes.Name, user.NamaUser),
                new Claim(ClaimTypes.Role, user.IdRole)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:ExpiresInMinutes"]!)),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiresIn = _config["Jwt:ExpiresInMinutes"] + " menit"
            });
        }
    }
}