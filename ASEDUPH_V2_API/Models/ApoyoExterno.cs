using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASEDUPH_V2_API.Models
{
    public class ApoyoExterno
    {
        public int ApoyoExternoId { get; set; }

        [Required]
        public int VisitaFamiliarId { get; set; }

        [StringLength(150)]
        public string? NombreBeneficiado { get; set; }

        [StringLength(80)]
        public string? Parentesco { get; set; }

        [StringLength(150)]
        public string? Institucion { get; set; }

        [StringLength(100)]
        public string? TipoAyuda { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? PorcentajeApoyo { get; set; }

        public bool? TerminoEstudios { get; set; }

        public string? Observaciones { get; set; }

        [ForeignKey("VisitaFamiliarId")]
        public VisitaFamiliar? VisitaFamiliar { get; set; }
    }
}