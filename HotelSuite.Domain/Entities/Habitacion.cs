using System.ComponentModel.DataAnnotations;

namespace HotelSuite.Domain.Entities;

public class Habitacion
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El número de habitación es obligatorio")]
    [StringLength(10, ErrorMessage = "El número no puede exceder los 10 caracteres")]
    public string Numero { get; set; } = string.Empty;

    [Required(ErrorMessage = "El tipo de habitación es obligatorio")]
  [StringLength(50, ErrorMessage = "El tipo no puede exceder los 50 caracteres")]
    public string Tipo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El precio por noche es obligatorio")]
    [Range(0.01, 999999.99, ErrorMessage = "El precio debe ser mayor a 0")]
    public decimal PrecioPorNoche { get; set; }

    [Required(ErrorMessage = "El estado es obligatorio")]
    [StringLength(50, ErrorMessage = "El estado no puede exceder los 50 caracteres")]
    public string Estado { get; set; } = string.Empty;

    [Required(ErrorMessage = "El IdHotel es obligatorio")]
    public int IdHotel { get; set; }

    // Nuevos campos descriptivos
    [Range(1, 10, ErrorMessage = "La capacidad debe ser entre 1 y 10 personas")]
    public int Capacidad { get; set; } = 2;

    [Range(1, 5, ErrorMessage = "El número de camas debe ser entre 1 y 5")]
    public int NumeroCamas { get; set; } = 1;

    [StringLength(50)]
    public string? TipoCama { get; set; } // King, Queen, Individual, Doble

    [StringLength(20)]
    public string? Piso { get; set; }

    public bool TieneVista { get; set; } = false;

    [StringLength(50)]
  public string? TipoVista { get; set; } // Mar, Ciudad, Montaña, Jardín

    [Range(0, 500, ErrorMessage = "Los metros cuadrados deben estar entre 0 y 500")]
    public decimal? MetrosCuadrados { get; set; }

    public bool TieneBanioPrivado { get; set; } = true;

    public bool TieneBalcon { get; set; } = false;

    [StringLength(500)]
    public string? Descripcion { get; set; }

    [StringLength(1000)]
    public string? Servicios { get; set; } // WiFi, TV, Minibar, etc. (separados por comas)

    // Relaciones
    public Hotel Hotel { get; set; } = null!;
    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
