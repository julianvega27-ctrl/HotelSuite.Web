# ?? HotelSuite - Sistema de Autenticación y Autorización

## ?? Descripción

Sistema completo de autenticación y autorización implementado con ASP.NET Core Identity, incluyendo roles personalizados, protección de controladores y vistas condicionales según permisos.

## ? Características Implementadas

### ?? **ASP.NET Core Identity Configurado**

**Base de Datos:** SQL Server con Entity Framework Core  
**Autenticación:** Cookies-based authentication  
**Usuario Personalizado:** `ApplicationUser` que extiende `IdentityUser`

#### Configuración de Contraseñas (Seguridad Alta)
```csharp
options.Password.RequireDigit = true;      // Requiere al menos un dígito
options.Password.RequireLowercase = true;           // Requiere al menos una minúscula
options.Password.RequireUppercase = true;           // Requiere al menos una mayúscula
options.Password.RequireNonAlphanumeric = true;     // Requiere carácter especial
options.Password.RequiredLength = 8;    // Mínimo 8 caracteres
```

#### Configuración de Bloqueo de Cuenta
```csharp
options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);  // 15 min de bloqueo
options.Lockout.MaxFailedAccessAttempts = 5;      // Máximo 5 intentos fallidos
options.Lockout.AllowedForNewUsers = true;       // Aplicar a usuarios nuevos
```

#### Configuración de Cookies
```csharp
options.LoginPath = "/Account/Login";               // Ruta de inicio de sesión
options.LogoutPath = "/Account/Logout";             // Ruta de cierre de sesión
options.AccessDeniedPath = "/Account/AccessDenied"; // Acceso denegado
options.ExpireTimeSpan = TimeSpan.FromHours(8); // Expiración de sesión
options.SlidingExpiration = true;       // Renovación automática
options.Cookie.HttpOnly = true;   // Seguridad XSS
```

### ?? **Roles del Sistema**

El sistema cuenta con 5 roles predefinidos:

| Rol | Descripción | Permisos |
|-----|-------------|----------|
| **Administrador** | Control total del sistema | Acceso completo a todas las funcionalidades |
| **Gerente** | Gestión del hotel | Reportes, reservas, pagos, habitaciones |
| **Recepcionista** | Operaciones diarias | Reservas, pagos, check-in/check-out |
| **Limpieza** | Personal de limpieza | Tablero de tareas, actualización de estados |
| **Mantenimiento** | Personal técnico | Tablero de tareas, mantenimiento |

### ?? **Usuarios Predefinidos (Demo)**

El sistema crea automáticamente usuarios de demostración:

```csharp
// Administrador
Email: admin@hotelsuite.com
Contraseña: Admin123!
Nombre: Administrador del Sistema

// Recepcionista
Email: recepcion@hotelsuite.com
Contraseña: Recepcion123!
Nombre: Andrea Morales Díaz

// Limpieza
Email: limpieza@hotelsuite.com
Contraseña: Limpieza123!
Nombre: Rosa Jiménez Pérez

// Mantenimiento
Email: mantenimiento@hotelsuite.com
Contraseña: Mantenimiento123!
Nombre: Alberto Vega Soto

// Gerente
Email: gerente@hotelsuite.com
Contraseña: Gerente123!
Nombre: Gabriela Campos Flores
```

### ?? **Páginas de Autenticación**

#### 1. **Login** (`/Account/Login`)
**Características:**
- ? Diseño moderno con gradiente
- ? Validaciones en tiempo real
- ? Opción "Recordarme"
- ? Return URL para redireccionamiento
- ? Mostrar usuarios de demostración
- ? Mensajes de error descriptivos
- ? Protección contra ataques de fuerza bruta

**Validaciones:**
```csharp
[Required(ErrorMessage = "El email es obligatorio")]
[EmailAddress(ErrorMessage = "El email no es válido")]
public string Email { get; set; }

[Required(ErrorMessage = "La contraseña es obligatoria")]
[DataType(DataType.Password)]
public string Password { get; set; }
```

#### 2. **Register** (`/Account/Register`)
**Características:**
- ? Diseño moderno con gradiente verde
- ? Validaciones de contraseña completas
- ? Confirmación de contraseña
- ? Registro automático de sesión
- ? Asignación automática de rol "Recepcionista"
- ? Requisitos de contraseña visibles

**Validaciones:**
```csharp
[Required(ErrorMessage = "La contraseña es obligatoria")]
[StringLength(100, MinimumLength = 8)]
[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", 
    ErrorMessage = "La contraseña debe contener mayúscula, minúscula, número y carácter especial")]
public string Password { get; set; }

[Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
public string ConfirmPassword { get; set; }
```

