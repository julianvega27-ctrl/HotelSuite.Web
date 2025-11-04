# ??? HotelSuite - DataSeeder y Migraciones

## ?? Descripción

Sistema completo de seeding de datos iniciales y configuración automática de migraciones para la base de datos del sistema HotelSuite.

## ? Características Implementadas

### ?? DataSeeder

**Ubicación:** `HotelSuite.Infrastructure/Data/DataSeeder.cs`

**Clase principal:**
```csharp
public class DataSeeder
{
    private readonly HotelDbContext _context;
    private readonly ILogger<DataSeeder> _logger;

    public async Task SeedAsync()
{
        // Verifica si ya hay datos
   if (await _context.Hoteles.AnyAsync())
 {
    _logger.LogInformation("La base de datos ya contiene datos. Seeding omitido.");
        return;
        }

        // Inserta datos en orden correcto (respetando relaciones)
        await SeedDepartamentosAsync();
        await SeedHotelesAsync();
await SeedHabitacionesAsync();
        await SeedHuespedesAsync();
     await SeedEmpleadosAsync();
        await SeedReservasAsync();
        await SeedPagosAsync();
      await SeedTareasDepartamentoAsync();
    }
}
```

### ?? Datos Insertados

#### 1. **Departamentos** (5 registros)
```csharp
? Recepción - Atención al cliente, check-in y check-out
? Limpieza - Mantenimiento y limpieza de habitaciones
? Mantenimiento - Mantenimiento de instalaciones y equipos
? Administración - Gestión administrativa del hotel
? Restaurante - Servicios de alimentación y bebidas
```

#### 2. **Hoteles** (3 registros)
```csharp
? Grand Hotel Plaza
   - Dirección: Av. Principal 123, Centro Histórico, CDMX
   - Teléfono: +52 55 1234 5678
   - Categoría: 5 estrellas

? Hotel Sunset Beach
   - Dirección: Boulevard Costero 456, Zona Hotelera, Cancún
   - Teléfono: +52 998 765 4321
   - Categoría: 4 estrellas

? Business Center Hotel
   - Dirección: Av. Corporativa 789, Distrito Financiero, Monterrey
   - Teléfono: +52 81 9876 5432
   - Categoría: 4 estrellas
```

#### 3. **Habitaciones** (95 registros)

**Grand Hotel Plaza (30 habitaciones):**
- 10 Individual (101-110) - $1,200/noche
- 15 Doble (201-215) - $1,800/noche
- 5 Suite (301-305) - $3,500/noche

**Hotel Sunset Beach (40 habitaciones):**
- 20 Doble (A01-A20) - $2,200/noche
- 15 Suite (B01-B15) - $4,200/noche
- 5 Presidencial (P01-P05) - $8,500/noche

**Business Center Hotel (25 habitaciones):**
- 15 Ejecutiva (E01-E15) - $1,600/noche
- 10 Suite Ejecutiva (S01-S10) - $2,800/noche

#### 4. **Huéspedes** (10 registros)
```csharp
? Carlos García Martínez - carlos.garcia@email.com
? María López Hernández - maria.lopez@email.com
? José Luis Rodríguez Pérez - jose.rodriguez@email.com
? Ana Martínez Sánchez - ana.martinez@email.com
? Roberto Fernández Gómez - roberto.fernandez@email.com
? Laura González Díaz - laura.gonzalez@email.com
? Pedro Sánchez Torres - pedro.sanchez@email.com
? Isabel Ramírez Castro - isabel.ramirez@email.com
? Miguel Torres Jiménez - miguel.torres@email.com
? Patricia Vargas Ruiz - patricia.vargas@email.com
```

#### 5. **Empleados** (12 registros)

**Recepción (3):**
- Andrea Morales Díaz - Recepcionista
- Fernando Castro López - Jefe de Recepción
- Claudia Mendoza Silva - Recepcionista Nocturna

**Limpieza (3):**
- Rosa Jiménez Pérez - Camarista
- Marta Ruiz Gómez - Supervisora de Limpieza
- Elena Hernández Cruz - Camarista

