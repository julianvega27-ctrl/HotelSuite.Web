# ?? HotelSuite - Módulo de Reportes

## ?? Descripción

Módulo completo de reportes con visualizaciones interactivas usando Chart.js, consultas LINQ optimizadas y selectores de rango de fechas para análisis de ocupación, ingresos y habitaciones populares.

## ? Características Implementadas

### ?? Reportes Disponibles

#### 1. **Reporte de Ocupación** ??
**URL:** `/Reportes/Ocupacion`

**Características:**
- ? Selector de rango de fechas (desde/hasta)
- ? Estadísticas generales:
  - Total de habitaciones
  - Habitaciones ocupadas
  - Porcentaje de ocupación
  - Total de reservas activas
- ? **Gráfico de línea**: Ocupación por día
- ? **Gráfico de barras**: Ocupación por tipo de habitación
- ? **Gráfico de dona**: Distribución de ocupación
- ? **Tabla detallada**: Ocupación por tipo con progress bars

**Consultas LINQ:**
```csharp
// Filtrar reservas en rango de fechas
var reservasEnRango = reservas.Where(r =>
    r.FechaEntrada <= fechaFin &&
    r.FechaSalida >= fechaInicio
).ToList();

// Ocupación por día
var ocupacionPorDia = new Dictionary<DateTime, int>();
while (fechaActual <= fechaFin)
{
    var ocupadasEnDia = reservasEnRango.Count(r =>
r.FechaEntrada.Date <= fechaActual &&
        r.FechaSalida.Date >= fechaActual &&
        (r.Estado == "Confirmada" || r.Estado == "En curso")
    );
    ocupacionPorDia[fechaActual] = ocupadasEnDia;
    fechaActual = fechaActual.AddDays(1);
}

// Ocupación por tipo
var ocupacionPorTipo = habitaciones
    .GroupBy(h => h.Tipo)
    .Select(g => new {
        Tipo = g.Key,
        Total = g.Count(),
   Ocupadas = reservasEnRango
     .Where(r => r.Estado == "Confirmada" || r.Estado == "En curso")
         .Join(g, r => r.IdHabitacion, h => h.Id, (r, h) => r)
    .Select(r => r.IdHabitacion)
 .Distinct()
            .Count()
    })
  .ToList();
```

#### 2. **Reporte de Ingresos** ??
**URL:** `/Reportes/Ingresos`

**Características:**
- ? Selector de mes y año
- ? Estadísticas generales:
  - Ingresos totales del mes
  - Total de pagos
  - Promedio por pago
- ? **Gráfico de barras**: Ingresos por día del mes
- ? **Gráfico de dona**: Distribución por método de pago
- ? **Gráfico de línea**: Comparación últimos 6 meses
- ? **Tablas**:
  - Desglose por método de pago
  - Top 5 reservas más costosas

**Consultas LINQ:**
```csharp
// Filtrar pagos del mes (excluir pendientes)
var pagosMes = pagos.Where(p =>
    p.FechaPago.Year == anio &&
    p.FechaPago.Month == mes &&
    p.Metodo != "Pendiente"
).ToList();

// Ingresos por día
var ingresosPorDia = pagosMes
 .GroupBy(p => p.FechaPago.Day)
    .Select(g => new {
        Dia = g.Key,
        Total = g.Sum(p => p.Monto)
    })
    .OrderBy(x => x.Dia)
    .ToList();

// Ingresos por método de pago
var ingresosPorMetodo = pagosMes
    .GroupBy(p => p.Metodo)
    .Select(g => new {
        Metodo = g.Key,
        Total = g.Sum(p => p.Monto),
   Cantidad = g.Count()
    })
    .OrderByDescending(x => x.Total)
    .ToList();

// Comparación con meses anteriores (últimos 6)
for (int i = 5; i >= 0; i--)
{
    var fecha = new DateTime(anio, mes, 1).AddMonths(-i);
    var ingresosMes = pagos
        .Where(p => p.FechaPago.Year == fecha.Year &&
         p.FechaPago.Month == fecha.Month &&
   p.Metodo != "Pendiente")
 .Sum(p => p.Monto);
}

// Top 5 reservas más costosas
var topReservas = pagosMes
    .OrderByDescending(p => p.Monto)
    .Take(5)
    .ToList();
```

