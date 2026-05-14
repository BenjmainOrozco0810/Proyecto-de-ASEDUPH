using ASEDUPH_V2_API.Data;
using ASEDUPH_V2_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASEDUPH_V2_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrador")]
    public class RolesController : ControllerBase
    {
        private readonly AseduphDbContext _context;

        public RolesController(AseduphDbContext context)
        {
            _context = context;
        }

        // GET: api/Roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rol>>> GetRoles()
        {
            var roles = await _context.Roles
                .Where(r => r.Estado == "Activo")
                .OrderBy(r => r.NombreRol)
                .ToListAsync();

            return Ok(roles);
        }

        // GET: api/Roles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Rol>> GetRol(int id)
        {
            var rol = await _context.Roles
                .FirstOrDefaultAsync(r => r.RolId == id);

            if (rol == null)
            {
                return NotFound(new { mensaje = "No se encontró el rol solicitado." });
            }

            return Ok(rol);
        }

        // POST: api/Roles
        [HttpPost]
        public async Task<ActionResult<Rol>> PostRol(Rol rol)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existeNombre = await _context.Roles
                .AnyAsync(r => r.NombreRol == rol.NombreRol);

            if (existeNombre)
            {
                return BadRequest(new { mensaje = "Ya existe un rol con ese nombre." });
            }

            rol.Estado = "Activo";
            _context.Roles.Add(rol);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRol), new { id = rol.RolId }, rol);
        }

        // PUT: api/Roles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRol(int id, Rol rol)
        {
            if (id != rol.RolId)
            {
                return BadRequest(new { mensaje = "El ID enviado no coincide con el ID del rol." });
            }

            var rolExistente = await _context.Roles.FirstOrDefaultAsync(r => r.RolId == id);

            if (rolExistente == null)
            {
                return NotFound(new { mensaje = "No se encontró el rol que desea actualizar." });
            }

            var nombreDuplicado = await _context.Roles
                .AnyAsync(r => r.NombreRol == rol.NombreRol && r.RolId != id);

            if (nombreDuplicado)
            {
                return BadRequest(new { mensaje = "Ya existe otro rol con ese nombre." });
            }

            rolExistente.NombreRol = rol.NombreRol;
            rolExistente.Descripcion = rol.Descripcion;
            rolExistente.Estado = rol.Estado;

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Rol actualizado correctamente." });
        }

        // DELETE: api/Roles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRol(int id)
        {
            var rol = await _context.Roles.FirstOrDefaultAsync(r => r.RolId == id);

            if (rol == null)
            {
                return NotFound(new { mensaje = "No se encontró el rol que desea desactivar." });
            }

            rol.Estado = "Inactivo";
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Rol desactivado correctamente." });
        }
    }
}