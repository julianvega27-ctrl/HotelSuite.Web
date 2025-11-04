using System.ComponentModel.DataAnnotations;
using HotelSuite.Application.DTOs;

namespace HotelSuite.Application.DTOs;

public class ReservaDTO
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "La fecha de reserva es obligatoria")]
    [Display(Name = "Fecha de Reserva")]
    [DataType(DataType.Date)]
    public DateTime FechaReserva { get; set; } = DateTime.Now;
    
    [Required(ErrorMessage = "La fecha de entrada es obligatoria")]
    [Display(Name = "Fecha de Entrada")]
    [DataType(DataType.Date)]
    public DateTime FechaEntrada { get; set; }
    
    [Required(ErrorMessage = "La fecha de salida es obligatoria")]
    [Display(Name = "Fecha de Salida")]
    [DataType(DataType.Date)]
    public DateTime FechaSalida { get; set; }
    
    [Required(ErrorMessage = "El estado es obligatorio")]
    [StringLength(50, ErrorMessage = "El estado no puede exceder los 50 caracteres")]
    [Display(Name = "Estado")]
    public string Estado { get; set; } = "Confirmada";
 
    [Required(ErrorMessage = "Debe seleccionar un huésped")]
    [Display(Name = "Huésped")]
    public int IdHuesped { get; set; }

    [Required(ErrorMessage = "Debe seleccionar una habitación")]
    [Display(Name = "Habitación")]
    public int IdHabitacion { get; set; }
    
    // Propiedades de navegación simplificadas (solo para mostrar información)
    public string? NombreHuesped { get; set; }
    public string? NumeroHabitacion { get; set; }
    public string? TipoHabitacion { get; set; }
    public string? NombreHotel { get; set; }
    
    // Propiedades adicionales para el formulario
    public decimal? PrecioHabitacion { get; set; }
    public decimal PrecioPorNoche { get; set; }
    public decimal? MontoTotal { get; set; }
    
    // Colección de pagos
    public List<PagoDTO>? Pagos { get; set; }
    
    // Propiedades calculadas
    public int DiasEstancia => FechaSalida > FechaEntrada ? (FechaSalida - FechaEntrada).Days : 0;
}
