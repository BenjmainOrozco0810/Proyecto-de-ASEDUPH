using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASEDUPH_V2_API.Models
{
    public class Aporte
    {
        public int AporteId { get; set; }

        [Required]
        public int BenefactorId { get; set; }

        public int? BecaId { get; set; }

        [Required]
        public DateTime FechaAporte { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Monto { get; set; }

        [StringLength(80)]
        public string? TipoAporte { get; set; }
        // Económico, Útiles, Uniformes, Zapatos, Alimentos, Otro

        [StringLength(80)]
        public string? FormaPago { get; set; }
        // Efectivo, Transferencia, Depósito, Cheque, Otro

        [StringLength(80)]
        public string? Periodo { get; set; }
        // Mensual, Semestral, Anual, Único, Otro

        [StringLength(100)]
        public string? NumeroComprobante { get; set; }

        public string? Observaciones { get; set; }

        // ── Navegación ───────────────────────────────────────────────
        [ForeignKey("BenefactorId")]
        public Benefactor? Benefactor { get; set; }

        [ForeignKey("BecaId")]
        public Beca? Beca { get; set; }
    }
}