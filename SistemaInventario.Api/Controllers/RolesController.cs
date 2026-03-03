// Controllers/RolesController.cs (COMPLETO CON CRUD)
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.Api.Data;
using SistemaInventario.Api.Models;

namespace SistemaInventario.Api.Controllers
{
    /// <summary>
    /// Gestiona la administración de  Roles del sistema.
    /// Permite el control de roles (Creacion y Modificacion).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RolesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Roles
        /// <summary>
        /// Obtiene la lista completa de roles definidos en el sistema.
        /// </summary>
        /// <returns>Una colección de objetos Rol.</returns>
        /// <response code="200">Lista de roles recuperada con éxito.</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rol>>> GetRoles()
        {
            var roles = await _context.Roles.ToListAsync();
            return Ok(roles);
        }

        // GET: api/Roles/1
        /// <summary>
        /// Obtiene la información de un rol específico mediante su ID.
        /// </summary>
        /// <param name="id">ID único del rol.</param>
        /// <returns>El objeto Rol solicitado.</returns>
        /// <response code="200">Rol encontrado.</response>
        /// <response code="404">No se encontró el rol con el ID proporcionado.</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<Rol>> GetRol(int id)
        {
            var rol = await _context.Roles.FindAsync(id);

            if (rol == null)
            {
                return NotFound();
            }
            return Ok(rol);
        }

        // Para crear un nuevo rol
        /// <summary>
        /// Registra un nuevo rol de usuario.
        /// </summary>
        /// <response code="201">Rol creado con éxito.</response>
        /// <response code="409">El nombre del rol ya existe.</response>
        [HttpPost]
        public async Task<ActionResult<Rol>> PostRol(Rol rol)
        {
            // Validar si el NombreRol ya existe
            if (await _context.Roles.AnyAsync(r => r.NombreRol == rol.NombreRol))
            {
                return Conflict($"Ya existe un rol con el nombre '{rol.NombreRol}'.");
            }

            _context.Roles.Add(rol);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRol), new { id = rol.RoleID }, rol);
        }

        // PUT: api/Roles/1
        /// <summary>
        /// Registra un nuevo rol de usuario.
        /// </summary>
        /// <response code="201">Rol creado con éxito.</response>
        /// <response code="409">El nombre del rol ya existe.</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRol(int id, Rol rol)
        {
            if (id != rol.RoleID)
            {
                return BadRequest("El ID del rol no coincide con el ID de la URL.");
            }

            // Opcional: Validar si el NombreRol se está cambiando a uno ya existente
            if (await _context.Roles.AnyAsync(r => r.NombreRol == rol.NombreRol && r.RoleID != id))
            {
                return Conflict($"Ya existe otro rol con el nombre '{rol.NombreRol}'.");
            }

            _context.Entry(rol).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RolExists(id))
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

        // DELETE: api/Roles/1
        /// <summary>
        /// Elimina un rol si no tiene usuarios asociados.
        /// </summary>
        /// <response code="204">Rol eliminado.</response>
        /// <response code="400">No se puede eliminar porque tiene usuarios vinculados.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRol(int id)
        {
            var rol = await _context.Roles.FindAsync(id);
            if (rol == null)
            {
                return NotFound();
            }

            // Considerar la lógica de negocio: ¿Qué pasa con los usuarios asignados a este rol?
            // Opciones: 
            // 1. Evitar la eliminación si hay usuarios (retornar BadRequest)
            // 2. Asignar esos usuarios a un rol por defecto (ej. "Invitado")
            // 3. Eliminar los usuarios también (PELIGROSO)
            // Por ahora, si hay usuarios, la FK en la BD no permitirá la eliminación.
            // Para un mejor UX, lo ideal es validarlo aquí antes.

            // Comprobación de usuarios asociados (opcional, para un mensaje más amigable)
            if (await _context.Usuarios.AnyAsync(u => u.RoleID == id))
            {
                return BadRequest($"No se puede eliminar el rol con ID {id} porque tiene usuarios asociados.");
            }

            _context.Roles.Remove(rol);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RolExists(int id)
        {
            return _context.Roles.Any(e => e.RoleID == id);
        }
    }
}