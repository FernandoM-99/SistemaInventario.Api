// Models/Producto.cs
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; // <-- 1. ¡Asegúrate de importar esto!

namespace SistemaInventario.Api.Models
{
    public class Producto
    {
        [Key]
        public int ProductoID { get; set; }

        [Required]
        [StringLength(50)]
        public string SKU { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descripcion { get; set; }

        [Range(0, int.MaxValue)]
        public int StockActual { get; set; }

        // --- 2. ESTA ES LA LÍNEA QUE ARREGLA EL ERROR CS1061 ---
        public ICollection<ProductoProveedor>? ProductosProveedores { get; set; }
    }
}