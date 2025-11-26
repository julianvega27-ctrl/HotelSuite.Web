# ? CORRECCIÓN - Visualización de Datos en Pagos

## ?? Problema Corregido

### **Problema: No se mostraban datos de huésped ni habitación en Pagos**

**Causa Identificada:**
- Los métodos `Index`, `Pendientes`, `Details` y `Registrar` del `PagosController` no cargaban las relaciones necesarias
- El `PagoProfile` ya tenía el mapeo configurado correctamente:
  ```csharp
  .ForMember(dest => dest.NombreHuesped,
      opt => opt.MapFrom(src => src.Reserva != null && src.Reserva.Huesped != null 
        ? $"{src.Reserva.Huesped.Nombres} {src.Reserva.Huesped.Apellidos}" 
  : null))
  .ForMember(dest => dest.NumeroHabitacion,
      opt => opt.MapFrom(src => src.Reserva != null && src.Reserva.Habitacion != null 
 ? src.Reserva.Habitacion.Numero 
      : null))
  ```
- Pero las relaciones (`Reserva`, `Huesped`, `Habitacion`) no se cargaban con `.Include()`
- Por lo tanto, las propiedades quedaban en `null`

## ? Solución Aplicada

### **1. Método Index Corregido**

**Antes (? Sin relaciones):**
```csharp
public async Task<IActionResult> Index(string? filtroEstado, int? pagina)
{
    var pagos = await _unitOfWork.Pagos.GetAllAsync(); // ? No carga relaciones
    var pagosDTO = _mapper.Map<IEnumerable<PagoDTO>>(pagos);
    // ? NombreHuesped y NumeroHabitacion quedan en null
    return View(pagosPaginados);
}
```

**Después (? Con relaciones):**
```csharp
public async Task<IActionResult> Index(string? filtroEstado, int? pagina)
{
    try
    {
        // ? Cargar pagos CON relaciones (Reserva, Huesped, Habitacion, Hotel)
  var pagos = await _unitOfWork.Pagos
            .GetAllQueryable()
.Include(p => p.Reserva)
  .ThenInclude(r => r.Huesped)     // ? Incluir Huésped
  .Include(p => p.Reserva)
      .ThenInclude(r => r.Habitacion)     // ? Incluir Habitación
 .ThenInclude(h => h.Hotel)     // ? Incluir Hotel
   .ToListAsync();

  // Aplicar filtro
 if (!string.IsNullOrWhiteSpace(filtroEstado))
  {
if (filtroEstado == "Pendiente")
        {
                pagos = pagos.Where(p => p.Metodo == "Pendiente").ToList();
      }
  else if (filtroEstado == "Pagado")
            {
            pagos = pagos.Where(p => p.Metodo != "Pendiente").ToList();
            }
       ViewBag.FiltroEstado = filtroEstado;
 }

   // Ordenar por fecha de pago descendente
        pagos = pagos.OrderByDescending(p => p.FechaPago).ToList();

  // ? Convertir a DTO (AutoMapper usa las relaciones cargadas)
        var pagosDTO = _mapper.Map<IEnumerable<PagoDTO>>(pagos);

    // Aplicar paginación
 int numeroPagina = pagina ?? 1;
        var pagosPaginados = pagosDTO.ToPagedList(numeroPagina, PageSize);

        return View(pagosPaginados);
    }
    catch (Exception ex)
  {
        TempData["Error"] = $"Error al cargar los pagos: {ex.Message}";
  return View(new List<PagoDTO>().ToPagedList(1, PageSize));
    }
}
```

**Mejoras Implementadas:**
- ? `.Include(p => p.Reserva).ThenInclude(r => r.Huesped)` - Carga huésped
- ? `.Include(p => p.Reserva).ThenInclude(r => r.Habitacion).ThenInclude(h => h.Hotel)` - Carga habitación y hotel
- ? AutoMapper automáticamente mapea `NombreHuesped` y `NumeroHabitacion`
- ? Filtros funcionan correctamente después de cargar datos

### **2. Método Pendientes Corregido**

