using System.ComponentModel.DataAnnotations;

namespace ASEDUPH_V2_API.Models
{
    public class Rol
    {
        public int RolId { get; set; }

        [Required]
        [StringLength(80)]
        public string NombreRol { get; set; } = string.Empty;
        // Administrador, Operador, Consulta, JuntaDirectiva

        [StringLength(250)]
        public string? Descripcion { get; set; }

        [StringLength(30)]
        public string Estado { get; set; } = "Activo";
        // Activo, Inactivo

        // ── Navegación ───────────────────────────────────────────────
        public ICollection<UsuarioRol>? UsuarioRoles { get; set; }
    }
}