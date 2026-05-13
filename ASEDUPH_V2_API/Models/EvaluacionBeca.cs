using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASEDUPH_V2_API.Models
{
    public class EvaluacionBeca
    {
        public int EvaluacionBecaId { get; set; }

        [Required]
        public int SolicitudBecaId { get; set; }

        public string? RecomendacionesResponsable { get; set; }

        [StringLength(80)]
        public string? ClasificacionOtorgada { get; set; }

        [StringLength(50)]
        public string? DecisionFinal { get; set; }

        public DateTime? FechaDecision { get; set; }

        public string? ObservacionesGenerales { get; set; }

        [StringLength(150)]
        public string? EvaluadoPor { get; set; }

        [ForeignKey("SolicitudBecaId")]
        public SolicitudBeca? SolicitudBeca { get; set; }
    }
}