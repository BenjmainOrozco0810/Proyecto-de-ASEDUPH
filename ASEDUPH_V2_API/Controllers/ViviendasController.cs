using ASEDUPH_V2_API.Data;
using ASEDUPH_V2_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASEDUPH_V2_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViviendasController : ControllerBase
    {
        private readonly AseduphDbContext _context;

        public ViviendasController(AseduphDbContext context)
        {
            _context = context;
        }

        // GET: api/Viviendas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vivienda>>> GetViviendas()
        {
            var viviendas = await _context.Viviendas
                .Include(v => v.VisitaFamiliar)
                    .ThenInclude(vf => vf.SolicitudBeca)
                        .ThenInclude(s => s.Estudiante)
                .ToListAsync();

            return Ok(viviendas);
        }

        // GET: api/Viviendas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Vivienda>> GetVivienda(int id)
        {
            var vivienda = await _context.Viviendas
                .Include(v => v.VisitaFamiliar)
                    .ThenInclude(vf => vf.SolicitudBeca)
                        .ThenInclude(s => s.Estudiante)
                .FirstOrDefaultAsync(v => v.ViviendaId == id);

            if (vivienda == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró la información de vivienda solicitada."
                });
            }

            return Ok(vivienda);
        }

        // GET: api/Viviendas/visita/5
        [HttpGet("visita/{visitaFamiliarId}")]
        public async Task<ActionResult<Vivienda>> GetViviendaPorVisita(int visitaFamiliarId)
        {
            var vivienda = await _context.Viviendas
                .Include(v => v.VisitaFamiliar)
                    .ThenInclude(vf => vf.SolicitudBeca)
                        .ThenInclude(s => s.Estudiante)
                .FirstOrDefaultAsync(v => v.VisitaFamiliarId == visitaFamiliarId);

            if (vivienda == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró información de vivienda para esta visita familiar."
                });
            }

            return Ok(vivienda);
        }

        // POST: api/Viviendas
        [HttpPost]
        public async Task<ActionResult<Vivienda>> PostVivienda(Vivienda vivienda)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existeVisita = await _context.VisitasFamiliares
                .AnyAsync(v => v.VisitaFamiliarId == vivienda.VisitaFamiliarId);

            if (!existeVisita)
            {
                return BadRequest(new
                {
                    mensaje = "La visita familiar indicada no existe."
                });
            }

            var yaExisteVivienda = await _context.Viviendas
                .AnyAsync(v => v.VisitaFamiliarId == vivienda.VisitaFamiliarId);

            if (yaExisteVivienda)
            {
                return BadRequest(new
                {
                    mensaje = "Esta visita familiar ya tiene información de vivienda registrada."
                });
            }

            var validacion = ValidarVivienda(vivienda);
            if (validacion != null)
            {
                return BadRequest(validacion);
            }

            _context.Viviendas.Add(vivienda);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetVivienda),
                new { id = vivienda.ViviendaId },
                vivienda
            );
        }

        // PUT: api/Viviendas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVivienda(int id, Vivienda vivienda)
        {
            if (id != vivienda.ViviendaId)
            {
                return BadRequest(new
                {
                    mensaje = "El ID enviado no coincide con el ID de vivienda."
                });
            }

            var viviendaExistente = await _context.Viviendas
                .FirstOrDefaultAsync(v => v.ViviendaId == id);

            if (viviendaExistente == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró la vivienda que desea actualizar."
                });
            }

            var existeVisita = await _context.VisitasFamiliares
                .AnyAsync(v => v.VisitaFamiliarId == vivienda.VisitaFamiliarId);

            if (!existeVisita)
            {
                return BadRequest(new
                {
                    mensaje = "La visita familiar indicada no existe."
                });
            }

            var viviendaDuplicada = await _context.Viviendas
                .AnyAsync(v =>
                    v.VisitaFamiliarId == vivienda.VisitaFamiliarId &&
                    v.ViviendaId != id);

            if (viviendaDuplicada)
            {
                return BadRequest(new
                {
                    mensaje = "Ya existe otra vivienda registrada para esta visita familiar."
                });
            }

            var validacion = ValidarVivienda(vivienda);
            if (validacion != null)
            {
                return BadRequest(validacion);
            }

            viviendaExistente.VisitaFamiliarId = vivienda.VisitaFamiliarId;
            viviendaExistente.TipoVivienda = vivienda.TipoVivienda;
            viviendaExistente.PagoMensual = vivienda.PagoMensual;
            viviendaExistente.AmortizacionMensual = vivienda.AmortizacionMensual;
            viviendaExistente.MostroRecibo = vivienda.MostroRecibo;
            viviendaExistente.DescripcionVivienda = vivienda.DescripcionVivienda;
            viviendaExistente.Observaciones = vivienda.Observaciones;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Información de vivienda actualizada correctamente."
            });
        }

        // DELETE: api/Viviendas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVivienda(int id)
        {
            var vivienda = await _context.Viviendas
                .FirstOrDefaultAsync(v => v.ViviendaId == id);

            if (vivienda == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró la vivienda que desea eliminar."
                });
            }

            _context.Viviendas.Remove(vivienda);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Información de vivienda eliminada correctamente."
            });
        }

        private object? ValidarVivienda(Vivienda vivienda)
        {
            var tiposValidos = new List<string>
            {
                "Propia",
                "Alquilada",
                "Familiar",
                "Prestada",
                "Otro"
            };

            if (!string.IsNullOrWhiteSpace(vivienda.TipoVivienda) &&
                !tiposValidos.Contains(vivienda.TipoVivienda))
            {
                return new
                {
                    mensaje = "Tipo de vivienda no válido. Use: Propia, Alquilada, Familiar, Prestada u Otro."
                };
            }

            if (vivienda.PagoMensual < 0)
            {
                return new
                {
                    mensaje = "El pago mensual no puede ser negativo."
                };
            }

            if (vivienda.AmortizacionMensual < 0)
            {
                return new
                {
                    mensaje = "La amortización mensual no puede ser negativa."
                };
            }

            return null;
        }
    }
}