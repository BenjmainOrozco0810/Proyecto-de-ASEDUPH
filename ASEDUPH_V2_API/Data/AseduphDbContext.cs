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

        // ── Módulo de Aspirantes ─────────────────────────────────────
        public DbSet<Estudiante> Estudiantes { get; set; }
        public DbSet<Encargado> Encargados { get; set; }
        public DbSet<CentroEducativo> CentrosEducativos { get; set; }
        public DbSet<TipoApoyo> TiposApoyo { get; set; }
        public DbSet<SolicitudBeca> SolicitudesBeca { get; set; }
        public DbSet<SolicitudTipoApoyo> SolicitudTiposApoyo { get; set; }

        // ── Módulo de Visita Familiar ────────────────────────────────
        public DbSet<VisitaFamiliar> VisitasFamiliares { get; set; }
        public DbSet<GrupoFamiliar> GrupoFamiliar { get; set; }
        public DbSet<SituacionEconomica> SituacionEconomica { get; set; }
        public DbSet<Vivienda> Viviendas { get; set; }
        public DbSet<BienFamiliar> BienesFamiliares { get; set; }
        public DbSet<ApoyoExterno> ApoyosExternos { get; set; }

        // ── Módulo de Evaluación ─────────────────────────────────────
        public DbSet<EvaluacionBeca> EvaluacionesBeca { get; set; }
        public DbSet<ListaCotejoEvaluacion> ListaCotejoEvaluacion { get; set; }

        // ── Módulo de Becas ──────────────────────────────────────────
        public DbSet<Beca> Becas { get; set; }
        public DbSet<SeguimientoAcademico> SeguimientoAcademico { get; set; }

        // ── Módulo de Renovación ─────────────────────────────────────
        public DbSet<RenovacionBeca> RenovacionesBeca { get; set; }

        // ── Módulo de Benefactores y Aportes ─────────────────────────
        public DbSet<Benefactor> Benefactores { get; set; }
        public DbSet<Aporte> Aportes { get; set; }

        // ── Módulo de Usuarios ───────────────────────────────────────
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<UsuarioRol> UsuarioRoles { get; set; }
        public DbSet<LogAuditoria> LogsAuditoria { get; set; }
    }
}