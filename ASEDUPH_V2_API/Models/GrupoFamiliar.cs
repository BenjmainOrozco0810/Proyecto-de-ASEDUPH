using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASEDUPH_V2_API.Models
{
    public class GrupoFamiliar
    {
        public int GrupoFamiliarId { get; set; }

        [Required]
        public int VisitaFamiliarId { get; set; }

        [Required]
        [StringLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(80)]
        public string? Parentesco { get; set; }

        public int? Edad { get; set; }

        [StringLength(150)]
        public string? LugarTrabajoEstudio { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Ingresos { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? GastosColegiatura { get; set; }

        public bool? ViveConFamilia { get; set; }

        [ForeignKey("VisitaFamiliarId")]
        public VisitaFamiliar? VisitaFamiliar { get; set; }
    }
}