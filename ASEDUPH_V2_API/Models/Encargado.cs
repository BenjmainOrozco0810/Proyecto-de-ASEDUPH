using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASEDUPH_V2_API.Models
{
    public class Encargado
    {
        public int EncargadoId { get; set; }

        [Required]
        public int EstudianteId { get; set; }

        [Required]
        [StringLength(150)]
        public string NombreCompleto { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Parentesco { get; set; }

        [StringLength(50)]
        public string? EstadoCivil { get; set; }

        [StringLength(25)]
        public string? DPI { get; set; }

        [StringLength(100)]
        public string? DpiExtendido { get; set; }

        [StringLength(20)]
        public string? TelefonoDomiciliar { get; set; }

        [StringLength(20)]
        public string? TelefonoCelular { get; set; }

        [StringLength(120)]
        public string? CorreoElectronico { get; set; }

        [StringLength(250)]
        public string? Direccion { get; set; }

        [StringLength(100)]
        public string? Ocupacion { get; set; }

        [StringLength(150)]
        public string? LugarTrabajo { get; set; }

        [StringLength(20)]
        public string? TelefonoTrabajo { get; set; }

        [StringLength(30)]
        public string Estado { get; set; } = "Activo";

        [ForeignKey("EstudianteId")]
        public Estudiante? Estudiante { get; set; }
    }
}