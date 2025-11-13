// Models/Rol.cs
using System.ComponentModel.DataAnnotations;

namespace SistemaInventario.Api.Models
{
    public class Rol
    {
        [Key]
        public int RoleID { get; set; }
        public string NombreRol { get; set; }

        // Propiedad de navegación (opcional)
        // Permite cargar la colección de Usuarios asociados a este Rol
        public ICollection<Usuario>? Usuarios { get; set; }
    }
}