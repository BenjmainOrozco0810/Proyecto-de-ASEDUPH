using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASEDUPH_V2_API.Models
{
    public class UsuarioRol
    {
        public int UsuarioRolId { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int RolId { get; set; }

        // ── Navegación ───────────────────────────────────────────────
        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        [ForeignKey("RolId")]
        public Rol? Rol { get; set; }
    }
}