**Antes (? Sin relaciones):**
```csharp
public async Task<IActionResult> Pendientes(int? pagina)
{
    var pagos = await _unitOfWork.Pagos.GetAllAsync();
    pagos = pagos.Where(p => p.Metodo == "Pendiente");
    // ? Sin información de huésped ni habitación
}
```

**Después (? Con relaciones):**
```csharp
public async Task<IActionResult> Pendientes(int? pagina)
{
    try
 {
   // ? Cargar pagos CON relaciones
      var pagos = await _unitOfWork.Pagos
            .GetAllQueryable()
  .Include(p => p.Reserva)
  .ThenInclude(r => r.Huesped)
  .Include(p => p.Reserva)
     .ThenInclude(r => r.Habitacion)
  .ThenInclude(h => h.Hotel)
.ToListAsync();

        // Filtrar solo pagos pendientes
    pagos = pagos.Where(p => p.Metodo == "Pendiente")
            .OrderByDescending(p => p.FechaPago)
            .ToList();

  // Convertir a DTO
 var pagosDTO = _mapper.Map<IEnumerable<PagoDTO>>(pagos);

   // Aplicar paginación
        int numeroPagina = pagina ?? 1;
   var pagosPaginados = pagosDTO.ToPagedList(numeroPagina, PageSize);

        return View(pagosPaginados);
    }
    catch (Exception ex)
    {
        TempData["Error"] = $"Error al cargar los pagos pendientes: {ex.Message}";
    return View(new List<PagoDTO>().ToPagedList(1, PageSize));
    }
}
```

### **3. Método Details Corregido**

**Antes (? Carga relaciones por separado):**
```csharp
public async Task<IActionResult> Details(int id)
{
    var pago = await _unitOfWork.Pagos.GetByIdAsync(id);
  
  // ? Luego carga reserva
    var reserva = await _unitOfWork.Reservas.GetByIdAsync(pago.IdReserva);
    
    // ? Luego carga habitación
    var habitacion = await _unitOfWork.Habitaciones.GetByIdAsync(reserva.IdHabitacion);

    // ? 3 consultas a la BD + mapeo manual
}
```

**Después (? Carga todo en una consulta):**
```csharp
public async Task<IActionResult> Details(int id)
{
    try
    {
 // ? Cargar pago CON todas las relaciones en UNA consulta
        var pago = await _unitOfWork.Pagos
.GetAllQueryable()
     .Include(p => p.Reserva)
    .ThenInclude(r => r.Huesped)
  .Include(p => p.Reserva)
            .ThenInclude(r => r.Habitacion)
            .ThenInclude(h => h.Hotel)
  .FirstOrDefaultAsync(p => p.Id == id);

        if (pago == null)
        {
     TempData["Error"] = "El pago no fue encontrado.";
  return RedirectToAction(nameof(Index));
        }

  var pagoDTO = _mapper.Map<PagoDTO>(pago);
    
// ? Mapear información adicional de la reserva
        if (pago.Reserva != null)
        {
   var reservaDTO = _mapper.Map<ReservaDTO>(pago.Reserva);
        
  // Mapear información del huésped
     if (pago.Reserva.Huesped != null)
     {
    reservaDTO.NombreHuesped = $"{pago.Reserva.Huesped.Nombres} {pago.Reserva.Huesped.Apellidos}";
  ViewBag.Huesped = _mapper.Map<HuespedDTO>(pago.Reserva.Huesped);
    }
     
   // Mapear información de la habitación
    if (pago.Reserva.Habitacion != null)
            {
   reservaDTO.NumeroHabitacion = pago.Reserva.Habitacion.Numero;
     reservaDTO.TipoHabitacion = pago.Reserva.Habitacion.Tipo;
reservaDTO.NombreHotel = pago.Reserva.Habitacion.Hotel?.Nombre ?? "";
    reservaDTO.PrecioPorNoche = pago.Reserva.Habitacion.PrecioPorNoche;
  reservaDTO.PrecioHabitacion = pago.Reserva.Habitacion.PrecioPorNoche;
    reservaDTO.MontoTotal = pago.Reserva.Habitacion.PrecioPorNoche * reservaDTO.DiasEstancia;
     ViewBag.Habitacion = _mapper.Map<HabitacionDTO>(pago.Reserva.Habitacion);
            }
       
ViewBag.Reserva = reservaDTO;
        }

        return View(pagoDTO);
    }
    catch (Exception ex)
    {
      TempData["Error"] = $"Error al cargar los detalles del pago: {ex.Message}";
        return RedirectToAction(nameof(Index));
    }
}
```

