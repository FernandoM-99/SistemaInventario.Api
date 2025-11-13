// DTOs/MovimientoCreacionDto.cs
using System.ComponentModel.DataAnnotations;

namespace SistemaInventario.Api.DTOs
{
    public class MovimientoCreacionDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "El ProductoID es obligatorio.")]
        public int ProductoID { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "El UsuarioID es obligatorio.")]
        public int UsuarioID { get; set; }

        public int? ProveedorID { get; set; } // Opcional

        [Required(ErrorMessage = "El tipo de movimiento es obligatorio.")]
        public string TipoMovimiento { get; set; } = string.Empty; // "Entrada", "Salida", "Ajuste"

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        public int Cantidad { get; set; }

        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "El costo unitario debe ser positivo.")]
        public decimal? CostoUnitario { get; set; }

        [StringLength(200)]
        public string? Referencia { get; set; }
    }
}