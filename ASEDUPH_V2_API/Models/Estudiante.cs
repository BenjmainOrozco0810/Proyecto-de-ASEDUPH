using System.ComponentModel.DataAnnotations;

namespace ASEDUPH_V2_API.Models
{
    public class Estudiante
    {
        public int EstudianteId { get; set; }

        [Required]
        [StringLength(150)]
        public string NombreCompleto { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Sexo { get; set; }

        public DateTime? FechaNacimiento { get; set; }

        public int? Edad { get; set; }

        [StringLength(250)]
        public string? Direccion { get; set; }

        [StringLength(100)]
        public string? Municipio { get; set; }

        [StringLength(100)]
        public string? Departamento { get; set; }

        [StringLength(30)]
        public string Estado { get; set; } = "Activo";

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public ICollection<Encargado>? Encargados { get; set; }
        public ICollection<SolicitudBeca>? SolicitudesBeca { get; set; }
        public ICollection<Beca>? Becas { get; set; }
        public ICollection<SeguimientoAcademico>? SeguimientosAcademicos { get; set; }
    }
}