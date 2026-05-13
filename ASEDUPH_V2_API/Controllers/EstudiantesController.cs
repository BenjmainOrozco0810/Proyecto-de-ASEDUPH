using ASEDUPH_V2_API.Data;
using ASEDUPH_V2_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASEDUPH_V2_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstudiantesController : ControllerBase
    {
        private readonly AseduphDbContext _context;

        public EstudiantesController(AseduphDbContext context)
        {
            _context = context;
        }

        // GET: api/Estudiantes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Estudiante>>> GetEstudiantes()
        {
            var estudiantes = await _context.Estudiantes
                .Where(e => e.Estado == "Activo")
                .OrderBy(e => e.NombreCompleto)
                .ToListAsync();

            return Ok(estudiantes);
        }

        // GET: api/Estudiantes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Estudiante>> GetEstudiante(int id)
        {
            var estudiante = await _context.Estudiantes
                .Include(e => e.Encargados)
                .Include(e => e.SolicitudesBeca)
                .FirstOrDefaultAsync(e => e.EstudianteId == id);

            if (estudiante == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el estudiante solicitado."
                });
            }

            return Ok(estudiante);
        }

        // POST: api/Estudiantes
        [HttpPost]
        public async Task<ActionResult<Estudiante>> PostEstudiante(Estudiante estudiante)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            estudiante.Estado = "Activo";
            estudiante.FechaRegistro = DateTime.Now;

            _context.Estudiantes.Add(estudiante);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEstudiante),
                new { id = estudiante.EstudianteId },
                estudiante
            );
        }

        // PUT: api/Estudiantes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEstudiante(int id, Estudiante estudiante)
        {
            if (id != estudiante.EstudianteId)
            {
                return BadRequest(new
                {
                    mensaje = "El ID enviado no coincide con el ID del estudiante."
                });
            }

            var estudianteExistente = await _context.Estudiantes
                .FirstOrDefaultAsync(e => e.EstudianteId == id);

            if (estudianteExistente == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el estudiante que desea actualizar."
                });
            }

            estudianteExistente.NombreCompleto = estudiante.NombreCompleto;
            estudianteExistente.Sexo = estudiante.Sexo;
            estudianteExistente.FechaNacimiento = estudiante.FechaNacimiento;
            estudianteExistente.Edad = estudiante.Edad;
            estudianteExistente.Direccion = estudiante.Direccion;
            estudianteExistente.Municipio = estudiante.Municipio;
            estudianteExistente.Departamento = estudiante.Departamento;
            estudianteExistente.Estado = estudiante.Estado;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Estudiante actualizado correctamente."
            });
        }

        // DELETE: api/Estudiantes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEstudiante(int id)
        {
            var estudiante = await _context.Estudiantes
                .FirstOrDefaultAsync(e => e.EstudianteId == id);

            if (estudiante == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el estudiante que desea eliminar."
                });
            }

            estudiante.Estado = "Inactivo";
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Estudiante desactivado correctamente."
            });
        }
    }
}