using System.ComponentModel.DataAnnotations;

namespace HotelSuite.Domain.Entities;

public class TareaDepartamento
{
  public int Id { get; set; }

    [Required(ErrorMessage = "El título es obligatorio")]
    [StringLength(200, ErrorMessage = "El título no puede exceder los 200 caracteres")]
    public string Titulo { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "La descripción no puede exceder los 1000 caracteres")]
    public string? Descripcion { get; set; }

  [Required(ErrorMessage = "El estado es obligatorio")]
    [StringLength(50, ErrorMessage = "El estado no puede exceder los 50 caracteres")]
    public string Estado { get; set; } = "Pendiente";

    [Required(ErrorMessage = "La prioridad es obligatoria")]
    [StringLength(20)]
    public string Prioridad { get; set; } = "Media";

    [Required]
    public DateTime FechaAsignacion { get; set; } = DateTime.Now;

    public DateTime? FechaFinalizacion { get; set; }

    [Required(ErrorMessage = "El IdDepartamento es obligatorio")]
    public int IdDepartamento { get; set; }

    public int? IdEmpleadoAsignado { get; set; }

    // Relaciones
    public Departamento Departamento { get; set; } = null!;
    public Empleado? Empleado { get; set; }
}
