using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using SistemaInventario.Api.DTOs;

namespace SistemaInventario.IntegrationTests
{
    public class InventarioIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public InventarioIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task TC_INT_001_CrearProducto_Retorna201_Y_PersisteEnBaseDeDatos()
        {
            // SOLUCIÓN: Generar un SKU único usando un GUID para evitar el error 409 Conflict
            var skuUnico = $"TEST-{Guid.NewGuid().ToString().Substring(0, 6)}";

            var nuevoProducto = new ProductoCreacionDto
            {
                SKU = skuUnico,
                Nombre = "Producto Integración",
                StockActual = 10
            };

            var response = await _client.PostAsJsonAsync("/api/productos", nuevoProducto);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var productoCreado = await response.Content.ReadFromJsonAsync<ProductoDto>();
            Assert.NotNull(productoCreado);

            var getResponse = await _client.GetAsync($"/api/productos/{productoCreado.ProductoID}");
            Assert.True(getResponse.IsSuccessStatusCode);
        }

        [Fact]
        public async Task TC_INT_002_RegistrarSalida_ActualizaStock_Correctamente()
        {
            var productoId = 1;

            // SOLUCIÓN: Consultar el stock actual ANTES del movimiento
            var productoInicial = await _client.GetFromJsonAsync<ProductoDto>($"/api/productos/{productoId}");
            Assert.NotNull(productoInicial);
            var stockInicial = productoInicial.StockActual;
            var cantidadASalir = 5;

            var movimientoSalida = new MovimientoCreacionDto
            {
                ProductoID = productoId,
                UsuarioID = 1,
                TipoMovimiento = "Salida",
                Cantidad = cantidadASalir,
                ProveedorID = 1,
            };

            var responseMovimiento = await _client.PostAsJsonAsync("/api/MovimientosInventario", movimientoSalida);

            Assert.Equal(HttpStatusCode.Created, responseMovimiento.StatusCode);

            // Validar que el nuevo stock sea exactamente el (Stock que había - la cantidad que salió)
            var getProductoResponse = await _client.GetFromJsonAsync<ProductoDto>($"/api/productos/{productoId}");
            Assert.NotNull(getProductoResponse);
            Assert.Equal(stockInicial - cantidadASalir, getProductoResponse.StockActual);
        }

        [Fact]
        public async Task TC_INT_003_SalidaMayorAlStock_AplicaRollback_Retorna400()
        {
            var productoId = 2;

            // SOLUCIÓN: Consultar el stock actual ANTES del movimiento
            var productoInicial = await _client.GetFromJsonAsync<ProductoDto>($"/api/productos/{productoId}");
            Assert.NotNull(productoInicial);
            var stockInicial = productoInicial.StockActual;

            var movimientoSalidaInvalido = new MovimientoCreacionDto
            {
                ProductoID = productoId,
                UsuarioID = 1,
                TipoMovimiento = "Salida",
                // Aseguramos pedir más de lo que hay, sin importar cuánto sea el stock
                Cantidad = stockInicial + 10
            };

            var response = await _client.PostAsJsonAsync("/api/MovimientosInventario", movimientoSalidaInvalido);

            // Verificamos que sea rechazado
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var errorContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("No hay suficiente stock para el producto", errorContent);

            // Verificamos que el stock no cambió en absoluto (el rollback funcionó)
            var getProductoResponse = await _client.GetFromJsonAsync<ProductoDto>($"/api/productos/{productoId}");
            Assert.NotNull(getProductoResponse);
            Assert.Equal(stockInicial, getProductoResponse.StockActual);
        }
    }
}