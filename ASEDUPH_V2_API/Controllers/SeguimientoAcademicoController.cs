using ASEDUPH_V2_API.Data;
using ASEDUPH_V2_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASEDUPH_V2_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeguimientoAcademicoController : ControllerBase
    {
        private readonly AseduphDbContext _context;

        public SeguimientoAcademicoController(AseduphDbContext context)
        {
            _context = context;
        }

        // GET: api/SeguimientoAcademico
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SeguimientoAcademico>>> GetSeguimientosAcademicos()
        {
            var seguimientos = await _context.SeguimientoAcademico
                .Include(s => s.Estudiante)
                .Include(s => s.Beca)
                .Include(s => s.CentroEducativo)
                .OrderByDescending(s => s.Anio)
                    .ThenBy(s => s.Estudiante!.NombreCompleto)
                .ToListAsync();

            return Ok(seguimientos);
        }

        // GET: api/SeguimientoAcademico/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SeguimientoAcademico>> GetSeguimientoAcademico(int id)
        {
            var seguimiento = await _context.SeguimientoAcademico
                .Include(s => s.Estudiante)
                .Include(s => s.Beca)
                .Include(s => s.CentroEducativo)
                .FirstOrDefaultAsync(s => s.SeguimientoAcademicoId == id);

            if (seguimiento == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el seguimiento académico solicitado."
                });
            }

            return Ok(seguimiento);
        }

        // GET: api/SeguimientoAcademico/estudiante/5
        [HttpGet("estudiante/{estudianteId}")]
        public async Task<ActionResult<IEnumerable<SeguimientoAcademico>>> GetSeguimientosPorEstudiante(int estudianteId)
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

            var seguimientos = await _context.SeguimientoAcademico
                .Include(s => s.Beca)
                .Include(s => s.CentroEducativo)
                .Where(s => s.EstudianteId == estudianteId)
                .OrderByDescending(s => s.Anio)
                .ToListAsync();

            return Ok(seguimientos);
        }

        // GET: api/SeguimientoAcademico/beca/5
        [HttpGet("beca/{becaId}")]
        public async Task<ActionResult<IEnumerable<SeguimientoAcademico>>> GetSeguimientosPorBeca(int becaId)
        {
            var existeBeca = await _context.Becas
                .AnyAsync(b => b.BecaId == becaId);

            if (!existeBeca)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró la beca indicada."
                });
            }

            var seguimientos = await _context.SeguimientoAcademico
                .Include(s => s.Estudiante)
                .Include(s => s.CentroEducativo)
                .Where(s => s.BecaId == becaId)
                .OrderByDescending(s => s.Anio)
                .ToListAsync();

            return Ok(seguimientos);
        }

        // POST: api/SeguimientoAcademico
        [HttpPost]
        public async Task<ActionResult<SeguimientoAcademico>> PostSeguimientoAcademico(SeguimientoAcademico seguimiento)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existeEstudiante = await _context.Estudiantes
                .AnyAsync(e => e.EstudianteId == seguimiento.EstudianteId && e.Estado == "Activo");

            if (!existeEstudiante)
            {
                return BadRequest(new
                {
                    mensaje = "El estudiante indicado no existe o está inactivo."
                });
            }

            if (seguimiento.BecaId != null)
            {
                var existeBeca = await _context.Becas
                    .AnyAsync(b => b.BecaId == seguimiento.BecaId);

                if (!existeBeca)
                {
                    return BadRequest(new
                    {
                        mensaje = "La beca indicada no existe."
                    });
                }
            }

            if (seguimiento.CentroEducativoId != null)
            {
                var existeCentro = await _context.CentrosEducativos
                    .AnyAsync(c => c.CentroEducativoId == seguimiento.CentroEducativoId && c.Estado == "Activo");

                if (!existeCentro)
                {
                    return BadRequest(new
                    {
                        mensaje = "El centro educativo indicado no existe o está inactivo."
                    });
                }
            }

            var validacion = ValidarSeguimiento(seguimiento);
            if (validacion != null)
            {
                return BadRequest(validacion);
            }

            seguimiento.FechaRegistro = DateTime.Now;

            _context.SeguimientoAcademico.Add(seguimiento);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetSeguimientoAcademico),
                new { id = seguimiento.SeguimientoAcademicoId },
                seguimiento
            );
        }

        // PUT: api/SeguimientoAcademico/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSeguimientoAcademico(int id, SeguimientoAcademico seguimiento)
        {
            if (id != seguimiento.SeguimientoAcademicoId)
            {
                return BadRequest(new
                {
                    mensaje = "El ID enviado no coincide con el ID del seguimiento académico."
                });
            }

            var seguimientoExistente = await _context.SeguimientoAcademico
                .FirstOrDefaultAsync(s => s.SeguimientoAcademicoId == id);

            if (seguimientoExistente == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el seguimiento académico que desea actualizar."
                });
            }

            var existeEstudiante = await _context.Estudiantes
                .AnyAsync(e => e.EstudianteId == seguimiento.EstudianteId);

            if (!existeEstudiante)
            {
                return BadRequest(new
                {
                    mensaje = "El estudiante indicado no existe."
                });
            }

            var validacion = ValidarSeguimiento(seguimiento);
            if (validacion != null)
            {
                return BadRequest(validacion);
            }

            seguimientoExistente.EstudianteId = seguimiento.EstudianteId;
            seguimientoExistente.BecaId = seguimiento.BecaId;
            seguimientoExistente.CentroEducativoId = seguimiento.CentroEducativoId;
            seguimientoExistente.Anio = seguimiento.Anio;
            seguimientoExistente.Grado = seguimiento.Grado;
            seguimientoExistente.NivelEducativo = seguimiento.NivelEducativo;
            seguimientoExistente.Promedio = seguimiento.Promedio;
            seguimientoExistente.EstadoAcademico = seguimiento.EstadoAcademico;
            seguimientoExistente.Observaciones = seguimiento.Observaciones;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Seguimiento académico actualizado correctamente."
            });
        }

        // DELETE: api/SeguimientoAcademico/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSeguimientoAcademico(int id)
        {
            var seguimiento = await _context.SeguimientoAcademico
                .FirstOrDefaultAsync(s => s.SeguimientoAcademicoId == id);

            if (seguimiento == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el seguimiento académico que desea eliminar."
                });
            }

            _context.SeguimientoAcademico.Remove(seguimiento);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Seguimiento académico eliminado correctamente."
            });
        }

        private object? ValidarSeguimiento(SeguimientoAcademico seguimiento)
        {
            if (seguimiento.Anio < 2000)
            {
                return new { mensaje = "El año no puede ser menor a 2000." };
            }

            if (seguimiento.Promedio < 0 || seguimiento.Promedio > 100)
            {
                return new { mensaje = "El promedio debe estar entre 0 y 100." };
            }

            var nivelesValidos = new List<string>
            {
                "Preprimaria", "Primaria", "Básicos", "Diversificado", "Universidad"
            };

            if (!string.IsNullOrWhiteSpace(seguimiento.NivelEducativo) &&
                !nivelesValidos.Contains(seguimiento.NivelEducativo))
            {
                return new
                {
                    mensaje = "Nivel educativo no válido. Use: Preprimaria, Primaria, Básicos, Diversificado o Universidad."
                };
            }

            var estadosValidos = new List<string>
            {
                "Aprobado", "Reprobado", "En Curso", "Retirado", "Finalizado"
            };

            if (!string.IsNullOrWhiteSpace(seguimiento.EstadoAcademico) &&
                !estadosValidos.Contains(seguimiento.EstadoAcademico))
            {
                return new
                {
                    mensaje = "Estado académico no válido. Use: Aprobado, Reprobado, En Curso, Retirado o Finalizado."
                };
            }

            return null;
        }
    }
}