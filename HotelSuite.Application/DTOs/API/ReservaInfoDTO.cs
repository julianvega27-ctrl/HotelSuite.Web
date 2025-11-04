namespace HotelSuite.Application.DTOs.API;

/// <summary>
/// DTO para información de reserva en la API
/// </summary>
public class ReservaInfoDTO
{
    public int Id { get; set; }
    public DateTime FechaReserva { get; set; }
    public DateTime FechaEntrada { get; set; }
    public DateTime FechaSalida { get; set; }
    public string Estado { get; set; } = string.Empty;
    public int DiasEstancia { get; set; }
    public decimal MontoTotal { get; set; }
    
    public HuespedInfoDTO? Huesped { get; set; }
    public HabitacionInfoDTO? Habitacion { get; set; }
    public List<PagoInfoDTO> Pagos { get; set; } = new();
}

public class HuespedInfoDTO
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
}

public class HabitacionInfoDTO
{
    public int Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public decimal PrecioPorNoche { get; set; }
    public string NombreHotel { get; set; } = string.Empty;
}

public class PagoInfoDTO
{
    public int Id { get; set; }
 public decimal Monto { get; set; }
    public DateTime FechaPago { get; set; }
    public string Metodo { get; set; } = string.Empty;
}
