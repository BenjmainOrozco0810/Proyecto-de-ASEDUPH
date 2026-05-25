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
    public class BecasController : ControllerBase
    {
        private readonly AseduphDbContext _context;
        private readonly AuditoriaService _auditoria;

        public BecasController(AseduphDbContext context, AuditoriaService auditoria)
        {
            _context = context;
            _auditoria = auditoria;
        }

        // GET: api/Becas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Beca>>> GetBecas()
        {
            var becas = await _context.Becas
                .Include(b => b.Estudiante)
                .Include(b => b.SolicitudBeca)
                .Where(b => b.EstadoBeca == "Activa")
                .OrderByDescending(b => b.AnioInicio)
                    .ThenBy(b => b.Estudiante!.NombreCompleto)
                .ToListAsync();

            return Ok(becas);
        }

        // GET: api/Becas/todas
        [HttpGet("todas")]
        public async Task<ActionResult<IEnumerable<Beca>>> GetTodasBecas()
        {
            var becas = await _context.Becas
                .Include(b => b.Estudiante)
                .Include(b => b.SolicitudBeca)
                .OrderByDescending(b => b.AnioInicio)
                    .ThenBy(b => b.Estudiante!.NombreCompleto)
                .ToListAsync();

            return Ok(becas);
        }

        // GET: api/Becas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Beca>> GetBeca(int id)
        {
            var beca = await _context.Becas
                .Include(b => b.Estudiante)
                .Include(b => b.SolicitudBeca)
                .Include(b => b.RenovacionesBeca)
                .Include(b => b.SeguimientosAcademicos)
                .FirstOrDefaultAsync(b => b.BecaId == id);

            if (beca == null)
                return NotFound(new { mensaje = "No se encontró la beca solicitada." });

            return Ok(beca);
        }

        // GET: api/Becas/estudiante/5
        [HttpGet("estudiante/{estudianteId}")]
        public async Task<ActionResult<IEnumerable<Beca>>> GetBecasPorEstudiante(int estudianteId)
        {
            var existeEstudiante = await _context.Estudiantes
                .AnyAsync(e => e.EstudianteId == estudianteId);

            if (!existeEstudiante)
                return NotFound(new { mensaje = "No se encontró el estudiante indicado." });

            var becas = await _context.Becas
                .Include(b => b.SolicitudBeca)
                .Include(b => b.RenovacionesBeca)
                .Where(b => b.EstudianteId == estudianteId)
                .OrderByDescending(b => b.AnioInicio)
                .ToListAsync();

            return Ok(becas);
        }

        // POST: api/Becas
        [HttpPost]
        public async Task<ActionResult<Beca>> PostBeca(Beca beca)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var estudiante = await _context.Estudiantes
                .FirstOrDefaultAsync(e => e.EstudianteId == beca.EstudianteId && e.Estado == "Activo");

            if (estudiante == null)
                return BadRequest(new { mensaje = "El estudiante indicado no existe o está inactivo." });

            if (beca.SolicitudBecaId != null)
            {
                var existeSolicitud = await _context.SolicitudesBeca
                    .AnyAsync(s => s.SolicitudBecaId == beca.SolicitudBecaId);

                if (!existeSolicitud)
                    return BadRequest(new { mensaje = "La solicitud de beca indicada no existe." });

                var becaDuplicada = await _context.Becas
                    .AnyAsync(b => b.SolicitudBecaId == beca.SolicitudBecaId);

                if (becaDuplicada)
                    return BadRequest(new { mensaje = "Esta solicitud de beca ya tiene una beca asignada." });
            }

            var validacion = ValidarBeca(beca);
            if (validacion != null)
                return BadRequest(validacion);

            beca.EstadoBeca = "Activa";
            _context.Becas.Add(beca);

            if (beca.SolicitudBecaId != null)
            {
                var solicitud = await _context.SolicitudesBeca
                    .FirstOrDefaultAsync(s => s.SolicitudBecaId == beca.SolicitudBecaId);

                if (solicitud != null)
                    solicitud.EstadoSolicitud = "Aprobada";
            }

            await _context.SaveChangesAsync();

            // ── Log de auditoría ──────────────────────────────────────────
            await _auditoria.RegistrarAsync(
                accion: "Crear",
                modulo: "Becas",
                descripcion: $"Se creó beca para '{estudiante.NombreCompleto}' — Nivel: {beca.NivelEducativo}, Año inicio: {beca.AnioInicio}.",
                entidadAfectada: estudiante.NombreCompleto,
                entidadId: beca.BecaId,
                ip: HttpContext.Connection.RemoteIpAddress?.ToString()
            );

            return CreatedAtAction(nameof(GetBeca), new { id = beca.BecaId }, beca);
        }

        // PUT: api/Becas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBeca(int id, Beca beca)
        {
            if (id != beca.BecaId)
                return BadRequest(new { mensaje = "El ID enviado no coincide con el ID de la beca." });

            var becaExistente = await _context.Becas
                .FirstOrDefaultAsync(b => b.BecaId == id);

            if (becaExistente == null)
                return NotFound(new { mensaje = "No se encontró la beca que desea actualizar." });

            var estudiante = await _context.Estudiantes
                .FirstOrDefaultAsync(e => e.EstudianteId == beca.EstudianteId);

            if (estudiante == null)
                return BadRequest(new { mensaje = "El estudiante indicado no existe." });

            var validacion = ValidarBeca(beca);
            if (validacion != null)
                return BadRequest(validacion);

            becaExistente.EstudianteId = beca.EstudianteId;
            becaExistente.SolicitudBecaId = beca.SolicitudBecaId;
            becaExistente.AnioInicio = beca.AnioInicio;
            becaExistente.AnioFin = beca.AnioFin;
            becaExistente.NivelEducativo = beca.NivelEducativo;
            becaExistente.TipoBeca = beca.TipoBeca;
            becaExistente.EstadoBeca = beca.EstadoBeca;
            becaExistente.MontoAprobado = beca.MontoAprobado;
            becaExistente.Observaciones = beca.Observaciones;
            becaExistente.FechaAprobacion = beca.FechaAprobacion;

            await _context.SaveChangesAsync();

            // ── Log de auditoría ──────────────────────────────────────────
            await _auditoria.RegistrarAsync(
                accion: "Editar",
                modulo: "Becas",
                descripcion: $"Se actualizó la beca de '{estudiante.NombreCompleto}' — Estado: {becaExistente.EstadoBeca}.",
                entidadAfectada: estudiante.NombreCompleto,
                entidadId: id,
                ip: HttpContext.Connection.RemoteIpAddress?.ToString()
            );

            return Ok(new { mensaje = "Beca actualizada correctamente." });
        }

        // PATCH: api/Becas/5/estado
        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> CambiarEstadoBeca(int id, [FromBody] string nuevoEstado)
        {
            var beca = await _context.Becas
                .Include(b => b.Estudiante)
                .FirstOrDefaultAsync(b => b.BecaId == id);

            if (beca == null)
                return NotFound(new { mensaje = "No se encontró la beca." });

            var estadosValidos = new List<string>
            {
                "Activa", "Finalizada", "Suspendida", "Cancelada"
            };

            if (!estadosValidos.Contains(nuevoEstado))
                return BadRequest(new { mensaje = "Estado no válido. Use: Activa, Finalizada, Suspendida o Cancelada." });

            var estadoAnterior = beca.EstadoBeca;
            beca.EstadoBeca = nuevoEstado;
            await _context.SaveChangesAsync();

            // ── Log de auditoría ──────────────────────────────────────────
            await _auditoria.RegistrarAsync(
                accion: "CambiarEstado",
                modulo: "Becas",
                descripcion: $"Estado de beca de '{beca.Estudiante?.NombreCompleto}' cambió de '{estadoAnterior}' a '{nuevoEstado}'.",
                entidadAfectada: beca.Estudiante?.NombreCompleto,
                entidadId: id,
                ip: HttpContext.Connection.RemoteIpAddress?.ToString()
            );

            return Ok(new { mensaje = "Estado de beca actualizado correctamente." });
        }

        // DELETE: api/Becas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBeca(int id)
        {
            var beca = await _context.Becas
                .Include(b => b.Estudiante)
                .FirstOrDefaultAsync(b => b.BecaId == id);

            if (beca == null)
                return NotFound(new { mensaje = "No se encontró la beca que desea cancelar." });

            beca.EstadoBeca = "Cancelada";
            await _context.SaveChangesAsync();

            // ── Log de auditoría ──────────────────────────────────────────
            await _auditoria.RegistrarAsync(
                accion: "Eliminar",
                modulo: "Becas",
                descripcion: $"Se canceló la beca de '{beca.Estudiante?.NombreCompleto}'.",
                entidadAfectada: beca.Estudiante?.NombreCompleto,
                entidadId: id,
                ip: HttpContext.Connection.RemoteIpAddress?.ToString()
            );

            return Ok(new { mensaje = "Beca cancelada correctamente." });
        }

        private object? ValidarBeca(Beca beca)
        {
            if (beca.AnioInicio < 2000)
                return new { mensaje = "El año de inicio no puede ser menor a 2000." };

            if (beca.AnioFin != null && beca.AnioFin < beca.AnioInicio)
                return new { mensaje = "El año de fin no puede ser menor al año de inicio." };

            if (beca.MontoAprobado < 0)
                return new { mensaje = "El monto aprobado no puede ser negativo." };

            var nivelesValidos = new List<string>
            {
                "Preprimaria", "Primaria", "Básicos", "Diversificado", "Universidad"
            };

            if (!string.IsNullOrWhiteSpace(beca.NivelEducativo) &&
                !nivelesValidos.Contains(beca.NivelEducativo))
                return new { mensaje = "Nivel educativo no válido. Use: Preprimaria, Primaria, Básicos, Diversificado o Universidad." };

            return null;
        }
    }
}
