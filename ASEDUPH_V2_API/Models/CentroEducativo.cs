using System.ComponentModel.DataAnnotations;

namespace ASEDUPH_V2_API.Models
{
    public class CentroEducativo
    {
        public int CentroEducativoId { get; set; }

        [Required]
        [StringLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(250)]
        public string? Direccion { get; set; }

        [StringLength(20)]
        public string? Telefono { get; set; }

        [StringLength(50)]
        public string? TipoCentro { get; set; }

        [StringLength(30)]
        public string Estado { get; set; } = "Activo";

        public ICollection<SolicitudBeca>? SolicitudesBeca { get; set; }
        /*public ICollection<SeguimientoAcademico>? SeguimientosAcademicos { get; set; }*/
    }
}