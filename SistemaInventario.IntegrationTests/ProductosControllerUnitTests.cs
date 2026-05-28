using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using SistemaInventario.Api.Controllers;
using SistemaInventario.Api.Data;
using SistemaInventario.Api.DTOs;
using SistemaInventario.Api.Models;
using Xunit;

namespace SistemaInventario.IntegrationTests // Reutilizamos nuestro proyecto actual
{
    public class ProductosControllerUnitTests
    {
        // Método auxiliar para no repetir código: Crea un DbContext falso
        private Mock<ApplicationDbContext> ObtenerMockContextoVacio()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().Options;
            return new Mock<ApplicationDbContext>(options);
        }

        // -------------------------------------------------------------
        // 1. EJEMPLO DE STUB (Proveer datos falsos)
        // -------------------------------------------------------------
        [Fact]
        public async Task GetProductos_RetornaListaDeProductos_UsandoStub()
        {
            // Arrange: Crear el STUB de datos en memoria
            // Estos datos NO existen en la base de datos real.
            var productosStub = new List<Producto>
            {
                new Producto { ProductoID = 1, SKU = "LAP-01", Nombre = "Laptop (Ejemplo STUB)", StockActual = 10 },
                new Producto { ProductoID = 2, SKU = "MOU-01", Nombre = "Mouse (Ejemplo STUB)", StockActual = 50 }
            };

            var mockContext = ObtenerMockContextoVacio();
            // Le decimos al contexto falso que cuando alguien pida "Productos", devuelva nuestro STUB
            mockContext.Setup(c => c.Productos).ReturnsDbSet(productosStub);

            // Instanciamos el controlador inyectándole el contexto falso en lugar del real
            var controller = new ProductosController(mockContext.Object);

            // Act: Llamamos al método
            var result = await controller.GetProductos();

            // Assert: Verificamos que el controlador procesó el STUB correctamente
            var actionResult = Assert.IsType<ActionResult<IEnumerable<ProductoDto>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnProductos = Assert.IsAssignableFrom<IEnumerable<ProductoDto>>(okResult.Value);

            Assert.Equal(2, returnProductos.Count());
        }

        // -------------------------------------------------------------
        // 2. EJEMPLO DE MOCK (Verificar comportamiento)
        // -------------------------------------------------------------
        [Fact]
        public async Task PostProducto_GuardaNuevoProducto_VerificaComportamientoConMock()
        {
            // Arrange
            var mockContext = ObtenerMockContextoVacio();
            // Preparamos una tabla virtual vacía
            mockContext.Setup(c => c.Productos).ReturnsDbSet(new List<Producto>());

            var nuevoProductoDto = new ProductoCreacionDto
            {
                SKU = "TEC-01",
                Nombre = "Teclado Mecánico",
                StockActual = 15
            };

            var controller = new ProductosController(mockContext.Object);

            // Act
            var result = await controller.PostProducto(nuevoProductoDto);

            // Assert: VERIFICACIÓN DEL MOCK
            // Aquí no verificamos los datos, verificamos EL COMPORTAMIENTO.
            // Validamos que el controlador realmente ejecutó la instrucción SaveChangesAsync() de EF Core
            // exactamente 1 vez. Si esto pasa, sabemos que el controlador hace su trabajo de guardado.
            mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            var actionResult = Assert.IsType<ActionResult<ProductoDto>>(result);
            Assert.IsType<CreatedAtActionResult>(actionResult.Result);
        }
    }
}