**Mantenimiento (2):**
- Alberto Vega Soto - Técnico de Mantenimiento
- Ricardo Ortiz Ramírez - Jefe de Mantenimiento

**Administración (2):**
- Gabriela Campos Flores - Gerente General
- Daniel Reyes Márquez - Contador

**Restaurante (2):**
- Luis Paredes Núñez - Chef Ejecutivo
- Sofía Guerrero Medina - Mesera

#### 6. **Reservas** (15 registros)
```csharp
Estados:
- ? Finalizadas (pasadas)
- ?? En curso (actuales)
- ?? Confirmadas (futuras)

Distribución:
- Fechas: Últimos 2 meses distribuidas
- Duración: 1-7 días
- Asignación: Aleatoria con seed fijo (42)
```

#### 7. **Pagos** (15 registros - uno por reserva)
```csharp
Métodos de pago:
- ?? Tarjeta de Crédito
- ?? Tarjeta de Débito
- ?? Efectivo
- ?? Transferencia
- ? Pendiente

Lógica de asignación:
- Finalizadas ? Pagadas (métodos variados)
- En curso ? 50% Pagadas / 50% Pendientes
- Futuras ? Pendientes

Montos calculados:
Monto = PrecioPorNoche × Días de estancia
```

#### 8. **Tareas de Departamento** (~22 registros)

**Limpieza (10 tareas):**
- Limpieza habitación 101-110
- Estados: Pendiente, En proceso, Completada
- Prioridades: Alta, Media, Baja

**Mantenimiento (7 tareas):**
- Revisar sistema de aire acondicionado
- Reparar fuga en baño
- Cambiar bombillas en pasillo
- Mantenimiento preventivo de elevadores
- Revisar sistema eléctrico
- Reparar puerta de habitación
- Mantenimiento de piscina

**Recepción (5 tareas):**
- Preparar bienvenida para grupo VIP
- Actualizar sistema de reservas
- Revisar inventario de llaves
- Coordinar check-in masivo
- Atender solicitudes especiales

## ?? Configuración Automática en Program.cs

**Ubicación:** `HotelSuite/Program.cs`

```csharp
// Registrar DataSeeder como servicio
builder.Services.AddScoped<DataSeeder>();

var app = builder.Build();

// Ejecutar migraciones y seeding automáticamente al iniciar
await using (var scope = app.Services.CreateAsyncScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    try
    {
  logger.LogInformation("Iniciando migración de base de datos...");
        
        // Obtener contexto
   var context = services.GetRequiredService<HotelDbContext>();
  
     // Ejecutar migraciones pendientes
        await context.Database.MigrateAsync();
    logger.LogInformation("Migraciones aplicadas exitosamente.");
        
        // Ejecutar seeding
logger.LogInformation("Iniciando seeding de datos iniciales...");
        var seeder = services.GetRequiredService<DataSeeder>();
        await seeder.SeedAsync();
 logger.LogInformation("Seeding completado exitosamente.");
    }
    catch (Exception ex)
    {
  logger.LogError(ex, "Ocurrió un error durante la migración o el seeding.");
        throw;
    }
}
```

## ?? Proceso de Ejecución

### Flujo Automático

```
1. Aplicación inicia
   ?
2. CreateAsyncScope()
   ?
3. Obtener HotelDbContext
   ?
4. context.Database.MigrateAsync()
   - Crea base de datos si no existe
   - Aplica migraciones pendientes
   ?
5. DataSeeder.SeedAsync()
   - Verifica si ya hay datos
   - Inserta datos en orden correcto
   ?
6. Aplicación continúa normalmente
```

### Orden de Inserción (Respeta Relaciones)

```
1. Departamentos (independiente)
   ?
2. Hoteles (independiente)
   ?
3. Habitaciones (FK: IdHotel)
   ?
4. Huéspedes (independiente)
   ?
5. Empleados (FK: IdDepartamento)
   ?
6. Reservas (FK: IdHuesped, IdHabitacion)
   ?
7. Pagos (FK: IdReserva)
   ?
8. TareasDepartamento (FK: IdDepartamento, IdEmpleadoAsignado)
```

