namespace HotelSuite.Application.DTOs.API;

/// <summary>
/// DTO para consultar disponibilidad de habitaciones
/// </summary>
public class DisponibilidadQueryDTO
{
 public DateTime? FechaEntrada { get; set; }
public DateTime? FechaSalida { get; set; }
    public string? Tipo { get; set; }
    public int? NumeroPersonas { get; set; }
    public decimal? PrecioMaximo { get; set; }
    public int? IdHotel { get; set; }
}