#### 3. **Logout** (`/Account/Logout` - POST)
**Características:**
- ? Cierre de sesión seguro
- ? Redirección a Login
- ? Mensaje de confirmación
- ? Limpieza de cookies

#### 4. **Access Denied** (`/Account/AccessDenied`)
**Características:**
- ? Página de error personalizada
- ? Información del usuario actual
- ? Roles del usuario mostrados
- ? Opciones de navegación

#### 5. **Profile** (`/Account/Profile`)
**Características:**
- ? Información del usuario
- ? Roles asignados
- ? Fecha de registro
- ? Estado de la cuenta

### ?? **Protección de Controladores**

#### **Nivel de Controlador**

```csharp
[Authorize] // Requiere autenticación
public class HabitacionesController : Controller

[Authorize(Roles = "Administrador,Gerente,Recepcionista")]
public class ReservasController : Controller

[Authorize(Roles = "Administrador,Gerente")]
public class ReportesController : Controller
```

#### **Nivel de Acción**

```csharp
// Todos pueden ver
[AllowAnonymous]
public async Task<IActionResult> Index()

// Solo Admin y Gerente pueden crear
[Authorize(Roles = "Administrador,Gerente")]
public async Task<IActionResult> Create()

// Solo Administrador puede eliminar
[Authorize(Roles = "Administrador")]
public async Task<IActionResult> Delete(int id)
```

### ?? **Protección por Roles Implementada**

#### **HabitacionesController**
```csharp
[Authorize] // Requiere autenticación
?? Index() - [AllowAnonymous] (todos pueden ver)
?? Details() - Autenticados
?? Create() - [Authorize(Roles = "Administrador,Gerente")]
?? Edit() - [Authorize(Roles = "Administrador,Gerente")]
?? Delete() - [Authorize(Roles = "Administrador")] (solo admin)
```

#### **ReservasController**
```csharp
[Authorize(Roles = "Administrador,Gerente,Recepcionista")]
?? Todas las acciones requieren estos roles
```

#### **PagosController**
```csharp
[Authorize(Roles = "Administrador,Gerente,Recepcionista")]
?? Todas las acciones requieren estos roles
```

#### **TareasDepartamentoController**
```csharp
[Authorize] // Requiere autenticación
?? TableroTareas() - Todos los autenticados
?? Create() - [Authorize(Roles = "Administrador,Gerente")]
```

#### **ReportesController**
```csharp
[Authorize(Roles = "Administrador,Gerente")]
?? Solo Administrador y Gerente pueden ver reportes
```

### ?? **Vistas Condicionales según Rol**

#### **Menú de Navegación (_Layout.cshtml)**

```razor
@if (User.Identity?.IsAuthenticated == true)
{
    @if (User.IsInRole("Administrador") || User.IsInRole("Gerente"))
    {
        <li><a asp-controller="Hoteles">Hoteles</a></li>
        <li><a asp-controller="Reportes">Reportes</a></li>
    }

    @if (User.IsInRole("Administrador") || User.IsInRole("Gerente") || User.IsInRole("Recepcionista"))
    {
   <li><a asp-controller="Reservas">Reservas</a></li>
        <li><a asp-controller="Pagos">Pagos</a></li>
    }

    <!-- Todos los autenticados -->
    <li><a asp-controller="Habitaciones">Habitaciones</a></li>
    <li><a asp-controller="TareasDepartamento">Tablero de Tareas</a></li>
}
else
{
    <!-- Usuarios no autenticados -->
    <li><a asp-controller="Habitaciones">Habitaciones</a></li>
}
```

#### **Menú de Usuario**

```razor
@if (User.Identity?.IsAuthenticated == true)
{
    <ul class="navbar-nav ms-auto">
        <li class="nav-item dropdown">
      <a class="nav-link dropdown-toggle">
   <i class="fas fa-user-circle"></i>
              @User.Identity.Name
            </a>
     <ul class="dropdown-menu">
        <li><a asp-controller="Account" asp-action="Profile">Mi Perfil</a></li>
     <li><hr class="dropdown-divider"></li>
       <li>
      <form asp-controller="Account" asp-action="Logout" method="post">
     <button type="submit">Cerrar Sesión</button>
  </form>
       </li>
            </ul>
        </li>
    </ul>
}
else
{
    <ul class="navbar-nav ms-auto">
        <li><a asp-controller="Account" asp-action="Login">Iniciar Sesión</a></li>
   <li><a asp-controller="Account" asp-action="Register">Registrarse</a></li>
    </ul>
}
```

#### **Botones Condicionales en Vistas**

