using ASEDUPH_V2_API.Data;
using ASEDUPH_V2_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASEDUPH_V2_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BienesFamiliaresController : ControllerBase
    {
        private readonly AseduphDbContext _context;

        public BienesFamiliaresController(AseduphDbContext context)
        {
            _context = context;
        }

        // GET: api/BienesFamiliares
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BienFamiliar>>> GetBienesFamiliares()
        {
            var bienes = await _context.BienesFamiliares
                .Include(b => b.VisitaFamiliar)
                    .ThenInclude(v => v.SolicitudBeca)
                        .ThenInclude(s => s.Estudiante)
                .OrderBy(b => b.TipoBien)
                .ToListAsync();

            return Ok(bienes);
        }

        // GET: api/BienesFamiliares/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BienFamiliar>> GetBienFamiliar(int id)
        {
            var bien = await _context.BienesFamiliares
                .Include(b => b.VisitaFamiliar)
                    .ThenInclude(v => v.SolicitudBeca)
                        .ThenInclude(s => s.Estudiante)
                .FirstOrDefaultAsync(b => b.BienFamiliarId == id);

            if (bien == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el bien familiar solicitado."
                });
            }

            return Ok(bien);
        }

        // GET: api/BienesFamiliares/visita/5
        [HttpGet("visita/{visitaFamiliarId}")]
        public async Task<ActionResult<IEnumerable<BienFamiliar>>> GetBienesPorVisita(int visitaFamiliarId)
        {
            var existeVisita = await _context.VisitasFamiliares
                .AnyAsync(v => v.VisitaFamiliarId == visitaFamiliarId);

            if (!existeVisita)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró la visita familiar indicada."
                });
            }

            var bienes = await _context.BienesFamiliares
                .Where(b => b.VisitaFamiliarId == visitaFamiliarId)
                .OrderBy(b => b.TipoBien)
                .ToListAsync();

            return Ok(bienes);
        }

        // POST: api/BienesFamiliares
        [HttpPost]
        public async Task<ActionResult<BienFamiliar>> PostBienFamiliar(BienFamiliar bien)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existeVisita = await _context.VisitasFamiliares
                .AnyAsync(v => v.VisitaFamiliarId == bien.VisitaFamiliarId);

            if (!existeVisita)
            {
                return BadRequest(new
                {
                    mensaje = "La visita familiar indicada no existe."
                });
            }

            var validacion = ValidarBienFamiliar(bien);
            if (validacion != null)
            {
                return BadRequest(validacion);
            }

            _context.BienesFamiliares.Add(bien);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetBienFamiliar),
                new { id = bien.BienFamiliarId },
                bien
            );
        }

        // PUT: api/BienesFamiliares/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBienFamiliar(int id, BienFamiliar bien)
        {
            if (id != bien.BienFamiliarId)
            {
                return BadRequest(new
                {
                    mensaje = "El ID enviado no coincide con el ID del bien familiar."
                });
            }

            var bienExistente = await _context.BienesFamiliares
                .FirstOrDefaultAsync(b => b.BienFamiliarId == id);

            if (bienExistente == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el bien familiar que desea actualizar."
                });
            }

            var existeVisita = await _context.VisitasFamiliares
                .AnyAsync(v => v.VisitaFamiliarId == bien.VisitaFamiliarId);

            if (!existeVisita)
            {
                return BadRequest(new
                {
                    mensaje = "La visita familiar indicada no existe."
                });
            }

            var validacion = ValidarBienFamiliar(bien);
            if (validacion != null)
            {
                return BadRequest(validacion);
            }

            bienExistente.VisitaFamiliarId = bien.VisitaFamiliarId;
            bienExistente.TipoBien = bien.TipoBien;
            bienExistente.Descripcion = bien.Descripcion;
            bienExistente.Cantidad = bien.Cantidad;
            bienExistente.Observaciones = bien.Observaciones;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Bien familiar actualizado correctamente."
            });
        }

        // DELETE: api/BienesFamiliares/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBienFamiliar(int id)
        {
            var bien = await _context.BienesFamiliares
                .FirstOrDefaultAsync(b => b.BienFamiliarId == id);

            if (bien == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el bien familiar que desea eliminar."
                });
            }

            _context.BienesFamiliares.Remove(bien);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Bien familiar eliminado correctamente."
            });
        }

        private object? ValidarBienFamiliar(BienFamiliar bien)
        {
            if (bien.Cantidad < 0)
            {
                return new
                {
                    mensaje = "La cantidad no puede ser negativa."
                };
            }

            return null;
        }
    }
}