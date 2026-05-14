using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASEDUPH_V2_API.Models
{
    public class SeguimientoAcademico
    {
        public int SeguimientoAcademicoId { get; set; }

        [Required]
        public int EstudianteId { get; set; }

        public int? BecaId { get; set; }

        public int? CentroEducativoId { get; set; }

        [Required]
        public int Anio { get; set; }

        [StringLength(50)]
        public string? Grado { get; set; }

        [StringLength(50)]
        public string? NivelEducativo { get; set; }
        // Preprimaria, Primaria, Básicos, Diversificado, Universidad

        [Column(TypeName = "decimal(5,2)")]
        public decimal? Promedio { get; set; }

        [StringLength(50)]
        public string? EstadoAcademico { get; set; }
        // Aprobado, Reprobado, En Curso, Retirado, Finalizado

        public string? Observaciones { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        // ── Navegación ───────────────────────────────────────────────
        [ForeignKey("EstudianteId")]
        public Estudiante? Estudiante { get; set; }

        [ForeignKey("BecaId")]
        public Beca? Beca { get; set; }

        [ForeignKey("CentroEducativoId")]
        public CentroEducativo? CentroEducativo { get; set; }
    }
}