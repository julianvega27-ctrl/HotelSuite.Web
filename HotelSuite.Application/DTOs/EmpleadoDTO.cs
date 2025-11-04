using System.ComponentModel.DataAnnotations;

namespace HotelSuite.Application.DTOs;

public class EmpleadoDTO
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Los nombres son obligatorios")]
    [StringLength(100)]
    [Display(Name = "Nombres")]
    public string Nombres { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Los apellidos son obligatorios")]
    [StringLength(100)]
   [Display(Name = "Apellidos")]
    public string Apellidos { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "El cargo es obligatorio")]
    [StringLength(100)]
    [Display(Name = "Cargo")]
    public string Cargo { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "El email es obligatorio")]
    [StringLength(150)]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "El teléfono es obligatorio")]
    [StringLength(20)]
    [Phone]
    [Display(Name = "Teléfono")]
    public string Telefono { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe seleccionar un departamento")]
    [Display(Name = "Departamento")]
 public int IdDepartamento { get; set; }
    
    // Propiedades de navegación simplificadas
    public string? NombreDepartamento { get; set; }
    public string NombreCompleto => $"{Nombres} {Apellidos}";
}
