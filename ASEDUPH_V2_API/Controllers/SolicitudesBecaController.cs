using ASEDUPH_V2_API.Data;
using ASEDUPH_V2_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ASEDUPH_V2_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolicitudesBecaController : ControllerBase
    {
        private readonly AseduphDbContext _context;

        public SolicitudesBecaController(AseduphDbContext context)
        {
            _context = context;
        }

        // GET: api/SolicitudesBeca
        [HttpGet]
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

        // GET: api/SolicitudesBeca/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SolicitudBeca>> GetSolicitudBeca(int id)
        {
            var solicitud = await _context.SolicitudesBeca
                .Include(s => s.Estudiante)
                .Include(s => s.CentroEducativo)
                .Include(s => s.SolicitudTiposApoyo)
                    .ThenInclude(sta => sta.TipoApoyo)
                .FirstOrDefaultAsync(s => s.SolicitudBecaId == id);

            if (solicitud == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró la solicitud de beca."
                });
            }

            return Ok(solicitud);
        }

        // GET: api/SolicitudesBeca/estudiante/5
        [HttpGet("estudiante/{estudianteId}")]
        public async Task<ActionResult<IEnumerable<SolicitudBeca>>> GetSolicitudesPorEstudiante(int estudianteId)
        {
            var existeEstudiante = await _context.Estudiantes
                .AnyAsync(e => e.EstudianteId == estudianteId);

            if (!existeEstudiante)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el estudiante indicado."
                });
            }

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
        public async Task<ActionResult<SolicitudBeca>> PostSolicitudBeca(SolicitudBeca solicitud)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existeEstudiante = await _context.Estudiantes
                .AnyAsync(e => e.EstudianteId == solicitud.EstudianteId && e.Estado == "Activo");

            if (!existeEstudiante)
            {
                return BadRequest(new
                {
                    mensaje = "El estudiante indicado no existe o está inactivo."
                });
            }

            if (solicitud.CentroEducativoId != null)
            {
                var existeCentro = await _context.CentrosEducativos
                    .AnyAsync(c => c.CentroEducativoId == solicitud.CentroEducativoId && c.Estado == "Activo");

                if (!existeCentro)
                {
                    return BadRequest(new
                    {
                        mensaje = "El centro educativo indicado no existe o está inactivo."
                    });
                }
            }

            solicitud.EstadoSolicitud = "Pendiente";

            _context.SolicitudesBeca.Add(solicitud);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetSolicitudBeca),
                new { id = solicitud.SolicitudBecaId },
                solicitud
            );
        }
        // POST: api/SolicitudesBeca/publica
        // Endpoint público - no requiere autenticación
        [HttpPost("publica")]
        [AllowAnonymous]
        public async Task<ActionResult<SolicitudBeca>> PostSolicitudPublica(SolicitudBeca solicitud)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verificar o crear estudiante
            var existeEstudiante = await _context.Estudiantes
                .AnyAsync(e => e.EstudianteId == solicitud.EstudianteId && e.Estado == "Activo");

            if (!existeEstudiante)
            {
                return BadRequest(new
                {
                    mensaje = "El estudiante indicado no existe o está inactivo."
                });
            }

            solicitud.EstadoSolicitud = "Pendiente";
            solicitud.FechaSolicitud = DateTime.Now;

            _context.SolicitudesBeca.Add(solicitud);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Solicitud enviada correctamente. Nos pondremos en contacto con usted pronto.",
                solicitudId = solicitud.SolicitudBecaId
            });
        }

        // PUT: api/SolicitudesBeca/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSolicitudBeca(int id, SolicitudBeca solicitud)
        {
            if (id != solicitud.SolicitudBecaId)
            {
                return BadRequest(new
                {
                    mensaje = "El ID enviado no coincide con el ID de la solicitud."
                });
            }

            var solicitudExistente = await _context.SolicitudesBeca
                .FirstOrDefaultAsync(s => s.SolicitudBecaId == id);

            if (solicitudExistente == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró la solicitud que desea actualizar."
                });
            }

            var existeEstudiante = await _context.Estudiantes
                .AnyAsync(e => e.EstudianteId == solicitud.EstudianteId);

            if (!existeEstudiante)
            {
                return BadRequest(new
                {
                    mensaje = "El estudiante indicado no existe."
                });
            }

            if (solicitud.CentroEducativoId != null)
            {
                var existeCentro = await _context.CentrosEducativos
                    .AnyAsync(c => c.CentroEducativoId == solicitud.CentroEducativoId);

                if (!existeCentro)
                {
                    return BadRequest(new
                    {
                        mensaje = "El centro educativo indicado no existe."
                    });
                }
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

            return Ok(new
            {
                mensaje = "Solicitud de beca actualizada correctamente."
            });
        }

        // PATCH: api/SolicitudesBeca/5/estado
        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> CambiarEstadoSolicitud(int id, [FromBody] string nuevoEstado)
        {
            var solicitud = await _context.SolicitudesBeca
                .FirstOrDefaultAsync(s => s.SolicitudBecaId == id);

            if (solicitud == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró la solicitud de beca."
                });
            }

            var estadosValidos = new List<string>
            {
                "Pendiente",
                "En Evaluación",
                "Aprobada",
                "Rechazada",
                "Cancelada"
            };

            if (!estadosValidos.Contains(nuevoEstado))
            {
                return BadRequest(new
                {
                    mensaje = "Estado no válido. Use: Pendiente, En Evaluación, Aprobada, Rechazada o Cancelada."
                });
            }

            solicitud.EstadoSolicitud = nuevoEstado;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Estado de solicitud actualizado correctamente."
            });
        }

        // DELETE: api/SolicitudesBeca/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSolicitudBeca(int id)
        {
            var solicitud = await _context.SolicitudesBeca
                .FirstOrDefaultAsync(s => s.SolicitudBecaId == id);

            if (solicitud == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró la solicitud que desea cancelar."
                });
            }

            solicitud.EstadoSolicitud = "Cancelada";
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Solicitud de beca cancelada correctamente."
            });
        }
    }
}