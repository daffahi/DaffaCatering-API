using DaffaCatering.API.Data;
using DaffaCatering.API.DTOs.MasterUser;
using DaffaCatering.API.DTOs.User;
using DaffaCatering.API.Models;
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
    public class UserController : ControllerBase
    {
        private readonly DaffaCateringContext _context;
        private readonly IConfiguration _config;

        public UserController(DaffaCateringContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool? status)
        {
            var query = _context.Users.AsQueryable();
            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);
            var data = await query.ToListAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var data = await _context.Users.FindAsync(id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserDto input)
        {
            try
            {
                var entity = new Models.User
                {
                    IdUser = input.IdUser,
                    IdRole = input.IdRole,
                    NamaUser = input.NamaUser,
                    Password = BCrypt.Net.BCrypt.HashPassword(input.Password),
                    Status = input.Status
                };

                _context.Users.Add(entity);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = entity.IdUser }, entity);
            }
            catch (DbUpdateException ex)
            {
                return Conflict($"Gagal menyimpan — cek ID atau data lain. Detail: {ex.InnerException?.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UserDto input)
        {
            if (id != input.IdUser) return BadRequest("ID tidak sesuai");

            var existing = await _context.Users.FindAsync(id);
            if (existing == null) return NotFound();

            existing.IdRole = input.IdRole;
            existing.NamaUser = input.NamaUser;
            existing.Password = BCrypt.Net.BCrypt.HashPassword(input.Password);
            existing.Status = input.Status;

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