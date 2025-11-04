using Microsoft.AspNetCore.Identity;

namespace HotelSuite.Domain.Entities;

public class ApplicationUser : IdentityUser
{
  public string NombresCompletos { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
    public bool EstaActivo { get; set; } = true;
}
