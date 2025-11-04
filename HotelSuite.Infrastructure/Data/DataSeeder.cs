using HotelSuite.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelSuite.Infrastructure.Data;

public class DataSeeder
{
    private readonly HotelDbContext _context;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(HotelDbContext context, ILogger<DataSeeder> logger)
    {
     _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
{
            _logger.LogInformation("Iniciando proceso de seeding de datos...");

 // Verificar si ya hay datos
            if (await _context.Hoteles.AnyAsync())
          {
 _logger.LogInformation("La base de datos ya contiene datos. Seeding omitido.");
 return;
      }

            await SeedDepartamentosAsync();
            await SeedHotelesAsync();
 await SeedHabitacionesAsync();
            await SeedHuespedesAsync();
    await SeedEmpleadosAsync();
            await SeedReservasAsync();
 await SeedPagosAsync();
 await SeedTareasDepartamentoAsync();

    _logger.LogInformation("Proceso de seeding completado exitosamente.");
        }
        catch (Exception ex)
        {
      _logger.LogError(ex, "Error durante el proceso de seeding de datos.");
            throw;
        }
    }

    private async Task SeedDepartamentosAsync()
    {
  _logger.LogInformation("Insertando departamentos...");

        var departamentos = new List<Departamento>
    {
            new Departamento
            {
 Nombre = "Recepción",
     Descripcion = "Departamento encargado de la atención al cliente, check-in y check-out"
            },
            new Departamento
      {
           Nombre = "Limpieza",
     Descripcion = "Departamento responsable del mantenimiento y limpieza de habitaciones"
            },
            new Departamento
  {
     Nombre = "Mantenimiento",
         Descripcion = "Departamento encargado del mantenimiento de instalaciones y equipos"
     },
            new Departamento
{
              Nombre = "Administración",
          Descripcion = "Departamento administrativo y de gestión del hotel"
  },
         new Departamento
     {
       Nombre = "Restaurante",
       Descripcion = "Departamento de servicios de alimentación y bebidas"
     }
        };

  await _context.Departamentos.AddRangeAsync(departamentos);
        await _context.SaveChangesAsync();

      _logger.LogInformation($"Se insertaron {departamentos.Count} departamentos.");
    }

    private async Task SeedHotelesAsync()
    {
        _logger.LogInformation("Insertando hoteles...");

        var hoteles = new List<Hotel>
     {
            new Hotel
   {
    Nombre = "Grand Hotel Plaza",
   Direccion = "Av. Principal 123, Centro Histórico, Ciudad de México",
        Telefono = "+52 55 1234 5678",
         Categoria = 5
            },
            new Hotel
   {
     Nombre = "Hotel Sunset Beach",
    Direccion = "Boulevard Costero 456, Zona Hotelera, Cancún",
       Telefono = "+52 998 765 4321",
    Categoria = 4
   },
       new Hotel
         {
     Nombre = "Business Center Hotel",
         Direccion = "Av. Corporativa 789, Distrito Financiero, Monterrey",
       Telefono = "+52 81 9876 5432",
       Categoria = 4
 }
   };

 await _context.Hoteles.AddRangeAsync(hoteles);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Se insertaron {hoteles.Count} hoteles.");
    }

