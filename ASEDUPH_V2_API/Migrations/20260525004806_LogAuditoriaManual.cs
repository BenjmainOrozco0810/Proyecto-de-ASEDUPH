using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASEDUPH_V2_API.Migrations
{
    /// <inheritdoc />
    public partial class LogAuditoriaManual : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Tabla creada manualmente en SQL Server
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "LogsAuditoria");
        }
    }
}
