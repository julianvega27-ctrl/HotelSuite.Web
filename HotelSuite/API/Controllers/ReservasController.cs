using AutoMapper;
using HotelSuite.Application.DTOs.API;
using HotelSuite.Domain.Entities;
using HotelSuite.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelSuite.API.Controllers;

/// <summary>
/// API para gestión de reservas
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class ReservasController : ControllerBase
{
  private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ReservasController> _logger;

    public ReservasController(
      IUnitOfWork unitOfWork,
  IMapper mapper,
  ILogger<ReservasController> logger)
    {
        _unitOfWork = unitOfWork;
 _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene una reserva por su ID
    /// </summary>
    [HttpGet("{id}")]
 [ProducesResponseType(typeof(ApiResponse<ReservaInfoDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ReservaInfoDTO>>> GetById(int id)
    {
     try
  {
_logger.LogInformation("Obteniendo reserva {Id}", id);

   var reserva = await _unitOfWork.Reservas
            .GetAllQueryable()
     .Include(r => r.Huesped)
           .Include(r => r.Habitacion)
      .ThenInclude(h => h.Hotel)
   .Include(r => r.Pagos)
   .FirstOrDefaultAsync(r => r.Id == id);

        if (reserva == null)
 {
      return NotFound(ApiResponse<ReservaInfoDTO>.ErrorResponse("Reserva no encontrada"));
    }

var dto = MapearReservaADTO(reserva);
      return Ok(ApiResponse<ReservaInfoDTO>.SuccessResponse(dto));
        }
     catch (Exception ex)
        {
_logger.LogError(ex, "Error al obtener reserva {Id}", id);
       return BadRequest(ApiResponse<ReservaInfoDTO>.ErrorResponse(
          "Error al procesar la solicitud",
    new List<string> { ex.Message }
            ));
        }
 }

    /// <summary>
/// Crea una nueva reserva
 /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ReservaInfoDTO>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<ReservaInfoDTO>>> Create([FromBody] CrearReservaDTO dto)
    {
   try
     {
   _logger.LogInformation("Creando nueva reserva para habitación {IdHabitacion}", dto.IdHabitacion);

   // Validar fechas
   if (dto.FechaEntrada >= dto.FechaSalida)
   {
       return BadRequest(ApiResponse<ReservaInfoDTO>.ErrorResponse(
        "La fecha de salida debe ser posterior a la fecha de entrada"
  ));
       }

        if (dto.FechaEntrada < DateTime.Now.Date)
       {
     return BadRequest(ApiResponse<ReservaInfoDTO>.ErrorResponse(
       "La fecha de entrada no puede ser anterior a hoy"
));
   }

// Verificar disponibilidad de la habitación
 var habitacion = await _unitOfWork.Habitaciones
    .GetByIdAsync(dto.IdHabitacion);

       if (habitacion == null)
       {
 return BadRequest(ApiResponse<ReservaInfoDTO>.ErrorResponse(
     "Habitación no encontrada"
    ));
    }

if (habitacion.Estado != "Disponible")
       {
    return BadRequest(ApiResponse<ReservaInfoDTO>.ErrorResponse(
      $"La habitación no está disponible. Estado actual: {habitacion.Estado}"
        ));
     }

/// Verificar conflictos con otras reservas
  var tieneConflicto = await _unitOfWork.Reservas
    .GetAllQueryable()
    .AnyAsync(r => r.IdHabitacion == dto.IdHabitacion &&
     r.Estado != "Cancelada" &&
    ((r.FechaEntrada <= dto.FechaSalida && r.FechaSalida >= dto.FechaEntrada)));

       if (tieneConflicto)
  {
    return BadRequest(ApiResponse<ReservaInfoDTO>.ErrorResponse(
 "La habitación ya tiene reservas para las fechas seleccionadas"
   ));
  }

   // Buscar o crear huésped
    var huesped = await _unitOfWork.Huespedes
   .GetAllQueryable()
      .FirstOrDefaultAsync(h => h.DocumentoIdentidad == dto.DocumentoIdentidad);

     if (huesped == null)
   {
       huesped = new Huesped
          {
Nombres = dto.Nombres,
          Apellidos = dto.Apellidos,
     Email = dto.Email,
    Telefono = dto.Telefono,
     DocumentoIdentidad = dto.DocumentoIdentidad
          };

 await _unitOfWork.Huespedes.AddAsync(huesped);
    await _unitOfWork.CommitAsync();
        _logger.LogInformation("Huésped creado con ID {Id}", huesped.Id);
   }

  // Crear reserva
         var reserva = new Reserva
        {
   FechaReserva = DateTime.Now,
    FechaEntrada = dto.FechaEntrada,
       FechaSalida = dto.FechaSalida,
   Estado = "Confirmada",
    IdHuesped = huesped.Id,
        IdHabitacion = dto.IdHabitacion
   };

     await _unitOfWork.Reservas.AddAsync(reserva);
       
  // Crear pago pendiente
       var diasEstancia = (dto.FechaSalida - dto.FechaEntrada).Days;
        var montoTotal = habitacion.PrecioPorNoche * diasEstancia;

            var pago = new Pago
 {
  Monto = montoTotal,
       FechaPago = DateTime.Now,
       Metodo = "Pendiente",
       IdReserva = reserva.Id
  };

   await _unitOfWork.Pagos.AddAsync(pago);
      await _unitOfWork.CommitAsync();

         _logger.LogInformation("Reserva creada exitosamente con ID {Id}", reserva.Id);

          // Recargar con includes para mapear
   reserva = await _unitOfWork.Reservas
  .GetAllQueryable()
  .Include(r => r.Huesped)
.Include(r => r.Habitacion)
       .ThenInclude(h => h.Hotel)
       .Include(r => r.Pagos)
          .FirstAsync(r => r.Id == reserva.Id);

var reservaDTO = MapearReservaADTO(reserva);

 return CreatedAtAction(
   nameof(GetById),
      new { id = reserva.Id },
      ApiResponse<ReservaInfoDTO>.SuccessResponse(reservaDTO, "Reserva creada exitosamente")
   );
        }
        catch (Exception ex)
   {
     _logger.LogError(ex, "Error al crear reserva");
   return BadRequest(ApiResponse<ReservaInfoDTO>.ErrorResponse(
       "Error al procesar la solicitud",
   new List<string> { ex.Message }
    ));
 }
    }

    /// <summary>
    /// Cancela una reserva
    /// </summary>
    [HttpPost("{id}/cancelar")]
    [ProducesResponseType(typeof(ApiResponse<ReservaInfoDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ReservaInfoDTO>>> Cancel(int id)
    {
     try
      {
     _logger.LogInformation("Cancelando reserva {Id}", id);

      var reserva = await _unitOfWork.Reservas
    .GetAllQueryable()
    .Include(r => r.Huesped)
   .Include(r => r.Habitacion)
     .ThenInclude(h => h.Hotel)
  .Include(r => r.Pagos)
     .FirstOrDefaultAsync(r => r.Id == id);

  if (reserva == null)
 {
      return NotFound(ApiResponse<ReservaInfoDTO>.ErrorResponse("Reserva no encontrada"));
         }

   if (reserva.Estado == "Cancelada")
 {
       return BadRequest(ApiResponse<ReservaInfoDTO>.ErrorResponse("La reserva ya está cancelada"));
        }

   if (reserva.Estado == "Finalizada")
  {
       return BadRequest(ApiResponse<ReservaInfoDTO>.ErrorResponse("No se puede cancelar una reserva finalizada"));
      }

 reserva.Estado = "Cancelada";
       await _unitOfWork.CommitAsync();

    _logger.LogInformation("Reserva {Id} cancelada exitosamente", id);

var dto = MapearReservaADTO(reserva);
   return Ok(ApiResponse<ReservaInfoDTO>.SuccessResponse(dto, "Reserva cancelada exitosamente"));
        }
   catch (Exception ex)
   {
   _logger.LogError(ex, "Error al cancelar reserva {Id}", id);
       return BadRequest(ApiResponse<ReservaInfoDTO>.ErrorResponse(
        "Error al procesar la solicitud",
     new List<string> { ex.Message }
   ));
  }
    }

    /// <summary>
    /// Busca reserva por documento de identidad
    /// </summary>
    [HttpGet("buscar")]
    [ProducesResponseType(typeof(ApiResponse<List<ReservaInfoDTO>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<ReservaInfoDTO>>>> BuscarPorDocumento(
  [FromQuery] string documentoIdentidad)
    {
  try
 {
            if (string.IsNullOrWhiteSpace(documentoIdentidad))
{
       return BadRequest(ApiResponse<List<ReservaInfoDTO>>.ErrorResponse(
  "Debe proporcionar un documento de identidad"
       ));
  }

    var reservas = await _unitOfWork.Reservas
    .GetAllQueryable()
    .Include(r => r.Huesped)
 .Include(r => r.Habitacion)
     .ThenInclude(h => h.Hotel)
     .Include(r => r.Pagos)
         .Where(r => r.Huesped.DocumentoIdentidad == documentoIdentidad)
   .OrderByDescending(r => r.FechaReserva)
       .ToListAsync();

            var dtos = reservas.Select(MapearReservaADTO).ToList();

   return Ok(ApiResponse<List<ReservaInfoDTO>>.SuccessResponse(
dtos,
         $"Se encontraron {dtos.Count} reservas"
   ));
 }
  catch (Exception ex)
{
 _logger.LogError(ex, "Error al buscar reservas");
     return BadRequest(ApiResponse<List<ReservaInfoDTO>>.ErrorResponse(
          "Error al procesar la solicitud",
        new List<string> { ex.Message }
     ));
 }
    }

    /// <summary>
 /// Lista paginada de reservas con filtros opcionales.
 /// </summary>
 [HttpGet]
 [ProducesResponseType(typeof(PagedApiResponse<ReservaInfoDTO>), StatusCodes.Status200OK)]
 public async Task<ActionResult<PagedApiResponse<ReservaInfoDTO>>> GetAll(
 [FromQuery] int pagina =1,
 [FromQuery] int tamanoPagina =10,
 [FromQuery] string? estado = null,
 [FromQuery] string? documentoIdentidad = null,
 [FromQuery] int? idHabitacion = null,
 [FromQuery] DateTime? fechaDesde = null,
 [FromQuery] DateTime? fechaHasta = null)
 {
 if (pagina <1) pagina =1;
 if (tamanoPagina <1 || tamanoPagina >100) tamanoPagina =10;

 var query = _unitOfWork.Reservas
 .GetAllQueryable()
 .Include(r => r.Huesped)
 .Include(r => r.Habitacion).ThenInclude(h => h.Hotel)
 .Include(r => r.Pagos)
 .AsQueryable();

 if (!string.IsNullOrWhiteSpace(estado))
 query = query.Where(r => r.Estado == estado);
 if (!string.IsNullOrWhiteSpace(documentoIdentidad))
 query = query.Where(r => r.Huesped.DocumentoIdentidad == documentoIdentidad);
 if (idHabitacion.HasValue)
 query = query.Where(r => r.IdHabitacion == idHabitacion.Value);
 if (fechaDesde.HasValue)
 query = query.Where(r => r.FechaEntrada >= fechaDesde.Value);
 if (fechaHasta.HasValue)
 query = query.Where(r => r.FechaSalida <= fechaHasta.Value);

 var total = await query.CountAsync();
 var reservas = await query
 .OrderByDescending(r => r.FechaReserva)
 .Skip((pagina -1) * tamanoPagina)
 .Take(tamanoPagina)
 .ToListAsync();

 var dtos = reservas.Select(MapearReservaADTO).ToList();

 var response = new PagedApiResponse<ReservaInfoDTO>
 {
 Success = true,
 Message = $"Página {pagina} de {Math.Ceiling(total / (double)tamanoPagina)}",
 Data = dtos,
 Pagination = new PaginationMetadata
 {
 CurrentPage = pagina,
 PageSize = tamanoPagina,
 TotalCount = total,
 TotalPages = (int)Math.Ceiling(total / (double)tamanoPagina),
 HasPrevious = pagina >1,
 HasNext = pagina * tamanoPagina < total
 }
 };

 return Ok(response);
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
