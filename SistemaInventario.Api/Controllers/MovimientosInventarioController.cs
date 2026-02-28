// Controllers/MovimientosInventarioController.cs
/// <summary>
/// Administra el registro de entradas, salidas y ajustes de stock.
/// Garantiza la integridad de los datos mediante el uso de transacciones asíncronas para actualizar el stock.
/// </summary>
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.Api.Data;
using SistemaInventario.Api.DTOs;
using SistemaInventario.Api.Models;

namespace SistemaInventario.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovimientosInventarioController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MovimientosInventarioController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- MÉTODO GET ---
        // GET: api/MovimientosInventario
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovimientoInventarioDto>>> GetMovimientosInventario()
        {
            var movimientos = await _context.MovimientosInventario
                .Include(m => m.Producto) // Incluir Producto
                .Include(m => m.Usuario)  // Incluir Usuario
                .Include(m => m.Proveedor) // Incluir Proveedor
                .Select(m => new MovimientoInventarioDto
                {
                    MovimientoID = m.MovimientoID,
                    FechaHora = m.FechaHora,
                    ProductoID = m.ProductoID,
                    ProductoNombre = m.Producto.Nombre,
                    ProductoSKU = m.Producto.SKU,
                    TipoMovimiento = m.TipoMovimiento,
                    Cantidad = m.Cantidad,
                    UsuarioID = m.UsuarioID,
                    UsuarioNombre = m.Usuario.NombreCompleto,
                    ProveedorID = m.ProveedorID,
                    ProveedorNombre = m.Proveedor != null ? m.Proveedor.NombreEmpresa : null,
                    CostoUnitario = m.CostoUnitario,
                    Referencia = m.Referencia
                })
                .OrderByDescending(m => m.FechaHora) // Mostrar los más recientes primero
                .ToListAsync();

            return Ok(movimientos);
        }

        // GET: api/MovimientosInventario/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MovimientoInventarioDto>> GetMovimientoInventario(int id)
        {
            var movimiento = await _context.MovimientosInventario
                .Include(m => m.Producto)
                .Include(m => m.Usuario)
                .Include(m => m.Proveedor)
                .Select(m => new MovimientoInventarioDto
                {
                    MovimientoID = m.MovimientoID,
                    FechaHora = m.FechaHora,
                    ProductoID = m.ProductoID,
                    ProductoNombre = m.Producto.Nombre,
                    ProductoSKU = m.Producto.SKU,
                    TipoMovimiento = m.TipoMovimiento,
                    Cantidad = m.Cantidad,
                    UsuarioID = m.UsuarioID,
                    UsuarioNombre = m.Usuario.NombreCompleto,
                    ProveedorID = m.ProveedorID,
                    ProveedorNombre = m.Proveedor != null ? m.Proveedor.NombreEmpresa : null,
                    CostoUnitario = m.CostoUnitario,
                    Referencia = m.Referencia
                })
                .FirstOrDefaultAsync(m => m.MovimientoID == id);

            if (movimiento == null)
            {
                return NotFound();
            }

            return Ok(movimiento);
        }

        // --- MÉTODO POST ---
        // POST: api/MovimientosInventario
        [HttpPost]
        public async Task<ActionResult<MovimientoInventarioDto>> PostMovimientoInventario(MovimientoCreacionDto movimientoDto)
        {
            // 1. Validar que las entidades existan
            var producto = await _context.Productos.FindAsync(movimientoDto.ProductoID);
            if (producto == null)
            {
                return BadRequest("El ProductoID no existe.");
            }
            if (!await _context.Usuarios.AnyAsync(u => u.UsuarioID == movimientoDto.UsuarioID))
            {
                return BadRequest("El UsuarioID no existe.");
            }
            if (movimientoDto.ProveedorID.HasValue && !await _context.Proveedores.AnyAsync(p => p.ProveedorID == movimientoDto.ProveedorID))
            {
                return BadRequest("El ProveedorID no existe.");
            }

            // 2. Iniciar Transacción
            // Esto asegura que o ambas operaciones (crear movimiento + actualizar stock)
            // se completan, o ninguna lo hace.
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 3. Lógica de negocio para actualizar el stock
                    string tipoMovimiento = movimientoDto.TipoMovimiento.ToLower();
                    int cantidad = movimientoDto.Cantidad;

                    if (tipoMovimiento == "entrada" || tipoMovimiento == "ajuste positivo")
                    {
                        producto.StockActual += cantidad;
                    }
                    else if (tipoMovimiento == "salida" || tipoMovimiento == "ajuste negativo")
                    {
                        if (producto.StockActual < cantidad)
                        {
                            // No hay suficiente stock, cancelar transacción
                            await transaction.RollbackAsync();
                            return BadRequest($"No hay suficiente stock para el producto '{producto.Nombre}'. Stock actual: {producto.StockActual}.");
                        }
                        producto.StockActual -= cantidad;
                    }
                    else
                    {
                        await transaction.RollbackAsync();
                        return BadRequest("Tipo de movimiento no válido. Use 'Entrada', 'Salida', 'Ajuste Positivo' o 'Ajuste Negativo'.");
                    }

                    _context.Productos.Update(producto);

                    // 4. Crear el nuevo movimiento
                    var movimiento = new MovimientoInventario
                    {
                        ProductoID = movimientoDto.ProductoID,
                        UsuarioID = movimientoDto.UsuarioID,
                        ProveedorID = movimientoDto.ProveedorID,
                        TipoMovimiento = movimientoDto.TipoMovimiento,
                        Cantidad = movimientoDto.Cantidad,
                        CostoUnitario = movimientoDto.CostoUnitario,
                        FechaHora = DateTime.Now,
                        Referencia = movimientoDto.Referencia
                    };

                    _context.MovimientosInventario.Add(movimiento);

                    // 5. Guardar cambios (ambas tablas)
                    await _context.SaveChangesAsync();

                    // 6. Confirmar la transacción
                    await transaction.CommitAsync();

                    // 7. Cargar datos para la respuesta DTO
                    var usuario = await _context.Usuarios.FindAsync(movimiento.UsuarioID);
                    var proveedor = movimiento.ProveedorID.HasValue ? await _context.Proveedores.FindAsync(movimiento.ProveedorID) : null;

                    var movimientoDtoRespuesta = new MovimientoInventarioDto
                    {
                        MovimientoID = movimiento.MovimientoID,
                        FechaHora = movimiento.FechaHora,
                        ProductoID = producto.ProductoID,
                        ProductoNombre = producto.Nombre,
                        ProductoSKU = producto.SKU,
                        TipoMovimiento = movimiento.TipoMovimiento,
                        Cantidad = movimiento.Cantidad,
                        UsuarioID = usuario!.UsuarioID,
                        UsuarioNombre = usuario.NombreCompleto,
                        ProveedorID = proveedor?.ProveedorID,
                        ProveedorNombre = proveedor?.NombreEmpresa,
                        CostoUnitario = movimiento.CostoUnitario,
                        Referencia = movimiento.Referencia
                    };

                    // 8. Retornar 201 Created
                    return CreatedAtAction(nameof(GetMovimientoInventario), new { id = movimiento.MovimientoID }, movimientoDtoRespuesta);
                }
                catch (Exception ex)
                {
                    // Si algo falla, revertir todo
                    await transaction.RollbackAsync();
                    return StatusCode(500, $"Error interno al procesar el movimiento: {ex.Message}");
                }
            }
        }
    }
}