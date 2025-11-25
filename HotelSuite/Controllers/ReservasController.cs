using AutoMapper;
using HotelSuite.Application.DTOs;
using HotelSuite.Domain.Entities;
using HotelSuite.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace HotelSuite.Controllers;

[Authorize(Roles = "Administrador,Gerente,Recepcionista")] // Requiere estos roles
public class ReservasController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private const int PageSize = 10;

 public ReservasController(IUnitOfWork unitOfWork, IMapper mapper)
 {
  _unitOfWork = unitOfWork;
  _mapper = mapper;
    }

    // GET: Reservas
 public async Task<IActionResult> Index(string? busquedaEstado, int? pagina)
    {
        try
   {
            var reservas = await _unitOfWork.Reservas.GetAllAsync();

    // Aplicar filtro de búsqueda
         if (!string.IsNullOrWhiteSpace(busquedaEstado))
    {
         reservas = reservas.Where(r => r.Estado.Contains(busquedaEstado, StringComparison.OrdinalIgnoreCase));
     ViewBag.BusquedaEstado = busquedaEstado;
      }

 // Ordenar por fecha de entrada descendente
    reservas = reservas.OrderByDescending(r => r.FechaEntrada);

            // Convertir a DTO
      var reservasDTO = _mapper.Map<IEnumerable<ReservaDTO>>(reservas);

            // Aplicar paginación
         int numeroPagina = pagina ?? 1;
            var reservasPaginadas = reservasDTO.ToPagedList(numeroPagina, PageSize);

        return View(reservasPaginadas);
        }
        catch (Exception ex)
     {
          TempData["Error"] = $"Error al cargar las reservas: {ex.Message}";
   return View(new List<ReservaDTO>().ToPagedList(1, PageSize));
        }
    }

    // GET: Reservas/Details/5
    public async Task<IActionResult> Details(int id)
    {
   try
        {
      var reserva = await _unitOfWork.Reservas.GetByIdAsync(id);

            if (reserva == null)
            {
         TempData["Error"] = "La reserva no fue encontrada.";
      return RedirectToAction(nameof(Index));
   }

     var reservaDTO = _mapper.Map<ReservaDTO>(reserva);
            
            // Obtener información adicional
         var habitacion = await _unitOfWork.Habitaciones.GetByIdAsync(reserva.IdHabitacion);
      if (habitacion != null)
          {
                reservaDTO.PrecioHabitacion = habitacion.PrecioPorNoche;
   reservaDTO.MontoTotal = habitacion.PrecioPorNoche * reservaDTO.DiasEstancia;
    }

  return View(reservaDTO);
   }
        catch (Exception ex)
      {
        TempData["Error"] = $"Error al cargar los detalles de la reserva: {ex.Message}";
            return RedirectToAction(nameof(Index));
     }
    }

    // GET: Reservas/Create
    public async Task<IActionResult> Create()
    {
        try
        {
            await CargarHuespedesYHabitacionesDisponibles();
            
// Crear un DTO con valores por defecto
            var reservaDTO = new ReservaDTO
 {
          FechaReserva = DateTime.Now,
      FechaEntrada = DateTime.Now.AddDays(1),
  FechaSalida = DateTime.Now.AddDays(2),
              Estado = "Confirmada"
    };

            return View(reservaDTO);
        }
        catch (Exception ex)
        {
       TempData["Error"] = $"Error al cargar el formulario: {ex.Message}";
       return RedirectToAction(nameof(Index));
        }
    }

    // POST: Reservas/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ReservaDTO reservaDTO)
    {
        if (!ModelState.IsValid)
        {
        await CargarHuespedesYHabitacionesDisponibles();
            return View(reservaDTO);
     }

        try
        {
 // Validar fechas
            if (reservaDTO.FechaEntrada < DateTime.Now.Date)
            {
      ModelState.AddModelError("FechaEntrada", "La fecha de entrada no puede ser anterior a hoy.");
    await CargarHuespedesYHabitacionesDisponibles();
       return View(reservaDTO);
       }

    if (reservaDTO.FechaSalida <= reservaDTO.FechaEntrada)
    {
           ModelState.AddModelError("FechaSalida", "La fecha de salida debe ser posterior a la fecha de entrada.");
     await CargarHuespedesYHabitacionesDisponibles();
     return View(reservaDTO);
    }

            // Validar disponibilidad de la habitación
            var habitacion = await _unitOfWork.Habitaciones.GetByIdAsync(reservaDTO.IdHabitacion);
      
            if (habitacion == null)
            {
       ModelState.AddModelError("IdHabitacion", "La habitación seleccionada no existe.");
     await CargarHuespedesYHabitacionesDisponibles();
     return View(reservaDTO);
   }

         if (habitacion.Estado != "Disponible")
   {
             ModelState.AddModelError("IdHabitacion", "La habitación seleccionada no está disponible.");
     await CargarHuespedesYHabitacionesDisponibles();
        return View(reservaDTO);
   }

        // Validar que no haya reservas superpuestas
      var reservas = await _unitOfWork.Reservas.GetAllAsync();
       var reservasSuperpuestas = reservas.Where(r => 
              r.IdHabitacion == reservaDTO.IdHabitacion &&
            r.Estado != "Cancelada" &&
      (
          (reservaDTO.FechaEntrada >= r.FechaEntrada && reservaDTO.FechaEntrada < r.FechaSalida) ||
    (reservaDTO.FechaSalida > r.FechaEntrada && reservaDTO.FechaSalida <= r.FechaSalida) ||
         (reservaDTO.FechaEntrada <= r.FechaEntrada && reservaDTO.FechaSalida >= r.FechaSalida)
     )
            );

            if (reservasSuperpuestas.Any())
{
     ModelState.AddModelError("IdHabitacion", "La habitación ya tiene reservas para las fechas seleccionadas.");
                await CargarHuespedesYHabitacionesDisponibles();
           return View(reservaDTO);
       }

            // Calcular el monto total
   var diasEstancia = (reservaDTO.FechaSalida - reservaDTO.FechaEntrada).Days;
     var montoTotal = habitacion.PrecioPorNoche * diasEstancia;

            // Convertir DTO a Entidad
            var reserva = _mapper.Map<Reserva>(reservaDTO);
         reserva.FechaReserva = DateTime.Now;
  reserva.Estado = "Confirmada";

     // Guardar reserva
          await _unitOfWork.Reservas.AddAsync(reserva);
        await _unitOfWork.CommitAsync();

         // Cambiar estado de la habitación a "Ocupada"
        habitacion.Estado = "Ocupada";
   _unitOfWork.Habitaciones.Update(habitacion);
  await _unitOfWork.CommitAsync();

   // Crear pago pendiente automáticamente
            var pago = new Pago
            {
      Monto = montoTotal,
     FechaPago = DateTime.Now,
       Metodo = "Pendiente",
        IdReserva = reserva.Id
};

 await _unitOfWork.Pagos.AddAsync(pago);
            await _unitOfWork.CommitAsync();

            TempData["Success"] = $"Reserva creada exitosamente. Total a pagar: {montoTotal:C}. Se ha generado un pago pendiente.";
        return RedirectToAction(nameof(Details), new { id = reserva.Id });
 }
        catch (Exception ex)
    {
    ModelState.AddModelError("", $"Error al crear la reserva: {ex.Message}");
          await CargarHuespedesYHabitacionesDisponibles();
 return View(reservaDTO);
        }
    }

    // GET: Reservas/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var reserva = await _unitOfWork.Reservas.GetByIdAsync(id);

   if (reserva == null)
            {
     TempData["Error"] = "La reserva no fue encontrada.";
      return RedirectToAction(nameof(Index));
     }

            var reservaDTO = _mapper.Map<ReservaDTO>(reserva);
   await CargarHuespedesYHabitacionesDisponibles(reserva.IdHabitacion);
         return View(reservaDTO);
        }
        catch (Exception ex)
   {
            TempData["Error"] = $"Error al cargar la reserva: {ex.Message}";
 return RedirectToAction(nameof(Index));
        }
    }

    // POST: Reservas/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ReservaDTO reservaDTO)
    {
        if (id != reservaDTO.Id)
        {
     TempData["Error"] = "ID de reserva no válido.";
      return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
  {
            await CargarHuespedesYHabitacionesDisponibles(reservaDTO.IdHabitacion);
            return View(reservaDTO);
        }

        try
        {
// Validar fechas
   if (reservaDTO.FechaSalida <= reservaDTO.FechaEntrada)
            {
       ModelState.AddModelError("FechaSalida", "La fecha de salida debe ser posterior a la fecha de entrada.");
 await CargarHuespedesYHabitacionesDisponibles(reservaDTO.IdHabitacion);
   return View(reservaDTO);
 }

var reserva = _mapper.Map<Reserva>(reservaDTO);
            _unitOfWork.Reservas.Update(reserva);
   await _unitOfWork.CommitAsync();

       TempData["Success"] = "Reserva actualizada exitosamente.";
    return RedirectToAction(nameof(Details), new { id });
        }
   catch (Exception ex)
      {
  ModelState.AddModelError("", $"Error al actualizar la reserva: {ex.Message}");
  await CargarHuespedesYHabitacionesDisponibles(reservaDTO.IdHabitacion);
            return View(reservaDTO);
      }
    }

    // GET: Reservas/Cancel/5
    public async Task<IActionResult> Cancel(int id)
    {
   try
        {
         var reserva = await _unitOfWork.Reservas.GetByIdAsync(id);

          if (reserva == null)
            {
 TempData["Error"] = "La reserva no fue encontrada.";
  return RedirectToAction(nameof(Index));
            }

          var reservaDTO = _mapper.Map<ReservaDTO>(reserva);
            return View(reservaDTO);
     }
        catch (Exception ex)
        {
   TempData["Error"] = $"Error al cargar la reserva: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Reservas/Cancel/5
    [HttpPost, ActionName("Cancel")]
[ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelConfirmed(int id)
    {
        try
    {
    var reserva = await _unitOfWork.Reservas.GetByIdAsync(id);

            if (reserva == null)
     {
         TempData["Error"] = "La reserva no fue encontrada.";
   return RedirectToAction(nameof(Index));
         }

            // Cambiar estado de la reserva a Cancelada
    reserva.Estado = "Cancelada";
            _unitOfWork.Reservas.Update(reserva);

     // Cambiar estado de la habitación a Disponible
            var habitacion = await _unitOfWork.Habitaciones.GetByIdAsync(reserva.IdHabitacion);
            if (habitacion != null)
       {
         habitacion.Estado = "Disponible";
                _unitOfWork.Habitaciones.Update(habitacion);
          }

      await _unitOfWork.CommitAsync();

 TempData["Success"] = "Reserva cancelada exitosamente. La habitación está nuevamente disponible.";
return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error al cancelar la reserva: {ex.Message}";
    return RedirectToAction(nameof(Cancel), new { id });
        }
    }

    // API para obtener información de habitación (para AJAX)
    [HttpGet]
    public async Task<IActionResult> GetHabitacionInfo(int id)
    {
        try
    {
    var habitacion = await _unitOfWork.Habitaciones.GetByIdAsync(id);

            if (habitacion == null)
            {
     return Json(new { success = false, message = "Habitación no encontrada" });
       }

            return Json(new 
            { 
        success = true,
         numero = habitacion.Numero,
      tipo = habitacion.Tipo,
       precio = habitacion.PrecioPorNoche,
   estado = habitacion.Estado
      });
    }
      catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    // Método auxiliar para cargar huéspedes y habitaciones
    private async Task CargarHuespedesYHabitacionesDisponibles(int? habitacionActualId = null)
    {
        // Cargar huéspedes
        var huespedes = await _unitOfWork.Huespedes.GetAllAsync();
        ViewBag.Huespedes = new SelectList(
            huespedes.OrderBy(h => h.Apellidos).ThenBy(h => h.Nombres).Select(h => new
            {
                h.Id,
                NombreCompleto = $"{h.Nombres} {h.Apellidos}"
            }),
            "Id",
            "NombreCompleto"
        );

        // Cargar habitaciones CON hotel incluido
        var habitaciones = await _unitOfWork.Habitaciones
            .GetAllQueryable()
            .Include(h => h.Hotel)
            .ToListAsync();

        // Filtrar solo disponibles o la habitación actual
        var habitacionesDisponibles = habitaciones
            .Where(h => h.Estado == "Disponible" || (habitacionActualId.HasValue && h.Id == habitacionActualId.Value))
            .OrderBy(h => h.Hotel.Nombre).ThenBy(h => h.Numero)
            .ToList();

        // Verificar si hay habitaciones disponibles
        if (!habitacionesDisponibles.Any())
        {
            ViewBag.Habitaciones = new SelectList(new List<object>());
            ViewBag.NoHabitacionesDisponibles = true;
            TempData["Warning"] = "?? No hay habitaciones disponibles en este momento. Por favor, cancele una reserva existente o espere a que haya disponibilidad.";
        }
        else
        {
            ViewBag.Habitaciones = new SelectList(
                habitacionesDisponibles.Select(h => new
                {
                    h.Id,
                    Display = $"{h.Hotel.Nombre} - #{h.Numero} - {h.Tipo} - {h.PrecioPorNoche:C}/noche"
                }),
                "Id",
                "Display"
            );
            ViewBag.NoHabitacionesDisponibles = false;
        }
    }
}
