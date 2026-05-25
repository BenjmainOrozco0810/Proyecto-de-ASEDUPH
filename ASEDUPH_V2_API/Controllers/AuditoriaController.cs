using ASEDUPH_V2_API.Data;
using ASEDUPH_V2_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASEDUPH_V2_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuditoriaController : ControllerBase
    {
        private readonly AseduphDbContext _context;

        public AuditoriaController(AseduphDbContext context)
        {
            _context = context;
        }

        // GET: api/Auditoria
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LogAuditoria>>> GetLogs(
            [FromQuery] string? modulo = null,
            [FromQuery] string? accion = null,
            [FromQuery] DateTime? fechaDesde = null,
            [FromQuery] DateTime? fechaHasta = null,
            [FromQuery] int pagina = 1,
            [FromQuery] int tamano = 50)
        {
            var query = _context.LogsAuditoria.AsQueryable();

            if (!string.IsNullOrEmpty(modulo))
                query = query.Where(l => l.Modulo == modulo);

            if (!string.IsNullOrEmpty(accion))
                query = query.Where(l => l.Accion == accion);

            if (fechaDesde.HasValue)
                query = query.Where(l => l.FechaHora >= fechaDesde.Value);

            if (fechaHasta.HasValue)
                query = query.Where(l => l.FechaHora <= fechaHasta.Value.AddDays(1));

            var total = await query.CountAsync();
            var logs = await query
                .OrderByDescending(l => l.FechaHora)
                .Skip((pagina - 1) * tamano)
                .Take(tamano)
                .ToListAsync();

            Response.Headers.Append("X-Total-Count", total.ToString());
            return Ok(logs);
        }

        // GET: api/Auditoria/modulos
        [HttpGet("modulos")]
        public async Task<ActionResult<IEnumerable<string>>> GetModulos()
        {
            var modulos = await _context.LogsAuditoria
                .Select(l => l.Modulo)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync();

            return Ok(modulos);
        }

        // DELETE: api/Auditoria/limpiar
        [HttpDelete("limpiar")]
        public async Task<IActionResult> LimpiarLogs([FromQuery] int diasAntiguedad = 90)
        {
            var fechaLimite = DateTime.Now.AddDays(-diasAntiguedad);
            var logsAEliminar = await _context.LogsAuditoria
                .Where(l => l.FechaHora < fechaLimite)
                .ToListAsync();

            _context.LogsAuditoria.RemoveRange(logsAEliminar);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = $"Se eliminaron {logsAEliminar.Count} registros de auditoría anteriores a {fechaLimite:dd/MM/yyyy}." });
        }
    }
}
