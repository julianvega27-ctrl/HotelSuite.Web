using System.ComponentModel.DataAnnotations;

namespace HotelSuite.Domain.Entities;

public class Huesped
{
    public int Id { get; set; }

[Required(ErrorMessage = "Los nombres son obligatorios")]
    [StringLength(100, ErrorMessage = "Los nombres no pueden exceder los 100 caracteres")]
    public string Nombres { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los apellidos son obligatorios")]
    [StringLength(100, ErrorMessage = "Los apellidos no pueden exceder los 100 caracteres")]
    public string Apellidos { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es obligatorio")]
    [StringLength(150, ErrorMessage = "El email no puede exceder los 150 caracteres")]
    [EmailAddress(ErrorMessage = "Formato de email inválido")]
  public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio")]
    [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres")]
    [Phone(ErrorMessage = "Formato de teléfono inválido")]
    public string Telefono { get; set; } = string.Empty;

    [Required(ErrorMessage = "El documento de identidad es obligatorio")]
    [StringLength(50, ErrorMessage = "El documento no puede exceder los 50 caracteres")]
    public string DocumentoIdentidad { get; set; } = string.Empty;

    // Relaciones
    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
