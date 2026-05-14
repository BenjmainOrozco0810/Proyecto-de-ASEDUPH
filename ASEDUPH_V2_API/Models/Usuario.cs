using System.ComponentModel.DataAnnotations;

namespace ASEDUPH_V2_API.Models
{
    public class Usuario
    {
        public int UsuarioId { get; set; }

        [Required]
        [StringLength(150)]
        public string NombreCompleto { get; set; } = string.Empty;

        [StringLength(120)]
        public string? Correo { get; set; }

        [Required]
        [StringLength(80)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        public string PasswordHash { get; set; } = string.Empty;

        [StringLength(30)]
        public string Estado { get; set; } = "Activo";
        // Activo, Inactivo

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        // ── Navegación ───────────────────────────────────────────────
        public ICollection<UsuarioRol>? UsuarioRoles { get; set; }
    }
}