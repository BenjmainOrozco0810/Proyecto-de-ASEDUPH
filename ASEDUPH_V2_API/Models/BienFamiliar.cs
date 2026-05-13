using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASEDUPH_V2_API.Models
{
    public class BienFamiliar
    {
        public int BienFamiliarId { get; set; }

        [Required]
        public int VisitaFamiliarId { get; set; }

        [StringLength(80)]
        public string? TipoBien { get; set; }

        [StringLength(250)]
        public string? Descripcion { get; set; }

        public int? Cantidad { get; set; }

        public string? Observaciones { get; set; }

        [ForeignKey("VisitaFamiliarId")]
        public VisitaFamiliar? VisitaFamiliar { get; set; }
    }
}