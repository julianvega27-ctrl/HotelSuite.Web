using System.ComponentModel.DataAnnotations;

namespace HotelSuite.Domain.Entities;

public class Reserva
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La fecha de reserva es obligatoria")]
    public DateTime FechaReserva { get; set; }

    [Required(ErrorMessage = "La fecha de entrada es obligatoria")]
    public DateTime FechaEntrada { get; set; }

    [Required(ErrorMessage = "La fecha de salida es obligatoria")]
    public DateTime FechaSalida { get; set; }

    [Required(ErrorMessage = "El estado es obligatorio")]
    [StringLength(50, ErrorMessage = "El estado no puede exceder los 50 caracteres")]
    public string Estado { get; set; } = string.Empty;

    [Required(ErrorMessage = "El IdHuesped es obligatorio")]
public int IdHuesped { get; set; }

    [Required(ErrorMessage = "El IdHabitacion es obligatorio")]
    public int IdHabitacion { get; set; }

    // Relaciones
    public Huesped Huesped { get; set; } = null!;
    public Habitacion Habitacion { get; set; } = null!;
    public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
