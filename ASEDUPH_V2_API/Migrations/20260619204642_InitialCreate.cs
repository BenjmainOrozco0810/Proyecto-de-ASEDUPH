using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASEDUPH_V2_API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Benefactores",
                columns: table => new
                {
                    BenefactorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreCompleto = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CorreoElectronico = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Direccion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    TipoBenefactor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Benefactores", x => x.BenefactorId);
                });

            migrationBuilder.CreateTable(
                name: "CentrosEducativos",
                columns: table => new
                {
                    CentroEducativoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TipoCentro = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CentrosEducativos", x => x.CentroEducativoId);
                });

            migrationBuilder.CreateTable(
                name: "Estudiantes",
                columns: table => new
                {
                    EstudianteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreCompleto = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Sexo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Edad = table.Column<int>(type: "int", nullable: true),
                    Direccion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Municipio = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Departamento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estudiantes", x => x.EstudianteId);
                });

            migrationBuilder.CreateTable(
                name: "LogsAuditoria",
                columns: table => new
                {
                    LogAuditoriaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: true),
                    NombreUsuario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Accion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EntidadAfectada = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EntidadId = table.Column<int>(type: "int", nullable: true),
                    FechaHora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DireccionIP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Resultado = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogsAuditoria", x => x.LogAuditoriaId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RolId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreRol = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RolId);
                });

            migrationBuilder.CreateTable(
                name: "TiposApoyo",
                columns: table => new
                {
                    TipoApoyoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposApoyo", x => x.TipoApoyoId);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreCompleto = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Username = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.UsuarioId);
                });

            migrationBuilder.CreateTable(
                name: "Encargados",
                columns: table => new
                {
                    EncargadoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EstudianteId = table.Column<int>(type: "int", nullable: false),
                    NombreCompleto = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Parentesco = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EstadoCivil = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DPI = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    DpiExtendido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TelefonoDomiciliar = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TelefonoCelular = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CorreoElectronico = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Direccion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Ocupacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LugarTrabajo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    TelefonoTrabajo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Encargados", x => x.EncargadoId);
                    table.ForeignKey(
                        name: "FK_Encargados_Estudiantes_EstudianteId",
                        column: x => x.EstudianteId,
                        principalTable: "Estudiantes",
                        principalColumn: "EstudianteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudesBeca",
                columns: table => new
                {
                    SolicitudBecaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EstudianteId = table.Column<int>(type: "int", nullable: false),
                    CentroEducativoId = table.Column<int>(type: "int", nullable: true),
                    AnioSolicitud = table.Column<int>(type: "int", nullable: false),
                    NivelEducativo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GradoSolicitado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PromedioActual = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    MotivoSolicitud = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstadoSolicitud = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NombrePersonaCompletaFormulario = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    FechaFormulario = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesBeca", x => x.SolicitudBecaId);
                    table.ForeignKey(
                        name: "FK_SolicitudesBeca_CentrosEducativos_CentroEducativoId",
                        column: x => x.CentroEducativoId,
                        principalTable: "CentrosEducativos",
                        principalColumn: "CentroEducativoId");
                    table.ForeignKey(
                        name: "FK_SolicitudesBeca_Estudiantes_EstudianteId",
                        column: x => x.EstudianteId,
                        principalTable: "Estudiantes",
                        principalColumn: "EstudianteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioRoles",
                columns: table => new
                {
                    UsuarioRolId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    RolId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioRoles", x => x.UsuarioRolId);
                    table.ForeignKey(
                        name: "FK_UsuarioRoles_Roles_RolId",
                        column: x => x.RolId,
                        principalTable: "Roles",
                        principalColumn: "RolId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioRoles_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Becas",
                columns: table => new
                {
                    BecaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EstudianteId = table.Column<int>(type: "int", nullable: false),
                    SolicitudBecaId = table.Column<int>(type: "int", nullable: true),
                    AnioInicio = table.Column<int>(type: "int", nullable: false),
                    AnioFin = table.Column<int>(type: "int", nullable: true),
                    NivelEducativo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TipoBeca = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    EstadoBeca = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    MontoAprobado = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaAprobacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Becas", x => x.BecaId);
                    table.ForeignKey(
                        name: "FK_Becas_Estudiantes_EstudianteId",
                        column: x => x.EstudianteId,
                        principalTable: "Estudiantes",
                        principalColumn: "EstudianteId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Becas_SolicitudesBeca_SolicitudBecaId",
                        column: x => x.SolicitudBecaId,
                        principalTable: "SolicitudesBeca",
                        principalColumn: "SolicitudBecaId");
                });

            migrationBuilder.CreateTable(
                name: "EvaluacionesBeca",
                columns: table => new
                {
                    EvaluacionBecaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SolicitudBecaId = table.Column<int>(type: "int", nullable: false),
                    RecomendacionesResponsable = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClasificacionOtorgada = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    DecisionFinal = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FechaDecision = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ObservacionesGenerales = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EvaluadoPor = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluacionesBeca", x => x.EvaluacionBecaId);
                    table.ForeignKey(
                        name: "FK_EvaluacionesBeca_SolicitudesBeca_SolicitudBecaId",
                        column: x => x.SolicitudBecaId,
                        principalTable: "SolicitudesBeca",
                        principalColumn: "SolicitudBecaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudTiposApoyo",
                columns: table => new
                {
                    SolicitudTipoApoyoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SolicitudBecaId = table.Column<int>(type: "int", nullable: false),
                    TipoApoyoId = table.Column<int>(type: "int", nullable: false),
                    DescripcionOtroApoyo = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    MontoEstimado = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudTiposApoyo", x => x.SolicitudTipoApoyoId);
                    table.ForeignKey(
                        name: "FK_SolicitudTiposApoyo_SolicitudesBeca_SolicitudBecaId",
                        column: x => x.SolicitudBecaId,
                        principalTable: "SolicitudesBeca",
                        principalColumn: "SolicitudBecaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SolicitudTiposApoyo_TiposApoyo_TipoApoyoId",
                        column: x => x.TipoApoyoId,
                        principalTable: "TiposApoyo",
                        principalColumn: "TipoApoyoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VisitasFamiliares",
                columns: table => new
                {
                    VisitaFamiliarId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SolicitudBecaId = table.Column<int>(type: "int", nullable: false),
                    TipoVisita = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LugarEntrevista = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    FechaVisita = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HoraVisita = table.Column<TimeSpan>(type: "time", nullable: true),
                    PersonaEntrevistada = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ParentescoEntrevistado = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    ActitudFamilia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApreciacionGeneral = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecomendacionJunta = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RealizadaPor = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Firma = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ObservacionesFinales = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitasFamiliares", x => x.VisitaFamiliarId);
                    table.ForeignKey(
                        name: "FK_VisitasFamiliares_SolicitudesBeca_SolicitudBecaId",
                        column: x => x.SolicitudBecaId,
                        principalTable: "SolicitudesBeca",
                        principalColumn: "SolicitudBecaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Aportes",
                columns: table => new
                {
                    AporteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BenefactorId = table.Column<int>(type: "int", nullable: false),
                    BecaId = table.Column<int>(type: "int", nullable: true),
                    FechaAporte = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TipoAporte = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    FormaPago = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    Periodo = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    NumeroComprobante = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aportes", x => x.AporteId);
                    table.ForeignKey(
                        name: "FK_Aportes_Becas_BecaId",
                        column: x => x.BecaId,
                        principalTable: "Becas",
                        principalColumn: "BecaId");
                    table.ForeignKey(
                        name: "FK_Aportes_Benefactores_BenefactorId",
                        column: x => x.BenefactorId,
                        principalTable: "Benefactores",
                        principalColumn: "BenefactorId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RenovacionesBeca",
                columns: table => new
                {
                    RenovacionBecaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BecaId = table.Column<int>(type: "int", nullable: false),
                    CentroEducativoAnteriorId = table.Column<int>(type: "int", nullable: true),
                    CentroEducativoNuevoId = table.Column<int>(type: "int", nullable: true),
                    AnioRenovacion = table.Column<int>(type: "int", nullable: false),
                    AnioCursadoAnterior = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AnioACursar = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PromedioFinalAnterior = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    TipoApoyoRecibido = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    MotivoRenovacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstadoRenovacion = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Recomendaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaDecision = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RenovacionesBeca", x => x.RenovacionBecaId);
                    table.ForeignKey(
                        name: "FK_RenovacionesBeca_Becas_BecaId",
                        column: x => x.BecaId,
                        principalTable: "Becas",
                        principalColumn: "BecaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RenovacionesBeca_CentrosEducativos_CentroEducativoAnteriorId",
                        column: x => x.CentroEducativoAnteriorId,
                        principalTable: "CentrosEducativos",
                        principalColumn: "CentroEducativoId");
                    table.ForeignKey(
                        name: "FK_RenovacionesBeca_CentrosEducativos_CentroEducativoNuevoId",
                        column: x => x.CentroEducativoNuevoId,
                        principalTable: "CentrosEducativos",
                        principalColumn: "CentroEducativoId");
                });

            migrationBuilder.CreateTable(
                name: "SeguimientoAcademico",
                columns: table => new
                {
                    SeguimientoAcademicoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EstudianteId = table.Column<int>(type: "int", nullable: false),
                    BecaId = table.Column<int>(type: "int", nullable: true),
                    CentroEducativoId = table.Column<int>(type: "int", nullable: true),
                    Anio = table.Column<int>(type: "int", nullable: false),
                    Grado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NivelEducativo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Promedio = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    EstadoAcademico = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeguimientoAcademico", x => x.SeguimientoAcademicoId);
                    table.ForeignKey(
                        name: "FK_SeguimientoAcademico_Becas_BecaId",
                        column: x => x.BecaId,
                        principalTable: "Becas",
                        principalColumn: "BecaId");
                    table.ForeignKey(
                        name: "FK_SeguimientoAcademico_CentrosEducativos_CentroEducativoId",
                        column: x => x.CentroEducativoId,
                        principalTable: "CentrosEducativos",
                        principalColumn: "CentroEducativoId");
                    table.ForeignKey(
                        name: "FK_SeguimientoAcademico_Estudiantes_EstudianteId",
                        column: x => x.EstudianteId,
                        principalTable: "Estudiantes",
                        principalColumn: "EstudianteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ListaCotejoEvaluacion",
                columns: table => new
                {
                    ListaCotejoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EvaluacionBecaId = table.Column<int>(type: "int", nullable: false),
                    Rubro = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Completado = table.Column<bool>(type: "bit", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListaCotejoEvaluacion", x => x.ListaCotejoId);
                    table.ForeignKey(
                        name: "FK_ListaCotejoEvaluacion_EvaluacionesBeca_EvaluacionBecaId",
                        column: x => x.EvaluacionBecaId,
                        principalTable: "EvaluacionesBeca",
                        principalColumn: "EvaluacionBecaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApoyosExternos",
                columns: table => new
                {
                    ApoyoExternoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VisitaFamiliarId = table.Column<int>(type: "int", nullable: false),
                    NombreBeneficiado = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Parentesco = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    Institucion = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    TipoAyuda = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PorcentajeApoyo = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    TerminoEstudios = table.Column<bool>(type: "bit", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApoyosExternos", x => x.ApoyoExternoId);
                    table.ForeignKey(
                        name: "FK_ApoyosExternos_VisitasFamiliares_VisitaFamiliarId",
                        column: x => x.VisitaFamiliarId,
                        principalTable: "VisitasFamiliares",
                        principalColumn: "VisitaFamiliarId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BienesFamiliares",
                columns: table => new
                {
                    BienFamiliarId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VisitaFamiliarId = table.Column<int>(type: "int", nullable: false),
                    TipoBien = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Cantidad = table.Column<int>(type: "int", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BienesFamiliares", x => x.BienFamiliarId);
                    table.ForeignKey(
                        name: "FK_BienesFamiliares_VisitasFamiliares_VisitaFamiliarId",
                        column: x => x.VisitaFamiliarId,
                        principalTable: "VisitasFamiliares",
                        principalColumn: "VisitaFamiliarId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GrupoFamiliar",
                columns: table => new
                {
                    GrupoFamiliarId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VisitaFamiliarId = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Parentesco = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    Edad = table.Column<int>(type: "int", nullable: true),
                    LugarTrabajoEstudio = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Ingresos = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    GastosColegiatura = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    ViveConFamilia = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrupoFamiliar", x => x.GrupoFamiliarId);
                    table.ForeignKey(
                        name: "FK_GrupoFamiliar_VisitasFamiliares_VisitaFamiliarId",
                        column: x => x.VisitaFamiliarId,
                        principalTable: "VisitasFamiliares",
                        principalColumn: "VisitaFamiliarId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SituacionEconomica",
                columns: table => new
                {
                    SituacionEconomicaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VisitaFamiliarId = table.Column<int>(type: "int", nullable: false),
                    TotalIngresos = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    GastoAlimentacion = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    GastoVivienda = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    GastoLuz = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    GastoTelefono = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    GastoAgua = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    GastoTransporte = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    GastoEducacion = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    GastoDiversion = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    GastoSalud = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    GastoAhorro = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    OtrosGastos = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    TotalEgresos = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    TieneTarjetaCredito = table.Column<bool>(type: "bit", nullable: true),
                    TieneCreditoBancario = table.Column<bool>(type: "bit", nullable: true),
                    TieneEndeudamiento = table.Column<bool>(type: "bit", nullable: true),
                    DescripcionEndeudamiento = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SituacionEconomica", x => x.SituacionEconomicaId);
                    table.ForeignKey(
                        name: "FK_SituacionEconomica_VisitasFamiliares_VisitaFamiliarId",
                        column: x => x.VisitaFamiliarId,
                        principalTable: "VisitasFamiliares",
                        principalColumn: "VisitaFamiliarId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Viviendas",
                columns: table => new
                {
                    ViviendaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VisitaFamiliarId = table.Column<int>(type: "int", nullable: false),
                    TipoVivienda = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PagoMensual = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    AmortizacionMensual = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    MostroRecibo = table.Column<bool>(type: "bit", nullable: true),
                    DescripcionVivienda = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Viviendas", x => x.ViviendaId);
                    table.ForeignKey(
                        name: "FK_Viviendas_VisitasFamiliares_VisitaFamiliarId",
                        column: x => x.VisitaFamiliarId,
                        principalTable: "VisitasFamiliares",
                        principalColumn: "VisitaFamiliarId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Aportes_BecaId",
                table: "Aportes",
                column: "BecaId");

            migrationBuilder.CreateIndex(
                name: "IX_Aportes_BenefactorId",
                table: "Aportes",
                column: "BenefactorId");

            migrationBuilder.CreateIndex(
                name: "IX_ApoyosExternos_VisitaFamiliarId",
                table: "ApoyosExternos",
                column: "VisitaFamiliarId");

            migrationBuilder.CreateIndex(
                name: "IX_Becas_EstudianteId",
                table: "Becas",
                column: "EstudianteId");

            migrationBuilder.CreateIndex(
                name: "IX_Becas_SolicitudBecaId",
                table: "Becas",
                column: "SolicitudBecaId");

            migrationBuilder.CreateIndex(
                name: "IX_BienesFamiliares_VisitaFamiliarId",
                table: "BienesFamiliares",
                column: "VisitaFamiliarId");

            migrationBuilder.CreateIndex(
                name: "IX_Encargados_EstudianteId",
                table: "Encargados",
                column: "EstudianteId");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluacionesBeca_SolicitudBecaId",
                table: "EvaluacionesBeca",
                column: "SolicitudBecaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GrupoFamiliar_VisitaFamiliarId",
                table: "GrupoFamiliar",
                column: "VisitaFamiliarId");

            migrationBuilder.CreateIndex(
                name: "IX_ListaCotejoEvaluacion_EvaluacionBecaId",
                table: "ListaCotejoEvaluacion",
                column: "EvaluacionBecaId");

            migrationBuilder.CreateIndex(
                name: "IX_RenovacionesBeca_BecaId",
                table: "RenovacionesBeca",
                column: "BecaId");

            migrationBuilder.CreateIndex(
                name: "IX_RenovacionesBeca_CentroEducativoAnteriorId",
                table: "RenovacionesBeca",
                column: "CentroEducativoAnteriorId");

            migrationBuilder.CreateIndex(
                name: "IX_RenovacionesBeca_CentroEducativoNuevoId",
                table: "RenovacionesBeca",
                column: "CentroEducativoNuevoId");

            migrationBuilder.CreateIndex(
                name: "IX_SeguimientoAcademico_BecaId",
                table: "SeguimientoAcademico",
                column: "BecaId");

            migrationBuilder.CreateIndex(
                name: "IX_SeguimientoAcademico_CentroEducativoId",
                table: "SeguimientoAcademico",
                column: "CentroEducativoId");

            migrationBuilder.CreateIndex(
                name: "IX_SeguimientoAcademico_EstudianteId",
                table: "SeguimientoAcademico",
                column: "EstudianteId");

            migrationBuilder.CreateIndex(
                name: "IX_SituacionEconomica_VisitaFamiliarId",
                table: "SituacionEconomica",
                column: "VisitaFamiliarId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesBeca_CentroEducativoId",
                table: "SolicitudesBeca",
                column: "CentroEducativoId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesBeca_EstudianteId",
                table: "SolicitudesBeca",
                column: "EstudianteId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudTiposApoyo_SolicitudBecaId",
                table: "SolicitudTiposApoyo",
                column: "SolicitudBecaId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudTiposApoyo_TipoApoyoId",
                table: "SolicitudTiposApoyo",
                column: "TipoApoyoId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioRoles_RolId",
                table: "UsuarioRoles",
                column: "RolId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioRoles_UsuarioId",
                table: "UsuarioRoles",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitasFamiliares_SolicitudBecaId",
                table: "VisitasFamiliares",
                column: "SolicitudBecaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Viviendas_VisitaFamiliarId",
                table: "Viviendas",
                column: "VisitaFamiliarId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Aportes");

            migrationBuilder.DropTable(
                name: "ApoyosExternos");

            migrationBuilder.DropTable(
                name: "BienesFamiliares");

            migrationBuilder.DropTable(
                name: "Encargados");

            migrationBuilder.DropTable(
                name: "GrupoFamiliar");

            migrationBuilder.DropTable(
                name: "ListaCotejoEvaluacion");

            migrationBuilder.DropTable(
                name: "LogsAuditoria");

            migrationBuilder.DropTable(
                name: "RenovacionesBeca");

            migrationBuilder.DropTable(
                name: "SeguimientoAcademico");

            migrationBuilder.DropTable(
                name: "SituacionEconomica");

            migrationBuilder.DropTable(
                name: "SolicitudTiposApoyo");

            migrationBuilder.DropTable(
                name: "UsuarioRoles");

            migrationBuilder.DropTable(
                name: "Viviendas");

            migrationBuilder.DropTable(
                name: "Benefactores");

            migrationBuilder.DropTable(
                name: "EvaluacionesBeca");

            migrationBuilder.DropTable(
                name: "Becas");

            migrationBuilder.DropTable(
                name: "TiposApoyo");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "VisitasFamiliares");

            migrationBuilder.DropTable(
                name: "SolicitudesBeca");

            migrationBuilder.DropTable(
                name: "CentrosEducativos");

            migrationBuilder.DropTable(
                name: "Estudiantes");
        }
    }
}
