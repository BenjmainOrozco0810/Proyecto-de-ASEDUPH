using System.ComponentModel.DataAnnotations;

namespace ASEDUPH_V2_API.Models
{
    public class TipoApoyo
    {
        public int TipoApoyoId { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(250)]
        public string? Descripcion { get; set; }

        [StringLength(30)]
        public string Estado { get; set; } = "Activo";

        public ICollection<SolicitudTipoApoyo>? SolicitudTiposApoyo { get; set; }
    }
}