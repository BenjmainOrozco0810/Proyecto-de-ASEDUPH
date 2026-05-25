using ASEDUPH_V2_API.Controllers;
using ASEDUPH_V2_API.Data;
using ASEDUPH_V2_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ASEDUPH_V2_API.Tests
{
    public class EstudiantesControllerTests
    {
        private AseduphDbContext CrearContextoEnMemoria()
        {
            var options = new DbContextOptionsBuilder<AseduphDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AseduphDbContext(options);
        }

        // ── GET ──────────────────────────────────────────────────────

        [Fact]
        public async Task GetEstudiantes_DebeRetornarListaVacia_CuandoNoHayEstudiantes()
        {
            // Arrange
            var context = CrearContextoEnMemoria();
            var controller = new EstudiantesController(context);

            // Act
            var resultado = await controller.GetEstudiantes();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var lista = Assert.IsType<List<Estudiante>>(okResult.Value);
            Assert.Empty(lista);
        }

        [Fact]
        public async Task GetEstudiantes_DebeRetornarSoloActivos()
        {
            // Arrange
            var context = CrearContextoEnMemoria();
            context.Estudiantes.AddRange(
                new Estudiante { NombreCompleto = "Juan Pérez", Estado = "Activo" },
                new Estudiante { NombreCompleto = "María López", Estado = "Inactivo" }
            );
            await context.SaveChangesAsync();
            var controller = new EstudiantesController(context);

            // Act
            var resultado = await controller.GetEstudiantes();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var lista = Assert.IsType<List<Estudiante>>(okResult.Value);
            Assert.Single(lista);
            Assert.Equal("Juan Pérez", lista[0].NombreCompleto);
        }

        [Fact]
        public async Task GetEstudiante_DebeRetornar404_CuandoNoExiste()
        {
            // Arrange
            var context = CrearContextoEnMemoria();
            var controller = new EstudiantesController(context);

            // Act
            var resultado = await controller.GetEstudiante(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(resultado.Result);
        }

        [Fact]
        public async Task GetEstudiante_DebeRetornarEstudiante_CuandoExiste()
        {
            // Arrange
            var context = CrearContextoEnMemoria();
            var estudiante = new Estudiante
            {
                NombreCompleto = "Carlos García",
                Estado = "Activo"
            };
            context.Estudiantes.Add(estudiante);
            await context.SaveChangesAsync();
            var controller = new EstudiantesController(context);

            // Act
            var resultado = await controller.GetEstudiante(estudiante.EstudianteId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var est = Assert.IsType<Estudiante>(okResult.Value);
            Assert.Equal("Carlos García", est.NombreCompleto);
        }

        // ── POST ─────────────────────────────────────────────────────

        [Fact]
        public async Task PostEstudiante_DebeCrearEstudiante_CuandoDatosValidos()
        {
            // Arrange
            var context = CrearContextoEnMemoria();
            var controller = new EstudiantesController(context);
            var nuevoEstudiante = new Estudiante
            {
                NombreCompleto = "Ana Martínez",
                Sexo = "Femenino",
                Edad = 12,
                Municipio = "Guatemala",
                Departamento = "Guatemala"
            };

            // Act
            var resultado = await controller.PostEstudiante(nuevoEstudiante);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(resultado.Result);
            var est = Assert.IsType<Estudiante>(createdResult.Value);
            Assert.Equal("Ana Martínez", est.NombreCompleto);
            Assert.Equal("Activo", est.Estado);
        }

        [Fact]
        public async Task PostEstudiante_DebeAsignarEstadoActivo_Automaticamente()
        {
            // Arrange
            var context = CrearContextoEnMemoria();
            var controller = new EstudiantesController(context);
            var nuevoEstudiante = new Estudiante
            {
                NombreCompleto = "Pedro Ramírez"
            };

            // Act
            var resultado = await controller.PostEstudiante(nuevoEstudiante);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(resultado.Result);
            var est = Assert.IsType<Estudiante>(createdResult.Value);
            Assert.Equal("Activo", est.Estado);
        }

        // ── PUT ──────────────────────────────────────────────────────

        [Fact]
        public async Task PutEstudiante_DebeActualizar_CuandoDatosValidos()
        {
            // Arrange
            var context = CrearContextoEnMemoria();
            var estudiante = new Estudiante
            {
                NombreCompleto = "Luis Hernández",
                Estado = "Activo"
            };
            context.Estudiantes.Add(estudiante);
            await context.SaveChangesAsync();
            var controller = new EstudiantesController(context);

            estudiante.NombreCompleto = "Luis Alberto Hernández";

            // Act
            var resultado = await controller.PutEstudiante(estudiante.EstudianteId, estudiante);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task PutEstudiante_DebeRetornar404_CuandoNoExiste()
        {
            // Arrange
            var context = CrearContextoEnMemoria();
            var controller = new EstudiantesController(context);
            var estudiante = new Estudiante
            {
                EstudianteId = 999,
                NombreCompleto = "No Existe"
            };

            // Act
            var resultado = await controller.PutEstudiante(999, estudiante);

            // Assert
            Assert.IsType<NotFoundObjectResult>(resultado);
        }

        // ── DELETE ───────────────────────────────────────────────────

        [Fact]
        public async Task DeleteEstudiante_DebeDesactivar_CuandoExiste()
        {
            // Arrange
            var context = CrearContextoEnMemoria();
            var estudiante = new Estudiante
            {
                NombreCompleto = "Rosa Díaz",
                Estado = "Activo"
            };
            context.Estudiantes.Add(estudiante);
            await context.SaveChangesAsync();
            var controller = new EstudiantesController(context);

            // Act
            var resultado = await controller.DeleteEstudiante(estudiante.EstudianteId);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
            var estudianteActualizado = await context.Estudiantes.FindAsync(estudiante.EstudianteId);
            Assert.Equal("Inactivo", estudianteActualizado!.Estado);
        }

        [Fact]
        public async Task DeleteEstudiante_DebeRetornar404_CuandoNoExiste()
        {
            // Arrange
            var context = CrearContextoEnMemoria();
            var controller = new EstudiantesController(context);

            // Act
            var resultado = await controller.DeleteEstudiante(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(resultado);
        }
    }
}
