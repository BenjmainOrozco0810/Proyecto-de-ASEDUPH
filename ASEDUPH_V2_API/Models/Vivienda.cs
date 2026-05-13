using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASEDUPH_V2_API.Models
{
    public class Vivienda
    {
        public int ViviendaId { get; set; }

        [Required]
        public int VisitaFamiliarId { get; set; }

        [StringLength(50)]
        public string? TipoVivienda { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? PagoMensual { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? AmortizacionMensual { get; set; }

        public bool? MostroRecibo { get; set; }

        public string? DescripcionVivienda { get; set; }

        public string? Observaciones { get; set; }

        [ForeignKey("VisitaFamiliarId")]
        public VisitaFamiliar? VisitaFamiliar { get; set; }
    }
}