    private async Task SeedHabitacionesAsync()
    {
        _logger.LogInformation("Insertando habitaciones...");

  var hoteles = await _context.Hoteles.ToListAsync();
      var habitaciones = new List<Habitacion>();

     // Grand Hotel Plaza - 30 habitaciones
      var grandHotel = hoteles.First(h => h.Nombre == "Grand Hotel Plaza");
      for (int i = 1; i <= 10; i++)
        {
   habitaciones.Add(new Habitacion
       {
    Numero = $"10{i:D2}",
           Tipo = "Individual",
   PrecioPorNoche = 1200m,
    Estado = "Disponible",
IdHotel = grandHotel.Id
 });
        }

        for (int i = 1; i <= 15; i++)
        {
     habitaciones.Add(new Habitacion
            {
      Numero = $"20{i:D2}",
       Tipo = "Doble",
     PrecioPorNoche = 1800m,
       Estado = "Disponible",
        IdHotel = grandHotel.Id
 });
        }

    for (int i = 1; i <= 5; i++)
        {
   habitaciones.Add(new Habitacion
      {
           Numero = $"30{i:D2}",
         Tipo = "Suite",
       PrecioPorNoche = 3500m,
Estado = "Disponible",
         IdHotel = grandHotel.Id
      });
     }

        // Hotel Sunset Beach - 40 habitaciones
  var sunsetBeach = hoteles.First(h => h.Nombre == "Hotel Sunset Beach");
  for (int i = 1; i <= 20; i++)
      {
   habitaciones.Add(new Habitacion
     {
   Numero = $"A{i:D2}",
          Tipo = "Doble",
       PrecioPorNoche = 2200m,
       Estado = "Disponible",
      IdHotel = sunsetBeach.Id
        });
        }

        for (int i = 1; i <= 15; i++)
        {
   habitaciones.Add(new Habitacion
  {
           Numero = $"B{i:D2}",
        Tipo = "Suite",
        PrecioPorNoche = 4200m,
    Estado = "Disponible",
       IdHotel = sunsetBeach.Id
   });
        }

for (int i = 1; i <= 5; i++)
        {
      habitaciones.Add(new Habitacion
       {
      Numero = $"P{i:D2}",
        Tipo = "Presidencial",
    PrecioPorNoche = 8500m,
        Estado = "Disponible",
       IdHotel = sunsetBeach.Id
      });
  }

    // Business Center Hotel - 25 habitaciones
       var businessCenter = hoteles.First(h => h.Nombre == "Business Center Hotel");
        for (int i = 1; i <= 15; i++)
        {
    habitaciones.Add(new Habitacion
      {
  Numero = $"E{i:D2}",
        Tipo = "Ejecutiva",
   PrecioPorNoche = 1600m,
          Estado = "Disponible",
   IdHotel = businessCenter.Id
     });
        }

        for (int i = 1; i <= 10; i++)
 {
        habitaciones.Add(new Habitacion
  {
       Numero = $"S{i:D2}",
        Tipo = "Suite Ejecutiva",
 PrecioPorNoche = 2800m,
      Estado = "Disponible",
            IdHotel = businessCenter.Id
     });
        }

        await _context.Habitaciones.AddRangeAsync(habitaciones);
   await _context.SaveChangesAsync();

      _logger.LogInformation($"Se insertaron {habitaciones.Count} habitaciones.");
    }

    private async Task SeedHuespedesAsync()
    {
    _logger.LogInformation("Insertando huéspedes...");

       var huespedes = new List<Huesped>
        {
   new Huesped
     {
        Nombres = "Carlos",
     Apellidos = "García Martínez",
        DocumentoIdentidad = "GAMC850615HDFRRL09",
     Email = "carlos.garcia@email.com",
     Telefono = "+52 55 1234 0001"
 },
        new Huesped
  {
       Nombres = "María",
    Apellidos = "López Hernández",
   DocumentoIdentidad = "LOHM900320MDFPRR04",
  Email = "maria.lopez@email.com",
     Telefono = "+52 55 1234 0002"
    },
    new Huesped
       {
        Nombres = "José Luis",
            Apellidos = "Rodríguez Pérez",
DocumentoIdentidad = "ROPJ880725HDFDRR02",
    Email = "jose.rodriguez@email.com",
    Telefono = "+52 998 7654 0001"
   },
  new Huesped
      {
       Nombres = "Ana",
Apellidos = "Martínez Sánchez",
          DocumentoIdentidad = "MASA921105MDFRNR08",
        Email = "ana.martinez@email.com",
     Telefono = "+52 998 7654 0002"
   },
       new Huesped
       {
 Nombres = "Roberto",
        Apellidos = "Fernández Gómez",
   DocumentoIdentidad = "FEGR870430HDFRMB06",
      Email = "roberto.fernandez@email.com",
       Telefono = "+52 81 9876 0001"
  },
     new Huesped
     {
 Nombres = "Laura",
       Apellidos = "González Díaz",
     DocumentoIdentidad = "GODL950815MDFNZR03",
      Email = "laura.gonzalez@email.com",
       Telefono = "+52 81 9876 0002"
   },
      new Huesped
         {
  Nombres = "Pedro",
    Apellidos = "Sánchez Torres",
         DocumentoIdentidad = "SATP891210HDFSRR07",
  Email = "pedro.sanchez@email.com",
Telefono = "+52 55 1234 0003"
      },
    new Huesped
       {
      Nombres = "Isabel",
Apellidos = "Ramírez Castro",
   DocumentoIdentidad = "RACI930225MDFSRB09",
Email = "isabel.ramirez@email.com",
     Telefono = "+52 55 1234 0004"
   },
   new Huesped
    {
      Nombres = "Miguel",
  Apellidos = "Torres Jiménez",
        DocumentoIdentidad = "TOJM860918HDFRGL05",
      Email = "miguel.torres@email.com",
   Telefono = "+52 998 7654 0003"
   },
      new Huesped
    {
          Nombres = "Patricia",
    Apellidos = "Vargas Ruiz",
    DocumentoIdentidad = "VARP940505MDFRGT08",
     Email = "patricia.vargas@email.com",
          Telefono = "+52 81 9876 0003"
  }
  };

        await _context.Huespedes.AddRangeAsync(huespedes);
   await _context.SaveChangesAsync();

        _logger.LogInformation($"Se insertaron {huespedes.Count} huéspedes.");
 }

