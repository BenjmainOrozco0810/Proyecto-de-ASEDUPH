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
    public class RenovacionBecaController : ControllerBase
    {
        private readonly AseduphDbContext _context;
        private readonly AuditoriaService _auditoria;

        public RenovacionBecaController(AseduphDbContext context, AuditoriaService auditoria)
        {
            _context = context;
            _auditoria = auditoria;
        }

        // GET: api/RenovacionBeca
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RenovacionBeca>>> GetRenovacionesBeca()
        {
            var renovaciones = await _context.RenovacionesBeca
                .Include(r => r.Beca)
                    .ThenInclude(b => b!.Estudiante)
                .Include(r => r.CentroEducativoAnterior)
                .Include(r => r.CentroEducativoNuevo)
                .OrderByDescending(r => r.AnioRenovacion)
                .ToListAsync();

            return Ok(renovaciones);
        }

        // GET: api/RenovacionBeca/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RenovacionBeca>> GetRenovacionBeca(int id)
        {
            var renovacion = await _context.RenovacionesBeca
                .Include(r => r.Beca)
                    .ThenInclude(b => b!.Estudiante)
                .Include(r => r.CentroEducativoAnterior)
                .Include(r => r.CentroEducativoNuevo)
                .FirstOrDefaultAsync(r => r.RenovacionBecaId == id);

            if (renovacion == null)
                return NotFound(new { mensaje = "No se encontró la renovación de beca solicitada." });

            return Ok(renovacion);
        }

        // GET: api/RenovacionBeca/beca/5
        [HttpGet("beca/{becaId}")]
        public async Task<ActionResult<IEnumerable<RenovacionBeca>>> GetRenovacionesPorBeca(int becaId)
        {
            var existeBeca = await _context.Becas.AnyAsync(b => b.BecaId == becaId);

            if (!existeBeca)
                return NotFound(new { mensaje = "No se encontró la beca indicada." });

            var renovaciones = await _context.RenovacionesBeca
                .Include(r => r.CentroEducativoAnterior)
                .Include(r => r.CentroEducativoNuevo)
                .Where(r => r.BecaId == becaId)
                .OrderByDescending(r => r.AnioRenovacion)
                .ToListAsync();

            return Ok(renovaciones);
        }

        // POST: api/RenovacionBeca
        [HttpPost]
        public async Task<ActionResult<RenovacionBeca>> PostRenovacionBeca(RenovacionBeca renovacion)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var beca = await _context.Becas
                .Include(b => b.Estudiante)
                .FirstOrDefaultAsync(b => b.BecaId == renovacion.BecaId && b.EstadoBeca == "Activa");

            if (beca == null)
                return BadRequest(new { mensaje = "La beca indicada no existe o no está activa." });

            var yaExiste = await _context.RenovacionesBeca
                .AnyAsync(r => r.BecaId == renovacion.BecaId && r.AnioRenovacion == renovacion.AnioRenovacion);

            if (yaExiste)
                return BadRequest(new { mensaje = "Esta beca ya tiene una renovación registrada para ese año." });

            if (renovacion.PromedioFinalAnterior < 0 || renovacion.PromedioFinalAnterior > 100)
                return BadRequest(new { mensaje = "El promedio debe estar entre 0 y 100." });

            renovacion.EstadoRenovacion = "Pendiente";

            _context.RenovacionesBeca.Add(renovacion);
            await _context.SaveChangesAsync();

            // ── Log de auditoría ──────────────────────────────────────────
            await _auditoria.RegistrarAsync(
                accion: "Crear",
                modulo: "Renovaciones",
                descripcion: $"Se registró renovación de beca para '{beca.Estudiante?.NombreCompleto}' — Año: {renovacion.AnioRenovacion}, Promedio anterior: {renovacion.PromedioFinalAnterior}.",
                entidadAfectada: beca.Estudiante?.NombreCompleto,
                entidadId: renovacion.RenovacionBecaId,
                ip: HttpContext.Connection.RemoteIpAddress?.ToString()
            );

            return CreatedAtAction(nameof(GetRenovacionBeca), new { id = renovacion.RenovacionBecaId }, renovacion);
        }

        // PUT: api/RenovacionBeca/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRenovacionBeca(int id, RenovacionBeca renovacion)
        {
            if (id != renovacion.RenovacionBecaId)
                return BadRequest(new { mensaje = "El ID enviado no coincide con el ID de la renovación." });

            var existente = await _context.RenovacionesBeca
                .FirstOrDefaultAsync(r => r.RenovacionBecaId == id);

            if (existente == null)
                return NotFound(new { mensaje = "No se encontró la renovación que desea actualizar." });

            var beca = await _context.Becas
                .Include(b => b.Estudiante)
                .FirstOrDefaultAsync(b => b.BecaId == renovacion.BecaId);

            var duplicada = await _context.RenovacionesBeca
                .AnyAsync(r => r.BecaId == renovacion.BecaId &&
                               r.AnioRenovacion == renovacion.AnioRenovacion &&
                               r.RenovacionBecaId != id);

            if (duplicada)
                return BadRequest(new { mensaje = "Ya existe otra renovación para esta beca en ese año." });

            existente.BecaId = renovacion.BecaId;
            existente.CentroEducativoAnteriorId = renovacion.CentroEducativoAnteriorId;
            existente.CentroEducativoNuevoId = renovacion.CentroEducativoNuevoId;
            existente.AnioRenovacion = renovacion.AnioRenovacion;
            existente.AnioCursadoAnterior = renovacion.AnioCursadoAnterior;
            existente.AnioACursar = renovacion.AnioACursar;
            existente.PromedioFinalAnterior = renovacion.PromedioFinalAnterior;
            existente.TipoApoyoRecibido = renovacion.TipoApoyoRecibido;
            existente.MotivoRenovacion = renovacion.MotivoRenovacion;
            existente.EstadoRenovacion = renovacion.EstadoRenovacion;
            existente.FechaSolicitud = renovacion.FechaSolicitud;
            existente.Recomendaciones = renovacion.Recomendaciones;
            existente.FechaDecision = renovacion.FechaDecision;

            await _context.SaveChangesAsync();

            // ── Log de auditoría ──────────────────────────────────────────
            await _auditoria.RegistrarAsync(
                accion: "Editar",
                modulo: "Renovaciones",
                descripcion: $"Se actualizó renovación de '{beca?.Estudiante?.NombreCompleto}' — Año: {existente.AnioRenovacion}, Estado: {existente.EstadoRenovacion}.",
                entidadAfectada: beca?.Estudiante?.NombreCompleto,
                entidadId: id,
                ip: HttpContext.Connection.RemoteIpAddress?.ToString()
            );

            return Ok(new { mensaje = "Renovación de beca actualizada correctamente." });
        }

        // PATCH: api/RenovacionBeca/5/estado
        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] string nuevoEstado)
        {
            var renovacion = await _context.RenovacionesBeca
                .Include(r => r.Beca)
                    .ThenInclude(b => b!.Estudiante)
                .FirstOrDefaultAsync(r => r.RenovacionBecaId == id);

            if (renovacion == null)
                return NotFound(new { mensaje = "No se encontró la renovación de beca." });

            var estadosValidos = new List<string>
            {
                "Pendiente", "En Evaluación", "Aprobada", "Rechazada", "Cancelada"
            };

            if (!estadosValidos.Contains(nuevoEstado))
                return BadRequest(new { mensaje = "Estado no válido. Use: Pendiente, En Evaluación, Aprobada, Rechazada o Cancelada." });

            var estadoAnterior = renovacion.EstadoRenovacion;
            renovacion.EstadoRenovacion = nuevoEstado;
            await _context.SaveChangesAsync();

            // ── Log de auditoría ──────────────────────────────────────────
            await _auditoria.RegistrarAsync(
                accion: "CambiarEstado",
                modulo: "Renovaciones",
                descripcion: $"Estado de renovación de '{renovacion.Beca?.Estudiante?.NombreCompleto}' cambió de '{estadoAnterior}' a '{nuevoEstado}'.",
                entidadAfectada: renovacion.Beca?.Estudiante?.NombreCompleto,
                entidadId: id,
                ip: HttpContext.Connection.RemoteIpAddress?.ToString()
            );

            return Ok(new { mensaje = "Estado de renovación actualizado correctamente." });
        }

        // DELETE: api/RenovacionBeca/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRenovacionBeca(int id)
        {
            var renovacion = await _context.RenovacionesBeca
                .Include(r => r.Beca)
                    .ThenInclude(b => b!.Estudiante)
                .FirstOrDefaultAsync(r => r.RenovacionBecaId == id);

            if (renovacion == null)
                return NotFound(new { mensaje = "No se encontró la renovación que desea cancelar." });

            var nombreEstudiante = renovacion.Beca?.Estudiante?.NombreCompleto ?? "Desconocido";
            var anio = renovacion.AnioRenovacion;

            renovacion.EstadoRenovacion = "Cancelada";
            await _context.SaveChangesAsync();

            // ── Log de auditoría ──────────────────────────────────────────
            await _auditoria.RegistrarAsync(
                accion: "Eliminar",
                modulo: "Renovaciones",
                descripcion: $"Se canceló renovación de '{nombreEstudiante}' — Año: {anio}.",
                entidadAfectada: nombreEstudiante,
                entidadId: id,
                ip: HttpContext.Connection.RemoteIpAddress?.ToString()
            );

            return Ok(new { mensaje = "Renovación de beca cancelada correctamente." });
        }
    }
}