## ?? Migración InitialCreate

### Generación

```bash
dotnet ef migrations add InitialCreate \
  --project HotelSuite.Infrastructure/HotelSuite.Infrastructure.csproj \
  --startup-project HotelSuite/HotelSuite.csproj \
  --context HotelDbContext
```

### Ubicación
```
HotelSuite.Infrastructure/Migrations/
??? {timestamp}_InitialCreate.cs
??? HotelDbContextModelSnapshot.cs
```

### Contenido de la Migración

**Up() - Crear tablas:**
```csharp
- Departamentos
- Hoteles
- Habitaciones (FK: IdHotel)
- Huespedes
- Empleados (FK: IdDepartamento)
- Reservas (FK: IdHuesped, IdHabitacion)
- Pagos (FK: IdReserva)
- TareasDepartamento (FK: IdDepartamento, IdEmpleadoAsignado)

+ Índices únicos (Email, DocumentoIdentidad)
+ Claves foráneas con restricciones
+ Configuración de tipos y longitudes
```

**Down() - Eliminar tablas:**
```csharp
- Drop TareasDepartamento
- Drop Pagos
- Drop Reservas
- Drop Empleados
- Drop Huespedes
- Drop Habitaciones
- Drop Hoteles
- Drop Departamentos
```

## ?? Validaciones y Seguridad

### 1. **Prevención de Duplicados**
```csharp
// El seeder verifica si ya hay datos
if (await _context.Hoteles.AnyAsync())
{
    _logger.LogInformation("La base de datos ya contiene datos. Seeding omitido.");
    return;
}
```

### 2. **Transacciones Implícitas**
```csharp
// Cada SaveChangesAsync() es una transacción
await _context.Departamentos.AddRangeAsync(departamentos);
await _context.SaveChangesAsync(); // Commit o Rollback automático
```

### 3. **Logging Detallado**
```csharp
_logger.LogInformation("Insertando departamentos...");
_logger.LogInformation($"Se insertaron {departamentos.Count} departamentos.");
```

### 4. **Manejo de Errores**
```csharp
try
{
    await SeedAsync();
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error durante el proceso de seeding de datos.");
    throw; // Re-lanza para detener la aplicación
}
```

## ?? Características Destacadas

### 1. **Datos Reproducibles**
```csharp
var random = new Random(42); // Seed fijo
// Siempre genera los mismos datos aleatorios
```

### 2. **Relaciones Correctas**
```csharp
// Obtener entidades relacionadas
var departamentos = await _context.Departamentos.ToListAsync();
var recepcion = departamentos.First(d => d.Nombre == "Recepción");

// Asignar FK correctamente
new Empleado
{
    Nombres = "Andrea",
    IdDepartamento = recepcion.Id // FK válida
}
```

### 3. **Datos Realistas**
- Nombres mexicanos
- Teléfonos con formato +52
- CURP válidos
- Emails profesionales
- Fechas coherentes

### 4. **Estados Lógicos**
```csharp
var estado = fechaSalida < DateTime.Now ? "Finalizada" :
             fechaEntrada <= DateTime.Now && fechaSalida >= DateTime.Now ? "En curso" :
    "Confirmada";
```

## ??? Comandos Útiles

### Ver Migraciones
```bash
dotnet ef migrations list \
  --project HotelSuite.Infrastructure/HotelSuite.Infrastructure.csproj \
  --startup-project HotelSuite/HotelSuite.csproj
```

### Aplicar Migración Manualmente
```bash
dotnet ef database update \
  --project HotelSuite.Infrastructure/HotelSuite.Infrastructure.csproj \
  --startup-project HotelSuite/HotelSuite.csproj
```

### Eliminar Última Migración
```bash
dotnet ef migrations remove \
  --project HotelSuite.Infrastructure/HotelSuite.Infrastructure.csproj \
  --startup-project HotelSuite/HotelSuite.csproj
```

