using System.ComponentModel.DataAnnotations;

namespace SistemaInventario.Api.Models
{
    public class Usuario
    {
        [Key]
        public int UsuarioID { get; set; }

        // Llave foránea a la tabla Roles
        public int RoleID { get; set; }

        public string NombreCompleto { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; } // ¡Importante: solo guardar el hash, no la contraseña en texto plano!
        public bool Activo { get; set; }

        // Propiedad de navegación (opcional, pero útil para EF Core)
        // Permite cargar directamente el objeto Rol asociado a este usuario
        public Rol? Rol { get; set; }
    }
}