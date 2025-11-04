using AutoMapper;
using HotelSuite.Application.DTOs.API;
using HotelSuite.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelSuite.API.Controllers;

/// <summary>
/// API para gestión de habitaciones y disponibilidad
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class HabitacionesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<HabitacionesController> _logger;

    public HabitacionesController(
        IUnitOfWork unitOfWork,
        IMapper mapper,
 ILogger<HabitacionesController> logger)
  {
    _unitOfWork = unitOfWork;
      _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todas las habitaciones disponibles
    /// </summary>
    /// <param name="query">Parámetros de búsqueda</param>
 /// <returns>Lista de habitaciones disponibles</returns>
    [HttpGet("disponibles")]
    [ProducesResponseType(typeof(PagedApiResponse<HabitacionDisponibleDTO>), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedApiResponse<HabitacionDisponibleDTO>>> GetDisponibles(
     [FromQuery] DisponibilidadQueryDTO query,
        [FromQuery] int pagina = 1,
  [FromQuery] int registrosPorPagina = 10)
  {
        try
   {
   _logger.LogInformation("Consultando habitaciones disponibles con filtros: {@Query}", query);

            // Construir query base
  var habitacionesQuery = _unitOfWork.Habitaciones
       .GetAllQueryable()
  .Include(h => h.Hotel)
     .Where(h => h.Estado == "Disponible");

 // Aplicar filtros
            if (query.IdHotel.HasValue)
     {
   habitacionesQuery = habitacionesQuery.Where(h => h.IdHotel == query.IdHotel.Value);
}

 if (!string.IsNullOrEmpty(query.Tipo))
    {
  habitacionesQuery = habitacionesQuery.Where(h => h.Tipo == query.Tipo);
  }

      if (query.PrecioMaximo.HasValue)
    {
     habitacionesQuery = habitacionesQuery.Where(h => h.PrecioPorNoche <= query.PrecioMaximo.Value);
  }

  // Si hay fechas, filtrar habitaciones que no tengan reservas en ese período
      if (query.FechaEntrada.HasValue && query.FechaSalida.HasValue)
            {
var habitacionesReservadas = _unitOfWork.Reservas
   .GetAllQueryable()
         .Where(r => r.Estado != "Cancelada" &&
   ((r.FechaEntrada <= query.FechaSalida && r.FechaSalida >= query.FechaEntrada)))
        .Select(r => r.IdHabitacion)
.Distinct();

   habitacionesQuery = habitacionesQuery.Where(h => !habitacionesReservadas.Contains(h.Id));
        }

   // Obtener total antes de paginar
var totalCount = await habitacionesQuery.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)registrosPorPagina);

     // Paginar
var habitaciones = await habitacionesQuery
     .OrderBy(h => h.Numero)
   .Skip((pagina - 1) * registrosPorPagina)
        .Take(registrosPorPagina)
       .ToListAsync();

            // Mapear a DTO
    var habitacionesDTO = habitaciones.Select(h => new HabitacionDisponibleDTO
       {
            Id = h.Id,
    Numero = h.Numero,
    Tipo = h.Tipo,
       PrecioPorNoche = h.PrecioPorNoche,
       Estado = h.Estado,
           Hotel = h.Hotel != null ? new HotelInfoDTO
    {
   Id = h.Hotel.Id,
  Nombre = h.Hotel.Nombre,
    Direccion = h.Hotel.Direccion,
     Telefono = h.Hotel.Telefono,
  Categoria = h.Hotel.Categoria
        } : null,
    Caracteristicas = ObtenerCaracteristicasPorTipo(h.Tipo)
            }).ToList();

  var response = new PagedApiResponse<HabitacionDisponibleDTO>
   {
    Success = true,
       Message = "Habitaciones obtenidas exitosamente",
    Data = habitacionesDTO,
       Pagination = new PaginationMetadata
     {
     CurrentPage = pagina,
      PageSize = registrosPorPagina,
          TotalPages = totalPages,
   TotalCount = totalCount,
    HasPrevious = pagina > 1,
      HasNext = pagina < totalPages
}
         };

       return Ok(response);
        }
      catch (Exception ex)
        {
     _logger.LogError(ex, "Error al obtener habitaciones disponibles");
   return BadRequest(ApiResponse<object>.ErrorResponse(
     "Error al procesar la solicitud",
      new List<string> { ex.Message }
       ));
 }
    }

    /// <summary>
    /// Obtiene una habitación específica por ID
  /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<HabitacionDisponibleDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<HabitacionDisponibleDTO>>> GetById(int id)
  {
      try
      {
     var habitacion = await _unitOfWork.Habitaciones
     .GetAllQueryable()
          .Include(h => h.Hotel)
         .FirstOrDefaultAsync(h => h.Id == id);

       if (habitacion == null)
         {
             return NotFound(ApiResponse<HabitacionDisponibleDTO>.ErrorResponse(
       "Habitación no encontrada"
          ));
            }

         var dto = new HabitacionDisponibleDTO
        {
           Id = habitacion.Id,
     Numero = habitacion.Numero,
       Tipo = habitacion.Tipo,
     PrecioPorNoche = habitacion.PrecioPorNoche,
   Estado = habitacion.Estado,
         Hotel = habitacion.Hotel != null ? new HotelInfoDTO
   {
       Id = habitacion.Hotel.Id,
      Nombre = habitacion.Hotel.Nombre,
      Direccion = habitacion.Hotel.Direccion,
        Telefono = habitacion.Hotel.Telefono,
         Categoria = habitacion.Hotel.Categoria
         } : null,
         Caracteristicas = ObtenerCaracteristicasPorTipo(habitacion.Tipo)
        };

  return Ok(ApiResponse<HabitacionDisponibleDTO>.SuccessResponse(dto));
        }
      catch (Exception ex)
    {
  _logger.LogError(ex, "Error al obtener habitación {Id}", id);
  return BadRequest(ApiResponse<HabitacionDisponibleDTO>.ErrorResponse(
     "Error al procesar la solicitud",
      new List<string> { ex.Message }
       ));
      }
    }

    /// <summary>
    /// Verifica disponibilidad de una habitación específica en fechas dadas
    /// </summary>
    [HttpGet("{id}/disponibilidad")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<bool>>> VerificarDisponibilidad(
  int id,
        [FromQuery] DateTime fechaEntrada,
        [FromQuery] DateTime fechaSalida)
{
      try
     {
  var habitacion = await _unitOfWork.Habitaciones
   .GetByIdAsync(id);

     if (habitacion == null)
       {
    return NotFound(ApiResponse<bool>.ErrorResponse("Habitación no encontrada"));
          }

    if (habitacion.Estado != "Disponible")
 {
        return Ok(ApiResponse<bool>.SuccessResponse(
     false,
   $"Habitación en estado: {habitacion.Estado}"
));
       }

        // Verificar reservas existentes
     var reservasConflicto = await _unitOfWork.Reservas
         .GetAllQueryable()
      .Where(r => r.IdHabitacion == id &&
      r.Estado != "Cancelada" &&
                ((r.FechaEntrada <= fechaSalida && r.FechaSalida >= fechaEntrada)))
  .AnyAsync();

            var disponible = !reservasConflicto;
     var mensaje = disponible
    ? "Habitación disponible para las fechas solicitadas"
    : "Habitación no disponible para las fechas solicitadas";

  return Ok(ApiResponse<bool>.SuccessResponse(disponible, mensaje));
     }
     catch (Exception ex)
     {
   _logger.LogError(ex, "Error al verificar disponibilidad de habitación {Id}", id);
            return BadRequest(ApiResponse<bool>.ErrorResponse(
     "Error al procesar la solicitud",
  new List<string> { ex.Message }
        ));
   }
    }

    /// <summary>
 /// Obtiene los tipos de habitación disponibles
    /// </summary>
    [HttpGet("tipos")]
    [ProducesResponseType(typeof(ApiResponse<List<string>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<string>>>> GetTipos()
    {
    try
{
      var tipos = await _unitOfWork.Habitaciones
       .GetAllQueryable()
       .Select(h => h.Tipo)
        .Distinct()
   .OrderBy(t => t)
  .ToListAsync();

    return Ok(ApiResponse<List<string>>.SuccessResponse(tipos));
        }
     catch (Exception ex)
    {
   _logger.LogError(ex, "Error al obtener tipos de habitación");
          return BadRequest(ApiResponse<List<string>>.ErrorResponse(
     "Error al procesar la solicitud",
     new List<string> { ex.Message }
     ));
  }
    }

    private List<string> ObtenerCaracteristicasPorTipo(string tipo)
 {
   return tipo switch
  {
       "Individual" => new List<string> { "1 cama individual", "TV", "Baño privado", "WiFi" },
"Doble" => new List<string> { "2 camas o 1 cama matrimonial", "TV", "Baño privado", "WiFi", "Minibar" },
            "Suite" => new List<string> { "Cama King", "Sala de estar", "TV", "Baño con jacuzzi", "WiFi", "Minibar", "Vista panorámica" },
    "Presidencial" => new List<string> { "Cama King", "Sala de estar amplia", "Comedor", "Cocina", "2+ baños", "WiFi", "Minibar", "Vista panorámica", "Terraza" },
 "Ejecutiva" => new List<string> { "Cama Queen", "Escritorio de trabajo", "TV", "Baño privado", "WiFi", "Minibar", "Cafetera" },
        _ => new List<string> { "TV", "Baño privado", "WiFi" }
        };
    }
}
