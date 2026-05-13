using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASEDUPH_V2_API.Models
{
    public class SolicitudBeca
    {
        public int SolicitudBecaId { get; set; }

        [Required]
        public int EstudianteId { get; set; }

        public int? CentroEducativoId { get; set; }

        [Required]
        public int AnioSolicitud { get; set; }

        [StringLength(50)]
        public string? NivelEducativo { get; set; }

        [StringLength(50)]
        public string? GradoSolicitado { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? PromedioActual { get; set; }

        public string? MotivoSolicitud { get; set; }

        [StringLength(40)]
        public string EstadoSolicitud { get; set; } = "Pendiente";

        [Required]
        public DateTime FechaSolicitud { get; set; }

        [StringLength(150)]
        public string? NombrePersonaCompletaFormulario { get; set; }

        public DateTime? FechaFormulario { get; set; }

        public string? Observaciones { get; set; }

        [ForeignKey("EstudianteId")]
        public Estudiante? Estudiante { get; set; }

        [ForeignKey("CentroEducativoId")]
        public CentroEducativo? CentroEducativo { get; set; }

        public ICollection<SolicitudTipoApoyo>? SolicitudTiposApoyo { get; set; }
        public VisitaFamiliar? VisitaFamiliar { get; set; }
        public EvaluacionBeca? EvaluacionBeca { get; set; }
        /*public Beca? Beca { get; set; }*/
    }
}