**Mejoras:**
- ? 1 consulta en lugar de 3
- ? Carga eager loading de todas las relaciones
- ? Mapeo completo de toda la información
- ? Cálculo automático de `MontoTotal`

### **4. Método Registrar (GET) Corregido**

**Antes (? Consultas separadas):**
```csharp
public async Task<IActionResult> Registrar(int id)
{
    var pagos = await _unitOfWork.Pagos.GetAllAsync();
    var pago = pagos.FirstOrDefault(...);
 
    var reserva = await _unitOfWork.Reservas.GetByIdAsync(id);
var habitacion = await _unitOfWork.Habitaciones.GetByIdAsync(...);
    var huesped = await _unitOfWork.Huespedes.GetByIdAsync(...);
    // ? Múltiples consultas
}
```

**Después (? Una consulta con .Include()):**
```csharp
public async Task<IActionResult> Registrar(int id)
{
    try
    {
        // ? Buscar el pago pendiente CON relaciones en UNA consulta
  var pago = await _unitOfWork.Pagos
       .GetAllQueryable()
      .Include(p => p.Reserva)
       .ThenInclude(r => r.Huesped)
    .Include(p => p.Reserva)
        .ThenInclude(r => r.Habitacion)
.ThenInclude(h => h.Hotel)
  .FirstOrDefaultAsync(p => p.IdReserva == id && p.Metodo == "Pendiente");

        if (pago == null)
        {
            TempData["Error"] = "No se encontró un pago pendiente para esta reserva.";
     return RedirectToAction("Details", "Reservas", new { id });
 }

   var pagoDTO = _mapper.Map<PagoDTO>(pago);
        
// ? Mapear información completa
        if (pago.Reserva != null)
 {
   var reservaDTO = _mapper.Map<ReservaDTO>(pago.Reserva);
    
   if (pago.Reserva.Huesped != null)
            {
   reservaDTO.NombreHuesped = $"{pago.Reserva.Huesped.Nombres} {pago.Reserva.Huesped.Apellidos}";
     ViewBag.Huesped = _mapper.Map<HuespedDTO>(pago.Reserva.Huesped);
  }
    
        if (pago.Reserva.Habitacion != null)
   {
 reservaDTO.NumeroHabitacion = pago.Reserva.Habitacion.Numero;
   reservaDTO.TipoHabitacion = pago.Reserva.Habitacion.Tipo;
      reservaDTO.NombreHotel = pago.Reserva.Habitacion.Hotel?.Nombre ?? "";
    reservaDTO.PrecioPorNoche = pago.Reserva.Habitacion.PrecioPorNoche;
    reservaDTO.PrecioHabitacion = pago.Reserva.Habitacion.PrecioPorNoche;
          reservaDTO.MontoTotal = pago.Reserva.Habitacion.PrecioPorNoche * reservaDTO.DiasEstancia;
      ViewBag.Habitacion = _mapper.Map<HabitacionDTO>(pago.Reserva.Habitacion);
            }
    
 ViewBag.Reserva = reservaDTO;
        }

   await CargarMetodosPago();
        return View(pagoDTO);
    }
    catch (Exception ex)
    {
        TempData["Error"] = $"Error al cargar el pago: {ex.Message}";
return RedirectToAction(nameof(Index));
    }
}
```

## ?? Comparación Antes/Después

### **Vista Index (/Pagos)**

