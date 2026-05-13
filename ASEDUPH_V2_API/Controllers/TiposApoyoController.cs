using ASEDUPH_V2_API.Data;
using ASEDUPH_V2_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASEDUPH_V2_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TiposApoyoController : ControllerBase
    {
        private readonly AseduphDbContext _context;

        public TiposApoyoController(AseduphDbContext context)
        {
            _context = context;
        }

        // GET: api/TiposApoyo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TipoApoyo>>> GetTiposApoyo()
        {
            var tiposApoyo = await _context.TiposApoyo
                .Where(t => t.Estado == "Activo")
                .OrderBy(t => t.Nombre)
                .ToListAsync();

            return Ok(tiposApoyo);
        }

        // GET: api/TiposApoyo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TipoApoyo>> GetTipoApoyo(int id)
        {
            var tipoApoyo = await _context.TiposApoyo
                .FirstOrDefaultAsync(t => t.TipoApoyoId == id);

            if (tipoApoyo == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el tipo de apoyo solicitado."
                });
            }

            return Ok(tipoApoyo);
        }

        // POST: api/TiposApoyo
        [HttpPost]
        public async Task<ActionResult<TipoApoyo>> PostTipoApoyo(TipoApoyo tipoApoyo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existeTipo = await _context.TiposApoyo
                .AnyAsync(t => t.Nombre == tipoApoyo.Nombre);

            if (existeTipo)
            {
                return BadRequest(new
                {
                    mensaje = "Ya existe un tipo de apoyo con ese nombre."
                });
            }

            tipoApoyo.Estado = "Activo";

            _context.TiposApoyo.Add(tipoApoyo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetTipoApoyo),
                new { id = tipoApoyo.TipoApoyoId },
                tipoApoyo
            );
        }

        // PUT: api/TiposApoyo/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTipoApoyo(int id, TipoApoyo tipoApoyo)
        {
            if (id != tipoApoyo.TipoApoyoId)
            {
                return BadRequest(new
                {
                    mensaje = "El ID enviado no coincide con el ID del tipo de apoyo."
                });
            }

            var tipoExistente = await _context.TiposApoyo
                .FirstOrDefaultAsync(t => t.TipoApoyoId == id);

            if (tipoExistente == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el tipo de apoyo que desea actualizar."
                });
            }

            var nombreDuplicado = await _context.TiposApoyo
                .AnyAsync(t => t.Nombre == tipoApoyo.Nombre && t.TipoApoyoId != id);

            if (nombreDuplicado)
            {
                return BadRequest(new
                {
                    mensaje = "Ya existe otro tipo de apoyo con ese nombre."
                });
            }

            tipoExistente.Nombre = tipoApoyo.Nombre;
            tipoExistente.Descripcion = tipoApoyo.Descripcion;
            tipoExistente.Estado = tipoApoyo.Estado;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Tipo de apoyo actualizado correctamente."
            });
        }

        // DELETE: api/TiposApoyo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTipoApoyo(int id)
        {
            var tipoApoyo = await _context.TiposApoyo
                .FirstOrDefaultAsync(t => t.TipoApoyoId == id);

            if (tipoApoyo == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el tipo de apoyo que desea eliminar."
                });
            }

            tipoApoyo.Estado = "Inactivo";
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Tipo de apoyo desactivado correctamente."
            });
        }
    }
}