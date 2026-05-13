using ASEDUPH_V2_API.Data;
using ASEDUPH_V2_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASEDUPH_V2_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CentrosEducativosController : ControllerBase
    {
        private readonly AseduphDbContext _context;

        public CentrosEducativosController(AseduphDbContext context)
        {
            _context = context;
        }

        // GET: api/CentrosEducativos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CentroEducativo>>> GetCentrosEducativos()
        {
            var centros = await _context.CentrosEducativos
                .Where(c => c.Estado == "Activo")
                .OrderBy(c => c.Nombre)
                .ToListAsync();

            return Ok(centros);
        }

        // GET: api/CentrosEducativos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CentroEducativo>> GetCentroEducativo(int id)
        {
            var centro = await _context.CentrosEducativos
                .FirstOrDefaultAsync(c => c.CentroEducativoId == id);

            if (centro == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el centro educativo solicitado."
                });
            }

            return Ok(centro);
        }

        // POST: api/CentrosEducativos
        [HttpPost]
        public async Task<ActionResult<CentroEducativo>> PostCentroEducativo(CentroEducativo centro)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            centro.Estado = "Activo";

            _context.CentrosEducativos.Add(centro);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCentroEducativo),
                new { id = centro.CentroEducativoId },
                centro
            );
        }

        // PUT: api/CentrosEducativos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCentroEducativo(int id, CentroEducativo centro)
        {
            if (id != centro.CentroEducativoId)
            {
                return BadRequest(new
                {
                    mensaje = "El ID enviado no coincide con el ID del centro educativo."
                });
            }

            var centroExistente = await _context.CentrosEducativos
                .FirstOrDefaultAsync(c => c.CentroEducativoId == id);

            if (centroExistente == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el centro educativo que desea actualizar."
                });
            }

            centroExistente.Nombre = centro.Nombre;
            centroExistente.Direccion = centro.Direccion;
            centroExistente.Telefono = centro.Telefono;
            centroExistente.TipoCentro = centro.TipoCentro;
            centroExistente.Estado = centro.Estado;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Centro educativo actualizado correctamente."
            });
        }

        // DELETE: api/CentrosEducativos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCentroEducativo(int id)
        {
            var centro = await _context.CentrosEducativos
                .FirstOrDefaultAsync(c => c.CentroEducativoId == id);

            if (centro == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el centro educativo que desea eliminar."
                });
            }

            centro.Estado = "Inactivo";
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Centro educativo desactivado correctamente."
            });
        }
    }
}