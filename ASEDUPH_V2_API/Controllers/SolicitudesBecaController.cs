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
    public class SolicitudesBecaController : ControllerBase
    {
        private readonly AseduphDbContext _context;
        private readonly AuditoriaService _auditoria;

        public SolicitudesBecaController(AseduphDbContext context, AuditoriaService auditoria)
        {
            _context = context;
            _auditoria = auditoria;
        }

        // GET: api/SolicitudesBeca
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<SolicitudBeca>>> GetSolicitudesBeca()
        {
            var solicitudes = await _context.SolicitudesBeca
                .Include(s => s.Estudiante)
                .Include(s => s.CentroEducativo)
                .Include(s => s.SolicitudTiposApoyo)
                    .ThenInclude(sta => sta.TipoApoyo)
                .OrderByDescending(s => s.FechaSolicitud)
                .ToListAsync();

            return Ok(solicitudes);
        }

        // GET: api/SolicitudesBeca/aprobadas-sin-beca
        // Devuelve las solicitudes Aprobadas que aún NO tienen una beca asignada
        [HttpGet("aprobadas-sin-beca")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<SolicitudBeca>>> GetAprobadasSinBeca()
        {
            // IDs de solicitudes que ya tienen una beca asociada
            var idsConBeca = await _context.Becas
                .Where(b => b.SolicitudBecaId != null)
                .Select(b => b.SolicitudBecaId!.Value)
                .ToListAsync();

            var solicitudes = await _context.SolicitudesBeca
                .Include(s => s.Estudiante)
                .Include(s => s.CentroEducativo)
                .Where(s => s.EstadoSolicitud == "Aprobada"
                         && !idsConBeca.Contains(s.SolicitudBecaId))
                .OrderByDescending(s => s.FechaSolicitud)
                .ToListAsync();

            return Ok(solicitudes);
        }

        // GET: api/SolicitudesBeca/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<SolicitudBeca>> GetSolicitudBeca(int id)
        {
            var solicitud = await _context.SolicitudesBeca
                .Include(s => s.Estudiante)
                .Include(s => s.CentroEducativo)
                .Include(s => s.SolicitudTiposApoyo)
                    .ThenInclude(sta => sta.TipoApoyo)
                .FirstOrDefaultAsync(s => s.SolicitudBecaId == id);

            if (solicitud == null)
                return NotFound(new { mensaje = "No se encontró la solicitud de beca." });

            return Ok(solicitud);
        }

        // GET: api/SolicitudesBeca/estudiante/5
        [HttpGet("estudiante/{estudianteId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<SolicitudBeca>>> GetSolicitudesPorEstudiante(int estudianteId)
        {
            var existeEstudiante = await _context.Estudiantes
                .AnyAsync(e => e.EstudianteId == estudianteId);

            if (!existeEstudiante)
                return NotFound(new { mensaje = "No se encontró el estudiante indicado." });

            var solicitudes = await _context.SolicitudesBeca
                .Include(s => s.CentroEducativo)
                .Include(s => s.SolicitudTiposApoyo)
                    .ThenInclude(sta => sta.TipoApoyo)
                .Where(s => s.EstudianteId == estudianteId)
                .OrderByDescending(s => s.FechaSolicitud)
                .ToListAsync();

            return Ok(solicitudes);
        }

        // POST: api/SolicitudesBeca
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<SolicitudBeca>> PostSolicitudBeca(SolicitudBeca solicitud)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existeEstudiante = await _context.Estudiantes
                .AnyAsync(e => e.EstudianteId == solicitud.EstudianteId && e.Estado == "Activo");

            if (!existeEstudiante)
                return BadRequest(new { mensaje = "El estudiante indicado no existe o está inactivo." });

            if (solicitud.CentroEducativoId != null)
            {
                var existeCentro = await _context.CentrosEducativos
                    .AnyAsync(c => c.CentroEducativoId == solicitud.CentroEducativoId && c.Estado == "Activo");

                if (!existeCentro)
                    return BadRequest(new { mensaje = "El centro educativo indicado no existe o está inactivo." });
            }

            solicitud.EstadoSolicitud = "Pendiente";

            _context.SolicitudesBeca.Add(solicitud);
            await _context.SaveChangesAsync();

            // ── Log de auditoría ──────────────────────────────────────────
            var nombreEstudiante = (await _context.Estudiantes.FindAsync(solicitud.EstudianteId))?.NombreCompleto ?? "Desconocido";
            await _auditoria.RegistrarAsync(
                accion: "Crear",
                modulo: "Solicitudes",
                descripcion: $"Se creó solicitud de beca para el estudiante '{nombreEstudiante}' — Año: {solicitud.AnioSolicitud}.",
                entidadAfectada: nombreEstudiante,
                entidadId: solicitud.SolicitudBecaId,
                ip: HttpContext.Connection.RemoteIpAddress?.ToString()
            );

            return CreatedAtAction(
                nameof(GetSolicitudBeca),
                new { id = solicitud.SolicitudBecaId },
                solicitud
            );
        }

        // POST: api/SolicitudesBeca/publica (sin autenticación)
        [HttpPost("publica")]
        [AllowAnonymous]
        public async Task<ActionResult<SolicitudBeca>> PostSolicitudPublica(SolicitudBeca solicitud)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existeEstudiante = await _context.Estudiantes
                .AnyAsync(e => e.EstudianteId == solicitud.EstudianteId && e.Estado == "Activo");

            if (!existeEstudiante)
                return BadRequest(new { mensaje = "El estudiante indicado no existe o está inactivo." });

            solicitud.EstadoSolicitud = "Pendiente";
            solicitud.FechaSolicitud = DateTime.Now;

            _context.SolicitudesBeca.Add(solicitud);
            await _context.SaveChangesAsync();

            // ── Log de auditoría ──────────────────────────────────────────
            var nombreEstudiante = (await _context.Estudiantes.FindAsync(solicitud.EstudianteId))?.NombreCompleto ?? "Desconocido";
            await _auditoria.RegistrarAsync(
                accion: "Crear",
                modulo: "Solicitudes",
                descripcion: $"Solicitud pública enviada para '{nombreEstudiante}' desde formulario público.",
                entidadAfectada: nombreEstudiante,
                entidadId: solicitud.SolicitudBecaId,
                ip: HttpContext.Connection.RemoteIpAddress?.ToString()
            );

            return Ok(new
            {
                mensaje = "Solicitud enviada correctamente. Nos pondremos en contacto con usted pronto.",
                solicitudId = solicitud.SolicitudBecaId
            });
        }

        // PUT: api/SolicitudesBeca/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutSolicitudBeca(int id, SolicitudBeca solicitud)
        {
            if (id != solicitud.SolicitudBecaId)
                return BadRequest(new { mensaje = "El ID enviado no coincide con el ID de la solicitud." });

            var solicitudExistente = await _context.SolicitudesBeca
                .FirstOrDefaultAsync(s => s.SolicitudBecaId == id);

            if (solicitudExistente == null)
                return NotFound(new { mensaje = "No se encontró la solicitud que desea actualizar." });

            var existeEstudiante = await _context.Estudiantes
                .AnyAsync(e => e.EstudianteId == solicitud.EstudianteId);

            if (!existeEstudiante)
                return BadRequest(new { mensaje = "El estudiante indicado no existe." });

            if (solicitud.CentroEducativoId != null)
            {
                var existeCentro = await _context.CentrosEducativos
                    .AnyAsync(c => c.CentroEducativoId == solicitud.CentroEducativoId);

                if (!existeCentro)
                    return BadRequest(new { mensaje = "El centro educativo indicado no existe." });
            }

            solicitudExistente.EstudianteId = solicitud.EstudianteId;
            solicitudExistente.CentroEducativoId = solicitud.CentroEducativoId;
            solicitudExistente.AnioSolicitud = solicitud.AnioSolicitud;
            solicitudExistente.NivelEducativo = solicitud.NivelEducativo;
            solicitudExistente.GradoSolicitado = solicitud.GradoSolicitado;
            solicitudExistente.PromedioActual = solicitud.PromedioActual;
            solicitudExistente.MotivoSolicitud = solicitud.MotivoSolicitud;
            solicitudExistente.EstadoSolicitud = solicitud.EstadoSolicitud;
            solicitudExistente.FechaSolicitud = solicitud.FechaSolicitud;
            solicitudExistente.NombrePersonaCompletaFormulario = solicitud.NombrePersonaCompletaFormulario;
            solicitudExistente.FechaFormulario = solicitud.FechaFormulario;
            solicitudExistente.Observaciones = solicitud.Observaciones;

            await _context.SaveChangesAsync();

            // ── Log de auditoría ──────────────────────────────────────────
            var nombreEstudiante = (await _context.Estudiantes.FindAsync(solicitudExistente.EstudianteId))?.NombreCompleto ?? "Desconocido";
            await _auditoria.RegistrarAsync(
                accion: "Editar",
                modulo: "Solicitudes",
                descripcion: $"Se actualizó la solicitud de beca de '{nombreEstudiante}' — Año: {solicitudExistente.AnioSolicitud}.",
                entidadAfectada: nombreEstudiante,
                entidadId: id,
                ip: HttpContext.Connection.RemoteIpAddress?.ToString()
            );

            return Ok(new { mensaje = "Solicitud de beca actualizada correctamente." });
        }

        // PATCH: api/SolicitudesBeca/5/estado
        [HttpPatch("{id}/estado")]
        [Authorize]
        public async Task<IActionResult> CambiarEstadoSolicitud(int id, [FromBody] string nuevoEstado)
        {
            var solicitud = await _context.SolicitudesBeca
                .Include(s => s.Estudiante)
                .FirstOrDefaultAsync(s => s.SolicitudBecaId == id);

            if (solicitud == null)
                return NotFound(new { mensaje = "No se encontró la solicitud de beca." });

            var estadosValidos = new List<string>
            {
                "Pendiente", "En Evaluación", "Aprobada", "Rechazada", "Cancelada"
            };

            if (!estadosValidos.Contains(nuevoEstado))
                return BadRequest(new { mensaje = "Estado no válido." });

            // ── Validación por EstudianteId ───────────────────────────────
            if (nuevoEstado == "Rechazada" || nuevoEstado == "Cancelada")
            {
                var tieneBecaActiva = await _context.Becas
                    .AnyAsync(b => b.EstudianteId == solicitud.EstudianteId
                                && b.EstadoBeca == "Activa");

                if (tieneBecaActiva)
                    return BadRequest(new
                    {
                        mensaje = "No se puede rechazar o cancelar una solicitud cuyo estudiante tiene una beca activa. Primero cancele o finalice la beca."
                    });
            }

            var estadoAnterior = solicitud.EstadoSolicitud;
            solicitud.EstadoSolicitud = nuevoEstado;
            await _context.SaveChangesAsync();

            await _auditoria.RegistrarAsync(
                accion: "CambiarEstado",
                modulo: "Solicitudes",
                descripcion: $"Estado de solicitud de '{solicitud.Estudiante?.NombreCompleto}' cambió de '{estadoAnterior}' a '{nuevoEstado}'.",
                entidadAfectada: solicitud.Estudiante?.NombreCompleto,
                entidadId: id,
                ip: HttpContext.Connection.RemoteIpAddress?.ToString()
            );

            return Ok(new { mensaje = "Estado de solicitud actualizado correctamente." });
        }

        // DELETE: api/SolicitudesBeca/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteSolicitudBeca(int id)
        {
            var solicitud = await _context.SolicitudesBeca
                .Include(s => s.Estudiante)
                .FirstOrDefaultAsync(s => s.SolicitudBecaId == id);

            if (solicitud == null)
                return NotFound(new { mensaje = "No se encontró la solicitud que desea cancelar." });

            solicitud.EstadoSolicitud = "Cancelada";
            await _context.SaveChangesAsync();

            // ── Log de auditoría ──────────────────────────────────────────
            await _auditoria.RegistrarAsync(
                accion: "Eliminar",
                modulo: "Solicitudes",
                descripcion: $"Se canceló la solicitud de beca de '{solicitud.Estudiante?.NombreCompleto}'.",
                entidadAfectada: solicitud.Estudiante?.NombreCompleto,
                entidadId: id,
                ip: HttpContext.Connection.RemoteIpAddress?.ToString()
            );

            return Ok(new { mensaje = "Solicitud de beca cancelada correctamente." });
        }
    }
}
