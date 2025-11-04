using HotelSuite.Domain.Entities;
using HotelSuite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HotelSuite.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        UserManager<ApplicationUser> userManager,
   SignInManager<ApplicationUser> signInManager,
   ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    // GET: /Account/Login
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
   {
            return RedirectToAction("Index", "Home");
        }

      ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

// POST: /Account/Login
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        ViewData["ReturnUrl"] = model.ReturnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
}

  try
      {
  var user = await _userManager.FindByEmailAsync(model.Email);
   if (user == null)
{
    ModelState.AddModelError(string.Empty, "Email o contraseña incorrectos.");
        return View(model);
            }

     if (!user.EstaActivo)
        {
       ModelState.AddModelError(string.Empty, "Su cuenta está desactivada. Contacte al administrador.");
                return View(model);
     }

 var result = await _signInManager.PasswordSignInAsync(
 user.UserName!,
    model.Password,
     model.RememberMe,
    lockoutOnFailure: true
  );

       if (result.Succeeded)
            {
    _logger.LogInformation($"Usuario {model.Email} inició sesión exitosamente.");
       
    // Obtener roles del usuario
    var roles = await _userManager.GetRolesAsync(user);
    _logger.LogInformation($"Usuario {model.Email} tiene los roles: {string.Join(", ", roles)}");

       if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
      {
   return Redirect(model.ReturnUrl);
      }

          return RedirectToAction("Index", "Home");
     }

         if (result.IsLockedOut)
    {
   _logger.LogWarning($"Usuario {model.Email} bloqueado por múltiples intentos fallidos.");
     TempData["Error"] = "Su cuenta ha sido bloqueada por múltiples intentos fallidos. Intente nuevamente en 15 minutos.";
    return View(model);
       }

      ModelState.AddModelError(string.Empty, "Email o contraseña incorrectos.");
     return View(model);
        }
        catch (Exception ex)
  {
  _logger.LogError(ex, $"Error durante el inicio de sesión del usuario {model.Email}");
      TempData["Error"] = "Ocurrió un error durante el inicio de sesión. Por favor intente nuevamente.";
         return View(model);
  }
    }

    // GET: /Account/Register
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
    {
return RedirectToAction("Index", "Home");
        }

     return View();
    }

    // POST: /Account/Register
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
    if (!ModelState.IsValid)
  {
 return View(model);
  }

  try
        {
var existingUser = await _userManager.FindByEmailAsync(model.Email);
          if (existingUser != null)
            {
    ModelState.AddModelError("Email", "Este email ya está registrado.");
   return View(model);
  }

     var user = new ApplicationUser
       {
  UserName = model.Email,
    Email = model.Email,
         EmailConfirmed = true,
     NombresCompletos = model.NombresCompletos,
      FechaRegistro = DateTime.Now,
         EstaActivo = true
     };

  var result = await _userManager.CreateAsync(user, model.Password);

     if (result.Succeeded)
   {
  _logger.LogInformation($"Usuario {model.Email} registrado exitosamente.");

    // Asignar rol por defecto (Recepcionista)
      await _userManager.AddToRoleAsync(user, "Recepcionista");
  _logger.LogInformation($"Rol 'Recepcionista' asignado a {model.Email}.");

                // Iniciar sesión automáticamente
    await _signInManager.SignInAsync(user, isPersistent: false);

       TempData["Success"] = "¡Registro exitoso! Bienvenido al sistema.";
    return RedirectToAction("Index", "Home");
      }

            foreach (var error in result.Errors)
            {
       ModelState.AddModelError(string.Empty, error.Description);
  }

   return View(model);
 }
   catch (Exception ex)
        {
 _logger.LogError(ex, $"Error durante el registro del usuario {model.Email}");
     TempData["Error"] = "Ocurrió un error durante el registro. Por favor intente nuevamente.";
            return View(model);
        }
    }

    // POST: /Account/Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
{
   try
   {
          var userEmail = User.Identity?.Name;
       await _signInManager.SignOutAsync();
  _logger.LogInformation($"Usuario {userEmail} cerró sesión.");
            
      TempData["Success"] = "Ha cerrado sesión exitosamente.";
     return RedirectToAction("Login", "Account");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante el cierre de sesión");
        TempData["Error"] = "Ocurrió un error al cerrar sesión.";
       return RedirectToAction("Index", "Home");
        }
    }

    // GET: /Account/AccessDenied
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    // GET: /Account/Profile (opcional)
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
   return RedirectToAction("Login");
     }

        var roles = await _userManager.GetRolesAsync(user);

        ViewBag.User = user;
    ViewBag.Roles = roles;

  return View();
    }
}
