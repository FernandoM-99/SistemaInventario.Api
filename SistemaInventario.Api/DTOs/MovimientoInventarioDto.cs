// DTOs/MovimientoInventarioDto.cs
namespace SistemaInventario.Api.DTOs
{
    public class MovimientoInventarioDto
    {
        public int MovimientoID { get; set; }
        public DateTime FechaHora { get; set; }

        public int ProductoID { get; set; }
        public string ProductoNombre { get; set; } = string.Empty; // Nombre del producto
        public string ProductoSKU { get; set; } = string.Empty; // SKU del producto

        public string TipoMovimiento { get; set; } = string.Empty;
        public int Cantidad { get; set; }

        public int UsuarioID { get; set; }
        public string UsuarioNombre { get; set; } = string.Empty; // Nombre del usuario

        public int? ProveedorID { get; set; }
        public string? ProveedorNombre { get; set; } // Nombre del proveedor (nulable)

        public decimal? CostoUnitario { get; set; }
        public string? Referencia { get; set; }
    }
}