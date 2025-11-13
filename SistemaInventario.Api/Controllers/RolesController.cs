// Controllers/RolesController.cs (COMPLETO CON CRUD)
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.Api.Data;
using SistemaInventario.Api.Models; // Aquí usamos el modelo directamente

namespace SistemaInventario.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RolesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- MÉTODOS GET ---
        // GET: api/Roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rol>>> GetRoles()
        {
            var roles = await _context.Roles.ToListAsync();
            return Ok(roles);
        }

        // GET: api/Roles/1
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

        // --- MÉTODOS CRUD ---

        // POST: api/Roles
        // Para crear un nuevo rol
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
        // Para actualizar un rol existente
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
        // Para eliminar un rol
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