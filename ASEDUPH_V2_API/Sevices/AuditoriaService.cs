using ASEDUPH_V2_API.Data;
using ASEDUPH_V2_API.Models;

namespace ASEDUPH_V2_API.Services
{
    public class AuditoriaService
    {
        private readonly AseduphDbContext _context;

        public AuditoriaService(AseduphDbContext context)
        {
            _context = context;
        }

        public async Task RegistrarAsync(
            string accion,
            string modulo,
            string descripcion,
            string? entidadAfectada = null,
            int? entidadId = null,
            int? usuarioId = null,
            string? nombreUsuario = null,
            string? resultado = "Exitoso",
            string? ip = null)
        {
            try
            {
                var log = new LogAuditoria
                {
                    Accion = accion,
                    Modulo = modulo,
                    Descripcion = descripcion,
                    EntidadAfectada = entidadAfectada,
                    EntidadId = entidadId,
                    UsuarioId = usuarioId,
                    NombreUsuario = nombreUsuario,
                    FechaHora = DateTime.Now,
                    Resultado = resultado,
                    DireccionIP = ip
                };

                _context.LogsAuditoria.Add(log);
                await _context.SaveChangesAsync();
            }
            catch
            {
                // El log nunca debe interrumpir el flujo principal
            }
        }
    }
}
