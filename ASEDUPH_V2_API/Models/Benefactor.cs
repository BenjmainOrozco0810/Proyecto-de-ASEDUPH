using System.ComponentModel.DataAnnotations;

namespace ASEDUPH_V2_API.Models
{
    public class Benefactor
    {
        public int BenefactorId { get; set; }

        [Required]
        [StringLength(150)]
        public string NombreCompleto { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Telefono { get; set; }

        [StringLength(120)]
        public string? CorreoElectronico { get; set; }

        [StringLength(250)]
        public string? Direccion { get; set; }

        [StringLength(50)]
        public string? TipoBenefactor { get; set; }
        // Padrino, Donante, Voluntario, Empresa, Otro

        [StringLength(30)]
        public string Estado { get; set; } = "Activo";
        // Activo, Inactivo

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        // ── Navegación ───────────────────────────────────────────────
        public ICollection<Aporte>? Aportes { get; set; }
    }
}