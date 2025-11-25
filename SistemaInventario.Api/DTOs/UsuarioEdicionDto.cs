// DTOs/UsuarioEdicionDto.cs
using System.ComponentModel.DataAnnotations;

namespace SistemaInventario.Api.DTOs
{
    public class UsuarioEdicionDto
    {
        [Required(ErrorMessage = "El ID del rol es obligatorio.")]
        public int RoleID { get; set; }

        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        public string NombreCompleto { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        public string Email { get; set; }

        // Aquí NO ponemos [Required], para permitir que sea nulo o vacío
        public string? Password { get; set; }

        public bool Activo { get; set; }
    }
}