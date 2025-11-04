namespace HotelSuite.Application.DTOs;

public class HotelDTO
{
    public int Id { get; set; }
  public string Nombre { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public int Categoria { get; set; }
}
