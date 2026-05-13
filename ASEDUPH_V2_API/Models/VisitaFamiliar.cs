using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASEDUPH_V2_API.Models
{
    public class VisitaFamiliar
    {
        public int VisitaFamiliarId { get; set; }

        [Required]
        public int SolicitudBecaId { get; set; }

        [StringLength(50)]
        public string? TipoVisita { get; set; }

        [StringLength(150)]
        public string? LugarEntrevista { get; set; }

        public DateTime? FechaVisita { get; set; }

        public TimeSpan? HoraVisita { get; set; }

        [StringLength(150)]
        public string? PersonaEntrevistada { get; set; }

        [StringLength(80)]
        public string? ParentescoEntrevistado { get; set; }

        public string? ActitudFamilia { get; set; }

        public string? ApreciacionGeneral { get; set; }

        public string? RecomendacionJunta { get; set; }

        [StringLength(150)]
        public string? RealizadaPor { get; set; }

        [StringLength(150)]
        public string? Firma { get; set; }

        public string? ObservacionesFinales { get; set; }

        [ForeignKey("SolicitudBecaId")]
        public SolicitudBeca? SolicitudBeca { get; set; }

        public ICollection<GrupoFamiliar>? GrupoFamiliar { get; set; }
        public SituacionEconomica? SituacionEconomica { get; set; }

        public Vivienda? Vivienda { get; set; }
        public ICollection<BienFamiliar>? BienesFamiliares { get; set; }
        public ICollection<ApoyoExterno>? ApoyosExternos { get; set; }
    }
}