using ASEDUPH_V2_API.Data;
using ASEDUPH_V2_API.Models;

namespace ASEDUPH_V2_API.Services
{
    public class AuditoriaService
    {
        private readonly AseduphDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditoriaService(AseduphDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
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
                var httpContext = _httpContextAccessor.HttpContext;

                if (httpContext != null)
                {
                    var user = httpContext.User;

                    // Buscar el username — el JWT lo guarda como "unique_name" o "name"
                    if (string.IsNullOrEmpty(nombreUsuario))
                    {
                        nombreUsuario =
                            user?.FindFirst("unique_name")?.Value ??
                            user?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value ??
                            user?.FindFirst("name")?.Value ??
                            user?.Identity?.Name;
                    }

                    // Buscar el ID — el JWT lo guarda como "nameid"
                    if (usuarioId == null)
                    {
                        var idStr =
                            user?.FindFirst("nameid")?.Value ??
                            user?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ??
                            user?.FindFirst("sub")?.Value;

                        if (!string.IsNullOrEmpty(idStr) && int.TryParse(idStr, out var parsedId))
                            usuarioId = parsedId;
                    }

                    // IP
                    if (string.IsNullOrEmpty(ip))
                        ip = httpContext.Connection.RemoteIpAddress?.ToString();
                }

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
