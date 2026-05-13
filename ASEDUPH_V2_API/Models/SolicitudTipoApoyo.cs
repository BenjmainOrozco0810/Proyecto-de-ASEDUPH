using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASEDUPH_V2_API.Models
{
    public class SolicitudTipoApoyo
    {
        public int SolicitudTipoApoyoId { get; set; }

        [Required]
        public int SolicitudBecaId { get; set; }

        [Required]
        public int TipoApoyoId { get; set; }

        [StringLength(250)]
        public string? DescripcionOtroApoyo { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? MontoEstimado { get; set; }

        [ForeignKey("SolicitudBecaId")]
        public SolicitudBeca? SolicitudBeca { get; set; }

        [ForeignKey("TipoApoyoId")]
        public TipoApoyo? TipoApoyo { get; set; }
    }
}