```razor
@if (User.IsInRole("Administrador") || User.IsInRole("Gerente"))
{
    <a asp-action="Create" class="btn btn-primary">Nuevo</a>
    <a asp-action="Edit" asp-route-id="@Model.Id" class="btn btn-warning">Editar</a>
}

@if (User.IsInRole("Administrador"))
{
    <a asp-action="Delete" asp-route-id="@Model.Id" class="btn btn-danger">Eliminar</a>
}
```

### ??? **Estructura de Archivos**

```
HotelSuite/
??? Domain/
?   ??? Entities/
? ??? ApplicationUser.cs (Usuario personalizado)
?
??? Infrastructure/
?   ??? Data/
?   ?   ??? HotelDbContext.cs (Heredade IdentityDbContext)
?   ?   ??? DataSeeder.cs
?   ?   ??? IdentitySeeder.cs (Roles y usuarios iniciales)
?   ??? Migrations/
?   ?   ??? {timestamp}_InitialCreate.cs
?   ?   ??? {timestamp}_AddIdentity.cs (Tablas de Identity)
?   ??? DependencyInjection.cs (Configuración simplificada)
?
??? HotelSuite/
    ??? Controllers/
    ?   ??? AccountController.cs (Login, Register, Logout)
    ?   ??? HabitacionesController.cs (Protegido con [Authorize])
    ?   ??? ReservasController.cs (Roles específicos)
    ?   ??? PagosController.cs (Roles específicos)
    ?   ??? TareasDepartamentoController.cs (Protegido)
    ?   ??? ReportesController.cs (Admin y Gerente solamente)
    ??? Models/
 ?   ??? LoginViewModel.cs
    ?   ??? RegisterViewModel.cs
    ??? Views/
    ?   ??? Account/
    ?   ?   ??? Login.cshtml
    ?   ?   ??? Register.cshtml
    ?   ?   ??? AccessDenied.cshtml
    ?   ?   ??? Profile.cshtml
    ?   ??? Shared/
    ?       ??? _Layout.cshtml (Menú condicional)
    ??? Program.cs (Configuración de Identity)
```

### ?? **Flujo de Autenticación**

```
1. Usuario no autenticado visita página protegida
   ?
2. Middleware de Authentication detecta falta de autenticación
   ?
3. Redirección automática a /Account/Login?returnUrl=/pagina
   ?
4. Usuario ingresa credenciales
   ?
5. AccountController valida credenciales
   ?
6. SignInManager.PasswordSignInAsync() crea cookie
   ?
7. Redirección a returnUrl o página principal
   ?
8. Usuario autenticado puede acceder según sus roles
```

### ?? **Flujo de Autorización**

```
1. Usuario autenticado intenta acceder a acción protegida
   ?
2. [Authorize(Roles = "...")] verifica roles del usuario
   ?
3. ¿Usuario tiene rol requerido?
   ?? Sí ? Permite acceso
   ?? No ? Redirección a /Account/AccessDenied
```

### ?? **Migraciones de Identity**

#### **Tablas Creadas por Identity:**

```sql
AspNetUsers             -- Usuarios del sistema
AspNetRoles         -- Roles disponibles
AspNetUserRoles  -- Relación usuarios-roles (muchos a muchos)
AspNetUserClaims   -- Claims personalizados por usuario
AspNetUserLogins    -- Inicio de sesión externos (Google, Facebook, etc.)
AspNetUserTokens          -- Tokens de recuperación/confirmación
AspNetRoleClaims-- Claims por rol
```

#### **Migración AddIdentity:**

```csharp
dotnet ef migrations add AddIdentity \
 --project HotelSuite.Infrastructure \
    --startup-project HotelSuite \
    --context HotelDbContext
```

**Resultado:**
- ? Migración creada exitosamente
- ? Tablas de Identity agregadas al schema
- ? Relaciones configuradas correctamente

### ?? **Ejecución Automática**

Al iniciar la aplicación (`dotnet run`):

```
1. Aplica migraciones pendientes
   ?
2. Ejecuta DataSeeder (hoteles, habitaciones, etc.)
 ?
3. Ejecuta IdentitySeeder:
   ?? Crea 5 roles (Administrador, Gerente, Recepcionista, Limpieza, Mantenimiento)
   ?? Crea 5 usuarios de demostración
   ?? Asigna roles correspondientes
   ?
4. Sistema listo con autenticación funcional
```

### ?? **Casos de Uso Comunes**

#### **1. Usuario Administrador**
```
? Puede ver, crear, editar y eliminar hoteles
? Puede ver, crear, editar y eliminar habitaciones
? Puede gestionar reservas y pagos
? Puede ver reportes financieros
? Puede crear y asignar tareas
? Acceso completo al sistema
```