### Generar Script SQL
```bash
dotnet ef migrations script \
  --project HotelSuite.Infrastructure/HotelSuite.Infrastructure.csproj \
  --startup-project HotelSuite/HotelSuite.csproj \
  --output migration.sql
```

### Ver Estado de Base de Datos
```bash
dotnet ef dbcontext info \
  --project HotelSuite.Infrastructure/HotelSuite.Infrastructure.csproj \
  --startup-project HotelSuite/HotelSuite.csproj
```

## ?? Resumen de Datos

| Entidad | Cantidad | Nota |
|---------|----------|------|
| Departamentos | 5 | Todos los departamentos principales |
| Hoteles | 3 | 5?, 4?, 4? |
| Habitaciones | 95 | Distribuidas en 3 hoteles |
| Huéspedes | 10 | Con datos completos |
| Empleados | 12 | Distribuidos por departamento |
| Reservas | 15 | Pasadas, actuales y futuras |
| Pagos | 15 | Uno por reserva |
| Tareas | ~22 | Distribuidas por departamento |

**Total de registros:** ~177 registros

## ?? Actualización de Datos

Si necesitas actualizar los datos iniciales:

1. **Eliminar base de datos:**
```bash
dotnet ef database drop --force
```

2. **Modificar DataSeeder.cs:**
```csharp
// Cambiar datos según necesites
```

3. **Ejecutar aplicación:**
```bash
dotnet run --project HotelSuite/HotelSuite.csproj
```

4. **Resultado:**
- Crea nueva BD
- Aplica migraciones
- Inserta nuevos datos

## ?? Casos de Uso

### 1. Primera Ejecución
```
Usuario ejecuta: dotnet run
?
No hay base de datos
?
context.Database.MigrateAsync() crea BD
?
seeder.SeedAsync() inserta datos
?
Aplicación lista con datos de prueba
```

### 2. Ejecución Posterior
```
Usuario ejecuta: dotnet run
?
Base de datos existe
?
context.Database.MigrateAsync() no hace nada
?
seeder.SeedAsync() detecta datos existentes
?
Logger: "La base de datos ya contiene datos. Seeding omitido."
?
Aplicación inicia normalmente
```

### 3. Nueva Migración
```
Desarrollador: dotnet ef migrations add NuevaFeature
?
Usuario ejecuta: dotnet run
?
context.Database.MigrateAsync() aplica nueva migración
?
seeder.SeedAsync() detecta datos existentes, no inserta
?
Aplicación con nueva estructura y datos preservados
```

## ?? Notas Importantes

### ? Ventajas
- ? **Automatizado**: No requiere scripts SQL manuales
- ? **Reproducible**: Mismos datos en todos los ambientes
- ? **Type-safe**: Usa entidades C# con validaciones
- ? **Logging**: Trazabilidad completa del proceso
- ? **Idempotente**: Puede ejecutarse múltiples veces sin duplicar
- ? **Orden correcto**: Respeta relaciones FK

### ?? Consideraciones
- ?? Solo para **desarrollo** y **pruebas**
- ?? En **producción**, usar scripts SQL controlados
- ?? Los datos son **públicos** (no contienen info sensible)
- ?? El seeder se ejecuta en **cada inicio** de la app
- ?? Si ya hay datos, el seeding se **omite**

### ?? Personalización
Para usar en producción:
```csharp
// En Program.cs
if (app.Environment.IsDevelopment())
{
    // Ejecutar seeding solo en desarrollo
  await seeder.SeedAsync();
}
```

## ?? Próximos Pasos

Después de ejecutar el seeding:

1. ? **Explorar datos** en las vistas del sistema
2. ? **Crear reservas** adicionales
3. ? **Registrar pagos** pendientes
4. ? **Asignar tareas** a empleados
5. ? **Generar reportes** con datos reales

---

**Versión**: 1.0.0  
**Última actualización**: 2025  
**Autor**: Sistema HotelSuite  
**Tecnologías**: EF Core 9, .NET 9, SQL Server, Async/Await
