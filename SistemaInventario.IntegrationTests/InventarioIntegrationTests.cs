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
            var nuevoProducto = new ProductoCreacionDto
            {
                SKU = "TEST-01",
                Nombre = "Producto Integración",
                StockActual = 10
            };

            var response = await _client.PostAsJsonAsync("/api/productos", nuevoProducto);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var responseString = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseString);
            var idGenerado = jsonResponse.GetProperty("id").GetInt32();

            var getResponse = await _client.GetAsync($"/api/productos/{idGenerado}");
            Assert.True(getResponse.IsSuccessStatusCode);
        }

        [Fact]
        public async Task TC_INT_002_RegistrarSalida_ActualizaStock_Correctamente()
        {
            var productoId = 1;

            var movimientoSalida = new MovimientoCreacionDto
            {
                ProductoID = productoId, // Corregido: ID en mayúscula según tu DTO
                UsuarioID = 1,           // Añadido: Es un campo [Required] en tu DTO
                TipoMovimiento = "Salida",
                Cantidad = 5
            };

            var responseMovimiento = await _client.PostAsJsonAsync("/api/movimientos", movimientoSalida);

            Assert.Equal(HttpStatusCode.OK, responseMovimiento.StatusCode);

            var getProductoResponse = await _client.GetFromJsonAsync<ProductoDto>($"/api/productos/{productoId}");

            Assert.NotNull(getProductoResponse); // Soluciona la advertencia CS8602
            Assert.Equal(45, getProductoResponse.StockActual);
        }

        [Fact]
        public async Task TC_INT_003_SalidaMayorAlStock_AplicaRollback_Retorna400()
        {
            var productoId = 2;
            var stockInicial = 10;

            var movimientoSalidaInvalido = new MovimientoCreacionDto
            {
                ProductoID = productoId, // Corregido
                UsuarioID = 1,           // Añadido
                TipoMovimiento = "Salida",
                Cantidad = 15
            };

            var response = await _client.PostAsJsonAsync("/api/movimientos", movimientoSalidaInvalido);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var errorContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("Stock insuficiente", errorContent);

            var getProductoResponse = await _client.GetFromJsonAsync<ProductoDto>($"/api/productos/{productoId}");

            Assert.NotNull(getProductoResponse); // Soluciona la advertencia CS8602
            Assert.Equal(stockInicial, getProductoResponse.StockActual);
        }
    }
}