using ASEDUPH_V2_API.Data;
using ASEDUPH_V2_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASEDUPH_V2_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SituacionEconomicaController : ControllerBase
    {
        private readonly AseduphDbContext _context;

        public SituacionEconomicaController(AseduphDbContext context)
        {
            _context = context;
        }

        // GET: api/SituacionEconomica
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SituacionEconomica>>> GetSituacionesEconomicas()
        {
            var situaciones = await _context.SituacionEconomica
                .Include(se => se.VisitaFamiliar)
                    .ThenInclude(v => v.SolicitudBeca)
                        .ThenInclude(s => s.Estudiante)
                .ToListAsync();

            return Ok(situaciones);
        }

        // GET: api/SituacionEconomica/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SituacionEconomica>> GetSituacionEconomica(int id)
        {
            var situacion = await _context.SituacionEconomica
                .Include(se => se.VisitaFamiliar)
                    .ThenInclude(v => v.SolicitudBeca)
                        .ThenInclude(s => s.Estudiante)
                .FirstOrDefaultAsync(se => se.SituacionEconomicaId == id);

            if (situacion == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró la situación económica solicitada."
                });
            }

            return Ok(situacion);
        }

        // GET: api/SituacionEconomica/visita/5
        [HttpGet("visita/{visitaFamiliarId}")]
        public async Task<ActionResult<SituacionEconomica>> GetSituacionPorVisita(int visitaFamiliarId)
        {
            var situacion = await _context.SituacionEconomica
                .Include(se => se.VisitaFamiliar)
                    .ThenInclude(v => v.SolicitudBeca)
                        .ThenInclude(s => s.Estudiante)
                .FirstOrDefaultAsync(se => se.VisitaFamiliarId == visitaFamiliarId);

            if (situacion == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró situación económica para esta visita familiar."
                });
            }

            return Ok(situacion);
        }

        // POST: api/SituacionEconomica
        [HttpPost]
        public async Task<ActionResult<SituacionEconomica>> PostSituacionEconomica(SituacionEconomica situacion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existeVisita = await _context.VisitasFamiliares
                .AnyAsync(v => v.VisitaFamiliarId == situacion.VisitaFamiliarId);

            if (!existeVisita)
            {
                return BadRequest(new
                {
                    mensaje = "La visita familiar indicada no existe."
                });
            }

            var yaExisteSituacion = await _context.SituacionEconomica
                .AnyAsync(se => se.VisitaFamiliarId == situacion.VisitaFamiliarId);

            if (yaExisteSituacion)
            {
                return BadRequest(new
                {
                    mensaje = "Esta visita familiar ya tiene una situación económica registrada."
                });
            }

            var validacion = ValidarMontos(situacion);
            if (validacion != null)
            {
                return BadRequest(validacion);
            }

            _context.SituacionEconomica.Add(situacion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetSituacionEconomica),
                new { id = situacion.SituacionEconomicaId },
                situacion
            );
        }

        // PUT: api/SituacionEconomica/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSituacionEconomica(int id, SituacionEconomica situacion)
        {
            if (id != situacion.SituacionEconomicaId)
            {
                return BadRequest(new
                {
                    mensaje = "El ID enviado no coincide con el ID de la situación económica."
                });
            }

            var situacionExistente = await _context.SituacionEconomica
                .FirstOrDefaultAsync(se => se.SituacionEconomicaId == id);

            if (situacionExistente == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró la situación económica que desea actualizar."
                });
            }

            var existeVisita = await _context.VisitasFamiliares
                .AnyAsync(v => v.VisitaFamiliarId == situacion.VisitaFamiliarId);

            if (!existeVisita)
            {
                return BadRequest(new
                {
                    mensaje = "La visita familiar indicada no existe."
                });
            }

            var situacionDuplicada = await _context.SituacionEconomica
                .AnyAsync(se =>
                    se.VisitaFamiliarId == situacion.VisitaFamiliarId &&
                    se.SituacionEconomicaId != id);

            if (situacionDuplicada)
            {
                return BadRequest(new
                {
                    mensaje = "Ya existe otra situación económica registrada para esta visita familiar."
                });
            }

            var validacion = ValidarMontos(situacion);
            if (validacion != null)
            {
                return BadRequest(validacion);
            }

            situacionExistente.VisitaFamiliarId = situacion.VisitaFamiliarId;
            situacionExistente.TotalIngresos = situacion.TotalIngresos;
            situacionExistente.GastoAlimentacion = situacion.GastoAlimentacion;
            situacionExistente.GastoVivienda = situacion.GastoVivienda;
            situacionExistente.GastoLuz = situacion.GastoLuz;
            situacionExistente.GastoTelefono = situacion.GastoTelefono;
            situacionExistente.GastoAgua = situacion.GastoAgua;
            situacionExistente.GastoTransporte = situacion.GastoTransporte;
            situacionExistente.GastoEducacion = situacion.GastoEducacion;
            situacionExistente.GastoDiversion = situacion.GastoDiversion;
            situacionExistente.GastoSalud = situacion.GastoSalud;
            situacionExistente.GastoAhorro = situacion.GastoAhorro;
            situacionExistente.OtrosGastos = situacion.OtrosGastos;
            situacionExistente.TotalEgresos = situacion.TotalEgresos;
            situacionExistente.TieneTarjetaCredito = situacion.TieneTarjetaCredito;
            situacionExistente.TieneCreditoBancario = situacion.TieneCreditoBancario;
            situacionExistente.TieneEndeudamiento = situacion.TieneEndeudamiento;
            situacionExistente.DescripcionEndeudamiento = situacion.DescripcionEndeudamiento;
            situacionExistente.Observaciones = situacion.Observaciones;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Situación económica actualizada correctamente."
            });
        }

        // DELETE: api/SituacionEconomica/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSituacionEconomica(int id)
        {
            var situacion = await _context.SituacionEconomica
                .FirstOrDefaultAsync(se => se.SituacionEconomicaId == id);

            if (situacion == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró la situación económica que desea eliminar."
                });
            }

            _context.SituacionEconomica.Remove(situacion);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Situación económica eliminada correctamente."
            });
        }

        private object? ValidarMontos(SituacionEconomica situacion)
        {
            var montos = new Dictionary<string, decimal?>
            {
                { "TotalIngresos", situacion.TotalIngresos },
                { "GastoAlimentacion", situacion.GastoAlimentacion },
                { "GastoVivienda", situacion.GastoVivienda },
                { "GastoLuz", situacion.GastoLuz },
                { "GastoTelefono", situacion.GastoTelefono },
                { "GastoAgua", situacion.GastoAgua },
                { "GastoTransporte", situacion.GastoTransporte },
                { "GastoEducacion", situacion.GastoEducacion },
                { "GastoDiversion", situacion.GastoDiversion },
                { "GastoSalud", situacion.GastoSalud },
                { "GastoAhorro", situacion.GastoAhorro },
                { "OtrosGastos", situacion.OtrosGastos },
                { "TotalEgresos", situacion.TotalEgresos }
            };

            foreach (var monto in montos)
            {
                if (monto.Value < 0)
                {
                    return new
                    {
                        mensaje = $"El campo {monto.Key} no puede tener un valor negativo."
                    };
                }
            }

            return null;
        }
    }
}