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
            var pagos = await _unitOfWork.Pagos.GetAllAsync();

       // Aplicar filtro
    if (!string.IsNullOrWhiteSpace(filtroEstado))
{
           if (filtroEstado == "Pendiente")
       {
              pagos = pagos.Where(p => p.Metodo == "Pendiente");
                }
     else if (filtroEstado == "Pagado")
    {
  pagos = pagos.Where(p => p.Metodo != "Pendiente");
             }
   ViewBag.FiltroEstado = filtroEstado;
   }

            // Ordenar por fecha de pago descendente
     pagos = pagos.OrderByDescending(p => p.FechaPago);

            // Convertir a DTO
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
 var pagos = await _unitOfWork.Pagos.GetAllAsync();
            
  // Filtrar solo pagos pendientes
            pagos = pagos.Where(p => p.Metodo == "Pendiente")
            .OrderByDescending(p => p.FechaPago);

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
       var pago = await _unitOfWork.Pagos.GetByIdAsync(id);

        if (pago == null)
            {
      TempData["Error"] = "El pago no fue encontrado.";
                return RedirectToAction(nameof(Index));
       }

        var pagoDTO = _mapper.Map<PagoDTO>(pago);
            
    // Obtener información de la reserva
   var reserva = await _unitOfWork.Reservas.GetByIdAsync(pago.IdReserva);
            if (reserva != null)
   {
            ViewBag.Reserva = _mapper.Map<ReservaDTO>(reserva);
          
         // Obtener información de la habitación
           var habitacion = await _unitOfWork.Habitaciones.GetByIdAsync(reserva.IdHabitacion);
             if (habitacion != null)
      {
            ViewBag.Habitacion = _mapper.Map<HabitacionDTO>(habitacion);
    }
                
        // Obtener información del huésped
      var huesped = await _unitOfWork.Huespedes.GetByIdAsync(reserva.IdHuesped);
       if (huesped != null)
          {
            ViewBag.Huesped = _mapper.Map<HuespedDTO>(huesped);
     }
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
            // Buscar el pago pendiente de la reserva
  var pagos = await _unitOfWork.Pagos.GetAllAsync();
       var pago = pagos.FirstOrDefault(p => p.IdReserva == id && p.Metodo == "Pendiente");

      if (pago == null)
 {
     TempData["Error"] = "No se encontró un pago pendiente para esta reserva.";
  return RedirectToAction("Details", "Reservas", new { id });
      }

       var pagoDTO = _mapper.Map<PagoDTO>(pago);
       
            // Obtener información de la reserva
            var reserva = await _unitOfWork.Reservas.GetByIdAsync(id);
            if (reserva != null)
            {
    ViewBag.Reserva = _mapper.Map<ReservaDTO>(reserva);
  
                var habitacion = await _unitOfWork.Habitaciones.GetByIdAsync(reserva.IdHabitacion);
     if (habitacion != null)
        {
           ViewBag.Habitacion = _mapper.Map<HabitacionDTO>(habitacion);
     }
   
    var huesped = await _unitOfWork.Huespedes.GetByIdAsync(reserva.IdHuesped);
       if (huesped != null)
            {
        ViewBag.Huesped = _mapper.Map<HuespedDTO>(huesped);
      }
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
            var pago = await _unitOfWork.Pagos.GetByIdAsync(id);

         if (pago == null)
        {
     TempData["Error"] = "El pago no fue encontrado.";
       return RedirectToAction(nameof(Index));
     }

  // Verificar que el pago esté registrado (no pendiente)
            if (pago.Metodo == "Pendiente")
       {
 TempData["Error"] = "No se puede generar comprobante de un pago pendiente.";
         return RedirectToAction(nameof(Details), new { id });
            }

            // Obtener datos relacionados
    var reserva = await _unitOfWork.Reservas.GetByIdAsync(pago.IdReserva);
            if (reserva == null)
            {
     TempData["Error"] = "No se encontró la reserva asociada.";
   return RedirectToAction(nameof(Details), new { id });
  }

          var huesped = await _unitOfWork.Huespedes.GetByIdAsync(reserva.IdHuesped);
   var habitacion = await _unitOfWork.Habitaciones.GetByIdAsync(reserva.IdHabitacion);

        if (huesped == null || habitacion == null)
            {
     TempData["Error"] = "Faltan datos para generar el comprobante.";
     return RedirectToAction(nameof(Details), new { id });
        }

       // Convertir a DTOs
            var pagoDTO = _mapper.Map<PagoDTO>(pago);
     var reservaDTO = _mapper.Map<ReservaDTO>(reserva);
            var huespedDTO = _mapper.Map<HuespedDTO>(huesped);
         var habitacionDTO = _mapper.Map<HabitacionDTO>(habitacion);

         // Calcular días de estancia
  reservaDTO.PrecioHabitacion = habitacion.PrecioPorNoche;

      // Generar PDF
    var pdfBytes = _comprobanteService.GenerarComprobantePDF(pagoDTO, reservaDTO, huespedDTO, habitacionDTO);

  // Retornar archivo PDF
            var fileName = $"Comprobante_Pago_{pago.Id}_{DateTime.Now:yyyyMMdd}.pdf";
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