#### 3. **Habitaciones Más Reservadas** ?
**URL:** `/Reportes/HabitacionesPopulares`

**Características:**
- ? Selector de rango de fechas (desde/hasta)
- ? Estadística general: Total de reservas
- ? **Top 10 habitaciones** con ranking (??????)
- ? **Gráfico circular**: Distribución por tipo
- ? **Gráfico de línea**: Tendencia mensual de reservas
- ? **Tabla completa** con:
  - Ranking
  - Número de habitación
  - Hotel
  - Tipo
  - Total reservas
  - Total noches
  - Ingresos generados
  - Tasa de ocupación (progress bar)
- ? **Cards por hotel**: Estadísticas individuales

**Consultas LINQ:**
```csharp
// Top 10 habitaciones más reservadas
var habitacionesPopulares = reservasEnRango
    .GroupBy(r => r.IdHabitacion)
    .Select(g => new {
        IdHabitacion = g.Key,
        TotalReservas = g.Count(),
   TotalNoches = g.Sum(r => (r.FechaSalida - r.FechaEntrada).Days),
        IngresoTotal = g.Sum(r => {
      var hab = habitaciones.FirstOrDefault(h => h.Id == r.IdHabitacion);
  var noches = (r.FechaSalida - r.FechaEntrada).Days;
        return hab != null ? hab.PrecioPorNoche * noches : 0;
 }),
     Habitacion = habitaciones.FirstOrDefault(h => h.Id == g.Key)
    })
    .OrderByDescending(x => x.TotalReservas)
    .Take(10)
    .ToList();

// Distribución por tipo
var reservasPorTipo = reservasEnRango
.Join(habitaciones, r => r.IdHabitacion, h => h.Id, (r, h) => h.Tipo)
    .GroupBy(tipo => tipo)
    .Select(g => new {
        Tipo = g.Key,
        Cantidad = g.Count()
    })
    .OrderByDescending(x => x.Cantidad)
    .ToList();

// Tendencia mensual
var tendenciaMensual = reservasEnRango
    .GroupBy(r => new { r.FechaReserva.Year, r.FechaReserva.Month })
    .Select(g => new {
Fecha = new DateTime(g.Key.Year, g.Key.Month, 1),
        Total = g.Count()
    })
    .OrderBy(x => x.Fecha)
    .ToList();

// Estadísticas por hotel
var estadisticasPorHotel = reservasEnRango
    .Join(habitaciones, r => r.IdHabitacion, h => h.Id, (r, h) => h)
    .GroupBy(h => h.IdHotel)
    .Select(g => new {
        IdHotel = g.Key,
        Hotel = hoteles.FirstOrDefault(h => h.Id == g.Key),
      TotalReservas = g.Count(),
        HabitacionesMasReservada = g.GroupBy(h => h.Id)
     .OrderByDescending(hg => hg.Count())
  .FirstOrDefault()?.First()
    })
    .ToList();
```

## ?? Visualizaciones con Chart.js

### Chart.js v4.4.0

**CDN incluido en las vistas:**
```html
<script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.0/dist/chart.umd.min.js"></script>
```

### Tipos de Gráficos Implementados

#### 1. **Line Chart** (Línea)
- Ocupación por día
- Tendencia mensual
- Comparación de ingresos

```javascript
new Chart(ctx, {
  type: 'line',
    data: {
        labels: fechas,
        datasets: [{
   label: 'Ocupación',
  data: valores,
     borderColor: 'rgb(75, 192, 192)',
    backgroundColor: 'rgba(75, 192, 192, 0.2)',
            tension: 0.4,
        fill: true
   }]
    },
    options: {
      responsive: true,
    plugins: {
         legend: { display: true }
        },
   scales: {
            y: { beginAtZero: true }
  }
    }
});
```

#### 2. **Bar Chart** (Barras)
- Ocupación por tipo
- Ingresos por día

```javascript
new Chart(ctx, {
    type: 'bar',
    data: {
  labels: tipos,
        datasets: [{
  label: 'Total',
       data: totales,
    backgroundColor: 'rgba(54, 162, 235, 0.5)',
       borderColor: 'rgb(54, 162, 235)',
            borderWidth: 2
     }]
    }
});
```