| Columna | Antes ? | Ahora ? |
|---------|----------|----------|
| **ID** | ? Se mostraba | ? Se muestra |
| **Huésped** | ? (null) | ? "Carlos García Martínez" |
| **Habitación** | ? (null) | ? "#A10" |
| **Monto** | ? Se mostraba | ? Se muestra |
| **Fecha** | ? Se mostraba | ? Se muestra |
| **Método** | ? Se mostraba | ? Se muestra |
| **Acciones** | ? Funcionaban | ? Funcionan |

### **Vista Pendientes (/Pagos/Pendientes)**

| Información | Antes ? | Ahora ? |
|-------------|----------|----------|
| **Nombre Huésped** | ? (null) | ? Completo |
| **Número Habitación** | ? (null) | ? "#101" |
| **Filtro Pendientes** | ? Funcionaba | ? Funciona |

### **Vista Details (/Pagos/Details/1)**

| Información | Antes ? | Ahora ? |
|-------------|----------|----------|
| **Información Pago** | ? Completa | ? Completa |
| **Información Huésped** | ? Cargaba en ViewBag | ? Carga en ViewBag |
| **Información Habitación** | ? Cargaba en ViewBag | ? Carga en ViewBag |
| **Consultas a BD** | ? 3-4 consultas | ? 1 consulta |
| **Rendimiento** | ? Lento | ? Rápido |

### **Vista Registrar (/Pagos/Registrar/1)**

| Aspecto | Antes ? | Ahora ? |
|---------|----------|----------|
| **Carga de Datos** | ? 4-5 consultas | ? 1 consulta |
| **Información Completa** | ? Disponible | ? Disponible |
| **Rendimiento** | ? Lento | ? Rápido |

## ?? Cómo Verificar la Corrección

### **1. Verificar Vista Index**

```sh
# 1. Ejecutar aplicación
dotnet run

# 2. Acceder
https://localhost:5001/Pagos

# 3. Verificar tabla:
? Columna "Huésped" muestra: "Nombres Apellidos"
? Columna "Habitación" muestra: "Número"
? Todos los datos están completos (no hay espacios en blanco)
```

**Ejemplo de fila esperada:**
```
| ID | Huésped     | Habitación | Monto    | Fecha      | Método   | Acciones |
|----|-------------|-----------|----------|-----------|----------|----------|
| 15 | Carlos García| A10      | $8,800.00| 04/11/2025| PayPal| ??? ??    |
```

### **2. Verificar Vista Pendientes**

```sh
# 1. Acceder
https://localhost:5001/Pagos/Pendientes

# 2. Verificar:
? Solo muestra pagos con Método = "Pendiente"
? Columna "Huésped" completa
? Columna "Habitación" completa
? Botón "Registrar Pago" visible
```

### **3. Verificar Vista Details**

```sh
# 1. Click en el icono del ojo (???) en cualquier pago
# 2. Verificar que se muestra:

? Información del Pago:
   - ID, Monto, Fecha, Método

? Información de la Reserva:
   - Fechas (entrada/salida)
   - Días de estancia
   - Monto total calculado

? Información del Huésped:
   - Nombre completo
   - Email, Teléfono, Documento

? Información de la Habitación:
   - Hotel
   - Número y tipo
   - Precio por noche
```

### **4. Verificar Vista Registrar**

```sh
# 1. Desde una reserva con pago pendiente
# 2. Click en "Registrar Pago"
# 3. Verificar:

? Se carga toda la información
? Huésped visible
? Habitación visible
? Monto correcto
? Selector de método de pago funcional
```

## ?? Consultas SQL para Verificar Datos

### **Ver pagos con datos relacionados:**

```sql
-- Consulta completa de pagos
SELECT 
    p.Id AS PagoID,
    h.Nombres + ' ' + h.Apellidos AS Huesped,
    hab.Numero AS NumeroHabitacion,
p.Monto,
    p.FechaPago,
 p.Metodo,
    r.FechaEntrada,
    r.FechaSalida,
  DATEDIFF(day, r.FechaEntrada, r.FechaSalida) AS Noches
FROM Pagos p
INNER JOIN Reservas r ON p.IdReserva = r.Id
INNER JOIN Huespedes h ON r.IdHuesped = h.Id
INNER JOIN Habitaciones hab ON r.IdHabitacion = hab.Id
INNER JOIN Hoteles hot ON hab.IdHotel = hot.Id
ORDER BY p.FechaPago DESC;
```

