# HotelSuite.Application - Capa de Aplicación

## Descripción

Esta capa contiene los DTOs (Data Transfer Objects) y los perfiles de AutoMapper para transformar las entidades del dominio en objetos optimizados para las vistas.

## Estructura del Proyecto

```
HotelSuite.Application/
??? DTOs/
?   ??? HotelDTO.cs
?   ??? HabitacionDTO.cs
?   ??? HuespedDTO.cs
?   ??? ReservaDTO.cs
?   ??? PagoDTO.cs
?   ??? EmpleadoDTO.cs
?   ??? DepartamentoDTO.cs
?   ??? TareaDepartamentoDTO.cs
??? Mapping/
?   ??? HotelProfile.cs
?   ??? HabitacionProfile.cs
?   ??? HuespedProfile.cs
?   ??? ReservaProfile.cs
?   ??? PagoProfile.cs
???? EmpleadoProfile.cs
?   ??? DepartamentoProfile.cs
???? TareaDepartamentoProfile.cs
??? DependencyInjection.cs
```

## DTOs Implementados

### 1. HotelDTO
Propiedades básicas de un hotel sin referencias circulares.

### 2. HabitacionDTO
Incluye información del hotel asociado mediante `NombreHotel`.

### 3. HuespedDTO
Incluye propiedad calculada `NombreCompleto`.

### 4. ReservaDTO
Incluye información del huésped y habitación, además de la propiedad calculada `DiasEstancia`.

### 5. PagoDTO
Incluye información del huésped y habitación relacionados a través de la reserva.

### 6. EmpleadoDTO
Incluye propiedad calculada `NombreCompleto`.

### 7. DepartamentoDTO
Propiedades básicas sin colecciones.

### 8. TareaDepartamentoDTO
Incluye nombres del departamento y empleado asociados.

## Configuración de AutoMapper

Los perfiles de AutoMapper están configurados con:

- **CreateMap<Entidad, DTO>().ReverseMap()**: Para mapeos bidireccionales
- **ForMember**: Para mapear propiedades de navegación a propiedades simples
- **Ignore**: Para evitar referencias circulares al hacer ReverseMap

## Uso en Program.cs

```csharp
using HotelSuite.Application;
using HotelSuite.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios de Application (AutoMapper)
builder.Services.AddApplication();

// Agregar servicios de Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);

// ... resto de configuración
```

## Ejemplos de Uso en Controladores

### Ejemplo 1: Obtener todas las habitaciones

```csharp
using AutoMapper;
using HotelSuite.Application.DTOs;
using HotelSuite.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

public class HabitacionesController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public HabitacionesController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
   _mapper = mapper;
    }

    public async Task<IActionResult> Index()
{
    var habitaciones = await _unitOfWork.Habitaciones.GetAllAsync();
        var habitacionesDTO = _mapper.Map<IEnumerable<HabitacionDTO>>(habitaciones);
     return View(habitacionesDTO);
    }
}
```

### Ejemplo 2: Crear una nueva reserva

```csharp
[HttpPost]
public async Task<IActionResult> Create(ReservaDTO reservaDTO)
{
 if (!ModelState.IsValid)
    {
      return View(reservaDTO);
    }

    // Convertir DTO a Entidad
    var reserva = _mapper.Map<Reserva>(reservaDTO);
  
    // Guardar en la base de datos
    await _unitOfWork.Reservas.AddAsync(reserva);
    await _unitOfWork.CommitAsync();

    return RedirectToAction(nameof(Index));
}
```

### Ejemplo 3: Obtener reserva con información relacionada

```csharp
public async Task<IActionResult> Details(int id)
{
    var reserva = await _unitOfWork.Reservas.GetByIdAsync(id);
    
    if (reserva == null)
    {
        return NotFound();
    }

    // AutoMapper mapeará automáticamente las propiedades de navegación
    var reservaDTO = _mapper.Map<ReservaDTO>(reserva);
    
    return View(reservaDTO);
}
```

### Ejemplo 4: Actualizar un huésped

```csharp
[HttpPost]
public async Task<IActionResult> Edit(int id, HuespedDTO huespedDTO)
{
    if (id != huespedDTO.Id)
    {
return BadRequest();
    }

    if (!ModelState.IsValid)
    {
        return View(huespedDTO);
    }

    // Convertir DTO a Entidad
    var huesped = _mapper.Map<Huesped>(huespedDTO);
    
    // Actualizar
    _unitOfWork.Huespedes.Update(huesped);
    await _unitOfWork.CommitAsync();

    return RedirectToAction(nameof(Index));
}
```

### Ejemplo 5: Crear pago con información relacionada

```csharp
[HttpPost]
public async Task<IActionResult> RegistrarPago(PagoDTO pagoDTO)
{
    if (!ModelState.IsValid)
    {
        return View(pagoDTO);
  }

  var pago = _mapper.Map<Pago>(pagoDTO);
 
    await _unitOfWork.Pagos.AddAsync(pago);
    await _unitOfWork.CommitAsync();

    return RedirectToAction("Details", "Reservas", new { id = pagoDTO.IdReserva });
}
```

## Beneficios de usar DTOs

1. **Evita referencias circulares**: Las entidades pueden tener referencias bidireccionales que causan problemas en la serialización JSON.
2. **Optimización**: Solo se envían los datos necesarios a la vista.
3. **Seguridad**: Evita exponer propiedades sensibles del dominio.
4. **Flexibilidad**: Se pueden agregar propiedades calculadas sin modificar las entidades.
5. **Versionado**: Facilita mantener diferentes versiones de la API.

## Propiedades Calculadas Implementadas

- **HuespedDTO.NombreCompleto**: Combina Nombres y Apellidos
- **EmpleadoDTO.NombreCompleto**: Combina Nombres y Apellidos
- **ReservaDTO.DiasEstancia**: Calcula la diferencia entre FechaSalida y FechaEntrada

## Mapeos Especiales

### HabitacionProfile
Mapea `Hotel.Nombre` a `HabitacionDTO.NombreHotel`

### ReservaProfile
Mapea información del huésped y habitación a propiedades simples del DTO

### PagoProfile
Mapea información del huésped y habitación a través de la reserva

### TareaDepartamentoProfile
Mapea nombres del departamento y empleado

## Notas Importantes

- Todos los perfiles usan `ReverseMap()` para permitir conversión bidireccional
- Las colecciones de navegación se ignoran en el mapeo inverso para evitar problemas
- Los DTOs no incluyen validaciones DataAnnotations (se asume que vienen de las entidades)
- Para consultas con navegación, considera usar Entity Framework Include() para cargar datos relacionados

## Dependencias

- AutoMapper 12.0.1
- AutoMapper.Extensions.Microsoft.DependencyInjection 12.0.1
- HotelSuite.Domain (referencia de proyecto)
