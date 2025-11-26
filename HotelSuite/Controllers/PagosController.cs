using AutoMapper;
using HotelSuite.Application.DTOs;
using HotelSuite.Domain.Entities;
using HotelSuite.Domain.Interfaces;
using HotelSuite.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace HotelSuite.Controllers;

[Authorize(Roles = "Administrador,Gerente,Recepcionista")] // Requiere estos roles
public class PagosController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ComprobanteService _comprobanteService;
    private const int PageSize = 10;

    public PagosController(IUnitOfWork unitOfWork, IMapper mapper, ComprobanteService comprobanteService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _comprobanteService = comprobanteService;
    }

    // GET: Pagos
    public async Task<IActionResult> Index(string? filtroEstado, int? pagina)
    {
        try
        {
   // ? Cargar pagos CON relaciones (Reserva, Huesped, Habitacion)
            var pagos = await _unitOfWork.Pagos
                .GetAllQueryable()
                .Include(p => p.Reserva)
             .ThenInclude(r => r.Huesped)
 .Include(p => p.Reserva)
     .ThenInclude(r => r.Habitacion)
      .ThenInclude(h => h.Hotel)
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

   // Convertir a DTO (el mapeo automático poblará NombreHuesped y NumeroHabitacion)
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

    // GET: Pagos/Pendientes
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

    // GET: Pagos/Details/5
    public async Task<IActionResult> Details(int id)
    {
        try
 {
            // ? Cargar pago CON todas las relaciones
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

    // GET: Pagos/Registrar/5 (IdReserva)
    public async Task<IActionResult> Registrar(int id)
    {
        try
 {
     // ? Buscar el pago pendiente CON relaciones
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
      
// ? Mapear información de la reserva
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

    await CargarMetodosPago();
            return View(pagoDTO);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error al cargar el pago: {ex.Message}";
   return RedirectToAction(nameof(Index));
        }
    }

    // POST: Pagos/Registrar/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Registrar(int id, PagoDTO pagoDTO)
    {
        if (id != pagoDTO.Id)
        {
     TempData["Error"] = "ID de pago no válido.";
          return RedirectToAction(nameof(Index));
      }

        if (!ModelState.IsValid)
        {
    await CargarMetodosPago();
var reserva = await _unitOfWork.Reservas.GetByIdAsync(pagoDTO.IdReserva);
        if (reserva != null)
       {
        ViewBag.Reserva = _mapper.Map<ReservaDTO>(reserva);
            }
      return View(pagoDTO);
        }

        try
     {
            // Validar método de pago
     if (pagoDTO.Metodo == "Pendiente")
            {
   ModelState.AddModelError("Metodo", "Debe seleccionar un método de pago válido.");
       await CargarMetodosPago();
 return View(pagoDTO);
            }

      var pago = _mapper.Map<Pago>(pagoDTO);
            pago.FechaPago = DateTime.Now;

       _unitOfWork.Pagos.Update(pago);
        await _unitOfWork.CommitAsync();

            TempData["Success"] = $"Pago registrado exitosamente. Método: {pago.Metodo}";
            return RedirectToAction(nameof(Details), new { id = pago.Id });
    }
        catch (Exception ex)
        {
     ModelState.AddModelError("", $"Error al registrar el pago: {ex.Message}");
      await CargarMetodosPago();
          return View(pagoDTO);
 }
    }

    // GET: Pagos/GenerarComprobante/5
    public async Task<IActionResult> GenerarComprobante(int id)
    {
        try
   {
            // ? Cargar pago CON todas las relaciones
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

         // Verificar que el pago esté registrado (no pendiente)
    if (pago.Metodo == "Pendiente")
            {
    TempData["Error"] = "No se puede generar comprobante de un pago pendiente. Primero debe registrar el pago.";
  return RedirectToAction(nameof(Details), new { id });
}

            // ? Verificar que existan todos los datos relacionados
    if (pago.Reserva == null)
            {
                TempData["Error"] = "No se encontró la reserva asociada al pago.";
           return RedirectToAction(nameof(Details), new { id });
         }

       if (pago.Reserva.Huesped == null)
     {
        TempData["Error"] = "No se encontró el huésped asociado a la reserva.";
     return RedirectToAction(nameof(Details), new { id });
            }

            if (pago.Reserva.Habitacion == null)
  {
                TempData["Error"] = "No se encontró la habitación asociada a la reserva.";
    return RedirectToAction(nameof(Details), new { id });
            }

        // ? Convertir a DTOs y mapear información completa
     var pagoDTO = _mapper.Map<PagoDTO>(pago);
  var reservaDTO = _mapper.Map<ReservaDTO>(pago.Reserva);
  var huespedDTO = _mapper.Map<HuespedDTO>(pago.Reserva.Huesped);
            var habitacionDTO = _mapper.Map<HabitacionDTO>(pago.Reserva.Habitacion);

         // ? Asegurar que los DTOs tengan toda la información
        reservaDTO.NombreHuesped = $"{pago.Reserva.Huesped.Nombres} {pago.Reserva.Huesped.Apellidos}";
          reservaDTO.NumeroHabitacion = pago.Reserva.Habitacion.Numero;
         reservaDTO.TipoHabitacion = pago.Reserva.Habitacion.Tipo;
        reservaDTO.NombreHotel = pago.Reserva.Habitacion.Hotel?.Nombre ?? "Hotel";
            reservaDTO.PrecioPorNoche = pago.Reserva.Habitacion.PrecioPorNoche;
      reservaDTO.PrecioHabitacion = pago.Reserva.Habitacion.PrecioPorNoche;
       reservaDTO.MontoTotal = pago.Monto;

       // ? Generar PDF
            var pdfBytes = _comprobanteService.GenerarComprobantePDF(pagoDTO, reservaDTO, huespedDTO, habitacionDTO);

     // ? Retornar archivo PDF con nombre descriptivo
 var fileName = $"Comprobante_Pago_{pago.Id}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
}
        catch (Exception ex)
        {
            TempData["Error"] = $"Error al generar el comprobante: {ex.Message}";
            return RedirectToAction(nameof(Details), new { id });
   }
    }

    // Método auxiliar para cargar métodos de pago
    private async Task CargarMetodosPago()
    {
        await Task.CompletedTask; // Para mantener la firma async
 
 var metodosPago = new List<SelectListItem>
        {
          new SelectListItem { Value = "", Text = "-- Seleccione un método --" },
        new SelectListItem { Value = "Efectivo", Text = "Efectivo" },
       new SelectListItem { Value = "Tarjeta de Crédito", Text = "Tarjeta de Crédito" },
   new SelectListItem { Value = "Tarjeta de Débito", Text = "Tarjeta de Débito" },
            new SelectListItem { Value = "Transferencia Bancaria", Text = "Transferencia Bancaria" },
            new SelectListItem { Value = "PayPal", Text = "PayPal" },
new SelectListItem { Value = "Cheque", Text = "Cheque" }
        };

   ViewBag.MetodosPago = metodosPago;
    }
}