#### **2. Usuario Gerente**
```
? Puede ver y crear habitaciones (NO eliminar)
? Puede gestionar reservas y pagos
? Puede ver reportes financieros
? Puede crear y asignar tareas
? NO puede eliminar habitaciones
```

#### **3. Usuario Recepcionista**
```
? Puede ver habitaciones
? Puede gestionar reservas (crear, editar, cancelar)
? Puede registrar pagos
? Puede ver tablero de tareas
? NO puede ver reportes
? NO puede crear/editar habitaciones
```

#### **4. Usuario Limpieza/Mantenimiento**
```
? Puede ver tablero de tareas
? Puede actualizar estado de sus tareas
? NO puede crear habitaciones
? NO puede gestionar reservas
? NO puede ver reportes
```

### ?? **Personalización**

#### **Agregar Nuevo Rol**

1. Modificar `IdentitySeeder.cs`:
```csharp
var roles = new[]
{
    "Administrador",
    "Recepcionista",
    "Limpieza",
    "Mantenimiento",
    "Gerente",
    "NuevoRol" // Agregar aquí
};
```

2. Crear usuario con ese rol:
```csharp
await CreateUserAsync(
    userName: "nuevorol@hotelsuite.com",
    email: "nuevorol@hotelsuite.com",
    nombresCompletos: "Usuario Nuevo Rol",
    password: "NuevoRol123!",
    role: "NuevoRol"
);
```

3. Ejecutar nueva migración:
```bash
dotnet run
```

#### **Proteger Nueva Acción**

```csharp
[Authorize(Roles = "Administrador,Gerente,NuevoRol")]
public async Task<IActionResult> NuevaAccion()
{
    // Código de la acción
}
```

#### **Verificar Rol en Vista**

```razor
@if (User.IsInRole("NuevoRol"))
{
    <a asp-action="NuevaAccion">Enlace para Nuevo Rol</a>
}
```

### ??? **Seguridad Implementada**

#### **Protección contra Ataques**

? **CSRF (Cross-Site Request Forgery)**
```csharp
[ValidateAntiForgeryToken] // En todos los POST
```

? **XSS (Cross-Site Scripting)**
```csharp
options.Cookie.HttpOnly = true; // Cookies no accesibles desde JavaScript
```

? **Fuerza Bruta**
```csharp
options.Lockout.MaxFailedAccessAttempts = 5; // Bloqueo después de 5 intentos
```

? **Contraseñas Débiles**
```csharp
options.Password.RequireDigit = true;
options.Password.RequireUppercase = true;
options.Password.RequireNonAlphanumeric = true;
options.Password.RequiredLength = 8;
```

? **SQL Injection**
- EF Core usa consultas parametrizadas automáticamente

? **Cookies Seguras**
```csharp
options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
```

### ?? **Comandos Útiles**

#### **Resetear Base de Datos con Identity**
```bash
dotnet ef database drop --force
dotnet run  # Crea BD, aplica migraciones y ejecuta seeders
```

#### **Crear Migración**
```bash
dotnet ef migrations add NombreMigracion \
    --project HotelSuite.Infrastructure \
    --startup-project HotelSuite
```

#### **Aplicar Migraciones**
```bash
dotnet ef database update \
    --project HotelSuite.Infrastructure \
    --startup-project HotelSuite
```

### ? **Checklist de Implementación**

- [x] ASP.NET Core Identity configurado
- [x] Base de datos SQL Server
- [x] ApplicationUser personalizado
- [x] 5 roles creados (Administrador, Gerente, Recepcionista, Limpieza, Mantenimiento)
- [x] 5 usuarios de demostración
- [x] Página de Login funcional
- [x] Página de Register funcional
- [x] Página de Logout funcional
- [x] Página de Access Denied
- [x] Página de Profile
- [x] Controladores protegidos con [Authorize]
- [x] Acciones protegidas con roles específicos
- [x] Menú de navegación condicional según roles
- [x] Vistas condicionales según permisos
- [x] Migraciones de Identity aplicadas
- [x] Seeding automático de roles y usuarios
- [x] Validaciones de contraseña implementadas
- [x] Bloqueo de cuenta por intentos fallidos
- [x] Cookies seguras configuradas
- [x] Protección CSRF activada

---

**Versión**: 1.0.0  
**Última actualización**: 2025  
**Autor**: Sistema HotelSuite  
**Tecnologías**: ASP.NET Core Identity 9, EF Core 9, SQL Server, Cookie Authentication
