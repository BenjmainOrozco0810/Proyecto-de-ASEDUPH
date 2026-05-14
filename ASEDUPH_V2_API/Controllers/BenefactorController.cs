using ASEDUPH_V2_API.Data;
using ASEDUPH_V2_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASEDUPH_V2_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BenefactoresController : ControllerBase
    {
        private readonly AseduphDbContext _context;

        public BenefactoresController(AseduphDbContext context)
        {
            _context = context;
        }

        // GET: api/Benefactores
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Benefactor>>> GetBenefactores()
        {
            var benefactores = await _context.Benefactores
                .Where(b => b.Estado == "Activo")
                .OrderBy(b => b.NombreCompleto)
                .ToListAsync();

            return Ok(benefactores);
        }

        // GET: api/Benefactores/todos
        [HttpGet("todos")]
        public async Task<ActionResult<IEnumerable<Benefactor>>> GetTodosBenefactores()
        {
            var benefactores = await _context.Benefactores
                .OrderBy(b => b.NombreCompleto)
                .ToListAsync();

            return Ok(benefactores);
        }

        // GET: api/Benefactores/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Benefactor>> GetBenefactor(int id)
        {
            var benefactor = await _context.Benefactores
                .Include(b => b.Aportes)
                .FirstOrDefaultAsync(b => b.BenefactorId == id);

            if (benefactor == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el benefactor solicitado."
                });
            }

            return Ok(benefactor);
        }

        // POST: api/Benefactores
        [HttpPost]
        public async Task<ActionResult<Benefactor>> PostBenefactor(Benefactor benefactor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var validacion = ValidarBenefactor(benefactor);
            if (validacion != null)
            {
                return BadRequest(validacion);
            }

            benefactor.Estado = "Activo";
            benefactor.FechaRegistro = DateTime.Now;

            _context.Benefactores.Add(benefactor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetBenefactor),
                new { id = benefactor.BenefactorId },
                benefactor
            );
        }

        // PUT: api/Benefactores/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBenefactor(int id, Benefactor benefactor)
        {
            if (id != benefactor.BenefactorId)
            {
                return BadRequest(new
                {
                    mensaje = "El ID enviado no coincide con el ID del benefactor."
                });
            }

            var benefactorExistente = await _context.Benefactores
                .FirstOrDefaultAsync(b => b.BenefactorId == id);

            if (benefactorExistente == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el benefactor que desea actualizar."
                });
            }

            var validacion = ValidarBenefactor(benefactor);
            if (validacion != null)
            {
                return BadRequest(validacion);
            }

            benefactorExistente.NombreCompleto = benefactor.NombreCompleto;
            benefactorExistente.Telefono = benefactor.Telefono;
            benefactorExistente.CorreoElectronico = benefactor.CorreoElectronico;
            benefactorExistente.Direccion = benefactor.Direccion;
            benefactorExistente.TipoBenefactor = benefactor.TipoBenefactor;
            benefactorExistente.Estado = benefactor.Estado;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Benefactor actualizado correctamente."
            });
        }

        // DELETE: api/Benefactores/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBenefactor(int id)
        {
            var benefactor = await _context.Benefactores
                .FirstOrDefaultAsync(b => b.BenefactorId == id);

            if (benefactor == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el benefactor que desea desactivar."
                });
            }

            benefactor.Estado = "Inactivo";
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Benefactor desactivado correctamente."
            });
        }

        private object? ValidarBenefactor(Benefactor benefactor)
        {
            var tiposValidos = new List<string>
            {
                "Padrino", "Donante", "Voluntario", "Empresa", "Otro"
            };

            if (!string.IsNullOrWhiteSpace(benefactor.TipoBenefactor) &&
                !tiposValidos.Contains(benefactor.TipoBenefactor))
            {
                return new
                {
                    mensaje = "Tipo de benefactor no válido. Use: Padrino, Donante, Voluntario, Empresa u Otro."
                };
            }

            return null;
        }
    }
}