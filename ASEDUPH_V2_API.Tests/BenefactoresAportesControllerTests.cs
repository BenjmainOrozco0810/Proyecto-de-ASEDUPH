using ASEDUPH_V2_API.Controllers;
using ASEDUPH_V2_API.Data;
using ASEDUPH_V2_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ASEDUPH_V2_API.Tests
{
    public class BenefactoresControllerTests
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
        public async Task GetBenefactores_DebeRetornarSoloActivos()
        {
            // Arrange
            var context = CrearContextoEnMemoria();
            context.Benefactores.AddRange(
                new Benefactor { NombreCompleto = "Roberto García", Estado = "Activo" },
                new Benefactor { NombreCompleto = "María López", Estado = "Inactivo" }
            );
            await context.SaveChangesAsync();
            var controller = new BenefactoresController(context);

            // Act
            var resultado = await controller.GetBenefactores();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var lista = Assert.IsType<List<Benefactor>>(okResult.Value);
            Assert.Single(lista);
            Assert.Equal("Roberto García", lista[0].NombreCompleto);
        }

        [Fact]
        public async Task GetBenefactor_DebeRetornar404_CuandoNoExiste()
        {
            // Arrange
            var context = CrearContextoEnMemoria();
            var controller = new BenefactoresController(context);

            // Act
            var resultado = await controller.GetBenefactor(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(resultado.Result);
        }

        // ── POST ─────────────────────────────────────────────────────

        [Fact]
        public async Task PostBenefactor_DebeCrear_CuandoDatosValidos()
        {
            // Arrange
            var context = CrearContextoEnMemoria();
            var controller = new BenefactoresController(context);
            var benefactor = new Benefactor
            {
                NombreCompleto = "Carlos Pérez",
                TipoBenefactor = "Padrino",
                Telefono = "55123456"
            };

            // Act
            var resultado = await controller.PostBenefactor(benefactor);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(resultado.Result);
            var ben = Assert.IsType<Benefactor>(createdResult.Value);
            Assert.Equal("Activo", ben.Estado);
        }

        [Fact]
        public async Task PostBenefactor_DebeRetornarBadRequest_CuandoTipoInvalido()
        {
            // Arrange
            var context = CrearContextoEnMemoria();
            var controller = new BenefactoresController(context);
            var benefactor = new Benefactor
            {
                NombreCompleto = "Juan López",
                TipoBenefactor = "TipoInvalido"
            };

            // Act
            var resultado = await controller.PostBenefactor(benefactor);

            // Assert
            Assert.IsType<BadRequestObjectResult>(resultado.Result);
        }

        // ── DELETE ───────────────────────────────────────────────────

        [Fact]
        public async Task DeleteBenefactor_DebeDesactivar_CuandoExiste()
        {
            // Arrange
            var context = CrearContextoEnMemoria();
            var benefactor = new Benefactor
            {
                NombreCompleto = "Ana Rodríguez",
                Estado = "Activo"
            };
            context.Benefactores.Add(benefactor);
            await context.SaveChangesAsync();
            var controller = new BenefactoresController(context);

            // Act
            var resultado = await controller.DeleteBenefactor(benefactor.BenefactorId);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
            var actualizado = await context.Benefactores.FindAsync(benefactor.BenefactorId);
            Assert.Equal("Inactivo", actualizado!.Estado);
        }
    }

    public class AportesControllerTests
    {
        private AseduphDbContext CrearContextoEnMemoria()
        {
            var options = new DbContextOptionsBuilder<AseduphDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AseduphDbContext(options);
        }

        private async Task<Benefactor> CrearBenefactorActivo(AseduphDbContext context)
        {
            var benefactor = new Benefactor
            {
                NombreCompleto = "Benefactor Prueba",
                Estado = "Activo"
            };
            context.Benefactores.Add(benefactor);
            await context.SaveChangesAsync();
            return benefactor;
        }

        // ── POST ─────────────────────────────────────────────────────

        [Fact]
        public async Task PostAporte_DebeCrear_CuandoDatosValidos()
        {
            // Arrange
            var context = CrearContextoEnMemoria();
            var benefactor = await CrearBenefactorActivo(context);
            var controller = new AportesController(context);

            var aporte = new Aporte
            {
                BenefactorId = benefactor.BenefactorId,
                FechaAporte = DateTime.Now,
                Monto = 500,
                TipoAporte = "Económico",
                FormaPago = "Transferencia",
                Periodo = "Mensual"
            };

            // Act
            var resultado = await controller.PostAporte(aporte);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(resultado.Result);
            var apo = Assert.IsType<Aporte>(createdResult.Value);
            Assert.Equal(500, apo.Monto);
        }

        [Fact]
        public async Task PostAporte_DebeRetornarBadRequest_CuandoMontoEsCero()
        {
            // Arrange
            var context = CrearContextoEnMemoria();
            var benefactor = await CrearBenefactorActivo(context);
            var controller = new AportesController(context);

            var aporte = new Aporte
            {
                BenefactorId = benefactor.BenefactorId,
                FechaAporte = DateTime.Now,
                Monto = 0 // Monto inválido
            };

            // Act
            var resultado = await controller.PostAporte(aporte);

            // Assert
            Assert.IsType<BadRequestObjectResult>(resultado.Result);
        }

        [Fact]
        public async Task PostAporte_DebeRetornarBadRequest_CuandoBenefactorNoExiste()
        {
            // Arrange
            var context = CrearContextoEnMemoria();
            var controller = new AportesController(context);

            var aporte = new Aporte
            {
                BenefactorId = 999, // No existe
                FechaAporte = DateTime.Now,
                Monto = 500
            };

            // Act
            var resultado = await controller.PostAporte(aporte);

            // Assert
            Assert.IsType<BadRequestObjectResult>(resultado.Result);
        }

        // ── DELETE ───────────────────────────────────────────────────

        [Fact]
        public async Task DeleteAporte_DebeEliminar_CuandoExiste()
        {
            // Arrange
            var context = CrearContextoEnMemoria();
            var benefactor = await CrearBenefactorActivo(context);
            var aporte = new Aporte
            {
                BenefactorId = benefactor.BenefactorId,
                FechaAporte = DateTime.Now,
                Monto = 300
            };
            context.Aportes.Add(aporte);
            await context.SaveChangesAsync();
            var controller = new AportesController(context);

            // Act
            var resultado = await controller.DeleteAporte(aporte.AporteId);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
            var aporteEliminado = await context.Aportes.FindAsync(aporte.AporteId);
            Assert.Null(aporteEliminado);
        }
    }
}
