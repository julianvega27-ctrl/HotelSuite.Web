namespace HotelSuite.Application.DTOs.API;

/// <summary>
/// DTO para check-in automático desde kiosco
/// </summary>
public class CheckInDTO
{
    public int IdReserva { get; set; }
    public string DocumentoIdentidad { get; set; } = string.Empty;
    public string? NumeroConfirmacion { get; set; }
}

/// <summary>
/// DTO para respuesta de check-in
/// </summary>
public class CheckInResponseDTO
{
    public bool Exitoso { get; set; }
public string Mensaje { get; set; } = string.Empty;
    public ReservaInfoDTO? Reserva { get; set; }
    public string? TarjetaAcceso { get; set; }
}