    private async Task SeedEmpleadosAsync()
    {
        _logger.LogInformation("Insertando empleados...");

   var departamentos = await _context.Departamentos.ToListAsync();
var recepcion = departamentos.First(d => d.Nombre == "Recepción");
        var limpieza = departamentos.First(d => d.Nombre == "Limpieza");
     var mantenimiento = departamentos.First(d => d.Nombre == "Mantenimiento");
        var administracion = departamentos.First(d => d.Nombre == "Administración");
        var restaurante = departamentos.First(d => d.Nombre == "Restaurante");

     var empleados = new List<Empleado>
        {
            // Recepción
  new Empleado
      {
      Nombres = "Andrea",
              Apellidos = "Morales Díaz",
        Cargo = "Recepcionista",
   Email = "andrea.morales@hotelsuite.com",
            Telefono = "+52 55 5000 0001",
                IdDepartamento = recepcion.Id
       },
new Empleado
          {
        Nombres = "Fernando",
 Apellidos = "Castro López",
     Cargo = "Jefe de Recepción",
          Email = "fernando.castro@hotelsuite.com",
         Telefono = "+52 55 5000 0002",
     IdDepartamento = recepcion.Id
      },
            new Empleado
       {
                Nombres = "Claudia",
  Apellidos = "Mendoza Silva",
                Cargo = "Recepcionista Nocturna",
              Email = "claudia.mendoza@hotelsuite.com",
 Telefono = "+52 55 5000 0003",
     IdDepartamento = recepcion.Id
        },
 // Limpieza
          new Empleado
      {
                Nombres = "Rosa",
      Apellidos = "Jiménez Pérez",
          Cargo = "Camarista",
      Email = "rosa.jimenez@hotelsuite.com",
      Telefono = "+52 55 5000 0004",
        IdDepartamento = limpieza.Id
            },
 new Empleado
  {
              Nombres = "Marta",
    Apellidos = "Ruiz Gómez",
         Cargo = "Supervisora de Limpieza",
     Email = "marta.ruiz@hotelsuite.com",
     Telefono = "+52 55 5000 0005",
         IdDepartamento = limpieza.Id
 },
     new Empleado
            {
   Nombres = "Elena",
 Apellidos = "Hernández Cruz",
              Cargo = "Camarista",
      Email = "elena.hernandez@hotelsuite.com",
      Telefono = "+52 55 5000 0006",
            IdDepartamento = limpieza.Id
         },
            // Mantenimiento
            new Empleado
    {
    Nombres = "Alberto",
    Apellidos = "Vega Soto",
   Cargo = "Técnico de Mantenimiento",
     Email = "alberto.vega@hotelsuite.com",
        Telefono = "+52 55 5000 0007",
          IdDepartamento = mantenimiento.Id
       },
       new Empleado
     {
                Nombres = "Ricardo",
    Apellidos = "Ortiz Ramírez",
     Cargo = "Jefe de Mantenimiento",
         Email = "ricardo.ortiz@hotelsuite.com",
    Telefono = "+52 55 5000 0008",
                IdDepartamento = mantenimiento.Id
     },
        // Administración
    new Empleado
       {
             Nombres = "Gabriela",
     Apellidos = "Campos Flores",
           Cargo = "Gerente General",
      Email = "gabriela.campos@hotelsuite.com",
    Telefono = "+52 55 5000 0009",
    IdDepartamento = administracion.Id
          },
     new Empleado
    {
 Nombres = "Daniel",
   Apellidos = "Reyes Márquez",
       Cargo = "Contador",
          Email = "daniel.reyes@hotelsuite.com",
 Telefono = "+52 55 5000 0010",
  IdDepartamento = administracion.Id
          },
      // Restaurante
       new Empleado
            {
      Nombres = "Luis",
    Apellidos = "Paredes Núñez",
       Cargo = "Chef Ejecutivo",
       Email = "luis.paredes@hotelsuite.com",
       Telefono = "+52 55 5000 0011",
        IdDepartamento = restaurante.Id
   },
  new Empleado
          {
   Nombres = "Sofía",
 Apellidos = "Guerrero Medina",
       Cargo = "Mesera",
    Email = "sofia.guerrero@hotelsuite.com",
            Telefono = "+52 55 5000 0012",
 IdDepartamento = restaurante.Id
            }
      };

        await _context.Empleados.AddRangeAsync(empleados);
        await _context.SaveChangesAsync();

   _logger.LogInformation($"Se insertaron {empleados.Count} empleados.");
    }

