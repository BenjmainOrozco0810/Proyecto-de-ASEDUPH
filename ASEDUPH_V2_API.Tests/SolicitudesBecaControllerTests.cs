using ASEDUPH_V2_API.Controllers;
using ASEDUPH_V2_API.Data;
using ASEDUPH_V2_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ASEDUPH_V2_API.Tests
{
    public class SolicitudesBecaControllerTests
    {
        private AseduphDbContext CrearContextoEnMemoria()
        {
            var options = new DbContextOptionsBuilder<AseduphDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AseduphDbContext(options);
        }

        private async Task<Estudiante> CrearEstudianteActivo(AseduphDbContext context)
        {
            var estudiante = new Estudiante
            {
                NombreCompleto = "Estudiante de Prueba",
                Estado = "Activo"
            };
            context.Estudiantes.Add(estudiante);
            await context.SaveChangesAsync();
            return estudiante;
        }

        // ── GET ──────────────────────────────────────────────────────

        [Fact]
        public async Task GetSolicitudesBeca_DebeRetornarListaVacia_CuandoNoHaySolicitudes()
        {
            // Arrange
            var context = CrearContextoEnMemoria();
            var controller = new SolicitudesBecaController(context);

            // Act
            var resultado = await controller.GetSolicitudesBeca();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var lista = Assert.IsType<List<SolicitudBeca>>(okResult.Value);
            Assert.Empty(lista);
        }

        [Fact]
        public async Task GetSolicitudBeca_DebeRetornar404_CuandoNoExiste()
        {
            // Arrange
            var context = CrearContextoEnMemoria();
            var controller = new SolicitudesBecaController(context);

            // Act
            var resultado = await controller.GetSolicitudBeca(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(resultado.Result);
        }

        // ── POST ─────────────────────────────────────────────────────

        [Fact]
        public async Task PostSolicitudBeca_DebeCrear_CuandoEstudianteExiste()
        {
            // Arrange
            var context = CrearContextoEnMemoria();
            var estudiante = await CrearEstudianteActivo(context);
            var controller = new SolicitudesBecaController(context);

            var solicitud = new SolicitudBeca
            {
                EstudianteId = estudiante.EstudianteId,
                AnioSolicitud = 2025,
                FechaSolicitud = DateTime.Now,
                NivelEducativo = "Primaria"
            };

            // Act
            var resultado = await controller.PostSolicitudBeca(solicitud);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(resultado.Result);
            var sol = Assert.IsType<SolicitudBeca>(createdResult.Value);
            Assert.Equal("Pendiente", sol.EstadoSolicitud);
        }

        [Fact]
        public async Task PostSolicitudBeca_DebeAsignarEstadoPendiente_Automaticamente()
        {
            // Arrange
            var context = CrearContextoEnMemoria();
            var estudiante = await CrearEstudianteActivo(context);
            var controller = new SolicitudesBecaController(context);

            var solicitud = new SolicitudBeca
            {
                EstudianteId = estudiante.EstudianteId,
                AnioSolicitud = 2025,
                FechaSolicitud = DateTime.Now,
                EstadoSolicitud = "Aprobada" // Intentar asignar otro estado
            };

            // Act
            var resultado = await controller.PostSolicitudBeca(solicitud);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(resultado.Result);
            var sol = Assert.IsType<SolicitudBeca>(createdResult.Value);
            Assert.Equal("Pendiente", sol.EstadoSolicitud); // Debe ser Pendiente siempre
        }

        [Fact]
        public async Task PostSolicitudBeca_DebeRetornarBadRequest_CuandoEstudianteNoExiste()
        {
            // Arrange
            var context = CrearContextoEnMemoria();
            var controller = new SolicitudesBecaController(context);

            var solicitud = new SolicitudBeca
            {
                EstudianteId = 999, // No existe
                AnioSolicitud = 2025,
                FechaSolicitud = DateTime.Now
            };

            // Act
            var resultado = await controller.PostSolicitudBeca(solicitud);

            // Assert
            Assert.IsType<BadRequestObjectResult>(resultado.Result);
        }

        // ── PATCH Estado ─────────────────────────────────────────────

        [Fact]
        public async Task CambiarEstadoSolicitud_DebeActualizar_CuandoEstadoValido()
        {
            // Arrange
            var context = CrearContextoEnMemoria();
            var estudiante = await CrearEstudianteActivo(context);
            var solicitud = new SolicitudBeca
            {
                EstudianteId = estudiante.EstudianteId,
                AnioSolicitud = 2025,
                FechaSolicitud = DateTime.Now,
                EstadoSolicitud = "Pendiente"
            };
            context.SolicitudesBeca.Add(solicitud);
            await context.SaveChangesAsync();
            var controller = new SolicitudesBecaController(context);

            // Act
            var resultado = await controller.CambiarEstadoSolicitud(
                solicitud.SolicitudBecaId, "Aprobada");

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
            var solicitudActualizada = await context.SolicitudesBeca
                .FindAsync(solicitud.SolicitudBecaId);
            Assert.Equal("Aprobada", solicitudActualizada!.EstadoSolicitud);
        }

        [Fact]
        public async Task CambiarEstadoSolicitud_DebeRetornarBadRequest_CuandoEstadoInvalido()
        {
            // Arrange
            var context = CrearContextoEnMemoria();
            var estudiante = await CrearEstudianteActivo(context);
            var solicitud = new SolicitudBeca
            {
                EstudianteId = estudiante.EstudianteId,
                AnioSolicitud = 2025,
                FechaSolicitud = DateTime.Now,
                EstadoSolicitud = "Pendiente"
            };
            context.SolicitudesBeca.Add(solicitud);
            await context.SaveChangesAsync();
            var controller = new SolicitudesBecaController(context);

            // Act
            var resultado = await controller.CambiarEstadoSolicitud(
                solicitud.SolicitudBecaId, "EstadoInvalido");

            // Assert
            Assert.IsType<BadRequestObjectResult>(resultado);
        }

        // ── DELETE ───────────────────────────────────────────────────

        [Fact]
        public async Task DeleteSolicitudBeca_DebeCancelar_CuandoExiste()
        {
            // Arrange
            var context = CrearContextoEnMemoria();
            var estudiante = await CrearEstudianteActivo(context);
            var solicitud = new SolicitudBeca
            {
                EstudianteId = estudiante.EstudianteId,
                AnioSolicitud = 2025,
                FechaSolicitud = DateTime.Now,
                EstadoSolicitud = "Pendiente"
            };
            context.SolicitudesBeca.Add(solicitud);
            await context.SaveChangesAsync();
            var controller = new SolicitudesBecaController(context);

            // Act
            var resultado = await controller.DeleteSolicitudBeca(solicitud.SolicitudBecaId);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
            var solicitudActualizada = await context.SolicitudesBeca
                .FindAsync(solicitud.SolicitudBecaId);
            Assert.Equal("Cancelada", solicitudActualizada!.EstadoSolicitud);
        }
    }
}
