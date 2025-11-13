// DTOs/UsuarioCreacionDto.cs
using System.ComponentModel.DataAnnotations;

namespace SistemaInventario.Api.DTOs
{
    public class UsuarioCreacionDto
    {
        [Required(ErrorMessage = "El ID del rol es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del rol debe ser un número positivo.")]
        public int RoleID { get; set; }

        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        [StringLength(200, ErrorMessage = "El nombre completo no puede exceder los 200 caracteres.")]
        public string NombreCompleto { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        [StringLength(150, ErrorMessage = "El correo electrónico no puede exceder los 150 caracteres.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 50 caracteres.")]
        public string Password { get; set; } // Contraseña en texto plano para hash

        public bool Activo { get; set; } = true; // Por defecto activo
    }
}