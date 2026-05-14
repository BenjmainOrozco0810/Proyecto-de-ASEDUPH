using ASEDUPH_V2_API.Data;
using ASEDUPH_V2_API.DTOs;
using ASEDUPH_V2_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ASEDUPH_V2_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AseduphDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AseduphDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Buscar usuario activo por username
            var usuario = await _context.Usuarios
                .Include(u => u.UsuarioRoles!)
                    .ThenInclude(ur => ur.Rol)
                .FirstOrDefaultAsync(u =>
                    u.Username == request.Username &&
                    u.Estado == "Activo");

            if (usuario == null)
            {
                return Unauthorized(new
                {
                    mensaje = "Usuario o contraseña incorrectos."
                });
            }

            // Verificar contraseña
            if (!VerificarPassword(request.Password, usuario.PasswordHash))
            {
                return Unauthorized(new
                {
                    mensaje = "Usuario o contraseña incorrectos."
                });
            }

            // Obtener roles del usuario
            var roles = usuario.UsuarioRoles?
                .Where(ur => ur.Rol?.Estado == "Activo")
                .Select(ur => ur.Rol!.NombreRol)
                .ToList() ?? new List<string>();

            // Generar token JWT
            var token = GenerarToken(usuario, roles);

            return Ok(new
            {
                token,
                tipo = "Bearer",
                usuario = new
                {
                    usuario.UsuarioId,
                    usuario.NombreCompleto,
                    usuario.Username,
                    usuario.Correo,
                    roles
                }
            });
        }

        // ── Métodos privados ─────────────────────────────────────────

        private string GenerarToken(Usuario usuario, List<string> roles)
        {
            var jwtKey = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT Key no configurada.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Claims del token
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
                new Claim(ClaimTypes.Name, usuario.Username),
                new Claim("NombreCompleto", usuario.NombreCompleto),
            };

            // Agregar cada rol como claim
            foreach (var rol in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, rol));
            }

            var expiracion = DateTime.UtcNow.AddHours(
                double.Parse(_configuration["Jwt:ExpiresInHours"] ?? "8")
            );

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiracion,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Verifica la contraseña usando SHA256
        private static bool VerificarPassword(string password, string hashGuardado)
        {
            var hashIngresado = HashPassword(password);
            return hashIngresado == hashGuardado;
        }

        // Genera hash SHA256 de la contraseña
        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToHexString(hash).ToLower();
        }
    }
}