using System.ComponentModel.DataAnnotations;

namespace HotelSuite.Application.DTOs.API;

/// <summary>
/// DTO para crear una nueva reserva desde la API
/// </summary>
public class CrearReservaDTO
{
    [Required(ErrorMessage = "La fecha de entrada es obligatoria")]
    public DateTime FechaEntrada { get; set; }

 [Required(ErrorMessage = "La fecha de salida es obligatoria")]
    public DateTime FechaSalida { get; set; }
 
    [Required(ErrorMessage = "Debe especificar la habitación")]
    public int IdHabitacion { get; set; }
    
    // Información del huésped
  [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100)]
    public string Nombres { get; set; } = string.Empty;
  
    [Required(ErrorMessage = "Los apellidos son obligatorios")]
    [StringLength(100)]
    public string Apellidos { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es obligatorio")]
  [EmailAddress(ErrorMessage = "Email inválido")]
    public string Email { get; set; } = string.Empty;
 
    [Required(ErrorMessage = "El teléfono es obligatorio")]
    [Phone(ErrorMessage = "Teléfono inválido")]
    public string Telefono { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "El documento de identidad es obligatorio")]
    [StringLength(50)]
    public string DocumentoIdentidad { get; set; } = string.Empty;
}
