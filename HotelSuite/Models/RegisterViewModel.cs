using System.ComponentModel.DataAnnotations;

namespace HotelSuite.Models;

public class RegisterViewModel
{
[Required(ErrorMessage = "El nombre completo es obligatorio")]
  [StringLength(200, ErrorMessage = "El nombre no puede exceder los 200 caracteres")]
    [Display(Name = "Nombre Completo")]
    public string NombresCompletos { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es obligatorio")]
 [EmailAddress(ErrorMessage = "El email no es válido")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [StringLength(100, ErrorMessage = "La contraseña debe tener al menos {2} caracteres.", MinimumLength = 8)]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", 
        ErrorMessage = "La contraseña debe contener al menos una mayúscula, una minúscula, un número y un carácter especial")]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Confirmar Contraseña")]
    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
