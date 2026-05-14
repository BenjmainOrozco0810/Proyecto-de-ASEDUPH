using ASEDUPH_V2_API.Data;
using ASEDUPH_V2_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASEDUPH_V2_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListaCotejoEvaluacionController : ControllerBase
    {
        private readonly AseduphDbContext _context;

        public ListaCotejoEvaluacionController(AseduphDbContext context)
        {
            _context = context;
        }

        // GET: api/ListaCotejoEvaluacion
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ListaCotejoEvaluacion>>> GetListasCotejo()
        {
            var listas = await _context.ListaCotejoEvaluacion
                .Include(l => l.EvaluacionBeca)
                    .ThenInclude(e => e!.SolicitudBeca)
                        .ThenInclude(s => s!.Estudiante)
                .OrderBy(l => l.EvaluacionBecaId)
                    .ThenBy(l => l.Rubro)
                .ToListAsync();

            return Ok(listas);
        }

        // GET: api/ListaCotejoEvaluacion/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ListaCotejoEvaluacion>> GetListaCotejoEvaluacion(int id)
        {
            var lista = await _context.ListaCotejoEvaluacion
                .Include(l => l.EvaluacionBeca)
                    .ThenInclude(e => e!.SolicitudBeca)
                        .ThenInclude(s => s!.Estudiante)
                .FirstOrDefaultAsync(l => l.ListaCotejoId == id);

            if (lista == null)
            {
                return NotFound(new { mensaje = "No se encontró el rubro de lista de cotejo solicitado." });
            }

            return Ok(lista);
        }

        // GET: api/ListaCotejoEvaluacion/evaluacion/5
        [HttpGet("evaluacion/{evaluacionBecaId}")]
        public async Task<ActionResult<IEnumerable<ListaCotejoEvaluacion>>> GetListaPorEvaluacion(int evaluacionBecaId)
        {
            var existeEvaluacion = await _context.EvaluacionesBeca
                .AnyAsync(e => e.EvaluacionBecaId == evaluacionBecaId);

            if (!existeEvaluacion)
            {
                return NotFound(new { mensaje = "No se encontró la evaluación de beca indicada." });
            }

            var rubros = await _context.ListaCotejoEvaluacion
                .Where(l => l.EvaluacionBecaId == evaluacionBecaId)
                .OrderBy(l => l.Rubro)
                .ToListAsync();

            return Ok(rubros);
        }

        // POST: api/ListaCotejoEvaluacion
        [HttpPost]
        public async Task<ActionResult<ListaCotejoEvaluacion>> PostListaCotejoEvaluacion(ListaCotejoEvaluacion lista)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existeEvaluacion = await _context.EvaluacionesBeca
                .AnyAsync(e => e.EvaluacionBecaId == lista.EvaluacionBecaId);

            if (!existeEvaluacion)
            {
                return BadRequest(new { mensaje = "La evaluación de beca indicada no existe." });
            }

            // Evitar rubros duplicados en la misma evaluación
            var rubroDuplicado = await _context.ListaCotejoEvaluacion
                .AnyAsync(l => l.EvaluacionBecaId == lista.EvaluacionBecaId &&
                               l.Rubro == lista.Rubro);

            if (rubroDuplicado)
            {
                return BadRequest(new { mensaje = "Este rubro ya fue registrado para esta evaluación." });
            }

            _context.ListaCotejoEvaluacion.Add(lista);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetListaCotejoEvaluacion), new { id = lista.ListaCotejoId }, lista);
        }

        // PUT: api/ListaCotejoEvaluacion/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutListaCotejoEvaluacion(int id, ListaCotejoEvaluacion lista)
        {
            if (id != lista.ListaCotejoId)
            {
                return BadRequest(new { mensaje = "El ID enviado no coincide con el ID del rubro." });
            }

            var existente = await _context.ListaCotejoEvaluacion.FirstOrDefaultAsync(l => l.ListaCotejoId == id);

            if (existente == null)
            {
                return NotFound(new { mensaje = "No se encontró el rubro de lista de cotejo que desea actualizar." });
            }

            existente.EvaluacionBecaId = lista.EvaluacionBecaId;
            existente.Rubro = lista.Rubro;
            existente.Completado = lista.Completado;
            existente.Observaciones = lista.Observaciones;

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Rubro de lista de cotejo actualizado correctamente." });
        }

        // DELETE: api/ListaCotejoEvaluacion/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteListaCotejoEvaluacion(int id)
        {
            var lista = await _context.ListaCotejoEvaluacion.FirstOrDefaultAsync(l => l.ListaCotejoId == id);

            if (lista == null)
            {
                return NotFound(new { mensaje = "No se encontró el rubro que desea eliminar." });
            }

            _context.ListaCotejoEvaluacion.Remove(lista);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Rubro de lista de cotejo eliminado correctamente." });
        }
    }
}