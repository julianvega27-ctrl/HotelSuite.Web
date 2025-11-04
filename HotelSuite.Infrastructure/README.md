# Patrón Repository y Unit of Work - HotelSuite

## Estructura de la Solución

### HotelSuite.Domain/Interfaces
- **IGenericRepository<T>**: Interfaz genérica para operaciones CRUD
- **IUnitOfWork**: Interfaz para manejo de transacciones y agrupación de repositorios

### HotelSuite.Infrastructure/Repositories
- **GenericRepository<T>**: Implementación del repositorio genérico usando EF Core
- **UnitOfWork**: Implementación del patrón Unit of Work con manejo de transacciones

## Características Implementadas

? **Async/Await**: Todos los métodos que acceden a la base de datos son asíncronos
? **AsNoTracking()**: Las consultas de lectura usan `AsNoTracking()` para mejor rendimiento
? **Manejo de Transacciones**: El método `CommitAsync()` maneja transacciones automáticamente
? **Lazy Loading de Repositorios**: Los repositorios se crean solo cuando se necesitan
? **Inyección de Dependencias**: Configuración centralizada en `DependencyInjection.cs`

## Uso

### 1. Configuración en Program.cs o Startup.cs

```csharp
using HotelSuite.Infrastructure;

// Agregar servicios de Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);
```

### 2. Configurar la cadena de conexión en appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=HotelSuiteDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### 3. Inyectar IUnitOfWork en tus servicios o páginas

```csharp
public class HabitacionService
{
    private readonly IUnitOfWork _unitOfWork;

    public HabitacionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Habitacion>> ObtenerTodasLasHabitacionesAsync()
    {
        return await _unitOfWork.Habitaciones.GetAllAsync();
    }

    public async Task<Habitacion?> ObtenerHabitacionPorIdAsync(int id)
    {
        return await _unitOfWork.Habitaciones.GetByIdAsync(id);
 }

    public async Task CrearHabitacionAsync(Habitacion habitacion)
    {
    await _unitOfWork.Habitaciones.AddAsync(habitacion);
        await _unitOfWork.CommitAsync();
    }

    public async Task ActualizarHabitacionAsync(Habitacion habitacion)
    {
        _unitOfWork.Habitaciones.Update(habitacion);
        await _unitOfWork.CommitAsync();
    }

    public async Task EliminarHabitacionAsync(int id)
    {
        var habitacion = await _unitOfWork.Habitaciones.GetByIdAsync(id);
        if (habitacion != null)
 {
       _unitOfWork.Habitaciones.Delete(habitacion);
     await _unitOfWork.CommitAsync();
        }
    }
}
```

### 4. Ejemplo de operación con múltiples entidades (Transacción)

```csharp
public async Task CrearReservaCompletaAsync(Reserva reserva, Pago pago)
{
    try
    {
        // Agregar reserva
     await _unitOfWork.Reservas.AddAsync(reserva);
      
        // Agregar pago
        await _unitOfWork.Pagos.AddAsync(pago);
    
        // Actualizar estado de habitación
        var habitacion = await _unitOfWork.Habitaciones.GetByIdAsync(reserva.IdHabitacion);
        if (habitacion != null)
        {
            habitacion.Estado = "Reservada";
            _unitOfWork.Habitaciones.Update(habitacion);
        }
        
        // Confirmar todas las operaciones en una transacción
      await _unitOfWork.CommitAsync();
    }
    catch (Exception ex)
    {
        // En caso de error, la transacción se revierte automáticamente
        throw new Exception("Error al crear la reserva", ex);
    }
}
```

### 5. Uso en Razor Pages

```csharp
public class IndexModel : PageModel
{
    private readonly IUnitOfWork _unitOfWork;

    public IndexModel(IUnitOfWork unitOfWork)
    {
     _unitOfWork = unitOfWork;
    }

    public IEnumerable<Hotel> Hoteles { get; set; } = new List<Hotel>();

    public async Task OnGetAsync()
    {
        Hoteles = await _unitOfWork.Hoteles.GetAllAsync();
    }
}
```

## Métodos Disponibles en IGenericRepository<T>

- `Task<IEnumerable<T>> GetAllAsync()`: Obtiene todos los registros
- `Task<T?> GetByIdAsync(int id)`: Obtiene un registro por ID
- `Task AddAsync(T entity)`: Agrega una nueva entidad
- `void Update(T entity)`: Marca una entidad para actualización
- `void Delete(T entity)`: Marca una entidad para eliminación
- `Task SaveChangesAsync()`: Guarda los cambios en la base de datos

## Repositorios Disponibles en IUnitOfWork

- `Hoteles`
- `Habitaciones`
- `Huespedes`
- `Reservas`
- `Pagos`
- `Empleados`
- `Departamentos`
- `TareasDepartamento`

## Ventajas del Patrón Implementado

1. **Separación de Responsabilidades**: La lógica de acceso a datos está separada de la lógica de negocio
2. **Transacciones Automáticas**: El UnitOfWork maneja las transacciones automáticamente
3. **Reutilización de Código**: El repositorio genérico evita código duplicado
4. **Testeable**: Fácil de crear mocks para pruebas unitarias
5. **Rendimiento**: Uso de `AsNoTracking()` para consultas de solo lectura
6. **Seguridad**: Manejo automático de rollback en caso de errores