#### 3. **Doughnut Chart** (Dona)
- Distribución de ocupación
- Ingresos por método de pago

```javascript
new Chart(ctx, {
    type: 'doughnut',
    data: {
        labels: metodos,
        datasets: [{
data: montos,
         backgroundColor: [
     'rgba(255, 99, 132, 0.7)',
  'rgba(54, 162, 235, 0.7)',
             'rgba(255, 206, 86, 0.7)',
    'rgba(75, 192, 192, 0.7)',
       'rgba(153, 102, 255, 0.7)'
  ]
        }]
    },
    options: {
        plugins: {
            legend: { position: 'bottom' }
   }
  }
});
```

#### 4. **Pie Chart** (Circular)
- Reservas por tipo de habitación

```javascript
new Chart(ctx, {
    type: 'pie',
  data: {
        labels: tipos,
        datasets: [{
  data: cantidades,
            backgroundColor: colores
        }]
    }
});
```

## ??? Selector de Rango de Fechas

### HTML5 Date Input

**Reporte de Ocupación:**
```html
<form asp-action="Ocupacion" method="get" class="row g-3">
    <div class="col-md-5">
        <label for="fechaInicio" class="form-label fw-bold">Fecha Inicio</label>
        <input type="date" 
      class="form-control form-control-lg" 
        id="fechaInicio" 
        name="fechaInicio" 
      value="@ViewBag.FechaInicio" 
             required>
    </div>
    <div class="col-md-5">
        <label for="fechaFin" class="form-label fw-bold">Fecha Fin</label>
    <input type="date" 
class="form-control form-control-lg" 
 id="fechaFin" 
           name="fechaFin" 
         value="@ViewBag.FechaFin" 
       required>
    </div>
    <div class="col-md-2 d-flex align-items-end">
 <button type="submit" class="btn btn-primary btn-lg w-100">
<i class="fas fa-search me-1"></i>Generar
        </button>
    </div>
</form>
```

**Reporte de Ingresos (Mes/Año):**
```html
<form asp-action="Ingresos" method="get" class="row g-3">
    <div class="col-md-5">
        <label for="anio" class="form-label fw-bold">Año</label>
        <select class="form-select form-select-lg" id="anio" name="anio">
            @for (int i = DateTime.Now.Year; i >= DateTime.Now.Year - 5; i--)
      {
            <option value="@i" selected="@(i == ViewBag.Anio)">@i</option>
            }
        </select>
    </div>
    <div class="col-md-5">
        <label for="mes" class="form-label fw-bold">Mes</label>
        <select class="form-select form-select-lg" id="mes" name="mes">
            <option value="1" selected="@(ViewBag.Mes == 1)">Enero</option>
            <!-- ... más meses ... -->
        </select>
    </div>
    <div class="col-md-2 d-flex align-items-end">
        <button type="submit" class="btn btn-success btn-lg w-100">
            <i class="fas fa-search me-1"></i>Generar
        </button>
    </div>
</form>
```

### Valores por Defecto

```csharp
// Ocupación: Último mes
fechaInicio ??= DateTime.Now.AddMonths(-1);
fechaFin ??= DateTime.Now;

// Ingresos: Mes actual
anio ??= DateTime.Now.Year;
mes ??= DateTime.Now.Month;

// Habitaciones Populares: Último año
fechaInicio ??= DateTime.Now.AddYears(-1);
fechaFin ??= DateTime.Now;
```

## ?? Estructura del Módulo

### Controlador

```
ReportesController.cs
??? Index() - Dashboard principal
??? Ocupacion(fechaInicio?, fechaFin?) - Reporte de ocupación
??? Ingresos(anio?, mes?) - Reporte de ingresos
??? HabitacionesPopulares(fechaInicio?, fechaFin?) - Top habitaciones
??? ObtenerDatosOcupacion(fechaInicio, fechaFin) - API AJAX
??? ObtenerDatosIngresos(anio, mes) - API AJAX
```

### Vistas

