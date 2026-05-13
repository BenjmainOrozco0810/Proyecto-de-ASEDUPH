using ASEDUPH_V2_API.Data;
using ASEDUPH_V2_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASEDUPH_V2_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolicitudTiposApoyoController : ControllerBase
    {
        private readonly AseduphDbContext _context;

        public SolicitudTiposApoyoController(AseduphDbContext context)
        {
            _context = context;
        }

        // GET: api/SolicitudTiposApoyo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SolicitudTipoApoyo>>> GetSolicitudTiposApoyo()
        {
            var apoyos = await _context.SolicitudTiposApoyo
                .Include(sta => sta.SolicitudBeca)
                    .ThenInclude(s => s.Estudiante)
                .Include(sta => sta.TipoApoyo)
                .OrderBy(sta => sta.SolicitudBecaId)
                .ToListAsync();

            return Ok(apoyos);
        }

        // GET: api/SolicitudTiposApoyo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SolicitudTipoApoyo>> GetSolicitudTipoApoyo(int id)
        {
            var apoyo = await _context.SolicitudTiposApoyo
                .Include(sta => sta.SolicitudBeca)
                    .ThenInclude(s => s.Estudiante)
                .Include(sta => sta.TipoApoyo)
                .FirstOrDefaultAsync(sta => sta.SolicitudTipoApoyoId == id);

            if (apoyo == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el apoyo solicitado para la solicitud de beca."
                });
            }

            return Ok(apoyo);
        }

        // GET: api/SolicitudTiposApoyo/solicitud/5
        [HttpGet("solicitud/{solicitudBecaId}")]
        public async Task<ActionResult<IEnumerable<SolicitudTipoApoyo>>> GetApoyosPorSolicitud(int solicitudBecaId)
        {
            var existeSolicitud = await _context.SolicitudesBeca
                .AnyAsync(s => s.SolicitudBecaId == solicitudBecaId);

            if (!existeSolicitud)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró la solicitud de beca indicada."
                });
            }

            var apoyos = await _context.SolicitudTiposApoyo
                .Include(sta => sta.TipoApoyo)
                .Where(sta => sta.SolicitudBecaId == solicitudBecaId)
                .OrderBy(sta => sta.TipoApoyo!.Nombre)
                .ToListAsync();

            return Ok(apoyos);
        }

        // POST: api/SolicitudTiposApoyo
        [HttpPost]
        public async Task<ActionResult<SolicitudTipoApoyo>> PostSolicitudTipoApoyo(SolicitudTipoApoyo solicitudTipoApoyo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existeSolicitud = await _context.SolicitudesBeca
                .AnyAsync(s => s.SolicitudBecaId == solicitudTipoApoyo.SolicitudBecaId);

            if (!existeSolicitud)
            {
                return BadRequest(new
                {
                    mensaje = "La solicitud de beca indicada no existe."
                });
            }

            var existeTipoApoyo = await _context.TiposApoyo
                .AnyAsync(t => t.TipoApoyoId == solicitudTipoApoyo.TipoApoyoId && t.Estado == "Activo");

            if (!existeTipoApoyo)
            {
                return BadRequest(new
                {
                    mensaje = "El tipo de apoyo indicado no existe o está inactivo."
                });
            }

            var apoyoDuplicado = await _context.SolicitudTiposApoyo
                .AnyAsync(sta =>
                    sta.SolicitudBecaId == solicitudTipoApoyo.SolicitudBecaId &&
                    sta.TipoApoyoId == solicitudTipoApoyo.TipoApoyoId);

            if (apoyoDuplicado)
            {
                return BadRequest(new
                {
                    mensaje = "Este tipo de apoyo ya fue asignado a la solicitud de beca."
                });
            }

            _context.SolicitudTiposApoyo.Add(solicitudTipoApoyo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetSolicitudTipoApoyo),
                new { id = solicitudTipoApoyo.SolicitudTipoApoyoId },
                solicitudTipoApoyo
            );
        }

        // PUT: api/SolicitudTiposApoyo/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSolicitudTipoApoyo(int id, SolicitudTipoApoyo solicitudTipoApoyo)
        {
            if (id != solicitudTipoApoyo.SolicitudTipoApoyoId)
            {
                return BadRequest(new
                {
                    mensaje = "El ID enviado no coincide con el ID del apoyo de la solicitud."
                });
            }

            var apoyoExistente = await _context.SolicitudTiposApoyo
                .FirstOrDefaultAsync(sta => sta.SolicitudTipoApoyoId == id);

            if (apoyoExistente == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el apoyo que desea actualizar."
                });
            }

            var existeSolicitud = await _context.SolicitudesBeca
                .AnyAsync(s => s.SolicitudBecaId == solicitudTipoApoyo.SolicitudBecaId);

            if (!existeSolicitud)
            {
                return BadRequest(new
                {
                    mensaje = "La solicitud de beca indicada no existe."
                });
            }

            var existeTipoApoyo = await _context.TiposApoyo
                .AnyAsync(t => t.TipoApoyoId == solicitudTipoApoyo.TipoApoyoId && t.Estado == "Activo");

            if (!existeTipoApoyo)
            {
                return BadRequest(new
                {
                    mensaje = "El tipo de apoyo indicado no existe o está inactivo."
                });
            }

            var apoyoDuplicado = await _context.SolicitudTiposApoyo
                .AnyAsync(sta =>
                    sta.SolicitudBecaId == solicitudTipoApoyo.SolicitudBecaId &&
                    sta.TipoApoyoId == solicitudTipoApoyo.TipoApoyoId &&
                    sta.SolicitudTipoApoyoId != id);

            if (apoyoDuplicado)
            {
                return BadRequest(new
                {
                    mensaje = "Este tipo de apoyo ya está asignado a la solicitud de beca."
                });
            }

            apoyoExistente.SolicitudBecaId = solicitudTipoApoyo.SolicitudBecaId;
            apoyoExistente.TipoApoyoId = solicitudTipoApoyo.TipoApoyoId;
            apoyoExistente.DescripcionOtroApoyo = solicitudTipoApoyo.DescripcionOtroApoyo;
            apoyoExistente.MontoEstimado = solicitudTipoApoyo.MontoEstimado;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Apoyo de solicitud actualizado correctamente."
            });
        }

        // DELETE: api/SolicitudTiposApoyo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSolicitudTipoApoyo(int id)
        {
            var apoyo = await _context.SolicitudTiposApoyo
                .FirstOrDefaultAsync(sta => sta.SolicitudTipoApoyoId == id);

            if (apoyo == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el apoyo que desea eliminar de la solicitud."
                });
            }

            _context.SolicitudTiposApoyo.Remove(apoyo);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Apoyo eliminado de la solicitud correctamente."
            });
        }
    }
}