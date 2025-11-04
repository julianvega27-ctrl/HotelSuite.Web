namespace HotelSuite.Application.DTOs.API;

/// <summary>
/// DTO para habitaciones disponibles en la API pública
/// </summary>
public class HabitacionDisponibleDTO
{
    public int Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
public decimal PrecioPorNoche { get; set; }
 public string Estado { get; set; } = string.Empty;
    public HotelInfoDTO? Hotel { get; set; }
    public List<string> Caracteristicas { get; set; } = new();
}

public class HotelInfoDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public int Categoria { get; set; }
}
