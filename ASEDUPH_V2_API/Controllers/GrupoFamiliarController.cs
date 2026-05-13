using ASEDUPH_V2_API.Data;
using ASEDUPH_V2_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASEDUPH_V2_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GrupoFamiliarController : ControllerBase
    {
        private readonly AseduphDbContext _context;

        public GrupoFamiliarController(AseduphDbContext context)
        {
            _context = context;
        }

        // GET: api/GrupoFamiliar
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GrupoFamiliar>>> GetGrupoFamiliar()
        {
            var grupo = await _context.GrupoFamiliar
                .Include(g => g.VisitaFamiliar)
                    .ThenInclude(v => v.SolicitudBeca)
                        .ThenInclude(s => s.Estudiante)
                .OrderBy(g => g.Nombre)
                .ToListAsync();

            return Ok(grupo);
        }

        // GET: api/GrupoFamiliar/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GrupoFamiliar>> GetMiembroGrupoFamiliar(int id)
        {
            var miembro = await _context.GrupoFamiliar
                .Include(g => g.VisitaFamiliar)
                    .ThenInclude(v => v.SolicitudBeca)
                        .ThenInclude(s => s.Estudiante)
                .FirstOrDefaultAsync(g => g.GrupoFamiliarId == id);

            if (miembro == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el miembro del grupo familiar solicitado."
                });
            }

            return Ok(miembro);
        }

        // GET: api/GrupoFamiliar/visita/5
        [HttpGet("visita/{visitaFamiliarId}")]
        public async Task<ActionResult<IEnumerable<GrupoFamiliar>>> GetGrupoPorVisita(int visitaFamiliarId)
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

            var grupo = await _context.GrupoFamiliar
                .Where(g => g.VisitaFamiliarId == visitaFamiliarId)
                .OrderBy(g => g.Nombre)
                .ToListAsync();

            return Ok(grupo);
        }

        // POST: api/GrupoFamiliar
        [HttpPost]
        public async Task<ActionResult<GrupoFamiliar>> PostMiembroGrupoFamiliar(GrupoFamiliar miembro)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existeVisita = await _context.VisitasFamiliares
                .AnyAsync(v => v.VisitaFamiliarId == miembro.VisitaFamiliarId);

            if (!existeVisita)
            {
                return BadRequest(new
                {
                    mensaje = "La visita familiar indicada no existe."
                });
            }

            if (miembro.Edad < 0)
            {
                return BadRequest(new
                {
                    mensaje = "La edad no puede ser negativa."
                });
            }

            if (miembro.Ingresos < 0 || miembro.GastosColegiatura < 0)
            {
                return BadRequest(new
                {
                    mensaje = "Los ingresos y gastos de colegiatura no pueden ser negativos."
                });
            }

            _context.GrupoFamiliar.Add(miembro);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetMiembroGrupoFamiliar),
                new { id = miembro.GrupoFamiliarId },
                miembro
            );
        }

        // PUT: api/GrupoFamiliar/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMiembroGrupoFamiliar(int id, GrupoFamiliar miembro)
        {
            if (id != miembro.GrupoFamiliarId)
            {
                return BadRequest(new
                {
                    mensaje = "El ID enviado no coincide con el ID del miembro familiar."
                });
            }

            var miembroExistente = await _context.GrupoFamiliar
                .FirstOrDefaultAsync(g => g.GrupoFamiliarId == id);

            if (miembroExistente == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el miembro familiar que desea actualizar."
                });
            }

            var existeVisita = await _context.VisitasFamiliares
                .AnyAsync(v => v.VisitaFamiliarId == miembro.VisitaFamiliarId);

            if (!existeVisita)
            {
                return BadRequest(new
                {
                    mensaje = "La visita familiar indicada no existe."
                });
            }

            if (miembro.Edad < 0)
            {
                return BadRequest(new
                {
                    mensaje = "La edad no puede ser negativa."
                });
            }

            if (miembro.Ingresos < 0 || miembro.GastosColegiatura < 0)
            {
                return BadRequest(new
                {
                    mensaje = "Los ingresos y gastos de colegiatura no pueden ser negativos."
                });
            }

            miembroExistente.VisitaFamiliarId = miembro.VisitaFamiliarId;
            miembroExistente.Nombre = miembro.Nombre;
            miembroExistente.Parentesco = miembro.Parentesco;
            miembroExistente.Edad = miembro.Edad;
            miembroExistente.LugarTrabajoEstudio = miembro.LugarTrabajoEstudio;
            miembroExistente.Ingresos = miembro.Ingresos;
            miembroExistente.GastosColegiatura = miembro.GastosColegiatura;
            miembroExistente.ViveConFamilia = miembro.ViveConFamilia;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Miembro del grupo familiar actualizado correctamente."
            });
        }

        // DELETE: api/GrupoFamiliar/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMiembroGrupoFamiliar(int id)
        {
            var miembro = await _context.GrupoFamiliar
                .FirstOrDefaultAsync(g => g.GrupoFamiliarId == id);

            if (miembro == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el miembro familiar que desea eliminar."
                });
            }

            _context.GrupoFamiliar.Remove(miembro);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Miembro del grupo familiar eliminado correctamente."
            });
        }
    }
}