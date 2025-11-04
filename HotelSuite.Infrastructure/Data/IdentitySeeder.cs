using HotelSuite.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace HotelSuite.Infrastructure.Data;

public class IdentitySeeder
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<IdentitySeeder> _logger;

    public IdentitySeeder(
     UserManager<ApplicationUser> userManager,
   RoleManager<IdentityRole> roleManager,
        ILogger<IdentitySeeder> logger)
    {
        _userManager = userManager;
    _roleManager = roleManager;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
  try
        {
            _logger.LogInformation("Iniciando seeding de Identity (roles y usuarios)...");

       await SeedRolesAsync();
            await SeedUsersAsync();

       _logger.LogInformation("Seeding de Identity completado exitosamente.");
        }
        catch (Exception ex)
        {
     _logger.LogError(ex, "Error durante el seeding de Identity.");
    throw;
        }
    }

    private async Task SeedRolesAsync()
    {
        _logger.LogInformation("Creando roles...");

        var roles = new[]
  {
     "Administrador",
            "Recepcionista",
       "Limpieza",
        "Mantenimiento",
          "Gerente"
   };

        foreach (var roleName in roles)
        {
          if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
       if (result.Succeeded)
             {
        _logger.LogInformation($"Rol '{roleName}' creado exitosamente.");
    }
          else
     {
     _logger.LogError($"Error al crear el rol '{roleName}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
    }
     }
            else
   {
              _logger.LogInformation($"Rol '{roleName}' ya existe.");
 }
        }
    }

    private async Task SeedUsersAsync()
    {
  _logger.LogInformation("Creando usuarios iniciales...");

        // Usuario Administrador
      await CreateUserAsync(
userName: "admin@hotelsuite.com",
            email: "admin@hotelsuite.com",
            nombresCompletos: "Administrador del Sistema",
            password: "Admin123!",
        role: "Administrador"
        );

        // Usuario Recepcionista
        await CreateUserAsync(
     userName: "recepcion@hotelsuite.com",
            email: "recepcion@hotelsuite.com",
            nombresCompletos: "Andrea Morales Díaz",
            password: "Recepcion123!",
   role: "Recepcionista"
        );

 // Usuario Limpieza
      await CreateUserAsync(
            userName: "limpieza@hotelsuite.com",
      email: "limpieza@hotelsuite.com",
      nombresCompletos: "Rosa Jiménez Pérez",
            password: "Limpieza123!",
            role: "Limpieza"
        );

        // Usuario Mantenimiento
        await CreateUserAsync(
     userName: "mantenimiento@hotelsuite.com",
   email: "mantenimiento@hotelsuite.com",
        nombresCompletos: "Alberto Vega Soto",
            password: "Mantenimiento123!",
 role: "Mantenimiento"
        );

        // Usuario Gerente
  await CreateUserAsync(
            userName: "gerente@hotelsuite.com",
            email: "gerente@hotelsuite.com",
      nombresCompletos: "Gabriela Campos Flores",
     password: "Gerente123!",
        role: "Gerente"
   );
    }

    private async Task CreateUserAsync(string userName, string email, string nombresCompletos, string password, string role)
    {
        var existingUser = await _userManager.FindByEmailAsync(email);
     if (existingUser != null)
   {
            _logger.LogInformation($"Usuario '{email}' ya existe.");
   return;
    }

    var user = new ApplicationUser
        {
    UserName = userName,
            Email = email,
 EmailConfirmed = true,
NombresCompletos = nombresCompletos,
  FechaRegistro = DateTime.Now,
EstaActivo = true
        };

        var result = await _userManager.CreateAsync(user, password);
        
        if (result.Succeeded)
        {
            _logger.LogInformation($"Usuario '{email}' creado exitosamente.");

            // Asignar rol
      var roleResult = await _userManager.AddToRoleAsync(user, role);
      if (roleResult.Succeeded)
          {
    _logger.LogInformation($"Rol '{role}' asignado a '{email}'.");
  }
     else
            {
       _logger.LogError($"Error al asignar rol '{role}' a '{email}': {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
     }
        }
        else
        {
            _logger.LogError($"Error al crear usuario '{email}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }
}
