// Controllers/ProveedoresController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.Api.Data;
using SistemaInventario.Api.DTOs;
using SistemaInventario.Api.Models;

namespace SistemaInventario.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProveedoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProveedoresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- MÉTODOS GET ---
        // GET: api/Proveedores
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

        // --- MÉTODOS CRUD ---

        // POST: api/Proveedores
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