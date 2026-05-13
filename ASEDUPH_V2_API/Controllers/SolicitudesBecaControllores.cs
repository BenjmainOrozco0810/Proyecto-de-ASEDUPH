using ASEDUPH_V2_API.Data;
using ASEDUPH_V2_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASEDUPH_V2_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EvaluacionesBecaController : ControllerBase
    {
        private readonly AseduphDbContext _context;

        public EvaluacionesBecaController(AseduphDbContext context)
        {
            _context = context;
        }

        // GET: api/EvaluacionesBeca
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EvaluacionBeca>>> GetEvaluacionesBeca()
        {
            var evaluaciones = await _context.EvaluacionesBeca
                .Include(e => e.SolicitudBeca)
                    .ThenInclude(s => s.Estudiante)
                .OrderByDescending(e => e.FechaDecision)
                .ToListAsync();

            return Ok(evaluaciones);
        }

        // GET: api/EvaluacionesBeca/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EvaluacionBeca>> GetEvaluacionBeca(int id)
        {
            var evaluacion = await _context.EvaluacionesBeca
                .Include(e => e.SolicitudBeca)
                    .ThenInclude(s => s.Estudiante)
                .FirstOrDefaultAsync(e => e.EvaluacionBecaId == id);

            if (evaluacion == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró la evaluación de beca solicitada."
                });
            }

            return Ok(evaluacion);
        }

        // GET: api/EvaluacionesBeca/solicitud/5
        [HttpGet("solicitud/{solicitudBecaId}")]
        public async Task<ActionResult<EvaluacionBeca>> GetEvaluacionPorSolicitud(int solicitudBecaId)
        {
            var evaluacion = await _context.EvaluacionesBeca
                .Include(e => e.SolicitudBeca)
                    .ThenInclude(s => s.Estudiante)
                .FirstOrDefaultAsync(e => e.SolicitudBecaId == solicitudBecaId);

            if (evaluacion == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró evaluación para esta solicitud de beca."
                });
            }

            return Ok(evaluacion);
        }

        // POST: api/EvaluacionesBeca
        [HttpPost]
        public async Task<ActionResult<EvaluacionBeca>> PostEvaluacionBeca(EvaluacionBeca evaluacion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var solicitud = await _context.SolicitudesBeca
                .FirstOrDefaultAsync(s => s.SolicitudBecaId == evaluacion.SolicitudBecaId);

            if (solicitud == null)
            {
                return BadRequest(new
                {
                    mensaje = "La solicitud de beca indicada no existe."
                });
            }

            var yaExisteEvaluacion = await _context.EvaluacionesBeca
                .AnyAsync(e => e.SolicitudBecaId == evaluacion.SolicitudBecaId);

            if (yaExisteEvaluacion)
            {
                return BadRequest(new
                {
                    mensaje = "Esta solicitud ya tiene una evaluación registrada."
                });
            }

            var validacion = ValidarEvaluacion(evaluacion);
            if (validacion != null)
            {
                return BadRequest(validacion);
            }

            _context.EvaluacionesBeca.Add(evaluacion);

            if (!string.IsNullOrWhiteSpace(evaluacion.DecisionFinal))
            {
                solicitud.EstadoSolicitud = evaluacion.DecisionFinal switch
                {
                    "Aprobada" => "Aprobada",
                    "Rechazada" => "Rechazada",
                    "Pendiente" => "Pendiente",
                    "En Análisis" => "En Evaluación",
                    _ => solicitud.EstadoSolicitud
                };
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEvaluacionBeca),
                new { id = evaluacion.EvaluacionBecaId },
                evaluacion
            );
        }

        // PUT: api/EvaluacionesBeca/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEvaluacionBeca(int id, EvaluacionBeca evaluacion)
        {
            if (id != evaluacion.EvaluacionBecaId)
            {
                return BadRequest(new
                {
                    mensaje = "El ID enviado no coincide con el ID de la evaluación."
                });
            }

            var evaluacionExistente = await _context.EvaluacionesBeca
                .FirstOrDefaultAsync(e => e.EvaluacionBecaId == id);

            if (evaluacionExistente == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró la evaluación que desea actualizar."
                });
            }

            var solicitud = await _context.SolicitudesBeca
                .FirstOrDefaultAsync(s => s.SolicitudBecaId == evaluacion.SolicitudBecaId);

            if (solicitud == null)
            {
                return BadRequest(new
                {
                    mensaje = "La solicitud de beca indicada no existe."
                });
            }

            var evaluacionDuplicada = await _context.EvaluacionesBeca
                .AnyAsync(e =>
                    e.SolicitudBecaId == evaluacion.SolicitudBecaId &&
                    e.EvaluacionBecaId != id);

            if (evaluacionDuplicada)
            {
                return BadRequest(new
                {
                    mensaje = "Ya existe otra evaluación registrada para esta solicitud."
                });
            }

            var validacion = ValidarEvaluacion(evaluacion);
            if (validacion != null)
            {
                return BadRequest(validacion);
            }

            evaluacionExistente.SolicitudBecaId = evaluacion.SolicitudBecaId;
            evaluacionExistente.RecomendacionesResponsable = evaluacion.RecomendacionesResponsable;
            evaluacionExistente.ClasificacionOtorgada = evaluacion.ClasificacionOtorgada;
            evaluacionExistente.DecisionFinal = evaluacion.DecisionFinal;
            evaluacionExistente.FechaDecision = evaluacion.FechaDecision;
            evaluacionExistente.ObservacionesGenerales = evaluacion.ObservacionesGenerales;
            evaluacionExistente.EvaluadoPor = evaluacion.EvaluadoPor;

            if (!string.IsNullOrWhiteSpace(evaluacion.DecisionFinal))
            {
                solicitud.EstadoSolicitud = evaluacion.DecisionFinal switch
                {
                    "Aprobada" => "Aprobada",
                    "Rechazada" => "Rechazada",
                    "Pendiente" => "Pendiente",
                    "En Análisis" => "En Evaluación",
                    _ => solicitud.EstadoSolicitud
                };
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Evaluación de beca actualizada correctamente."
            });
        }

        // PATCH: api/EvaluacionesBeca/5/decision
        [HttpPatch("{id}/decision")]
        public async Task<IActionResult> CambiarDecisionFinal(int id, [FromBody] string decisionFinal)
        {
            var evaluacion = await _context.EvaluacionesBeca
                .FirstOrDefaultAsync(e => e.EvaluacionBecaId == id);

            if (evaluacion == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró la evaluación de beca."
                });
            }

            var estadosValidos = new List<string>
            {
                "Aprobada",
                "Rechazada",
                "Pendiente",
                "En Análisis"
            };

            if (!estadosValidos.Contains(decisionFinal))
            {
                return BadRequest(new
                {
                    mensaje = "Decisión no válida. Use: Aprobada, Rechazada, Pendiente o En Análisis."
                });
            }

            evaluacion.DecisionFinal = decisionFinal;
            evaluacion.FechaDecision = DateTime.Now;

            var solicitud = await _context.SolicitudesBeca
                .FirstOrDefaultAsync(s => s.SolicitudBecaId == evaluacion.SolicitudBecaId);

            if (solicitud != null)
            {
                solicitud.EstadoSolicitud = decisionFinal switch
                {
                    "Aprobada" => "Aprobada",
                    "Rechazada" => "Rechazada",
                    "Pendiente" => "Pendiente",
                    "En Análisis" => "En Evaluación",
                    _ => solicitud.EstadoSolicitud
                };
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Decisión final actualizada correctamente."
            });
        }

        // DELETE: api/EvaluacionesBeca/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvaluacionBeca(int id)
        {
            var evaluacion = await _context.EvaluacionesBeca
                .FirstOrDefaultAsync(e => e.EvaluacionBecaId == id);

            if (evaluacion == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró la evaluación que desea eliminar."
                });
            }

            _context.EvaluacionesBeca.Remove(evaluacion);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Evaluación de beca eliminada correctamente."
            });
        }

        private object? ValidarEvaluacion(EvaluacionBeca evaluacion)
        {
            var decisionesValidas = new List<string>
            {
                "Aprobada",
                "Rechazada",
                "Pendiente",
                "En Análisis"
            };

            if (!string.IsNullOrWhiteSpace(evaluacion.DecisionFinal) &&
                !decisionesValidas.Contains(evaluacion.DecisionFinal))
            {
                return new
                {
                    mensaje = "Decisión final no válida. Use: Aprobada, Rechazada, Pendiente o En Análisis."
                };
            }

            return null;
        }
    }
}