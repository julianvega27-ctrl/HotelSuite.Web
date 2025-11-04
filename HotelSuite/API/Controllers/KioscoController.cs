using HotelSuite.Application.DTOs.API;
using HotelSuite.Domain.Entities;
using HotelSuite.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelSuite.API.Controllers;

/// <summary>
/// API para kioscos de check-in/check-out automático
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class KioscoController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<KioscoController> _logger;

    public KioscoController(
        IUnitOfWork unitOfWork,
        ILogger<KioscoController> logger)
    {
 _unitOfWork = unitOfWork;
     _logger = logger;
    }

    /// <summary>
    /// Realiza check-in automático
    /// </summary>
    [HttpPost("checkin")]
    [ProducesResponseType(typeof(ApiResponse<CheckInResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CheckInResponseDTO>>> CheckIn([FromBody] CheckInDTO dto)
    {
  try
        {
    _logger.LogInformation("Procesando check-in para documento {Documento}", dto.DocumentoIdentidad);

            // Buscar reserva
            var reserva = await _unitOfWork.Reservas
  .GetAllQueryable()
     .Include(r => r.Huesped)
.Include(r => r.Habitacion)
      .ThenInclude(h => h.Hotel)
   .Include(r => r.Pagos)
       .Where(r => r.Id == dto.IdReserva &&
       r.Huesped.DocumentoIdentidad == dto.DocumentoIdentidad)
        .FirstOrDefaultAsync();

   if (reserva == null)
       {
    return NotFound(ApiResponse<CheckInResponseDTO>.ErrorResponse(
      "No se encontró una reserva con los datos proporcionados"
 ));
 }

   // Validaciones
       if (reserva.Estado == "Cancelada")
  {
    return BadRequest(ApiResponse<CheckInResponseDTO>.ErrorResponse(
   "La reserva ha sido cancelada"
   ));
  }

     if (reserva.Estado == "En curso")
    {
  return BadRequest(ApiResponse<CheckInResponseDTO>.ErrorResponse(
      "Ya se realizó el check-in para esta reserva"
  ));
         }

   if (reserva.Estado == "Finalizada")
  {
    return BadRequest(ApiResponse<CheckInResponseDTO>.ErrorResponse(
 "Esta reserva ya ha finalizado"
      ));
 }

   // Verificar que es el día correcto
  if (reserva.FechaEntrada.Date > DateTime.Now.Date)
       {
      return BadRequest(ApiResponse<CheckInResponseDTO>.ErrorResponse(
   $"El check-in está programado para el {reserva.FechaEntrada:dd/MM/yyyy}"
       ));
  }

            // Verificar pagos pendientes
var pagosPendientes = reserva.Pagos?.Any(p => p.Metodo == "Pendiente") ?? false;
 if (pagosPendientes)
   {
    return BadRequest(ApiResponse<CheckInResponseDTO>.ErrorResponse(
 "Debe completar el pago antes del check-in. Diríjase a recepción."
   ));
      }

   // Realizar check-in
    reserva.Estado = "En curso";
     await _unitOfWork.CommitAsync();

  // Generar tarjeta de acceso (código único)
var tarjetaAcceso = GenerarCodigoTarjeta(reserva.Id, reserva.IdHabitacion);

   _logger.LogInformation("Check-in exitoso para reserva {Id}", reserva.Id);

      var response = new CheckInResponseDTO
      {
   Exitoso = true,
      Mensaje = $"¡Bienvenido! Check-in completado exitosamente. Habitación: {reserva.Habitacion?.Numero}",
       Reserva = MapearReservaADTO(reserva),
       TarjetaAcceso = tarjetaAcceso
    };

    return Ok(ApiResponse<CheckInResponseDTO>.SuccessResponse(response));
 }
      catch (Exception ex)
 {
  _logger.LogError(ex, "Error durante check-in");
    return BadRequest(ApiResponse<CheckInResponseDTO>.ErrorResponse(
      "Error al procesar el check-in",
      new List<string> { ex.Message }
   ));
        }
    }

    /// <summary>
 /// Realiza check-out automático
 /// </summary>
    [HttpPost("checkout")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<string>>> CheckOut([FromBody] CheckInDTO dto)
    {
        try
        {
    _logger.LogInformation("Procesando check-out para documento {Documento}", dto.DocumentoIdentidad);

     var reserva = await _unitOfWork.Reservas
    .GetAllQueryable()
    .Include(r => r.Huesped)
      .Include(r => r.Habitacion)
    .Where(r => r.Id == dto.IdReserva &&
        r.Huesped.DocumentoIdentidad == dto.DocumentoIdentidad)
        .FirstOrDefaultAsync();

   if (reserva == null)
      {
    return NotFound(ApiResponse<string>.ErrorResponse(
      "No se encontró una reserva con los datos proporcionados"
      ));
   }

   if (reserva.Estado != "En curso")
     {
   return BadRequest(ApiResponse<string>.ErrorResponse(
          $"No se puede realizar check-out. Estado actual: {reserva.Estado}"
   ));
  }

   // Realizar check-out
     reserva.Estado = "Finalizada";
       await _unitOfWork.CommitAsync();

     _logger.LogInformation("Check-out exitoso para reserva {Id}", reserva.Id);

   return Ok(ApiResponse<string>.SuccessResponse(
     $"Check-out completado exitosamente. Habitación {reserva.Habitacion?.Numero} liberada. ¡Gracias por su estadía!"
  ));
 }
   catch (Exception ex)
        {
      _logger.LogError(ex, "Error durante check-out");
     return BadRequest(ApiResponse<string>.ErrorResponse(
     "Error al procesar el check-out",
   new List<string> { ex.Message }
       ));
  }
    }

    /// <summary>
    /// Obtiene información de reserva para el kiosco
    /// </summary>
    [HttpGet("reserva")]
    [ProducesResponseType(typeof(ApiResponse<ReservaInfoDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ReservaInfoDTO>>> GetReservaInfo(
        [FromQuery] string documentoIdentidad,
    [FromQuery] int? idReserva = null)
    {
 try
        {
     var query = _unitOfWork.Reservas
    .GetAllQueryable()
     .Include(r => r.Huesped)
 .Include(r => r.Habitacion)
      .ThenInclude(h => h.Hotel)
       .Include(r => r.Pagos)
  .Where(r => r.Huesped.DocumentoIdentidad == documentoIdentidad &&
        r.Estado != "Cancelada");

  if (idReserva.HasValue)
       {
      query = query.Where(r => r.Id == idReserva.Value);
   }

  var reserva = await query
      .OrderByDescending(r => r.FechaReserva)
   .FirstOrDefaultAsync();

if (reserva == null)
          {
return NotFound(ApiResponse<ReservaInfoDTO>.ErrorResponse(
  "No se encontró ninguna reserva activa"
      ));
   }

        var dto = MapearReservaADTO(reserva);
   return Ok(ApiResponse<ReservaInfoDTO>.SuccessResponse(dto));
    }
        catch (Exception ex)
   {
 _logger.LogError(ex, "Error al obtener información de reserva");
            return BadRequest(ApiResponse<ReservaInfoDTO>.ErrorResponse(
    "Error al procesar la solicitud",
    new List<string> { ex.Message }
   ));
 }
    }

    private string GenerarCodigoTarjeta(int idReserva, int idHabitacion)
  {
 // Formato: RXXXX-HXXXX-TXXXXXXXX (R=Reserva, H=Habitación, T=Timestamp)
      var timestamp = DateTime.Now.ToString("yyyyMMdd");
      return $"R{idReserva:D4}-H{idHabitacion:D4}-T{timestamp}";
    }

    private ReservaInfoDTO MapearReservaADTO(Reserva reserva)
    {
    var diasEstancia = (reserva.FechaSalida - reserva.FechaEntrada).Days;
  var montoTotal = reserva.Habitacion != null
   ? reserva.Habitacion.PrecioPorNoche * diasEstancia
   : 0;

return new ReservaInfoDTO
    {
    Id = reserva.Id,
     FechaReserva = reserva.FechaReserva,
  FechaEntrada = reserva.FechaEntrada,
    FechaSalida = reserva.FechaSalida,
  Estado = reserva.Estado,
       DiasEstancia = diasEstancia,
      MontoTotal = montoTotal,
  Huesped = reserva.Huesped != null ? new HuespedInfoDTO
     {
       Id = reserva.Huesped.Id,
      NombreCompleto = $"{reserva.Huesped.Nombres} {reserva.Huesped.Apellidos}",
     Email = reserva.Huesped.Email,
       Telefono = reserva.Huesped.Telefono
   } : null,
Habitacion = reserva.Habitacion != null ? new HabitacionInfoDTO
      {
    Id = reserva.Habitacion.Id,
  Numero = reserva.Habitacion.Numero,
Tipo = reserva.Habitacion.Tipo,
   PrecioPorNoche = reserva.Habitacion.PrecioPorNoche,
     NombreHotel = reserva.Habitacion.Hotel?.Nombre ?? string.Empty
       } : null,
       Pagos = reserva.Pagos?.Select(p => new PagoInfoDTO
   {
      Id = p.Id,
       Monto = p.Monto,
  FechaPago = p.FechaPago,
      Metodo = p.Metodo
       }).ToList() ?? new List<PagoInfoDTO>()
    };
    }
}
