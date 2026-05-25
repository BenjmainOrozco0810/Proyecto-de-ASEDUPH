using ASEDUPH_V2_API.Data;
using ASEDUPH_V2_API.Models;
using ASEDUPH_V2_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ASEDUPH_V2_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly AseduphDbContext _context;
        private readonly AuditoriaService _auditoria;

        public UsuariosController(AseduphDbContext context, AuditoriaService auditoria)
        {
            _context = context;
            _auditoria = auditoria;
        }

        // GET: api/Usuarios
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<IEnumerable<object>>> GetUsuarios()
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.UsuarioRoles!)
                    .ThenInclude(ur => ur.Rol)
                .Where(u => u.Estado == "Activo")
                .OrderBy(u => u.NombreCompleto)
                .Select(u => new
                {
                    u.UsuarioId,
                    u.NombreCompleto,
                    u.Correo,
                    u.Username,
                    u.Estado,
                    u.FechaRegistro,
                    Roles = u.UsuarioRoles!.Select(ur => ur.Rol!.NombreRol)
                })
                .ToListAsync();

            return Ok(usuarios);
        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<object>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.UsuarioRoles!)
                    .ThenInclude(ur => ur.Rol)
                .Where(u => u.UsuarioId == id)
                .Select(u => new
                {
                    u.UsuarioId,
                    u.NombreCompleto,
                    u.Correo,
                    u.Username,
                    u.Estado,
                    u.FechaRegistro,
                    Roles = u.UsuarioRoles!.Select(ur => ur.Rol!.NombreRol)
                })
                .FirstOrDefaultAsync();

            if (usuario == null)
                return NotFound(new { mensaje = "No se encontró el usuario solicitado." });

            return Ok(usuario);
        }

        // POST: api/Usuarios
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> PostUsuario([FromBody] CrearUsuarioRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existeUsername = await _context.Usuarios
                .AnyAsync(u => u.Username == request.Username);

            if (existeUsername)
                return BadRequest(new { mensaje = "El nombre de usuario ya está en uso." });

            if (!string.IsNullOrWhiteSpace(request.Correo))
            {
                var existeCorreo = await _context.Usuarios
                    .AnyAsync(u => u.Correo == request.Correo);

                if (existeCorreo)
                    return BadRequest(new { mensaje = "El correo electrónico ya está registrado." });
            }

            var usuario = new Usuario
            {
                NombreCompleto = request.NombreCompleto,
                Correo = request.Correo,
                Username = request.Username,
                PasswordHash = HashPassword(request.Password),
                Estado = "Activo",
                FechaRegistro = DateTime.Now
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            if (request.RolIds != null && request.RolIds.Any())
            {
                foreach (var rolId in request.RolIds)
                {
                    var existeRol = await _context.Roles
                        .AnyAsync(r => r.RolId == rolId && r.Estado == "Activo");

                    if (existeRol)
                    {
                        _context.UsuarioRoles.Add(new UsuarioRol
                        {
                            UsuarioId = usuario.UsuarioId,
                            RolId = rolId
                        });
                    }
                }

                await _context.SaveChangesAsync();
            }

            // ── Log de auditoría ──────────────────────────────────────────
            await _auditoria.RegistrarAsync(
                accion: "Crear",
                modulo: "Usuarios",
                descripcion: $"Se creó el usuario '{usuario.Username}' — Nombre: {usuario.NombreCompleto}.",
                entidadAfectada: usuario.Username,
                entidadId: usuario.UsuarioId,
                ip: HttpContext.Connection.RemoteIpAddress?.ToString()
            );

            return Ok(new
            {
                mensaje = "Usuario creado correctamente.",
                usuarioId = usuario.UsuarioId
            });
        }

        // PUT: api/Usuarios/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> PutUsuario(int id, [FromBody] ActualizarUsuarioRequest request)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.UsuarioId == id);

            if (usuario == null)
                return NotFound(new { mensaje = "No se encontró el usuario que desea actualizar." });

            var existeUsername = await _context.Usuarios
                .AnyAsync(u => u.Username == request.Username && u.UsuarioId != id);

            if (existeUsername)
                return BadRequest(new { mensaje = "El nombre de usuario ya está en uso." });

            if (!string.IsNullOrWhiteSpace(request.Correo))
            {
                var existeCorreo = await _context.Usuarios
                    .AnyAsync(u => u.Correo == request.Correo && u.UsuarioId != id);

                if (existeCorreo)
                    return BadRequest(new { mensaje = "El correo electrónico ya está registrado." });
            }

            usuario.NombreCompleto = request.NombreCompleto;
            usuario.Correo = request.Correo;
            usuario.Username = request.Username;
            usuario.Estado = request.Estado;

            await _context.SaveChangesAsync();

            // ── Log de auditoría ──────────────────────────────────────────
            await _auditoria.RegistrarAsync(
                accion: "Editar",
                modulo: "Usuarios",
                descripcion: $"Se actualizó el usuario '{usuario.Username}' — Estado: {usuario.Estado}.",
                entidadAfectada: usuario.Username,
                entidadId: id,
                ip: HttpContext.Connection.RemoteIpAddress?.ToString()
            );

            return Ok(new { mensaje = "Usuario actualizado correctamente." });
        }

        // PATCH: api/Usuarios/5/password
        [HttpPatch("{id}/password")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> CambiarPassword(int id, [FromBody] CambiarPasswordRequest request)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.UsuarioId == id);

            if (usuario == null)
                return NotFound(new { mensaje = "No se encontró el usuario." });

            if (string.IsNullOrWhiteSpace(request.NuevaPassword) || request.NuevaPassword.Length < 6)
                return BadRequest(new { mensaje = "La contraseña debe tener al menos 6 caracteres." });

            usuario.PasswordHash = HashPassword(request.NuevaPassword);
            await _context.SaveChangesAsync();

            // ── Log de auditoría ──────────────────────────────────────────
            await _auditoria.RegistrarAsync(
                accion: "CambiarPassword",
                modulo: "Usuarios",
                descripcion: $"Se cambió la contraseña del usuario '{usuario.Username}'.",
                entidadAfectada: usuario.Username,
                entidadId: id,
                ip: HttpContext.Connection.RemoteIpAddress?.ToString()
            );

            return Ok(new { mensaje = "Contraseña actualizada correctamente." });
        }

        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.UsuarioId == id);

            if (usuario == null)
                return NotFound(new { mensaje = "No se encontró el usuario que desea desactivar." });

            usuario.Estado = "Inactivo";
            await _context.SaveChangesAsync();

            // ── Log de auditoría ──────────────────────────────────────────
            await _auditoria.RegistrarAsync(
                accion: "Eliminar",
                modulo: "Usuarios",
                descripcion: $"Se desactivó el usuario '{usuario.Username}' — Nombre: {usuario.NombreCompleto}.",
                entidadAfectada: usuario.Username,
                entidadId: id,
                ip: HttpContext.Connection.RemoteIpAddress?.ToString()
            );

            return Ok(new { mensaje = "Usuario desactivado correctamente." });
        }

        // POST: api/Usuarios/5/roles
        [HttpPost("{id}/roles")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> AsignarRol(int id, [FromBody] AsignarRolRequest request)
        {
            var existeUsuario = await _context.Usuarios
                .AnyAsync(u => u.UsuarioId == id && u.Estado == "Activo");

            if (!existeUsuario)
                return NotFound(new { mensaje = "No se encontró el usuario indicado." });

            var rol = await _context.Roles
                .FirstOrDefaultAsync(r => r.RolId == request.RolId && r.Estado == "Activo");

            if (rol == null)
                return BadRequest(new { mensaje = "El rol indicado no existe o está inactivo." });

            var yaAsignado = await _context.UsuarioRoles
                .AnyAsync(ur => ur.UsuarioId == id && ur.RolId == request.RolId);

            if (yaAsignado)
                return BadRequest(new { mensaje = "El usuario ya tiene asignado ese rol." });

            _context.UsuarioRoles.Add(new UsuarioRol
            {
                UsuarioId = id,
                RolId = request.RolId
            });

            await _context.SaveChangesAsync();

            // ── Log de auditoría ──────────────────────────────────────────
            await _auditoria.RegistrarAsync(
                accion: "AsignarRol",
                modulo: "Usuarios",
                descripcion: $"Se asignó el rol '{rol.NombreRol}' al usuario ID {id}.",
                entidadAfectada: rol.NombreRol,
                entidadId: id,
                ip: HttpContext.Connection.RemoteIpAddress?.ToString()
            );

            return Ok(new { mensaje = "Rol asignado correctamente." });
        }

        // DELETE: api/Usuarios/5/roles/2
        [HttpDelete("{id}/roles/{rolId}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> RemoverRol(int id, int rolId)
        {
            var usuarioRol = await _context.UsuarioRoles
                .FirstOrDefaultAsync(ur => ur.UsuarioId == id && ur.RolId == rolId);

            if (usuarioRol == null)
                return NotFound(new { mensaje = "El usuario no tiene asignado ese rol." });

            var nombreRol = (await _context.Roles.FindAsync(rolId))?.NombreRol ?? "Desconocido";

            _context.UsuarioRoles.Remove(usuarioRol);
            await _context.SaveChangesAsync();

            // ── Log de auditoría ──────────────────────────────────────────
            await _auditoria.RegistrarAsync(
                accion: "RemoverRol",
                modulo: "Usuarios",
                descripcion: $"Se removió el rol '{nombreRol}' del usuario ID {id}.",
                entidadAfectada: nombreRol,
                entidadId: id,
                ip: HttpContext.Connection.RemoteIpAddress?.ToString()
            );

            return Ok(new { mensaje = "Rol removido correctamente." });
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToHexString(hash).ToLower();
        }
    }

    // ── DTOs internos del controlador ────────────────────────────────

    public class CrearUsuarioRequest
    {
        [System.ComponentModel.DataAnnotations.Required]
        public string NombreCompleto { get; set; } = string.Empty;
        public string? Correo { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        public string Username { get; set; } = string.Empty;
        [System.ComponentModel.DataAnnotations.Required]
        public string Password { get; set; } = string.Empty;
        public List<int>? RolIds { get; set; }
    }

    public class ActualizarUsuarioRequest
    {
        [System.ComponentModel.DataAnnotations.Required]
        public string NombreCompleto { get; set; } = string.Empty;
        public string? Correo { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        public string Username { get; set; } = string.Empty;
        public string Estado { get; set; } = "Activo";
    }

    public class CambiarPasswordRequest
    {
        [System.ComponentModel.DataAnnotations.Required]
        public string NuevaPassword { get; set; } = string.Empty;
    }

    public class AsignarRolRequest
    {
        [System.ComponentModel.DataAnnotations.Required]
        public int RolId { get; set; }
    }
}
