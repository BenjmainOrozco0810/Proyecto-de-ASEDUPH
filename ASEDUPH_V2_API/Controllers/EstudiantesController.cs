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
    public class EstudiantesController : ControllerBase
    {
        private readonly AseduphDbContext _context;
        private readonly AuditoriaService _auditoria;

        public EstudiantesController(AseduphDbContext context, AuditoriaService auditoria)
        {
            _context = context;
            _auditoria = auditoria;
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

        // GET: api/Estudiantes/todos
        [HttpGet("todos")]
        public async Task<ActionResult<IEnumerable<Estudiante>>> GetTodosEstudiantes()
        {
            var estudiantes = await _context.Estudiantes
                .OrderBy(e => e.NombreCompleto)
                .ToListAsync();

            return Ok(estudiantes);
        }

        // GET: api/Estudiantes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Estudiante>> GetEstudiante(int id)
        {
            var estudiante = await _context.Estudiantes.FindAsync(id);

            if (estudiante == null)
                return NotFound(new { mensaje = "Estudiante no encontrado." });

            return Ok(estudiante);
        }

        // POST: api/Estudiantes
        [HttpPost]
        public async Task<ActionResult<Estudiante>> PostEstudiante(Estudiante estudiante)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            estudiante.Estado = "Activo";
            estudiante.FechaRegistro = DateTime.Now;

            _context.Estudiantes.Add(estudiante);
            await _context.SaveChangesAsync();

            // ── Log de auditoría ──────────────────────────────────────────
            await _auditoria.RegistrarAsync(
                accion: "Crear",
                modulo: "Estudiantes",
                descripcion: $"Se creó el estudiante '{estudiante.NombreCompleto}'.",
                entidadAfectada: estudiante.NombreCompleto,
                entidadId: estudiante.EstudianteId,
                ip: HttpContext.Connection.RemoteIpAddress?.ToString()
            );

            return CreatedAtAction(
                nameof(GetEstudiante),
                new { id = estudiante.EstudianteId },
                estudiante
            );
        }

        // POST: api/Estudiantes/publico (sin autenticación)
        [HttpPost("publico")]
        [AllowAnonymous]
        public async Task<ActionResult> PostEstudiantePublico(Estudiante estudiante)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            estudiante.Estado = "Activo";
            estudiante.FechaRegistro = DateTime.Now;

            _context.Estudiantes.Add(estudiante);
            await _context.SaveChangesAsync();

            await _auditoria.RegistrarAsync(
                accion: "Crear",
                modulo: "Estudiantes",
                descripcion: $"Registro público: estudiante '{estudiante.NombreCompleto}' registrado desde formulario público.",
                entidadAfectada: estudiante.NombreCompleto,
                entidadId: estudiante.EstudianteId,
                ip: HttpContext.Connection.RemoteIpAddress?.ToString()
            );

            return Ok(new
            {
                mensaje = "Estudiante registrado correctamente.",
                estudianteId = estudiante.EstudianteId
            });
        }

        // PUT: api/Estudiantes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEstudiante(int id, Estudiante estudiante)
        {
            if (id != estudiante.EstudianteId)
                return BadRequest(new { mensaje = "El ID no coincide." });

            var estudianteExistente = await _context.Estudiantes.FindAsync(id);
            if (estudianteExistente == null)
                return NotFound(new { mensaje = "Estudiante no encontrado." });

            estudianteExistente.NombreCompleto = estudiante.NombreCompleto;
            estudianteExistente.Sexo = estudiante.Sexo;
            estudianteExistente.FechaNacimiento = estudiante.FechaNacimiento;
            estudianteExistente.Edad = estudiante.Edad;
            estudianteExistente.Direccion = estudiante.Direccion;
            estudianteExistente.Municipio = estudiante.Municipio;
            estudianteExistente.Departamento = estudiante.Departamento;
            estudianteExistente.Estado = estudiante.Estado;

            await _context.SaveChangesAsync();

            // ── Log de auditoría ──────────────────────────────────────────
            await _auditoria.RegistrarAsync(
                accion: "Editar",
                modulo: "Estudiantes",
                descripcion: $"Se actualizó el estudiante '{estudianteExistente.NombreCompleto}'.",
                entidadAfectada: estudianteExistente.NombreCompleto,
                entidadId: id,
                ip: HttpContext.Connection.RemoteIpAddress?.ToString()
            );

            return Ok(new { mensaje = "Estudiante actualizado correctamente." });
        }

        // DELETE: api/Estudiantes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEstudiante(int id)
        {
            var estudiante = await _context.Estudiantes.FindAsync(id);

            if (estudiante == null)
                return NotFound(new { mensaje = "Estudiante no encontrado." });

            estudiante.Estado = "Inactivo";
            await _context.SaveChangesAsync();

            // ── Log de auditoría ──────────────────────────────────────────
            await _auditoria.RegistrarAsync(
                accion: "Eliminar",
                modulo: "Estudiantes",
                descripcion: $"Se desactivó el estudiante '{estudiante.NombreCompleto}'.",
                entidadAfectada: estudiante.NombreCompleto,
                entidadId: id,
                ip: HttpContext.Connection.RemoteIpAddress?.ToString()
            );

            return Ok(new { mensaje = "Estudiante desactivado correctamente." });
        }
    }
}
