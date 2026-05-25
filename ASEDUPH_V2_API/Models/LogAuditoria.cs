using System.ComponentModel.DataAnnotations;

namespace ASEDUPH_V2_API.Models
{
    public class LogAuditoria
    {
        [Key]
        public int LogAuditoriaId { get; set; }

        // Usuario que realizó la acción
        public int? UsuarioId { get; set; }
        public string? NombreUsuario { get; set; }

        // Detalle de la acción
        public string Accion { get; set; } = string.Empty;      // Crear, Editar, Eliminar, CambiarEstado
        public string Modulo { get; set; } = string.Empty;      // Estudiantes, Becas, Solicitudes...
        public string? Descripcion { get; set; }                 // Descripción detallada
        public string? EntidadAfectada { get; set; }             // Nombre del registro afectado
        public int? EntidadId { get; set; }                      // ID del registro afectado

        // Metadata
        public DateTime FechaHora { get; set; } = DateTime.Now;
        public string? DireccionIP { get; set; }
        public string? Resultado { get; set; } = "Exitoso";     // Exitoso, Fallido
    }
}
