using ASEDUPH_V2_API.Data;
using ASEDUPH_V2_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASEDUPH_V2_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisitasFamiliaresController : ControllerBase
    {
        private readonly AseduphDbContext _context;

        public VisitasFamiliaresController(AseduphDbContext context)
        {
            _context = context;
        }

        // GET: api/VisitasFamiliares
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VisitaFamiliar>>> GetVisitasFamiliares()
        {
            var visitas = await _context.VisitasFamiliares
                .Include(v => v.SolicitudBeca)
                    .ThenInclude(s => s.Estudiante)
                .OrderByDescending(v => v.FechaVisita)
                .ToListAsync();

            return Ok(visitas);
        }

        // GET: api/VisitasFamiliares/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VisitaFamiliar>> GetVisitaFamiliar(int id)
        {
            var visita = await _context.VisitasFamiliares
                .Include(v => v.SolicitudBeca)
                    .ThenInclude(s => s.Estudiante)
                .FirstOrDefaultAsync(v => v.VisitaFamiliarId == id);

            if (visita == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró la visita familiar solicitada."
                });
            }

            return Ok(visita);
        }

        // GET: api/VisitasFamiliares/solicitud/5
        [HttpGet("solicitud/{solicitudBecaId}")]
        public async Task<ActionResult<VisitaFamiliar>> GetVisitaPorSolicitud(int solicitudBecaId)
        {
            var visita = await _context.VisitasFamiliares
                .Include(v => v.SolicitudBeca)
                    .ThenInclude(s => s.Estudiante)
                .FirstOrDefaultAsync(v => v.SolicitudBecaId == solicitudBecaId);

            if (visita == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró visita familiar para esta solicitud."
                });
            }

            return Ok(visita);
        }

        // POST: api/VisitasFamiliares
        [HttpPost]
        public async Task<ActionResult<VisitaFamiliar>> PostVisitaFamiliar(VisitaFamiliar visita)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existeSolicitud = await _context.SolicitudesBeca
                .AnyAsync(s => s.SolicitudBecaId == visita.SolicitudBecaId);

            if (!existeSolicitud)
            {
                return BadRequest(new
                {
                    mensaje = "La solicitud de beca indicada no existe."
                });
            }

            var yaTieneVisita = await _context.VisitasFamiliares
                .AnyAsync(v => v.SolicitudBecaId == visita.SolicitudBecaId);

            if (yaTieneVisita)
            {
                return BadRequest(new
                {
                    mensaje = "Esta solicitud ya tiene una visita familiar registrada."
                });
            }

            _context.VisitasFamiliares.Add(visita);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetVisitaFamiliar),
                new { id = visita.VisitaFamiliarId },
                visita
            );
        }

        // PUT: api/VisitasFamiliares/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVisitaFamiliar(int id, VisitaFamiliar visita)
        {
            if (id != visita.VisitaFamiliarId)
            {
                return BadRequest(new
                {
                    mensaje = "El ID enviado no coincide con el ID de la visita familiar."
                });
            }

            var visitaExistente = await _context.VisitasFamiliares
                .FirstOrDefaultAsync(v => v.VisitaFamiliarId == id);

            if (visitaExistente == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró la visita familiar que desea actualizar."
                });
            }

            var existeSolicitud = await _context.SolicitudesBeca
                .AnyAsync(s => s.SolicitudBecaId == visita.SolicitudBecaId);

            if (!existeSolicitud)
            {
                return BadRequest(new
                {
                    mensaje = "La solicitud de beca indicada no existe."
                });
            }

            var visitaDuplicada = await _context.VisitasFamiliares
                .AnyAsync(v =>
                    v.SolicitudBecaId == visita.SolicitudBecaId &&
                    v.VisitaFamiliarId != id);

            if (visitaDuplicada)
            {
                return BadRequest(new
                {
                    mensaje = "Ya existe otra visita familiar registrada para esta solicitud."
                });
            }

            visitaExistente.SolicitudBecaId = visita.SolicitudBecaId;
            visitaExistente.TipoVisita = visita.TipoVisita;
            visitaExistente.LugarEntrevista = visita.LugarEntrevista;
            visitaExistente.FechaVisita = visita.FechaVisita;
            visitaExistente.HoraVisita = visita.HoraVisita;
            visitaExistente.PersonaEntrevistada = visita.PersonaEntrevistada;
            visitaExistente.ParentescoEntrevistado = visita.ParentescoEntrevistado;
            visitaExistente.ActitudFamilia = visita.ActitudFamilia;
            visitaExistente.ApreciacionGeneral = visita.ApreciacionGeneral;
            visitaExistente.RecomendacionJunta = visita.RecomendacionJunta;
            visitaExistente.RealizadaPor = visita.RealizadaPor;
            visitaExistente.Firma = visita.Firma;
            visitaExistente.ObservacionesFinales = visita.ObservacionesFinales;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Visita familiar actualizada correctamente."
            });
        }

        // DELETE: api/VisitasFamiliares/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVisitaFamiliar(int id)
        {
            var visita = await _context.VisitasFamiliares
                .FirstOrDefaultAsync(v => v.VisitaFamiliarId == id);

            if (visita == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró la visita familiar que desea eliminar."
                });
            }

            _context.VisitasFamiliares.Remove(visita);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Visita familiar eliminada correctamente."
            });
        }
    }
}