```
Views/Reportes/
??? Index.cshtml - Dashboard con cards de reportes
??? Ocupacion.cshtml - Reporte ocupación con 3 gráficos
??? Ingresos.cshtml - Reporte ingresos con 3 gráficos
??? HabitacionesPopulares.cshtml - Top 10 con 2 gráficos
```

## ?? Funcionalidades Destacadas

### 1. **Dashboard Interactivo**
- Cards con hover effects
- Iconos animados
- Gradientes modernos
- Enlaces directos a cada reporte

### 2. **Estadísticas en Tiempo Real**
- Consultas LINQ optimizadas
- Datos actualizados desde DB
- Sin caché (datos frescos)

### 3. **Gráficos Responsivos**
- Se adaptan al tamaño de pantalla
- Tooltips informativos
- Leyendas configurables
- Animaciones suaves

### 4. **Tablas Detalladas**
- Progress bars visuales
- Badges de colores
- Rankings con emojis (??????)
- Enlaces a detalles

### 5. **Exportación**
- Gráficos imprimibles
- Tablas formateadas
- CSS print-friendly

## ?? Diseño UI/UX

### Colores del Dashboard

```css
/* Ocupación */
.bg-gradient-info {
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

/* Ingresos */
.bg-gradient-success {
    background: linear-gradient(135deg, #11998e 0%, #38ef7d 100%);
}

/* Habitaciones Populares */
.bg-gradient-warning {
    background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
}
```

### Animaciones

```css
.hover-lift {
    transition: transform 0.3s ease, box-shadow 0.3s ease;
}

.hover-lift:hover {
    transform: translateY(-10px);
    box-shadow: 0 20px 40px rgba(0,0,0,0.2) !important;
}
```

### Progress Bars Dinámicos

```html
<div class="progress" style="height: 25px;">
    <div class="progress-bar @(porcentaje > 75 ? "bg-success" : porcentaje > 50 ? "bg-info" : "bg-warning")" 
  role="progressbar" 
      style="width: @porcentaje%" 
         aria-valuenow="@porcentaje">
 @porcentaje%
  </div>
</div>
```

## ?? Ejemplos de Datos Generados

### Reporte de Ocupación

```json
{
  "TotalHabitaciones": 50,
  "HabitacionesOcupadas": 38,
  "PorcentajeOcupacion": 76.00,
  "TotalReservas": 42,
  "OcupacionPorDia": {
    "2025-01-01": 35,
    "2025-01-02": 38,
    "2025-01-03": 40
  },
  "OcupacionPorTipo": [
    {
      "Tipo": "Suite",
      "Total": 10,
      "Ocupadas": 8
  },
    {
      "Tipo": "Doble",
      "Total": 25,
      "Ocupadas": 20
    }
  ]
}
```

### Reporte de Ingresos

```json
{
  "IngresosTotales": 156750.50,
  "TotalPagos": 42,
  "PromedioIngreso": 3732.15,
  "IngresosPorDia": [
    { "Dia": 1, "Total": 5200.00 },
    { "Dia": 2, "Total": 7150.00 }
  ],
  "IngresosPorMetodo": [
    { "Metodo": "Tarjeta de Crédito", "Total": 85000, "Cantidad": 25 },
    { "Metodo": "Efectivo", "Total": 45000, "Cantidad": 12 }
  ]
}
```

### Habitaciones Populares

```json
{
  "TotalReservas": 156,
  "HabitacionesPopulares": [
    {
   "IdHabitacion": 5,
      "TotalReservas": 28,
      "TotalNoches": 84,
      "IngresoTotal": 12600.00,
      "Habitacion": {
        "Numero": "101",
        "Tipo": "Suite",
     "Hotel": "Grand Hotel"
      }
    }
  ]
}
```

## ?? Validaciones y Manejo de Errores

### Controlador

```csharp
try
{
    // Lógica del reporte
    var datos = await GenerarReporte();
  ViewBag.Datos = datos;
    return View();
}
catch (Exception ex)
{
    TempData["Error"] = $"Error al generar reporte: {ex.Message}";
    return RedirectToAction(nameof(Index));
}
```

### Vistas

```html
@if (TempData["Error"] != null)
{
    <div class="alert alert-danger alert-dismissible fade show">
        <i class="fas fa-exclamation-circle me-2"></i>
        @TempData["Error"]
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    </div>
}
```

