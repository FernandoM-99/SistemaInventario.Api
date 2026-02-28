// Controllers/ProductosController.cs
/// <summary>
/// Proporciona servicios para la gestión del catálogo de productos.
/// Incluye operaciones para la creación, consulta, actualización y eliminación de artículos e inventario.
/// </summary>
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.Api.Data;
using SistemaInventario.Api.DTOs; // Importar DTOs
using SistemaInventario.Api.Models;

namespace SistemaInventario.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Productos
        /// <summary>
        /// Obtiene la lista completa de productos registrados en el sistema.
        /// </summary>
        /// <returns>Una lista de objetos ProductoDto.</returns>
        /// <response code="200">Retorna la lista de productos con éxito.</response>

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> GetProductos()
        {
            var productos = await _context.Productos
                                        .Select(p => new ProductoDto
                                        {
                                            ProductoID = p.ProductoID,
                                            SKU = p.SKU,
                                            Nombre = p.Nombre,
                                            Descripcion = p.Descripcion,
                                            StockActual = p.StockActual
                                        })
                                        .ToListAsync();
            return Ok(productos);
        }

        // GET: api/Productos/5 
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductoDto>> GetProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            // Mapear a DTO
            var productoDto = new ProductoDto
            {
                ProductoID = producto.ProductoID,
                SKU = producto.SKU,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                StockActual = producto.StockActual
            };
            return Ok(productoDto);
        }

        // GET: api/Productos/buscar/LAP-001
        [HttpGet("buscar/{sku}")]
        public async Task<ActionResult<ProductoDto>> GetProductoPorSku(string sku)
        {
            var producto = await _context.Productos
                                .FirstOrDefaultAsync(p => p.SKU == sku);

            if (producto == null)
            {
                return NotFound("No se encontró ningún producto con ese SKU.");
            }

            // Mapear a DTO
            var productoDto = new ProductoDto
            {
                ProductoID = producto.ProductoID,
                SKU = producto.SKU,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                StockActual = producto.StockActual
            };
            return Ok(productoDto);
        }

        // POST: api/Productos
        // Para crear un nuevo producto
        [HttpPost]
        public async Task<ActionResult<ProductoDto>> PostProducto(ProductoCreacionDto productoDto)
        {
            // Validar si el SKU ya existe
            if (await _context.Productos.AnyAsync(p => p.SKU == productoDto.SKU))
            {
                return Conflict($"Ya existe un producto con el SKU '{productoDto.SKU}'.");
            }

            var producto = new Producto
            {
                SKU = productoDto.SKU,
                Nombre = productoDto.Nombre,
                Descripcion = productoDto.Descripcion,
                StockActual = productoDto.StockActual
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            // Devolver el producto creado mapeado a ProductoDto
            var nuevoProductoDto = new ProductoDto
            {
                ProductoID = producto.ProductoID, // El ID se genera en la BD
                SKU = producto.SKU,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                StockActual = producto.StockActual
            };

            // Retorna 201 Created y la URL al nuevo recurso
            return CreatedAtAction(nameof(GetProducto), new { id = nuevoProductoDto.ProductoID }, nuevoProductoDto);
        }

        // PUT: api/Productos/5
        // Para actualizar un producto existente
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducto(int id, ProductoCreacionDto productoDto)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound(); // 404 si no encuentra el producto
            }

            // Opcional: Validar si el SKU se está cambiando a uno ya existente
            if (producto.SKU != productoDto.SKU && await _context.Productos.AnyAsync(p => p.SKU == productoDto.SKU))
            {
                return Conflict($"Ya existe otro producto con el SKU '{productoDto.SKU}'.");
            }

            // Actualizar propiedades del modelo
            producto.SKU = productoDto.SKU;
            producto.Nombre = productoDto.Nombre;
            producto.Descripcion = productoDto.Descripcion;
            producto.StockActual = productoDto.StockActual;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); // 204 No Content para actualización exitosa
        }

        // DELETE: api/Productos/5
        // Para eliminar un producto
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return NoContent(); // 204 No Content para eliminación exitosa
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.ProductoID == id);
        }
    }
}