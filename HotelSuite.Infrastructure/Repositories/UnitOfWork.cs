using HotelSuite.Domain.Entities;
using HotelSuite.Domain.Interfaces;
using HotelSuite.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace HotelSuite.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly HotelDbContext _context;
    private IDbContextTransaction? _transaction;

    // Repositorios
    private IGenericRepository<Hotel>? _hoteles;
    private IGenericRepository<Habitacion>? _habitaciones;
    private IGenericRepository<Huesped>? _huespedes;
    private IGenericRepository<Reserva>? _reservas;
    private IGenericRepository<Pago>? _pagos;
  private IGenericRepository<Empleado>? _empleados;
    private IGenericRepository<Departamento>? _departamentos;
    private IGenericRepository<TareaDepartamento>? _tareasDepartamento;

    public UnitOfWork(HotelDbContext context)
    {
        _context = context;
    }

    public IGenericRepository<Hotel> Hoteles
    {
        get
        {
            return _hoteles ??= new GenericRepository<Hotel>(_context);
     }
    }

public IGenericRepository<Habitacion> Habitaciones
    {
        get
        {
    return _habitaciones ??= new GenericRepository<Habitacion>(_context);
        }
    }

    public IGenericRepository<Huesped> Huespedes
 {
        get
        {
    return _huespedes ??= new GenericRepository<Huesped>(_context);
        }
    }

    public IGenericRepository<Reserva> Reservas
    {
   get
        {
    return _reservas ??= new GenericRepository<Reserva>(_context);
        }
    }

    public IGenericRepository<Pago> Pagos
    {
        get
        {
   return _pagos ??= new GenericRepository<Pago>(_context);
    }
 }

    public IGenericRepository<Empleado> Empleados
    {
   get
        {
   return _empleados ??= new GenericRepository<Empleado>(_context);
        }
    }

    public IGenericRepository<Departamento> Departamentos
    {
        get
        {
       return _departamentos ??= new GenericRepository<Departamento>(_context);
        }
    }

    public IGenericRepository<TareaDepartamento> TareasDepartamento
    {
        get
        {
    return _tareasDepartamento ??= new GenericRepository<TareaDepartamento>(_context);
        }
    }

    public async Task<int> CommitAsync()
    {
      try
      {
     // Iniciar transacción si no existe
            _transaction ??= await _context.Database.BeginTransactionAsync();

   // Guardar cambios
 var result = await _context.SaveChangesAsync();

            // Confirmar transacción
     await _transaction.CommitAsync();

            return result;
        }
        catch
        {
        // Revertir transacción en caso de error
       if (_transaction != null)
        {
     await _transaction.RollbackAsync();
            }
       throw;
   }
        finally
 {
            // Liberar la transacción
 if (_transaction != null)
            {
      await _transaction.DisposeAsync();
    _transaction = null;
            }
        }
    }

    public void Dispose()
    {
 _transaction?.Dispose();
      _context.Dispose();
    }
}