## ?? Responsive Design

- ? Cards apilables en móviles
- ? Gráficos adaptables
- ? Tablas con scroll horizontal
- ? Formularios responsive
- ? Touch-friendly

## ?? Optimizaciones Implementadas

### 1. **Consultas LINQ Eficientes**
- Uso de `Where` antes de `ToList()`
- `GroupBy` para agregaciones
- `Select` para proyecciones
- `Join` para relaciones

### 2. **Lazy Loading**
- Gráficos se renderizan después del DOM
- Datos serializados una vez
- Mínimas consultas a DB

### 3. **Caching (Opcional)**
```csharp
// Se puede agregar en futuras versiones
[ResponseCache(Duration = 300)] // 5 minutos
public async Task<IActionResult> Ocupacion(...)
```

## ?? Dependencias

### NuGet Packages

```xml
<PackageReference Include="Newtonsoft.Json" Version="13.0.4" />
```

### CDN (Chart.js)

```html
<script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.0/dist/chart.umd.min.js"></script>
```

## ?? Flujo de Uso

### 1. Generar Reporte de Ocupación

```
Usuario ? /Reportes/Index
  ?
Click en "Ver Reporte" de Ocupación
  ?
/Reportes/Ocupacion (con fechas por defecto)
  ?
Controlador consulta DB con LINQ
  ?
Calcula estadísticas y ocupación por día/tipo
  ?
Retorna ViewBag con datos
  ?
Vista serializa datos a JSON con Newtonsoft.Json
  ?
Chart.js renderiza gráficos
  ?
Usuario puede cambiar fechas y regenerar
```

### 2. Cambiar Rango de Fechas

```
Usuario en /Reportes/Ocupacion
  ?
Selecciona fecha inicio: 01/01/2025
  ?
Selecciona fecha fin: 31/01/2025
  ?
Click en "Generar"
  ?
GET request con parámetros fechaInicio y fechaFin
  ?
Controlador filtra reservas en ese rango
  ?
Recalcula estadísticas
  ?
Actualiza gráficos con nuevos datos
```

## ?? Métricas Calculadas

### Ocupación
- **Porcentaje de Ocupación**: `(Ocupadas * 100) / Total`
- **Tasa por Habitación**: `(Noches Ocupadas * 100) / Días en Rango`

### Ingresos
- **Ingresos Totales**: `Sum(Monto)`
- **Promedio por Pago**: `Total / Cantidad`
- **Crecimiento Mensual**: `((Mes Actual - Mes Anterior) / Mes Anterior) * 100`

### Habitaciones
- **Ingresos por Habitación**: `Precio * Noches`
- **Ranking**: `OrderByDescending(Reservas).Position`

## ?? Casos de Uso

### 1. Análisis de Temporada Alta
```
Gerente quiere ver ocupación de diciembre
? Reportes/Ocupacion
? Selecciona: 01/12/2024 - 31/12/2024
? Ve que Suite tiene 95% ocupación
? Decide aumentar precios para siguiente año
```

### 2. Evaluación Financiera
```
Contador necesita ingresos de enero
? Reportes/Ingresos
? Selecciona: Año 2025, Mes Enero
? Ve ingresos totales y por método
? Exporta datos para contabilidad
```

### 3. Estrategia de Marketing
```
Marketing quiere identificar habitaciones populares
? Reportes/HabitacionesPopulares
? Ve que Suite 101 es #1
? Decide hacer campaña enfocada en suites
? Analiza tendencia mensual de reservas
```

## ?? Mejoras Futuras

- [ ] Exportar a Excel/PDF
- [ ] Comparación año anterior
- [ ] Predicciones con IA
- [ ] Alertas automáticas
- [ ] Dashboard en tiempo real
- [ ] Gráficos interactivos (drill-down)
- [ ] Filtros avanzados
- [ ] Reportes personalizados
- [ ] Envío por email
- [ ] Historial de reportes

---

**Versión**: 1.0.0  
**Última actualización**: 2025  
**Autor**: Sistema HotelSuite  
**Tecnologías**: ASP.NET Core 9, EF Core 9, Chart.js 4.4, LINQ, Newtonsoft.Json 13, Bootstrap 5
