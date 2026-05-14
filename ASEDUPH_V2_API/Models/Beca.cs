using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASEDUPH_V2_API.Models
{
    public class Beca
    {
        public int BecaId { get; set; }

        [Required]
        public int EstudianteId { get; set; }

        public int? SolicitudBecaId { get; set; }

        [Required]
        public int AnioInicio { get; set; }

        public int? AnioFin { get; set; }

        [StringLength(50)]
        public string? NivelEducativo { get; set; }
        // Preprimaria, Primaria, Básicos, Diversificado, Universidad

        [StringLength(80)]
        public string? TipoBeca { get; set; }

        [StringLength(40)]
        public string EstadoBeca { get; set; } = "Activa";
        // Activa, Finalizada, Suspendida, Cancelada

        [Column(TypeName = "decimal(10,2)")]
        public decimal? MontoAprobado { get; set; }

        public string? Observaciones { get; set; }

        public DateTime? FechaAprobacion { get; set; }

        // ── Navegación ───────────────────────────────────────────────
        [ForeignKey("EstudianteId")]
        public Estudiante? Estudiante { get; set; }

        [ForeignKey("SolicitudBecaId")]
        public SolicitudBeca? SolicitudBeca { get; set; }

        public ICollection<RenovacionBeca>? RenovacionesBeca { get; set; }
        public ICollection<SeguimientoAcademico>? SeguimientosAcademicos { get; set; }

        public ICollection<Aporte>? Aportes { get; set; }
    }
}