namespace HotelSuite.Application.DTOs;

public class HuespedDTO
{
    public int Id { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string DocumentoIdentidad { get; set; } = string.Empty;
  
    // Propiedad calculada
    public string NombreCompleto => $"{Nombres} {Apellidos}";
}