    private async Task SeedReservasAsync()
    {
     _logger.LogInformation("Insertando reservas...");

    var huespedes = await _context.Huespedes.ToListAsync();
        var habitaciones = await _context.Habitaciones.ToListAsync();

        var reservas = new List<Reserva>();
        var random = new Random(42); // Seed fijo para datos reproducibles

   // Crear reservas pasadas, actuales y futuras
    var fechaBase = DateTime.Now.AddMonths(-2);

        for (int i = 0; i < 15; i++)
        {
    var huesped = huespedes[random.Next(huespedes.Count)];
            var habitacion = habitaciones[random.Next(habitaciones.Count)];
            var diasAnticipacion = random.Next(1, 30);
       var diasEstancia = random.Next(1, 7);

            var fechaReserva = fechaBase.AddDays(i * 5);
       var fechaEntrada = fechaReserva.AddDays(diasAnticipacion);
       var fechaSalida = fechaEntrada.AddDays(diasEstancia);

            var estado = fechaSalida < DateTime.Now ? "Finalizada" :
   fechaEntrada <= DateTime.Now && fechaSalida >= DateTime.Now ? "En curso" :
    "Confirmada";

   reservas.Add(new Reserva
    {
     FechaReserva = fechaReserva,
            FechaEntrada = fechaEntrada,
        FechaSalida = fechaSalida,
 Estado = estado,
       IdHuesped = huesped.Id,
       IdHabitacion = habitacion.Id
        });
   }

   await _context.Reservas.AddRangeAsync(reservas);
    await _context.SaveChangesAsync();

     _logger.LogInformation($"Se insertaron {reservas.Count} reservas.");
    }

    private async Task SeedPagosAsync()
    {
  _logger.LogInformation("Insertando pagos...");

        var reservas = await _context.Reservas
            .Include(r => r.Habitacion)
  .ToListAsync();

        var pagos = new List<Pago>();
        var random = new Random(42);
      var metodosPago = new[] { "Efectivo", "Tarjeta de Crédito", "Tarjeta de Débito", "Transferencia", "Pendiente" };

        foreach (var reserva in reservas)
        {
     var diasEstancia = (reserva.FechaSalida - reserva.FechaEntrada).Days;
     var montoTotal = reserva.Habitacion.PrecioPorNoche * diasEstancia;

          // Determinar método de pago
    string metodoPago;
   DateTime fechaPago;

            if (reserva.Estado == "Finalizada")
            {
          // Reservas finalizadas: pagos completos
                metodoPago = metodosPago[random.Next(metodosPago.Length - 1)]; // Excluir "Pendiente"
         fechaPago = reserva.FechaSalida.AddHours(-2);
      }
  else if (reserva.Estado == "En curso")
     {
    // Reservas en curso: mezcla de pagados y pendientes
    metodoPago = random.Next(2) == 0 ? "Pendiente" : metodosPago[random.Next(metodosPago.Length - 1)];
        fechaPago = reserva.FechaEntrada.AddHours(1);
            }
        else
          {
            // Reservas futuras: pendientes
     metodoPago = "Pendiente";
 fechaPago = reserva.FechaReserva.AddHours(1);
    }

            pagos.Add(new Pago
       {
    Monto = montoTotal,
     FechaPago = fechaPago,
      Metodo = metodoPago,
          IdReserva = reserva.Id
            });
        }

        await _context.Pagos.AddRangeAsync(pagos);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Se insertaron {pagos.Count} pagos.");
    }

