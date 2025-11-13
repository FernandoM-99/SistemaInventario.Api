// Models/ProductoProveedor.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaInventario.Api.Models
{
    public class ProductoProveedor
    {
        // Llave primaria compuesta
        [Key, Column(Order = 0)]
        public int ProductoID { get; set; }
        [Key, Column(Order = 1)]
        public int ProveedorID { get; set; }

        public decimal? CostoUltimaCompra { get; set; }
        public string? SKUProveedor { get; set; }

        // Propiedades de navegación
        public Producto Producto { get; set; } = null!;
        public Proveedor Proveedor { get; set; } = null!;
    }
}