using ASEDUPH_V2_API.Models;
using Microsoft.EntityFrameworkCore;

namespace ASEDUPH_V2_API.Data
{
    public class AseduphDbContext : DbContext
    {
        public AseduphDbContext(DbContextOptions<AseduphDbContext> options)
            : base(options)
        {
        }

        public DbSet<Estudiante> Estudiantes { get; set; }
        public DbSet<Encargado> Encargados { get; set; }
        public DbSet<CentroEducativo> CentrosEducativos { get; set; }
        public DbSet<TipoApoyo> TiposApoyo { get; set; }
        public DbSet<SolicitudBeca> SolicitudesBeca { get; set; }
        public DbSet<SolicitudTipoApoyo> SolicitudTiposApoyo { get; set; }
        public DbSet<VisitaFamiliar> VisitasFamiliares { get; set; }
        public DbSet<GrupoFamiliar> GrupoFamiliar { get; set; }
        public DbSet<SituacionEconomica> SituacionEconomica { get; set; }
        public DbSet<Vivienda> Viviendas { get; set; }
        public DbSet<BienFamiliar> BienesFamiliares { get; set; }
        public DbSet<ApoyoExterno> ApoyosExternos { get; set; }
        public DbSet<EvaluacionBeca> EvaluacionesBeca { get; set; }
        /*public DbSet<ListaCotejoEvaluacion> ListaCotejoEvaluacion { get; set; }
        public DbSet<Beca> Becas { get; set; }
        public DbSet<RenovacionBeca> RenovacionesBeca { get; set; }
        public DbSet<SeguimientoAcademico> SeguimientoAcademico { get; set; }
        public DbSet<Benefactor> Benefactores { get; set; }
        public DbSet<Aporte> Aportes { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<UsuarioRol> UsuarioRoles { get; set; }*/
    }
}