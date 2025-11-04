using AutoMapper;
using HotelSuite.Application.DTOs;
using HotelSuite.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelSuite.Controllers;

[Authorize(Roles = "Administrador,Gerente")] // Solo admin y gerente pueden ver reportes
public class ReportesController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ReportesController(IUnitOfWork unitOfWork, IMapper mapper)
  {
      _unitOfWork = unitOfWork;
     _mapper = mapper;
    }

    // GET: Reportes
    public IActionResult Index()
  {
        return View();
    }

    // GET: Reportes/Ocupacion
    public async Task<IActionResult> Ocupacion(DateTime? fechaInicio, DateTime? fechaFin)
    {
        // Establecer fechas por defecto (último mes)
        fechaInicio ??= DateTime.Now.AddMonths(-1);
        fechaFin ??= DateTime.Now;

        ViewBag.FechaInicio = fechaInicio.Value.ToString("yyyy-MM-dd");
        ViewBag.FechaFin = fechaFin.Value.ToString("yyyy-MM-dd");

        try
        {
    var reservas = await _unitOfWork.Reservas.GetAllAsync();
  var habitaciones = await _unitOfWork.Habitaciones.GetAllAsync();

       // Filtrar reservas en el rango de fechas
            var reservasEnRango = reservas.Where(r =>
       r.FechaEntrada <= fechaFin.Value &&
        r.FechaSalida >= fechaInicio.Value
          ).ToList();

// Calcular estadísticas
     var totalHabitaciones = habitaciones.Count();
       var habitacionesOcupadas = reservasEnRango
         .Where(r => r.Estado == "Confirmada" || r.Estado == "En curso")
           .Select(r => r.IdHabitacion)
        .Distinct()
         .Count();

            var porcentajeOcupacion = totalHabitaciones > 0 
     ? (habitacionesOcupadas * 100.0 / totalHabitaciones) 
          : 0;

            // Ocupación por día
 var ocupacionPorDia = new Dictionary<DateTime, int>();
   var fechaActual = fechaInicio.Value.Date;
       
         while (fechaActual <= fechaFin.Value.Date)
         {
             var ocupadasEnDia = reservasEnRango.Count(r =>
             r.FechaEntrada.Date <= fechaActual &&
           r.FechaSalida.Date >= fechaActual &&
          (r.Estado == "Confirmada" || r.Estado == "En curso")
          );
       
            ocupacionPorDia[fechaActual] = ocupadasEnDia;
  fechaActual = fechaActual.AddDays(1);
            }

 // Ocupación por tipo de habitación
        var ocupacionPorTipo = habitaciones
                .GroupBy(h => h.Tipo)
     .Select(g => new
 {
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

  ViewBag.TotalHabitaciones = totalHabitaciones;
  ViewBag.HabitacionesOcupadas = habitacionesOcupadas;
            ViewBag.PorcentajeOcupacion = Math.Round(porcentajeOcupacion, 2);
            ViewBag.OcupacionPorDia = ocupacionPorDia;
      ViewBag.OcupacionPorTipo = ocupacionPorTipo;
         ViewBag.TotalReservas = reservasEnRango.Count;

     return View();
        }
        catch (Exception ex)
        {
    TempData["Error"] = $"Error al generar reporte de ocupación: {ex.Message}";
        return RedirectToAction(nameof(Index));
        }
    }

    // GET: Reportes/Ingresos
    public async Task<IActionResult> Ingresos(int? anio, int? mes)
    {
        // Establecer fecha por defecto (mes actual)
     anio ??= DateTime.Now.Year;
        mes ??= DateTime.Now.Month;

        ViewBag.Anio = anio.Value;
      ViewBag.Mes = mes.Value;

        try
        {
     var pagos = await _unitOfWork.Pagos.GetAllAsync();
            var reservas = await _unitOfWork.Reservas.GetAllAsync();

          // Filtrar pagos del mes seleccionado (excluir pendientes)
  var pagosMes = pagos.Where(p =>
      p.FechaPago.Year == anio.Value &&
    p.FechaPago.Month == mes.Value &&
                p.Metodo != "Pendiente"
            ).ToList();

            // Ingresos totales del mes
            var ingresosTotales = pagosMes.Sum(p => p.Monto);

            // Ingresos por día
    var ingresosPorDia = pagosMes
         .GroupBy(p => p.FechaPago.Day)
     .Select(g => new
       {
     Dia = g.Key,
    Total = g.Sum(p => p.Monto)
    })
       .OrderBy(x => x.Dia)
     .ToList();

            // Ingresos por método de pago
    var ingresosPorMetodo = pagosMes
        .GroupBy(p => p.Metodo)
     .Select(g => new
   {
      Metodo = g.Key,
           Total = g.Sum(p => p.Monto),
        Cantidad = g.Count()
         })
     .OrderByDescending(x => x.Total)
       .ToList();

   // Comparación con meses anteriores (últimos 6 meses)
            var comparacionMeses = new List<object>();
            for (int i = 5; i >= 0; i--)
  {
         var fecha = new DateTime(anio.Value, mes.Value, 1).AddMonths(-i);
       var ingresosMes = pagos
              .Where(p => p.FechaPago.Year == fecha.Year &&
         p.FechaPago.Month == fecha.Month &&
       p.Metodo != "Pendiente")
     .Sum(p => p.Monto);

        comparacionMeses.Add(new
            {
 Mes = fecha.ToString("MMM yyyy"),
             Total = ingresosMes
       });
       }

          // Top 5 reservas más costosas
         var topReservas = pagosMes
     .OrderByDescending(p => p.Monto)
     .Take(5)
  .Select(p => new
         {
      p.Id,
 p.Monto,
      p.FechaPago,
          Reserva = reservas.FirstOrDefault(r => r.Id == p.IdReserva)
      })
                .ToList();

            ViewBag.IngresosTotales = ingresosTotales;
            ViewBag.IngresosPorDia = ingresosPorDia;
   ViewBag.IngresosPorMetodo = ingresosPorMetodo;
 ViewBag.ComparacionMeses = comparacionMeses;
            ViewBag.TopReservas = topReservas;
  ViewBag.TotalPagos = pagosMes.Count;
            ViewBag.PromedioIngreso = pagosMes.Any() ? ingresosTotales / pagosMes.Count : 0;

    return View();
        }
        catch (Exception ex)
        {
   TempData["Error"] = $"Error al generar reporte de ingresos: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: Reportes/HabitacionesPopulares
    public async Task<IActionResult> HabitacionesPopulares(DateTime? fechaInicio, DateTime? fechaFin)
    {
        // Establecer fechas por defecto (último año)
   fechaInicio ??= DateTime.Now.AddYears(-1);
    fechaFin ??= DateTime.Now;

        ViewBag.FechaInicio = fechaInicio.Value.ToString("yyyy-MM-dd");
        ViewBag.FechaFin = fechaFin.Value.ToString("yyyy-MM-dd");

   try
     {
       var reservas = await _unitOfWork.Reservas.GetAllAsync();
        var habitaciones = await _unitOfWork.Habitaciones.GetAllAsync();
     var hoteles = await _unitOfWork.Hoteles.GetAllAsync();

     // Filtrar reservas en el rango
   var reservasEnRango = reservas.Where(r =>
             r.FechaReserva >= fechaInicio.Value &&
                r.FechaReserva <= fechaFin.Value
            ).ToList();

            // Top 10 habitaciones más reservadas
     var habitacionesPopulares = reservasEnRango
         .GroupBy(r => r.IdHabitacion)
  .Select(g => new
      {
       IdHabitacion = g.Key,
       TotalReservas = g.Count(),
    TotalNoches = g.Sum(r => (r.FechaSalida - r.FechaEntrada).Days),
     IngresoTotal = g.Sum(r =>
         {
                var hab = habitaciones.FirstOrDefault(h => h.Id == r.IdHabitacion);
      var noches = (r.FechaSalida - r.FechaEntrada).Days;
           return hab != null ? hab.PrecioPorNoche * noches : 0;
 }),
      Habitacion = habitaciones.FirstOrDefault(h => h.Id == g.Key)
        })
    .OrderByDescending(x => x.TotalReservas)
     .Take(10)
              .ToList();

 // Distribución por tipo de habitación
    var reservasPorTipo = reservasEnRango
   .Join(habitaciones, r => r.IdHabitacion, h => h.Id, (r, h) => h.Tipo)
        .GroupBy(tipo => tipo)
      .Select(g => new
 {
       Tipo = g.Key,
           Cantidad = g.Count()
       })
                .OrderByDescending(x => x.Cantidad)
           .ToList();

            // Tasa de ocupación por habitación
            var diasEnRango = (fechaFin.Value - fechaInicio.Value).Days + 1;
 var tasaOcupacionPorHabitacion = habitacionesPopulares
            .Select(hp => new
      {
    hp.Habitacion?.Numero,
           TasaOcupacion = Math.Round((hp.TotalNoches * 100.0) / diasEnRango, 2)
       })
    .ToList();

    // Estadísticas por hotel
      var estadisticasPorHotel = reservasEnRango
          .Join(habitaciones, r => r.IdHabitacion, h => h.Id, (r, h) => h)
   .GroupBy(h => h.IdHotel)
             .Select(g => new
      {
       IdHotel = g.Key,
          Hotel = hoteles.FirstOrDefault(h => h.Id == g.Key),
        TotalReservas = g.Count(),
          HabitacionesMasReservada = g.GroupBy(h => h.Id)
           .OrderByDescending(hg => hg.Count())
     .FirstOrDefault()?.First()
              })
        .ToList();

      // Tendencia mensual de reservas
 var tendenciaMensual = reservasEnRango
           .GroupBy(r => new { r.FechaReserva.Year, r.FechaReserva.Month })
     .Select(g => new
{
            Fecha = new DateTime(g.Key.Year, g.Key.Month, 1),
   Total = g.Count()
       })
   .OrderBy(x => x.Fecha)
  .ToList();

      ViewBag.HabitacionesPopulares = habitacionesPopulares;
    ViewBag.ReservasPorTipo = reservasPorTipo;
            ViewBag.TasaOcupacionPorHabitacion = tasaOcupacionPorHabitacion;
 ViewBag.EstadisticasPorHotel = estadisticasPorHotel;
          ViewBag.TendenciaMensual = tendenciaMensual;
    ViewBag.TotalReservas = reservasEnRango.Count;

return View();
        }
        catch (Exception ex)
        {
   TempData["Error"] = $"Error al generar reporte de habitaciones populares: {ex.Message}";
         return RedirectToAction(nameof(Index));
        }
    }

    // API: Obtener datos para gráficos (AJAX)
    [HttpGet]
 public async Task<IActionResult> ObtenerDatosOcupacion(DateTime fechaInicio, DateTime fechaFin)
    {
  try
        {
        var reservas = await _unitOfWork.Reservas.GetAllAsync();
            var reservasEnRango = reservas.Where(r =>
     r.FechaEntrada <= fechaFin &&
        r.FechaSalida >= fechaInicio
    ).ToList();

        var ocupacionPorDia = new Dictionary<string, int>();
  var fechaActual = fechaInicio.Date;

          while (fechaActual <= fechaFin.Date)
            {
                var ocupadas = reservasEnRango.Count(r =>
   r.FechaEntrada.Date <= fechaActual &&
       r.FechaSalida.Date >= fechaActual &&
        (r.Estado == "Confirmada" || r.Estado == "En curso")
   );

        ocupacionPorDia[fechaActual.ToString("dd/MM")] = ocupadas;
              fechaActual = fechaActual.AddDays(1);
    }

            return Json(new
       {
                success = true,
       labels = ocupacionPorDia.Keys.ToArray(),
        data = ocupacionPorDia.Values.ToArray()
   });
        }
  catch (Exception ex)
     {
          return Json(new { success = false, message = ex.Message });
        }
    }

    // API: Obtener datos de ingresos (AJAX)
    [HttpGet]
    public async Task<IActionResult> ObtenerDatosIngresos(int anio, int mes)
    {
        try
        {
          var pagos = await _unitOfWork.Pagos.GetAllAsync();
            var pagosMes = pagos.Where(p =>
              p.FechaPago.Year == anio &&
         p.FechaPago.Month == mes &&
       p.Metodo != "Pendiente"
            ).ToList();

            var ingresosPorDia = pagosMes
           .GroupBy(p => p.FechaPago.Day)
         .Select(g => new
                {
          Dia = g.Key,
           Total = g.Sum(p => p.Monto)
       })
  .OrderBy(x => x.Dia)
  .ToDictionary(x => x.Dia.ToString(), x => x.Total);

            return Json(new
       {
  success = true,
           labels = ingresosPorDia.Keys.ToArray(),
    data = ingresosPorDia.Values.ToArray()
       });
        }
   catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}