    private async Task SeedTareasDepartamentoAsync()
    {
        _logger.LogInformation("Insertando tareas de departamento...");

   var departamentos = await _context.Departamentos.ToListAsync();
  var empleados = await _context.Empleados.ToListAsync();

        var tareas = new List<TareaDepartamento>();
        var random = new Random(42);
        var prioridades = new[] { "Baja", "Media", "Alta" };
  var estados = new[] { "Pendiente", "En proceso", "Completada" };

   // Tareas de Limpieza
        var limpieza = departamentos.First(d => d.Nombre == "Limpieza");
        var empleadosLimpieza = empleados.Where(e => e.IdDepartamento == limpieza.Id).ToList();

        for (int i = 1; i <= 10; i++)
   {
            var empleado = empleadosLimpieza[random.Next(empleadosLimpieza.Count)];
            var estado = estados[random.Next(estados.Length)];
   var fechaAsignacion = DateTime.Now.AddDays(-random.Next(1, 15));

            tareas.Add(new TareaDepartamento
            {
     Titulo = $"Limpieza habitación {100 + i}",
    Descripcion = $"Realizar limpieza completa de la habitación {100 + i}, cambiar sábanas y reponer amenidades",
 Estado = estado,
    Prioridad = prioridades[random.Next(prioridades.Length)],
           FechaAsignacion = fechaAsignacion,
  FechaFinalizacion = estado == "Completada" ? fechaAsignacion.AddHours(random.Next(2, 6)) : null,
                IdDepartamento = limpieza.Id,
         IdEmpleadoAsignado = empleado.Id
            });
        }

        // Tareas de Mantenimiento
    var mantenimiento = departamentos.First(d => d.Nombre == "Mantenimiento");
        var empleadosMantenimiento = empleados.Where(e => e.IdDepartamento == mantenimiento.Id).ToList();

    var tareasMantenimiento = new[]
        {
            "Revisar sistema de aire acondicionado",
            "Reparar fuga en baño",
         "Cambiar bombillas en pasillo",
            "Mantenimiento preventivo de elevadores",
        "Revisar sistema eléctrico",
            "Reparar puerta de habitación",
  "Mantenimiento de piscina"
        };

        foreach (var tarea in tareasMantenimiento)
        {
    var empleado = empleadosMantenimiento[random.Next(empleadosMantenimiento.Count)];
 var estado = estados[random.Next(estados.Length)];
            var fechaAsignacion = DateTime.Now.AddDays(-random.Next(1, 10));

            tareas.Add(new TareaDepartamento
    {
   Titulo = tarea,
    Descripcion = $"Tarea de mantenimiento: {tarea}",
Estado = estado,
        Prioridad = prioridades[random.Next(prioridades.Length)],
 FechaAsignacion = fechaAsignacion,
        FechaFinalizacion = estado == "Completada" ? fechaAsignacion.AddHours(random.Next(3, 8)) : null,
  IdDepartamento = mantenimiento.Id,
      IdEmpleadoAsignado = empleado.Id
         });
  }

        // Tareas de Recepción
    var recepcion = departamentos.First(d => d.Nombre == "Recepción");
      var empleadosRecepcion = empleados.Where(e => e.IdDepartamento == recepcion.Id).ToList();

        var tareasRecepcion = new[]
        {
      "Preparar bienvenida para grupo VIP",
 "Actualizar sistema de reservas",
      "Revisar inventario de llaves",
            "Coordinar check-in masivo",
 "Atender solicitudes especiales"
        };

 foreach (var tarea in tareasRecepcion)
        {
      var empleado = empleadosRecepcion[random.Next(empleadosRecepcion.Count)];
    var estado = estados[random.Next(estados.Length)];
            var fechaAsignacion = DateTime.Now.AddDays(-random.Next(1, 7));

        tareas.Add(new TareaDepartamento
 {
       Titulo = tarea,
  Descripcion = $"Tarea de recepción: {tarea}",
     Estado = estado,
     Prioridad = prioridades[random.Next(prioridades.Length)],
         FechaAsignacion = fechaAsignacion,
     FechaFinalizacion = estado == "Completada" ? fechaAsignacion.AddHours(random.Next(1, 4)) : null,
   IdDepartamento = recepcion.Id,
       IdEmpleadoAsignado = empleado.Id
            });
        }

        await _context.TareasDepartamento.AddRangeAsync(tareas);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Se insertaron {tareas.Count} tareas de departamento.");
    }
}
