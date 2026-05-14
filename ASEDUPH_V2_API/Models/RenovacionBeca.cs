using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASEDUPH_V2_API.Models
{
    public class RenovacionBeca
    {
        public int RenovacionBecaId { get; set; }

        [Required]
        public int BecaId { get; set; }

        public int? CentroEducativoAnteriorId { get; set; }

        public int? CentroEducativoNuevoId { get; set; }

        [Required]
        public int AnioRenovacion { get; set; }

        [StringLength(50)]
        public string? AnioCursadoAnterior { get; set; }

        [StringLength(50)]
        public string? AnioACursar { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? PromedioFinalAnterior { get; set; }

        [StringLength(150)]
        public string? TipoApoyoRecibido { get; set; }

        public string? MotivoRenovacion { get; set; }

        [StringLength(40)]
        public string EstadoRenovacion { get; set; } = "Pendiente";
        // Pendiente, En Evaluación, Aprobada, Rechazada, Cancelada

        public DateTime? FechaSolicitud { get; set; }

        public string? Recomendaciones { get; set; }

        public DateTime? FechaDecision { get; set; }

        // ── Navegación ───────────────────────────────────────────────
        [ForeignKey("BecaId")]
        public Beca? Beca { get; set; }

        [ForeignKey("CentroEducativoAnteriorId")]
        public CentroEducativo? CentroEducativoAnterior { get; set; }

        [ForeignKey("CentroEducativoNuevoId")]
        public CentroEducativo? CentroEducativoNuevo { get; set; }
    }
}