**Resultado esperado:**
```
PagoID | Huesped              | NumeroHabitacion | Monto    | FechaPago  | Metodo   | FechaEntrada | FechaSalida | Noches
-------|---------------------|------------------|----------|------------|----------|-------------|-------------|-------
15     | Carlos García       | A10       | 8800.00  | 2025-11-04 | PayPal | 2025-12-04  | 2025-12-08  | 4
14  | María López         | 1005            | 4800.00  | 2025-11-08 | Pendiente| 2025-11-26  | 2025-11-30  | 4
```

## ? Checklist de Verificación

**Método Index:**
- [x] ? Usa `.Include(p => p.Reserva).ThenInclude(r => r.Huesped)`
- [x] ? Usa `.Include(p => p.Reserva).ThenInclude(r => r.Habitacion).ThenInclude(h => h.Hotel)`
- [x] ? AutoMapper mapea `NombreHuesped` y `NumeroHabitacion` automáticamente
- [x] ? Filtros funcionan correctamente
- [x] ? Paginación funcional

**Método Pendientes:**
- [x] ? Carga todas las relaciones
- [x] ? Filtra solo Método = "Pendiente"
- [x] ? Muestra datos completos

**Método Details:**
- [x] ? Una consulta con todos los `.Include()`
- [x] ? Mapea toda la información
- [x] ? Calcula MontoTotal correctamente
- [x] ? ViewBag.Reserva, ViewBag.Huesped, ViewBag.Habitacion completos

**Método Registrar:**
- [x] ? Carga pago con todas las relaciones
- [x] ? Mapea información completa
- [x] ? ViewBag completos

**Compilación:**
- [x] ? Sin errores
- [x] ? Sin advertencias críticas

## ?? Resumen de Cambios

**Archivo modificado:**
1. ? `HotelSuite/Controllers/PagosController.cs`
   - Método `Index`: Agregado `.Include()` completo
 - Método `Pendientes`: Agregado `.Include()` completo
   - Método `Details`: Refactorizado para usar 1 consulta con `.Include()`
   - Método `Registrar` (GET): Refactorizado para usar 1 consulta con `.Include()`

**Líneas de código modificadas:**
- Index: ~20 líneas
- Pendientes: ~15 líneas
- Details: ~30 líneas
- Registrar: ~30 líneas
- Total: ~95 líneas

**Mejoras implementadas:**
- ? Carga de relaciones con Entity Framework (.Include/.ThenInclude)
- ? Mapeo automático con AutoMapper
- ? Reducción de consultas a BD (de 3-4 a 1)
- ? Mejor rendimiento
- ? Código más mantenible

## ?? Estado Final

```
? Vista Index: Muestra huésped y habitación correctamente
? Vista Pendientes: Datos completos visibles
? Vista Details: Información completa en 1 consulta
? Vista Registrar: Carga eficiente de datos
? AutoMapper: Funciona correctamente con relaciones
? Rendimiento: Mejorado (1 consulta vs 3-4)
? Compilación: Exitosa sin errores

?? MÓDULO DE PAGOS 100% FUNCIONAL
```

---

**Fecha:** 2025-01-04  
**Versión:** 4.2.0  
**Estado:** ? **CORREGIDO**  
**Problema:** Visualización de datos en Pagos  
**Archivos Modificados:** 1 (PagosController.cs)

---

## ?? ¡Problema Resuelto!

**Ahora puedes:**
- ? Ver el nombre completo del huésped en cada pago
- ? Ver el número de habitación claramente
- ? Ver toda la información en Index y Pendientes
- ? Cargar Details en 1 consulta eficiente
- ? Registrar pagos con información completa visible
- ? Generar comprobantes con todos los datos

**¡El módulo de Pagos está completamente funcional!** ??
