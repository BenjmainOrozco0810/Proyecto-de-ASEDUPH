using ASEDUPH_V2_API.Data;
using ASEDUPH_V2_API.Models;
using ASEDUPH_V2_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASEDUPH_V2_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VisitasFamiliaresController : ControllerBase
    {
        private readonly AseduphDbContext _context;
        private readonly AuditoriaService _auditoria;

        public VisitasFamiliaresController(AseduphDbContext context, AuditoriaService auditoria)
        {
            _context = context;
            _auditoria = auditoria;
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
                return NotFound(new { mensaje = "No se encontró la visita familiar solicitada." });

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
                return NotFound(new { mensaje = "No se encontró visita familiar para esta solicitud." });

            return Ok(visita);
        }

        // POST: api/VisitasFamiliares
        [HttpPost]
        public async Task<ActionResult<VisitaFamiliar>> PostVisitaFamiliar(VisitaFamiliar visita)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var solicitud = await _context.SolicitudesBeca
                .Include(s => s.Estudiante)
                .FirstOrDefaultAsync(s => s.SolicitudBecaId == visita.SolicitudBecaId);

            if (solicitud == null)
                return BadRequest(new { mensaje = "La solicitud de beca indicada no existe." });

            var yaTieneVisita = await _context.VisitasFamiliares
                .AnyAsync(v => v.SolicitudBecaId == visita.SolicitudBecaId);

            if (yaTieneVisita)
                return BadRequest(new { mensaje = "Esta solicitud ya tiene una visita familiar registrada." });

            _context.VisitasFamiliares.Add(visita);
            await _context.SaveChangesAsync();

            // ── Log de auditoría ──────────────────────────────────────────
            await _auditoria.RegistrarAsync(
                accion: "Crear",
                modulo: "VisitasFamiliares",
                descripcion: $"Se registró visita familiar para '{solicitud.Estudiante?.NombreCompleto}' — Tipo: {visita.TipoVisita ?? "No especificado"}, Realizada por: {visita.RealizadaPor ?? "No especificado"}.",
                entidadAfectada: solicitud.Estudiante?.NombreCompleto,
                entidadId: visita.VisitaFamiliarId,
                ip: HttpContext.Connection.RemoteIpAddress?.ToString()
            );

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
                return BadRequest(new { mensaje = "El ID enviado no coincide con el ID de la visita familiar." });

            var visitaExistente = await _context.VisitasFamiliares
                .FirstOrDefaultAsync(v => v.VisitaFamiliarId == id);

            if (visitaExistente == null)
                return NotFound(new { mensaje = "No se encontró la visita familiar que desea actualizar." });

            var solicitud = await _context.SolicitudesBeca
                .Include(s => s.Estudiante)
                .FirstOrDefaultAsync(s => s.SolicitudBecaId == visita.SolicitudBecaId);

            if (solicitud == null)
                return BadRequest(new { mensaje = "La solicitud de beca indicada no existe." });

            var visitaDuplicada = await _context.VisitasFamiliares
                .AnyAsync(v => v.SolicitudBecaId == visita.SolicitudBecaId &&
                               v.VisitaFamiliarId != id);

            if (visitaDuplicada)
                return BadRequest(new { mensaje = "Ya existe otra visita familiar registrada para esta solicitud." });

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

            // ── Log de auditoría ──────────────────────────────────────────
            await _auditoria.RegistrarAsync(
                accion: "Editar",
                modulo: "VisitasFamiliares",
                descripcion: $"Se actualizó visita familiar de '{solicitud.Estudiante?.NombreCompleto}' — Tipo: {visitaExistente.TipoVisita ?? "No especificado"}.",
                entidadAfectada: solicitud.Estudiante?.NombreCompleto,
                entidadId: id,
                ip: HttpContext.Connection.RemoteIpAddress?.ToString()
            );

            return Ok(new { mensaje = "Visita familiar actualizada correctamente." });
        }

        // DELETE: api/VisitasFamiliares/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVisitaFamiliar(int id)
        {
            var visita = await _context.VisitasFamiliares
                .Include(v => v.SolicitudBeca)
                    .ThenInclude(s => s.Estudiante)
                .FirstOrDefaultAsync(v => v.VisitaFamiliarId == id);

            if (visita == null)
                return NotFound(new { mensaje = "No se encontró la visita familiar que desea eliminar." });

            var nombreEstudiante = visita.SolicitudBeca?.Estudiante?.NombreCompleto ?? "Desconocido";

            _context.VisitasFamiliares.Remove(visita);
            await _context.SaveChangesAsync();

            // ── Log de auditoría ──────────────────────────────────────────
            await _auditoria.RegistrarAsync(
                accion: "Eliminar",
                modulo: "VisitasFamiliares",
                descripcion: $"Se eliminó visita familiar de '{nombreEstudiante}'.",
                entidadAfectada: nombreEstudiante,
                entidadId: id,
                ip: HttpContext.Connection.RemoteIpAddress?.ToString()
            );

            return Ok(new { mensaje = "Visita familiar eliminada correctamente." });
        }
    }
}
