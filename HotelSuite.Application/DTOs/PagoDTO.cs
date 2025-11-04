using System.ComponentModel.DataAnnotations;

namespace HotelSuite.Application.DTOs;

public class PagoDTO
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "El monto es obligatorio")]
    [Range(0.01, 999999.99, ErrorMessage = "El monto debe estar entre 0.01 y 999,999.99")]
    [Display(Name = "Monto")]
    [DataType(DataType.Currency)]
    public decimal Monto { get; set; }
    
    [Required(ErrorMessage = "La fecha de pago es obligatoria")]
    [Display(Name = "Fecha de Pago")]
    [DataType(DataType.Date)]
    public DateTime FechaPago { get; set; } = DateTime.Now;
    
    [Required(ErrorMessage = "El método de pago es obligatorio")]
    [StringLength(50, ErrorMessage = "El método no puede exceder los 50 caracteres")]
    [Display(Name = "Método de Pago")]
    public string Metodo { get; set; } = "Pendiente";
    
    [Required(ErrorMessage = "El IdReserva es obligatorio")]
    public int IdReserva { get; set; }
    
    // Propiedades de navegación simplificadas (solo para mostrar información)
    public string? NombreHuesped { get; set; }
    public string? NumeroHabitacion { get; set; }
}
