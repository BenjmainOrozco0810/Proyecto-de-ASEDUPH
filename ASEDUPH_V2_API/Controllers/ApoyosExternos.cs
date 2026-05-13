using ASEDUPH_V2_API.Data;
using ASEDUPH_V2_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASEDUPH_V2_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApoyosExternosController : ControllerBase
    {
        private readonly AseduphDbContext _context;

        public ApoyosExternosController(AseduphDbContext context)
        {
            _context = context;
        }

        // GET: api/ApoyosExternos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApoyoExterno>>> GetApoyosExternos()
        {
            var apoyos = await _context.ApoyosExternos
                .Include(a => a.VisitaFamiliar)
                    .ThenInclude(v => v.SolicitudBeca)
                        .ThenInclude(s => s.Estudiante)
                .OrderBy(a => a.NombreBeneficiado)
                .ToListAsync();

            return Ok(apoyos);
        }

        // GET: api/ApoyosExternos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApoyoExterno>> GetApoyoExterno(int id)
        {
            var apoyo = await _context.ApoyosExternos
                .Include(a => a.VisitaFamiliar)
                    .ThenInclude(v => v.SolicitudBeca)
                        .ThenInclude(s => s.Estudiante)
                .FirstOrDefaultAsync(a => a.ApoyoExternoId == id);

            if (apoyo == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el apoyo externo solicitado."
                });
            }

            return Ok(apoyo);
        }

        // GET: api/ApoyosExternos/visita/5
        [HttpGet("visita/{visitaFamiliarId}")]
        public async Task<ActionResult<IEnumerable<ApoyoExterno>>> GetApoyosPorVisita(int visitaFamiliarId)
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

            var apoyos = await _context.ApoyosExternos
                .Where(a => a.VisitaFamiliarId == visitaFamiliarId)
                .OrderBy(a => a.NombreBeneficiado)
                .ToListAsync();

            return Ok(apoyos);
        }

        // POST: api/ApoyosExternos
        [HttpPost]
        public async Task<ActionResult<ApoyoExterno>> PostApoyoExterno(ApoyoExterno apoyo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existeVisita = await _context.VisitasFamiliares
                .AnyAsync(v => v.VisitaFamiliarId == apoyo.VisitaFamiliarId);

            if (!existeVisita)
            {
                return BadRequest(new
                {
                    mensaje = "La visita familiar indicada no existe."
                });
            }

            var validacion = ValidarApoyoExterno(apoyo);
            if (validacion != null)
            {
                return BadRequest(validacion);
            }

            _context.ApoyosExternos.Add(apoyo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetApoyoExterno),
                new { id = apoyo.ApoyoExternoId },
                apoyo
            );
        }

        // PUT: api/ApoyosExternos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutApoyoExterno(int id, ApoyoExterno apoyo)
        {
            if (id != apoyo.ApoyoExternoId)
            {
                return BadRequest(new
                {
                    mensaje = "El ID enviado no coincide con el ID del apoyo externo."
                });
            }

            var apoyoExistente = await _context.ApoyosExternos
                .FirstOrDefaultAsync(a => a.ApoyoExternoId == id);

            if (apoyoExistente == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el apoyo externo que desea actualizar."
                });
            }

            var existeVisita = await _context.VisitasFamiliares
                .AnyAsync(v => v.VisitaFamiliarId == apoyo.VisitaFamiliarId);

            if (!existeVisita)
            {
                return BadRequest(new
                {
                    mensaje = "La visita familiar indicada no existe."
                });
            }

            var validacion = ValidarApoyoExterno(apoyo);
            if (validacion != null)
            {
                return BadRequest(validacion);
            }

            apoyoExistente.VisitaFamiliarId = apoyo.VisitaFamiliarId;
            apoyoExistente.NombreBeneficiado = apoyo.NombreBeneficiado;
            apoyoExistente.Parentesco = apoyo.Parentesco;
            apoyoExistente.Institucion = apoyo.Institucion;
            apoyoExistente.TipoAyuda = apoyo.TipoAyuda;
            apoyoExistente.PorcentajeApoyo = apoyo.PorcentajeApoyo;
            apoyoExistente.TerminoEstudios = apoyo.TerminoEstudios;
            apoyoExistente.Observaciones = apoyo.Observaciones;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Apoyo externo actualizado correctamente."
            });
        }

        // DELETE: api/ApoyosExternos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApoyoExterno(int id)
        {
            var apoyo = await _context.ApoyosExternos
                .FirstOrDefaultAsync(a => a.ApoyoExternoId == id);

            if (apoyo == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el apoyo externo que desea eliminar."
                });
            }

            _context.ApoyosExternos.Remove(apoyo);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Apoyo externo eliminado correctamente."
            });
        }

        private object? ValidarApoyoExterno(ApoyoExterno apoyo)
        {
            if (apoyo.PorcentajeApoyo < 0 || apoyo.PorcentajeApoyo > 100)
            {
                return new
                {
                    mensaje = "El porcentaje de apoyo debe estar entre 0 y 100."
                };
            }

            return null;
        }
    }
}