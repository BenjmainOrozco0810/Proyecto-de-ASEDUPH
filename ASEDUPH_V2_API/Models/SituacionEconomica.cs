using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASEDUPH_V2_API.Models
{
    public class SituacionEconomica
    {
        public int SituacionEconomicaId { get; set; }

        [Required]
        public int VisitaFamiliarId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? TotalIngresos { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? GastoAlimentacion { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? GastoVivienda { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? GastoLuz { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? GastoTelefono { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? GastoAgua { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? GastoTransporte { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? GastoEducacion { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? GastoDiversion { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? GastoSalud { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? GastoAhorro { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? OtrosGastos { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? TotalEgresos { get; set; }

        public bool? TieneTarjetaCredito { get; set; }

        public bool? TieneCreditoBancario { get; set; }

        public bool? TieneEndeudamiento { get; set; }

        [StringLength(250)]
        public string? DescripcionEndeudamiento { get; set; }

        public string? Observaciones { get; set; }

        [ForeignKey("VisitaFamiliarId")]
        public VisitaFamiliar? VisitaFamiliar { get; set; }
    }
}