/// <summary>
/// Gestiona los procesos de autenticación y acceso al sistema.
/// Permite el inicio de sesión de usuarios mediante la validación de credenciales hasheadas.
/// </summary>
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.Api.Data;
using SistemaInventario.Api.Dtos;
using SistemaInventario.Api.Models; // Asumiendo que tu modelo Usuario está aquí

namespace SistemaInventario.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 1. Buscar el usuario por email
            var usuario = await _context.Usuarios
                                        .Include(u => u.Rol) // Para obtener el nombre del rol en la respuesta
                                        .SingleOrDefaultAsync(u => u.Email == loginDto.Email);

            // 2. Validar si el usuario existe
            if (usuario == null)
            {
                return Unauthorized("Credenciales incorrectas. Intenta de nuevo.");
            }

            // 3. Validar si el usuario está activo
            if (!usuario.Activo)
            {
                return Unauthorized("Tu cuenta está inactiva. Contacta al administrador.");
            }

            // 4. Verificar la contraseña (¡USAR EL MISMO HASH DE UsuariosController!)
            var hashedPassword = HashPassword(loginDto.Password);
            if (usuario.PasswordHash != hashedPassword)
            {
                return Unauthorized("Credenciales incorrectas. Intenta de nuevo.");
            }

            // 5. Si todo es correcto, crear la respuesta de autenticación
            var authResponse = new AuthResponseDto
            {
                UsuarioID = usuario.UsuarioID,
                NombreCompleto = usuario.NombreCompleto,
                Email = usuario.Email,
                Role = usuario.Rol?.NombreRol ?? "Sin Rol", // Si el rol es nulo, asignar "Sin Rol"
                Message = "Login exitoso"
            };

            return Ok(authResponse);
        }

        // Función para hashear la contraseña
        // ¡DEBE SER IDÉNTICA A LA DE UsuariosController!
        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}