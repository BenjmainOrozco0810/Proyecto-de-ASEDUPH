using ASEDUPH_V2_API.Data;
using ASEDUPH_V2_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASEDUPH_V2_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AportesController : ControllerBase
    {
        private readonly AseduphDbContext _context;

        public AportesController(AseduphDbContext context)
        {
            _context = context;
        }

        // GET: api/Aportes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Aporte>>> GetAportes()
        {
            var aportes = await _context.Aportes
                .Include(a => a.Benefactor)
                .Include(a => a.Beca)
                    .ThenInclude(b => b!.Estudiante)
                .OrderByDescending(a => a.FechaAporte)
                .ToListAsync();

            return Ok(aportes);
        }

        // GET: api/Aportes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Aporte>> GetAporte(int id)
        {
            var aporte = await _context.Aportes
                .Include(a => a.Benefactor)
                .Include(a => a.Beca)
                    .ThenInclude(b => b!.Estudiante)
                .FirstOrDefaultAsync(a => a.AporteId == id);

            if (aporte == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el aporte solicitado."
                });
            }

            return Ok(aporte);
        }

        // GET: api/Aportes/benefactor/5
        [HttpGet("benefactor/{benefactorId}")]
        public async Task<ActionResult<IEnumerable<Aporte>>> GetAportesPorBenefactor(int benefactorId)
        {
            var existeBenefactor = await _context.Benefactores
                .AnyAsync(b => b.BenefactorId == benefactorId);

            if (!existeBenefactor)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el benefactor indicado."
                });
            }

            var aportes = await _context.Aportes
                .Include(a => a.Beca)
                    .ThenInclude(b => b!.Estudiante)
                .Where(a => a.BenefactorId == benefactorId)
                .OrderByDescending(a => a.FechaAporte)
                .ToListAsync();

            return Ok(aportes);
        }

        // GET: api/Aportes/beca/5
        [HttpGet("beca/{becaId}")]
        public async Task<ActionResult<IEnumerable<Aporte>>> GetAportesPorBeca(int becaId)
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

            var aportes = await _context.Aportes
                .Include(a => a.Benefactor)
                .Where(a => a.BecaId == becaId)
                .OrderByDescending(a => a.FechaAporte)
                .ToListAsync();

            return Ok(aportes);
        }

        // GET: api/Aportes/periodo?inicio=2025-01-01&fin=2025-12-31
        [HttpGet("periodo")]
        public async Task<ActionResult<IEnumerable<Aporte>>> GetAportesPorPeriodo(
            [FromQuery] DateTime inicio,
            [FromQuery] DateTime fin)
        {
            if (fin < inicio)
            {
                return BadRequest(new
                {
                    mensaje = "La fecha de fin no puede ser menor a la fecha de inicio."
                });
            }

            var aportes = await _context.Aportes
                .Include(a => a.Benefactor)
                .Include(a => a.Beca)
                    .ThenInclude(b => b!.Estudiante)
                .Where(a => a.FechaAporte >= inicio && a.FechaAporte <= fin)
                .OrderByDescending(a => a.FechaAporte)
                .ToListAsync();

            return Ok(aportes);
        }

        // POST: api/Aportes
        [HttpPost]
        public async Task<ActionResult<Aporte>> PostAporte(Aporte aporte)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existeBenefactor = await _context.Benefactores
                .AnyAsync(b => b.BenefactorId == aporte.BenefactorId && b.Estado == "Activo");

            if (!existeBenefactor)
            {
                return BadRequest(new
                {
                    mensaje = "El benefactor indicado no existe o está inactivo."
                });
            }

            if (aporte.BecaId != null)
            {
                var existeBeca = await _context.Becas
                    .AnyAsync(b => b.BecaId == aporte.BecaId);

                if (!existeBeca)
                {
                    return BadRequest(new
                    {
                        mensaje = "La beca indicada no existe."
                    });
                }
            }

            var validacion = ValidarAporte(aporte);
            if (validacion != null)
            {
                return BadRequest(validacion);
            }

            _context.Aportes.Add(aporte);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetAporte),
                new { id = aporte.AporteId },
                aporte
            );
        }

        // PUT: api/Aportes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAporte(int id, Aporte aporte)
        {
            if (id != aporte.AporteId)
            {
                return BadRequest(new
                {
                    mensaje = "El ID enviado no coincide con el ID del aporte."
                });
            }

            var aporteExistente = await _context.Aportes
                .FirstOrDefaultAsync(a => a.AporteId == id);

            if (aporteExistente == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el aporte que desea actualizar."
                });
            }

            var existeBenefactor = await _context.Benefactores
                .AnyAsync(b => b.BenefactorId == aporte.BenefactorId);

            if (!existeBenefactor)
            {
                return BadRequest(new
                {
                    mensaje = "El benefactor indicado no existe."
                });
            }

            var validacion = ValidarAporte(aporte);
            if (validacion != null)
            {
                return BadRequest(validacion);
            }

            aporteExistente.BenefactorId = aporte.BenefactorId;
            aporteExistente.BecaId = aporte.BecaId;
            aporteExistente.FechaAporte = aporte.FechaAporte;
            aporteExistente.Monto = aporte.Monto;
            aporteExistente.TipoAporte = aporte.TipoAporte;
            aporteExistente.FormaPago = aporte.FormaPago;
            aporteExistente.Periodo = aporte.Periodo;
            aporteExistente.NumeroComprobante = aporte.NumeroComprobante;
            aporteExistente.Observaciones = aporte.Observaciones;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Aporte actualizado correctamente."
            });
        }

        // DELETE: api/Aportes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAporte(int id)
        {
            var aporte = await _context.Aportes
                .FirstOrDefaultAsync(a => a.AporteId == id);

            if (aporte == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el aporte que desea eliminar."
                });
            }

            _context.Aportes.Remove(aporte);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Aporte eliminado correctamente."
            });
        }

        private object? ValidarAporte(Aporte aporte)
        {
            if (aporte.Monto <= 0)
            {
                return new { mensaje = "El monto del aporte debe ser mayor a 0." };
            }

            var tiposValidos = new List<string>
            {
                "Económico", "Útiles", "Uniformes", "Zapatos", "Alimentos", "Otro"
            };

            if (!string.IsNullOrWhiteSpace(aporte.TipoAporte) &&
                !tiposValidos.Contains(aporte.TipoAporte))
            {
                return new
                {
                    mensaje = "Tipo de aporte no válido. Use: Económico, Útiles, Uniformes, Zapatos, Alimentos u Otro."
                };
            }

            var formasPagoValidas = new List<string>
            {
                "Efectivo", "Transferencia", "Depósito", "Cheque", "Otro"
            };

            if (!string.IsNullOrWhiteSpace(aporte.FormaPago) &&
                !formasPagoValidas.Contains(aporte.FormaPago))
            {
                return new
                {
                    mensaje = "Forma de pago no válida. Use: Efectivo, Transferencia, Depósito, Cheque u Otro."
                };
            }

            var periodosValidos = new List<string>
            {
                "Mensual", "Semestral", "Anual", "Único", "Otro"
            };

            if (!string.IsNullOrWhiteSpace(aporte.Periodo) &&
                !periodosValidos.Contains(aporte.Periodo))
            {
                return new
                {
                    mensaje = "Período no válido. Use: Mensual, Semestral, Anual, Único u Otro."
                };
            }

            return null;
        }
    }
}