// Controllers/UsuariosController.cs (COMPLETO CON CRUD)
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.Api.Data;
using SistemaInventario.Api.DTOs;
using SistemaInventario.Api.Models;
using System.Security.Cryptography;
using System.Text;

namespace SistemaInventario.Api.Controllers
{
    /// <summary>
    /// Gestiona la administración de cuentas de usuario del sistema.
    /// Permite el control de perfiles, asignación de roles y estado de activación de los empleados.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: api/Usuarios
        /// <summary>
        /// Obtiene la lista de todos los usuarios registrados, incluyendo su rol asignado.
        /// </summary>
        /// <returns>Lista de objetos UsuarioDto.</returns>
        /// <response code="200">Lista de usuarios obtenida con éxito.</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetUsuarios()
        {
            var usuarios = await _context.Usuarios
                                        .Include(u => u.Rol)
                                        .Select(u => new UsuarioDto
                                        {
                                            UsuarioID = u.UsuarioID,
                                            RoleID = u.RoleID,
                                            NombreCompleto = u.NombreCompleto,
                                            Email = u.Email,
                                            Activo = u.Activo,
                                            NombreRol = u.Rol != null ? u.Rol.NombreRol : "Sin Rol"
                                        })
                                        .ToListAsync();
            return Ok(usuarios);
        }

        // GET: api/Usuarios/1
        /// <summary>
        /// Obtiene la información detallada de un usuario específico por su ID.
        /// </summary>
        /// <param name="id">ID del usuario.</param>
        /// <response code="200">Usuario encontrado.</response>
        /// <response code="404">Usuario no encontrado.</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioDto>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios
                                        .Include(u => u.Rol)
                                        .FirstOrDefaultAsync(u => u.UsuarioID == id);

            if (usuario == null)
            {
                return NotFound();
            }

            var usuarioDto = new UsuarioDto
            {
                UsuarioID = usuario.UsuarioID,
                RoleID = usuario.RoleID,
                NombreCompleto = usuario.NombreCompleto,
                Email = usuario.Email,
                Activo = usuario.Activo,
                NombreRol = usuario.Rol != null ? usuario.Rol.NombreRol : "Sin Rol"
            };
            return Ok(usuarioDto);
        }



        // POST: api/Usuarios
        /// <summary>
        /// Crea un nuevo usuario en el sistema con contraseña hasheada en SHA256.
        /// </summary>
        /// <response code="201">Usuario creado con éxito.</response>
        /// <response code="409">El correo electrónico ya está en uso.</response>
        [HttpPost]
        public async Task<ActionResult<UsuarioDto>> PostUsuario(UsuarioCreacionDto usuarioDto)
        {
            // 1. Validar si el RoleID existe
            if (!await _context.Roles.AnyAsync(r => r.RoleID == usuarioDto.RoleID))
            {
                return BadRequest($"El RoleID '{usuarioDto.RoleID}' no existe.");
            }

            // 2. Validar si el Email ya existe
            if (await _context.Usuarios.AnyAsync(u => u.Email == usuarioDto.Email))
            {
                return Conflict($"Ya existe un usuario con el correo electrónico '{usuarioDto.Email}'.");
            }

            // 3. Crear el objeto Usuario a partir del DTO
            var usuario = new Usuario
            {
                RoleID = usuarioDto.RoleID,
                NombreCompleto = usuarioDto.NombreCompleto,
                Email = usuarioDto.Email,
                PasswordHash = HashPassword(usuarioDto.Password), // Hashear la contraseña
                Activo = usuarioDto.Activo
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // Cargar el rol para el DTO de respuesta
            var rol = await _context.Roles.FindAsync(usuario.RoleID);

            // 4. Mapear a UsuarioDto para la respuesta
            var nuevoUsuarioDto = new UsuarioDto
            {
                UsuarioID = usuario.UsuarioID,
                RoleID = usuario.RoleID,
                NombreCompleto = usuario.NombreCompleto,
                Email = usuario.Email,
                Activo = usuario.Activo,
                NombreRol = rol != null ? rol.NombreRol : "Sin Rol"
            };

            // 5. Retorna 201 Created
            return CreatedAtAction(nameof(GetUsuario), new { id = nuevoUsuarioDto.UsuarioID }, nuevoUsuarioDto);
        }

        // PUT: api/Usuarios/5
        /// <summary>
        /// Modifica los datos de un usuario. Permite actualizar la contraseña de forma opcional.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, UsuarioEdicionDto usuarioDto) // <--- ¡AQUÍ ESTÁ LA CLAVE!
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            // 1. Validar si el RoleID existe
            if (!await _context.Roles.AnyAsync(r => r.RoleID == usuarioDto.RoleID))
            {
                return BadRequest($"El RoleID '{usuarioDto.RoleID}' no existe.");
            }

            // 2. Validar email duplicado
            if (usuario.Email != usuarioDto.Email && await _context.Usuarios.AnyAsync(u => u.Email == usuarioDto.Email && u.UsuarioID != id))
            {
                return Conflict($"Ya existe otro usuario con el correo electrónico '{usuarioDto.Email}'.");
            }

            // 3. Actualizar propiedades
            usuario.RoleID = usuarioDto.RoleID;
            usuario.NombreCompleto = usuarioDto.NombreCompleto;
            usuario.Email = usuarioDto.Email;
            usuario.Activo = usuarioDto.Activo;

            // 4. Actualizar contraseña SOLO si el DTO trae texto (no es nulo ni vacío)
            if (!string.IsNullOrEmpty(usuarioDto.Password))
            {
                usuario.PasswordHash = HashPassword(usuarioDto.Password);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); // Esto devuelve un 204 (Cuerpo vacío, pero Éxito)
        }

        // DELETE: api/Usuarios/5
        /// <summary>
        /// Elimina permanentemente un usuario del sistema.
        /// </summary>
        /// <param name="id">ID del usuario a eliminar.</param>
        /// <response code="204">Usuario eliminado exitosamente.</response>
        /// <response code="404">El usuario no existe.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent(); // 204 No Content
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.UsuarioID == id);
        }

        // --- Método de Hashing SIMPLE (Para DEMOSTRACIÓN) ---
        // En producción, usar una librería robusta como BCrypt.NET-Next
        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}