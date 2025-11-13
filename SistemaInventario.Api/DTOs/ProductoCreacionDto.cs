// DTOs/ProductoCreacionDto.cs
using System.ComponentModel.DataAnnotations;

namespace SistemaInventario.Api.DTOs
{
    public class ProductoCreacionDto
    {
        [Required(ErrorMessage = "El SKU es obligatorio.")]
        [StringLength(50, ErrorMessage = "El SKU no puede exceder los 50 caracteres.")]
        public string SKU { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(200, ErrorMessage = "El nombre no puede exceder los 200 caracteres.")]
        public string Nombre { get; set; }

        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres.")]
        public string? Descripcion { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El stock actual no puede ser negativo.")]
        public int StockActual { get; set; }
    }
}