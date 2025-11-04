using HotelSuite.Domain.Entities;

namespace HotelSuite.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Hotel> Hoteles { get; }
    IGenericRepository<Habitacion> Habitaciones { get; }
    IGenericRepository<Huesped> Huespedes { get; }
    IGenericRepository<Reserva> Reservas { get; }
    IGenericRepository<Pago> Pagos { get; }
    IGenericRepository<Empleado> Empleados { get; }
    IGenericRepository<Departamento> Departamentos { get; }
    IGenericRepository<TareaDepartamento> TareasDepartamento { get; }
    
    Task<int> CommitAsync();
}
