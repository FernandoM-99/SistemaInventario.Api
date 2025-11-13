// DTOs/UsuarioDto.cs
namespace SistemaInventario.Api.DTOs
{
    public class UsuarioDto
    {
        public int UsuarioID { get; set; }
        public int RoleID { get; set; }
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
        public bool Activo { get; set; }
        public string? NombreRol { get; set; } // Propiedad para el nombre del rol
    }
}