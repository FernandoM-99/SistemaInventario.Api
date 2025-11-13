// Models/Proveedor.cs
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; // Necesario para ICollection

namespace SistemaInventario.Api.Models
{
    public class Proveedor
    {
        [Key]
        public int ProveedorID { get; set; }

        [Required]
        [StringLength(200)]
        public string NombreEmpresa { get; set; } = string.Empty; // Inicializar para evitar nulos

        [StringLength(200)]
        public string? NombreContacto { get; set; }

        [StringLength(150)]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(50)]
        public string? Telefono { get; set; }

        // Propiedad de navegación para la relación con ProductosProveedores (Muchos a Muchos)
        // Esto permite a EF Core entender la relación
        public ICollection<ProductoProveedor>? ProductosProveedores { get; set; }

        // Propiedad de navegación para la relación con MovimientosInventario (Uno a Muchos)
        public ICollection<MovimientoInventario>? MovimientosInventario { get; set; }
    }
}