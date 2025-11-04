using System.ComponentModel.DataAnnotations;

namespace HotelSuite.Domain.Entities;

public class Departamento
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del departamento es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]
    public string? Descripcion { get; set; }

    // Relaciones
    public ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
    public ICollection<TareaDepartamento> TareasDepartamento { get; set; } = new List<TareaDepartamento>();
}
