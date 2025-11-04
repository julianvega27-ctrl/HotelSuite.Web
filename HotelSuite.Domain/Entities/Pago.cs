using System.ComponentModel.DataAnnotations;

namespace HotelSuite.Domain.Entities;

public class Pago
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El monto es obligatorio")]
    [Range(0.01, 999999.99, ErrorMessage = "El monto debe ser mayor a 0")]
    public decimal Monto { get; set; }

    [Required(ErrorMessage = "La fecha de pago es obligatoria")]
    public DateTime FechaPago { get; set; }

 [Required(ErrorMessage = "El método de pago es obligatorio")]
  [StringLength(50, ErrorMessage = "El método no puede exceder los 50 caracteres")]
    public string Metodo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El IdReserva es obligatorio")]
    public int IdReserva { get; set; }

// Relaciones
    public Reserva Reserva { get; set; } = null!;
}
