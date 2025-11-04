using System.ComponentModel.DataAnnotations;

namespace HotelSuite.Application.DTOs;

public class HabitacionDTO
{
  public int Id { get; set; }
    
    [Required(ErrorMessage = "El número de habitación es obligatorio")]
    [StringLength(10, ErrorMessage = "El número no puede exceder los 10 caracteres")]
    [Display(Name = "Número")]
public string Numero { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "El tipo de habitación es obligatorio")]
    [StringLength(50, ErrorMessage = "El tipo no puede exceder los 50 caracteres")]
    [Display(Name = "Tipo de Habitación")]
    public string Tipo { get; set; } = string.Empty;
    
 [Required(ErrorMessage = "El precio por noche es obligatorio")]
    [Range(0.01, 999999.99, ErrorMessage = "El precio debe ser mayor a 0")]
    [Display(Name = "Precio por Noche")]
    [DataType(DataType.Currency)]
    public decimal PrecioPorNoche { get; set; }
    
    [Required(ErrorMessage = "El estado es obligatorio")]
    [StringLength(50, ErrorMessage = "El estado no puede exceder los 50 caracteres")]
    [Display(Name = "Estado")]
    public string Estado { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Debe seleccionar un hotel")]
    [Display(Name = "Hotel")]
    public int IdHotel { get; set; }
    
    // Campos adicionales
    [Range(1, 10, ErrorMessage = "La capacidad debe ser entre 1 y 10 personas")]
    [Display(Name = "Capacidad (personas)")]
    public int Capacidad { get; set; } = 2;

    [Range(1, 5, ErrorMessage = "El número de camas debe ser entre 1 y 5")]
    [Display(Name = "Número de Camas")]
    public int NumeroCamas { get; set; } = 1;

    [StringLength(50)]
    [Display(Name = "Tipo de Cama")]
    public string? TipoCama { get; set; }

    [StringLength(20)]
    [Display(Name = "Piso")]
    public string? Piso { get; set; }

    [Display(Name = "Tiene Vista")]
    public bool TieneVista { get; set; } = false;

    [StringLength(50)]
    [Display(Name = "Tipo de Vista")]
    public string? TipoVista { get; set; }

    [Range(0, 500, ErrorMessage = "Los metros cuadrados deben estar entre 0 y 500")]
    [Display(Name = "Metros Cuadrados")]
    public decimal? MetrosCuadrados { get; set; }

    [Display(Name = "Baño Privado")]
    public bool TieneBanioPrivado { get; set; } = true;

    [Display(Name = "Tiene Balcón")]
    public bool TieneBalcon { get; set; } = false;

    [StringLength(500)]
    [Display(Name = "Descripción")]
    [DataType(DataType.MultilineText)]
    public string? Descripcion { get; set; }

    [StringLength(1000)]
    [Display(Name = "Servicios Incluidos")]
    [DataType(DataType.MultilineText)]
    public string? Servicios { get; set; }

    // Para mostrar nombre del hotel
    public string? NombreHotel { get; set; }
}
