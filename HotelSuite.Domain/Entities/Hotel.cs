using System.ComponentModel.DataAnnotations;

namespace HotelSuite.Domain.Entities;

public class Hotel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del hotel es obligatorio")]
    [StringLength(200, ErrorMessage = "El nombre no puede exceder los 200 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La dirección es obligatoria")]
    [StringLength(300, ErrorMessage = "La dirección no puede exceder los 300 caracteres")]
    public string Direccion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio")]
    [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres")]
    [Phone(ErrorMessage = "Formato de teléfono inválido")]
    public string Telefono { get; set; } = string.Empty;

    [Required(ErrorMessage = "La categoría es obligatoria")]
    [Range(1, 5, ErrorMessage = "La categoría debe estar entre 1 y 5 estrellas")]
    public int Categoria { get; set; }

    // Relaciones
    public ICollection<Habitacion> Habitaciones { get; set; } = new List<Habitacion>();
}
