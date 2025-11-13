// DTOs/ProveedorCreacionDto.cs
using System.ComponentModel.DataAnnotations;

namespace SistemaInventario.Api.DTOs
{
    public class ProveedorCreacionDto
    {
        [Required(ErrorMessage = "El nombre de la empresa es obligatorio.")]
        [StringLength(200, ErrorMessage = "El nombre de la empresa no puede exceder los 200 caracteres.")]
        public string NombreEmpresa { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "El nombre de contacto no puede exceder los 200 caracteres.")]
        public string? NombreContacto { get; set; }

        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        [StringLength(150, ErrorMessage = "El correo electrónico no puede exceder los 150 caracteres.")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "El formato del teléfono no es válido.")]
        [StringLength(50, ErrorMessage = "El teléfono no puede exceder los 50 caracteres.")]
        public string? Telefono { get; set; }
    }
}