namespace SistemaInventario.Api.Dtos
{
    public class AuthResponseDto
    {
        public int UsuarioID { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; 
        public string Message { get; set; } = string.Empty;

        // Puedes añadir un Token JWT aquí si lo implementas más adelante
        // public string Token { get; set; } = string.Empty; 
    }
}