using System.ComponentModel.DataAnnotations;

namespace HotelSuite.Application.DTOs;

public class TareaDepartamentoDTO
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El título es obligatorio")]
    [StringLength(200, ErrorMessage = "El título no puede exceder los 200 caracteres")]
    [Display(Name = "Título")]
    public string Titulo { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "La descripción no puede exceder los 1000 caracteres")]
    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }

    [Required(ErrorMessage = "El estado es obligatorio")]
    [Display(Name = "Estado")]
    public string Estado { get; set; } = "Pendiente";

    [Required(ErrorMessage = "La prioridad es obligatoria")]
    [Display(Name = "Prioridad")]
    public string Prioridad { get; set; } = "Media";

    [Required]
    [Display(Name = "Fecha de Asignación")]
    public DateTime FechaAsignacion { get; set; } = DateTime.Now;

    [Display(Name = "Fecha de Finalización")]
    public DateTime? FechaFinalizacion { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un departamento")]
    [Display(Name = "Departamento")]
    public int IdDepartamento { get; set; }

    [Display(Name = "Empleado Asignado")]
    public int? IdEmpleadoAsignado { get; set; }
    
    // Propiedades de navegación simplificadas
 public string? NombreDepartamento { get; set; }
    public string? NombreEmpleado { get; set; }
}
