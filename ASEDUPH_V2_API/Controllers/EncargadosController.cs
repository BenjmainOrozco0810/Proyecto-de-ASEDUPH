using ASEDUPH_V2_API.Data;
using ASEDUPH_V2_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASEDUPH_V2_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EncargadosController : ControllerBase
    {
        private readonly AseduphDbContext _context;

        public EncargadosController(AseduphDbContext context)
        {
            _context = context;
        }

        // GET: api/Encargados
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Encargado>>> GetEncargados()
        {
            var encargados = await _context.Encargados
                .Include(e => e.Estudiante)
                .Where(e => e.Estado == "Activo")
                .OrderBy(e => e.NombreCompleto)
                .ToListAsync();

            return Ok(encargados);
        }

        // GET: api/Encargados/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Encargado>> GetEncargado(int id)
        {
            var encargado = await _context.Encargados
                .Include(e => e.Estudiante)
                .FirstOrDefaultAsync(e => e.EncargadoId == id);

            if (encargado == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el encargado solicitado."
                });
            }

            return Ok(encargado);
        }

        // GET: api/Encargados/estudiante/5
        [HttpGet("estudiante/{estudianteId}")]
        public async Task<ActionResult<IEnumerable<Encargado>>> GetEncargadosPorEstudiante(int estudianteId)
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

            var encargados = await _context.Encargados
                .Where(e => e.EstudianteId == estudianteId && e.Estado == "Activo")
                .OrderBy(e => e.NombreCompleto)
                .ToListAsync();

            return Ok(encargados);
        }

        // POST: api/Encargados
        [HttpPost]
        public async Task<ActionResult<Encargado>> PostEncargado(Encargado encargado)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existeEstudiante = await _context.Estudiantes
                .AnyAsync(e => e.EstudianteId == encargado.EstudianteId && e.Estado == "Activo");

            if (!existeEstudiante)
            {
                return BadRequest(new
                {
                    mensaje = "El estudiante indicado no existe o está inactivo."
                });
            }

            encargado.Estado = "Activo";

            _context.Encargados.Add(encargado);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEncargado),
                new { id = encargado.EncargadoId },
                encargado
            );
        }

        // PUT: api/Encargados/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEncargado(int id, Encargado encargado)
        {
            if (id != encargado.EncargadoId)
            {
                return BadRequest(new
                {
                    mensaje = "El ID enviado no coincide con el ID del encargado."
                });
            }

            var encargadoExistente = await _context.Encargados
                .FirstOrDefaultAsync(e => e.EncargadoId == id);

            if (encargadoExistente == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el encargado que desea actualizar."
                });
            }

            var existeEstudiante = await _context.Estudiantes
                .AnyAsync(e => e.EstudianteId == encargado.EstudianteId);

            if (!existeEstudiante)
            {
                return BadRequest(new
                {
                    mensaje = "El estudiante indicado no existe."
                });
            }

            encargadoExistente.EstudianteId = encargado.EstudianteId;
            encargadoExistente.NombreCompleto = encargado.NombreCompleto;
            encargadoExistente.Parentesco = encargado.Parentesco;
            encargadoExistente.EstadoCivil = encargado.EstadoCivil;
            encargadoExistente.DPI = encargado.DPI;
            encargadoExistente.DpiExtendido = encargado.DpiExtendido;
            encargadoExistente.TelefonoDomiciliar = encargado.TelefonoDomiciliar;
            encargadoExistente.TelefonoCelular = encargado.TelefonoCelular;
            encargadoExistente.CorreoElectronico = encargado.CorreoElectronico;
            encargadoExistente.Direccion = encargado.Direccion;
            encargadoExistente.Ocupacion = encargado.Ocupacion;
            encargadoExistente.LugarTrabajo = encargado.LugarTrabajo;
            encargadoExistente.TelefonoTrabajo = encargado.TelefonoTrabajo;
            encargadoExistente.Estado = encargado.Estado;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Encargado actualizado correctamente."
            });
        }

        // DELETE: api/Encargados/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEncargado(int id)
        {
            var encargado = await _context.Encargados
                .FirstOrDefaultAsync(e => e.EncargadoId == id);

            if (encargado == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el encargado que desea eliminar."
                });
            }

            encargado.Estado = "Inactivo";
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Encargado desactivado correctamente."
            });
        }
    }
}