// Controllers/ProveedoresController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.Api.Data;
using SistemaInventario.Api.DTOs;
using SistemaInventario.Api.Models;

namespace SistemaInventario.Api.Controllers
{
    /// <summary>
    /// Módulo encargado de la gestión de la base de datos de proveedores.
    /// Almacena información de contacto y relaciones comerciales con las entidades de suministro.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProveedoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProveedoresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Proveedores
        /// <summary>
        /// Obtiene el catálogo de proveedores registrados en el sistema.
        /// </summary>
        /// <returns>Lista de objetos ProveedorDto.</returns>
        /// <response code="200">Consulta exitosa.</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProveedorDto>>> GetProveedores()
        {
            var proveedores = await _context.Proveedores
                                            .Select(p => new ProveedorDto
                                            {
                                                ProveedorID = p.ProveedorID,
                                                NombreEmpresa = p.NombreEmpresa,
                                                NombreContacto = p.NombreContacto,
                                                Email = p.Email,
                                                Telefono = p.Telefono
                                            })
                                            .ToListAsync();
            return Ok(proveedores);
        }

        // GET: api/Proveedores/1
        /// <summary>
        /// Obtiene los datos detallados de un proveedor por su ID.
        /// </summary>
        /// <param name="id">ID del proveedor.</param>
        /// <response code="200">Proveedor encontrado.</response>
        /// <response code="404">Proveedor no encontrado.</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProveedorDto>> GetProveedor(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);

            if (proveedor == null)
            {
                return NotFound();
            }

            var proveedorDto = new ProveedorDto
            {
                ProveedorID = proveedor.ProveedorID,
                NombreEmpresa = proveedor.NombreEmpresa,
                NombreContacto = proveedor.NombreContacto,
                Email = proveedor.Email,
                Telefono = proveedor.Telefono
            };
            return Ok(proveedorDto);
        }


        // POST: api/Proveedores
        /// <summary>
        /// Registra un nuevo proveedor en la base de datos de KeyStock.
        /// </summary>
        /// <param name="proveedorDto">Información de contacto y empresa del proveedor.</param>
        /// <returns>El proveedor recién creado.</returns>
        /// <response code="201">Registro exitoso.</response>
        /// <response code="409">Ya existe una empresa registrada con ese nombre.</response>
        [HttpPost]
        public async Task<ActionResult<ProveedorDto>> PostProveedor(ProveedorCreacionDto proveedorDto)
        {
            // Validar si el NombreEmpresa ya existe
            if (await _context.Proveedores.AnyAsync(p => p.NombreEmpresa == proveedorDto.NombreEmpresa))
            {
                return Conflict($"Ya existe un proveedor con el nombre de empresa '{proveedorDto.NombreEmpresa}'.");
            }

            var proveedor = new Proveedor
            {
                NombreEmpresa = proveedorDto.NombreEmpresa,
                NombreContacto = proveedorDto.NombreContacto,
                Email = proveedorDto.Email,
                Telefono = proveedorDto.Telefono
            };

            _context.Proveedores.Add(proveedor);
            await _context.SaveChangesAsync();

            var nuevoProveedorDto = new ProveedorDto
            {
                ProveedorID = proveedor.ProveedorID,
                NombreEmpresa = proveedor.NombreEmpresa,
                NombreContacto = proveedor.NombreContacto,
                Email = proveedor.Email,
                Telefono = proveedor.Telefono
            };

            return CreatedAtAction(nameof(GetProveedor), new { id = nuevoProveedorDto.ProveedorID }, nuevoProveedorDto);
        }

        // PUT: api/Proveedores/1
        /// <summary>
        /// Actualiza la información de contacto o el nombre de un proveedor existente.
        /// </summary>
        /// <param name="id">ID del proveedor a modificar.</param>
        /// <param name="proveedorDto">Nuevos datos del proveedor.</param>
        /// <response code="204">Actualización realizada correctamente.</response>
        /// <response code="404">El proveedor no existe.</response>
        /// <response code="409">El nuevo nombre de empresa ya está ocupado por otro proveedor.</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProveedor(int id, ProveedorCreacionDto proveedorDto)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor == null)
            {
                return NotFound();
            }

            // Opcional: Validar si el NombreEmpresa se está cambiando a uno ya existente
            if (proveedor.NombreEmpresa != proveedorDto.NombreEmpresa && await _context.Proveedores.AnyAsync(p => p.NombreEmpresa == proveedorDto.NombreEmpresa && p.ProveedorID != id))
            {
                return Conflict($"Ya existe otro proveedor con el nombre de empresa '{proveedorDto.NombreEmpresa}'.");
            }

            proveedor.NombreEmpresa = proveedorDto.NombreEmpresa;
            proveedor.NombreContacto = proveedorDto.NombreContacto;
            proveedor.Email = proveedorDto.Email;
            proveedor.Telefono = proveedorDto.Telefono;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProveedorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Proveedores/1
        /// <summary>
        /// Elimina un proveedor del sistema.
        /// </summary>
        /// <remarks>
        /// La operación fallará si el proveedor tiene productos o movimientos de inventario asociados.
        /// </remarks>
        /// <response code="204">Eliminación exitosa.</response>
        /// <response code="400">No se puede eliminar debido a dependencias en el inventario.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProveedor(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor == null)
            {
                return NotFound();
            }

            // Lógica de negocio: ¿Qué hacer si hay productos asociados a este proveedor en ProductosProveedores?
            // O si hay movimientos de inventario?
            // Por ahora, la base de datos (FK) no permitirá la eliminación si hay referencias.
            // Para un mejor UX, lo ideal es validarlo aquí.
            if (await _context.ProductosProveedores.AnyAsync(pp => pp.ProveedorID == id))
            {
                return BadRequest($"No se puede eliminar el proveedor con ID {id} porque tiene productos asociados.");
            }
            if (await _context.MovimientosInventario.AnyAsync(mi => mi.ProveedorID == id))
            {
                return BadRequest($"No se puede eliminar el proveedor con ID {id} porque tiene movimientos de inventario asociados.");
            }

            _context.Proveedores.Remove(proveedor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProveedorExists(int id)
        {
            return _context.Proveedores.Any(e => e.ProveedorID == id);
        }
    }
}