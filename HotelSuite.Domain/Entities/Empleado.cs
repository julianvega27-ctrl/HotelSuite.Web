using System.ComponentModel.DataAnnotations;

namespace HotelSuite.Domain.Entities;

public class Empleado
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Los nombres son obligatorios")]
    [StringLength(100, ErrorMessage = "Los nombres no pueden exceder los 100 caracteres")]
    public string Nombres { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los apellidos son obligatorios")]
    [StringLength(100, ErrorMessage = "Los apellidos no pueden exceder los 100 caracteres")]
    public string Apellidos { get; set; } = string.Empty;

    [Required(ErrorMessage = "El cargo es obligatorio")]
    [StringLength(100, ErrorMessage = "El cargo no puede exceder los 100 caracteres")]
    public string Cargo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es obligatorio")]
    [StringLength(150, ErrorMessage = "El email no puede exceder los 150 caracteres")]
    [EmailAddress(ErrorMessage = "Formato de email inválido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio")]
    [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres")]
    [Phone(ErrorMessage = "Formato de teléfono inválido")]
    public string Telefono { get; set; } = string.Empty;

    [Required(ErrorMessage = "El departamento es obligatorio")]
    public int IdDepartamento { get; set; }

    // Relaciones
    public Departamento Departamento { get; set; } = null!;
    public ICollection<TareaDepartamento> TareasDepartamento { get; set; } = new List<TareaDepartamento>();
}
