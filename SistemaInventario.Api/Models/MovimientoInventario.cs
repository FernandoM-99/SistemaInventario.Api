// Models/MovimientoInventario.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaInventario.Api.Models
{
    public class MovimientoInventario
    {
        [Key]
        public int MovimientoID { get; set; }
        public int ProductoID { get; set; }
        public int UsuarioID { get; set; } // En tu BD dice UserID, en mi modelo de Usuario es UsuarioID, aquí debería coincidir con la BD
        public int? ProveedorID { get; set; }
        public string TipoMovimiento { get; set; }
        public int Cantidad { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? CostoUnitario { get; set; }
        public DateTime FechaHora { get; set; } = DateTime.Now; // Valor por defecto
        public string? Referencia { get; set; }

        // Propiedades de navegación
        public Producto Producto { get; set; } = null!;
        public Usuario Usuario { get; set; } = null!; // Asumiendo que UserID de BD = UsuarioID de Modelo
        public Proveedor? Proveedor { get; set; }
    }
}