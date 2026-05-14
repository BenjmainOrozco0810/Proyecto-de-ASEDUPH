using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASEDUPH_V2_API.Models
{
    public class ListaCotejoEvaluacion
    {
        public int ListaCotejoId { get; set; }

        [Required]
        public int EvaluacionBecaId { get; set; }

        [Required]
        [StringLength(120)]
        public string Rubro { get; set; } = string.Empty;
        // Fe de edad, DPI de padres/encargados, Entrega de notas,
        // Rendimiento académico, Número de años con beca, Tipo de beca

        public bool Completado { get; set; } = false;

        [StringLength(250)]
        public string? Observaciones { get; set; }

        // ── Navegación ───────────────────────────────────────────────
        [ForeignKey("EvaluacionBecaId")]
        public EvaluacionBeca? EvaluacionBeca { get; set; }
    }
}