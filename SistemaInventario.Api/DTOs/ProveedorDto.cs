// DTOs/ProveedorDto.cs
namespace SistemaInventario.Api.DTOs
{
    public class ProveedorDto
    {
        public int ProveedorID { get; set; }
        public string NombreEmpresa { get; set; } = string.Empty;
        public string? NombreContacto { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
    }
}