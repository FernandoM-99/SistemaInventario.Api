/// <summary>
/// Gestiona los procesos de autenticación y acceso al sistema.
/// Permite el inicio de sesión de usuarios mediante la validación de credenciales hasheadas.
/// </summary>
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.Api.Data;
using SistemaInventario.Api.Dtos;

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
        /// <summary>
        /// Valida las credenciales de un usuario para permitir el acceso al sistema.
        /// </summary>
        /// <param name="loginDto">Objeto que contiene Email y Password.</param>
        /// <returns>Respuesta de autenticación con datos del usuario y rol.</returns>
        /// <response code="200">Login exitoso.</response>
        /// <response code="401">Credenciales inválidas o cuenta inactiva.</response>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuario = await _context.Usuarios
                                        .Include(u => u.Rol) // Para obtener el nombre del rol en la respuesta
                                        .SingleOrDefaultAsync(u => u.Email == loginDto.Email);
            if (usuario == null)
            {
                return Unauthorized("Credenciales incorrectas. Intenta de nuevo.");
            }

            if (!usuario.Activo)
            {
                return Unauthorized("Tu cuenta está inactiva. Contacta al administrador.");
            }

            var hashedPassword = HashPassword(loginDto.Password);
            if (usuario.PasswordHash != hashedPassword)
            {
                return Unauthorized("Credenciales incorrectas. Intenta de nuevo.");
            }

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