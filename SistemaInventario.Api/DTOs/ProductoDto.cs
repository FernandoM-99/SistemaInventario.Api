// DTOs/ProductoDto.cs
namespace SistemaInventario.Api.DTOs
{
    public class ProductoDto
    {
        public int ProductoID { get; set; }
        public string SKU { get; set; }
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public int StockActual { get; set; }

    }
}