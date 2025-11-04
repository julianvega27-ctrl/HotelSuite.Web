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

[Authorize] // Requiere autenticación para todas las acciones
public class HabitacionesController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private const int PageSize = 10;

    public HabitacionesController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
   _mapper = mapper;
    }

    // GET: Habitaciones
    [AllowAnonymous] // Permitir ver habitaciones sin autenticación
    public async Task<IActionResult> Index(
 string? busquedaTipo, 
     string? busquedaEstado, 
        int? busquedaHotel,
        int? busquedaCapacidad,
        decimal? precioMinimo,
        decimal? precioMaximo,
 string? busquedaTipoCama,
bool? soloConVista,
        int? pagina)
    {
        try
        {
   // Obtener habitaciones con hotel incluido
      var habitaciones = await _unitOfWork.Habitaciones
.GetAllQueryable()
   .Include(h => h.Hotel)
       .ToListAsync();
  
      // Aplicar filtros de búsqueda
            if (!string.IsNullOrWhiteSpace(busquedaTipo))
      {
 habitaciones = habitaciones.Where(h => h.Tipo.Contains(busquedaTipo, StringComparison.OrdinalIgnoreCase)).ToList();
 ViewBag.BusquedaTipo = busquedaTipo;
  }

            if (!string.IsNullOrWhiteSpace(busquedaEstado))
            {
         habitaciones = habitaciones.Where(h => h.Estado.Contains(busquedaEstado, StringComparison.OrdinalIgnoreCase)).ToList();
 ViewBag.BusquedaEstado = busquedaEstado;
            }

      if (busquedaHotel.HasValue && busquedaHotel.Value > 0)
            {
      habitaciones = habitaciones.Where(h => h.IdHotel == busquedaHotel.Value).ToList();
  ViewBag.BusquedaHotel = busquedaHotel.Value;
            }

   if (busquedaCapacidad.HasValue && busquedaCapacidad.Value > 0)
      {
   habitaciones = habitaciones.Where(h => h.Capacidad >= busquedaCapacidad.Value).ToList();
       ViewBag.BusquedaCapacidad = busquedaCapacidad.Value;
       }

       if (precioMinimo.HasValue)
          {
    habitaciones = habitaciones.Where(h => h.PrecioPorNoche >= precioMinimo.Value).ToList();
       ViewBag.PrecioMinimo = precioMinimo.Value;
      }

     if (precioMaximo.HasValue)
    {
habitaciones = habitaciones.Where(h => h.PrecioPorNoche <= precioMaximo.Value).ToList();
       ViewBag.PrecioMaximo = precioMaximo.Value;
            }

      if (!string.IsNullOrWhiteSpace(busquedaTipoCama))
     {
        habitaciones = habitaciones.Where(h => 
   h.TipoCama != null && h.TipoCama.Contains(busquedaTipoCama, StringComparison.OrdinalIgnoreCase)).ToList();
       ViewBag.BusquedaTipoCama = busquedaTipoCama;
         }

            if (soloConVista.HasValue && soloConVista.Value)
            {
     habitaciones = habitaciones.Where(h => h.TieneVista).ToList();
                ViewBag.SoloConVista = soloConVista.Value;
            }

   // Ordenar por número de habitación
     habitaciones = habitaciones.OrderBy(h => h.Numero).ToList();

      // Convertir a DTO
 var habitacionesDTO = _mapper.Map<IEnumerable<HabitacionDTO>>(habitaciones);

 // Aplicar paginación
            int numeroPagina = pagina ?? 1;
            var habitacionesPaginadas = habitacionesDTO.ToPagedList(numeroPagina, PageSize);

 // Cargar datos para filtros
     await CargarDatosFiltros();

            return View(habitacionesPaginadas);
      }
        catch (Exception ex)
      {
    TempData["Error"] = $"Error al cargar las habitaciones: {ex.Message}";
   return View(new List<HabitacionDTO>().ToPagedList(1, PageSize));
        }
    }

    // GET: Habitaciones/Details/5
    public async Task<IActionResult> Details(int id)
    {
try
        {
  var habitacion = await _unitOfWork.Habitaciones.GetByIdAsync(id);
     
      if (habitacion == null)
{
           TempData["Error"] = "La habitación no fue encontrada.";
        return RedirectToAction(nameof(Index));
        }

  var habitacionDTO = _mapper.Map<HabitacionDTO>(habitacion);
            return View(habitacionDTO);
        }
    catch (Exception ex)
        {
            TempData["Error"] = $"Error al cargar los detalles de la habitación: {ex.Message}";
 return RedirectToAction(nameof(Index));
        }
    }

    // GET: Habitaciones/Create
    [Authorize(Roles = "Administrador,Gerente")] // Solo Administrador y Gerente pueden crear
    public async Task<IActionResult> Create()
    {
        await CargarHotelesEnViewBag();
        return View();
    }

  // POST: Habitaciones/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador,Gerente")]
    public async Task<IActionResult> Create(HabitacionDTO habitacionDTO)
    {
        if (!ModelState.IsValid)
        {
            await CargarHotelesEnViewBag();
          return View(habitacionDTO);
        }

        try
        {
      // Validar que el número de habitación sea único
    var habitaciones = await _unitOfWork.Habitaciones.GetAllAsync();
  if (habitaciones.Any(h => h.Numero == habitacionDTO.Numero && h.IdHotel == habitacionDTO.IdHotel))
            {
                ModelState.AddModelError("Numero", "Ya existe una habitación con este número en el hotel seleccionado.");
    await CargarHotelesEnViewBag();
          return View(habitacionDTO);
  }

            // Convertir DTO a Entidad
            var habitacion = _mapper.Map<Habitacion>(habitacionDTO);
            
      // Guardar en la base de datos
      await _unitOfWork.Habitaciones.AddAsync(habitacion);
   await _unitOfWork.CommitAsync();

     TempData["Success"] = "Habitación creada exitosamente.";
   return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error al crear la habitación: {ex.Message}");
            await CargarHotelesEnViewBag();
       return View(habitacionDTO);
        }
    }

    // GET: Habitaciones/Edit/5
    [Authorize(Roles = "Administrador,Gerente")]
 public async Task<IActionResult> Edit(int id)
    {
 try
   {
      var habitacion = await _unitOfWork.Habitaciones.GetByIdAsync(id);
  
     if (habitacion == null)
            {
  TempData["Error"] = "La habitación no fue encontrada.";
                return RedirectToAction(nameof(Index));
      }

   var habitacionDTO = _mapper.Map<HabitacionDTO>(habitacion);
  await CargarHotelesEnViewBag();
    return View(habitacionDTO);
        }
  catch (Exception ex)
     {
            TempData["Error"] = $"Error al cargar la habitación: {ex.Message}";
    return RedirectToAction(nameof(Index));
      }
    }

    // POST: Habitaciones/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador,Gerente")]
    public async Task<IActionResult> Edit(int id, HabitacionDTO habitacionDTO)
    {
        if (id != habitacionDTO.Id)
        {
      TempData["Error"] = "ID de habitación no válido.";
         return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
   {
            await CargarHotelesEnViewBag();
          return View(habitacionDTO);
   }

        try
        {
            // Validar que el número de habitación sea único (excepto para esta habitación)
            var habitaciones = await _unitOfWork.Habitaciones.GetAllAsync();
      if (habitaciones.Any(h => h.Numero == habitacionDTO.Numero && h.IdHotel == habitacionDTO.IdHotel && h.Id != id))
         {
  ModelState.AddModelError("Numero", "Ya existe una habitación con este número en el hotel seleccionado.");
 await CargarHotelesEnViewBag();
     return View(habitacionDTO);
  }

         // Convertir DTO a Entidad
 var habitacion = _mapper.Map<Habitacion>(habitacionDTO);
            
            // Actualizar
       _unitOfWork.Habitaciones.Update(habitacion);
            await _unitOfWork.CommitAsync();

  TempData["Success"] = "Habitación actualizada exitosamente.";
          return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error al actualizar la habitación: {ex.Message}");
     await CargarHotelesEnViewBag();
            return View(habitacionDTO);
      }
    }

    // GET: Habitaciones/Delete/5
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var habitacion = await _unitOfWork.Habitaciones.GetByIdAsync(id);
      
    if (habitacion == null)
        {
      TempData["Error"] = "La habitación no fue encontrada.";
       return RedirectToAction(nameof(Index));
            }

var habitacionDTO = _mapper.Map<HabitacionDTO>(habitacion);
     return View(habitacionDTO);
        }
  catch (Exception ex)
  {
        TempData["Error"] = $"Error al cargar la habitación: {ex.Message}";
        return RedirectToAction(nameof(Index));
        }
    }

    // POST: Habitaciones/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")]
  public async Task<IActionResult> DeleteConfirmed(int id)
  {
        try
        {
     var habitacion = await _unitOfWork.Habitaciones.GetByIdAsync(id);
    
            if (habitacion == null)
            {
  TempData["Error"] = "La habitación no fue encontrada.";
          return RedirectToAction(nameof(Index));
  }

         _unitOfWork.Habitaciones.Delete(habitacion);
    await _unitOfWork.CommitAsync();

            TempData["Success"] = "Habitación eliminada exitosamente.";
   return RedirectToAction(nameof(Index));
        }
      catch (Exception ex)
      {
  TempData["Error"] = $"Error al eliminar la habitación: {ex.Message}. Verifique que no tenga reservas asociadas.";
            return RedirectToAction(nameof(Delete), new { id });
        }
    }

    // Método auxiliar para cargar hoteles en el ViewBag
    private async Task CargarHotelesEnViewBag()
    {
        var hoteles = await _unitOfWork.Hoteles.GetAllAsync();
        ViewBag.Hoteles = new SelectList(hoteles.OrderBy(h => h.Nombre), "Id", "Nombre");
    }

    // Método auxiliar para cargar datos de filtros
    private async Task CargarDatosFiltros()
    {
        var hoteles = await _unitOfWork.Hoteles.GetAllAsync();
        ViewBag.HotelesFiltro = new SelectList(hoteles.OrderBy(h => h.Nombre), "Id", "Nombre");

    // Tipos de habitación predefinidos
        ViewBag.TiposHabitacion = new List<string>
        {
     "Individual",
            "Doble",
            "Matrimonial",
"Suite",
   "Suite Junior",
       "Suite Ejecutiva",
  "Suite Presidencial",
    "Familiar",
  "Deluxe"
};

   // Estados predefinidos
        ViewBag.Estados = new List<string>
        {
         "Disponible",
"Ocupada",
   "Reservada",
    "Mantenimiento",
     "Fuera de Servicio"
};

        // Tipos de cama
        ViewBag.TiposCama = new List<string>
        {
   "Individual",
       "Doble",
     "Queen",
 "King",
    "Dos Individuales",
  "Dos Dobles",
   "Litera"
 };

  // Capacidades comunes
     ViewBag.Capacidades = new List<int> { 1, 2, 3, 4, 5, 6 };
    }
}
