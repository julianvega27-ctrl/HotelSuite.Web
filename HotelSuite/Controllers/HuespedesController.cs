using AutoMapper;
using HotelSuite.Application.DTOs;
using HotelSuite.Domain.Entities;
using HotelSuite.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace HotelSuite.Controllers;

[Authorize(Roles = "Administrador,Gerente,Recepcionista")]
public class HuespedesController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private const int PageSize = 10;

    public HuespedesController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
      _mapper = mapper;
    }

    // GET: Huespedes
    public async Task<IActionResult> Index(string? busqueda, int? pagina)
    {
        try
        {
      var huespedes = await _unitOfWork.Huespedes.GetAllAsync();

            // Aplicar búsqueda
          if (!string.IsNullOrWhiteSpace(busqueda))
       {
      huespedes = huespedes.Where(h =>
     h.Nombres.Contains(busqueda, StringComparison.OrdinalIgnoreCase) ||
       h.Apellidos.Contains(busqueda, StringComparison.OrdinalIgnoreCase) ||
        h.Email.Contains(busqueda, StringComparison.OrdinalIgnoreCase) ||
         h.DocumentoIdentidad.Contains(busqueda, StringComparison.OrdinalIgnoreCase));
      ViewBag.Busqueda = busqueda;
            }

      // Ordenar por apellidos
       huespedes = huespedes.OrderBy(h => h.Apellidos).ThenBy(h => h.Nombres);

            // Convertir a DTO
            var huespedesDTO = _mapper.Map<IEnumerable<HuespedDTO>>(huespedes);

        // Aplicar paginación
      int numeroPagina = pagina ?? 1;
            var huespedesPaginados = huespedesDTO.ToPagedList(numeroPagina, PageSize);

          return View(huespedesPaginados);
        }
   catch (Exception ex)
        {
   TempData["Error"] = $"Error al cargar los huéspedes: {ex.Message}";
return View(new List<HuespedDTO>().ToPagedList(1, PageSize));
        }
    }

    // GET: Huespedes/Details/5
    public async Task<IActionResult> Details(int id)
    {
   try
        {
            var huesped = await _unitOfWork.Huespedes
                .GetAllQueryable()
       .Include(h => h.Reservas)
    .ThenInclude(r => r.Habitacion)
            .FirstOrDefaultAsync(h => h.Id == id);

     if (huesped == null)
            {
    TempData["Error"] = "El huésped no fue encontrado.";
           return RedirectToAction(nameof(Index));
            }

         var huespedDTO = _mapper.Map<HuespedDTO>(huesped);
 return View(huespedDTO);
        }
  catch (Exception ex)
        {
     TempData["Error"] = $"Error al cargar los detalles del huésped: {ex.Message}";
 return RedirectToAction(nameof(Index));
        }
    }

    // GET: Huespedes/Create
    public IActionResult Create()
    {
    return View(new HuespedDTO());
    }

    // POST: Huespedes/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(HuespedDTO huespedDTO)
    {
        if (!ModelState.IsValid)
        {
            return View(huespedDTO);
        }

    try
   {
            // Validar que el email no exista
   var huespedes = await _unitOfWork.Huespedes.GetAllAsync();
     if (huespedes.Any(h => h.Email.Equals(huespedDTO.Email, StringComparison.OrdinalIgnoreCase)))
          {
          ModelState.AddModelError("Email", "Ya existe un huésped con este email.");
             return View(huespedDTO);
       }

 // Validar que el documento no exista
            if (huespedes.Any(h => h.DocumentoIdentidad.Equals(huespedDTO.DocumentoIdentidad, StringComparison.OrdinalIgnoreCase)))
            {
 ModelState.AddModelError("DocumentoIdentidad", "Ya existe un huésped con este documento de identidad.");
       return View(huespedDTO);
}

            var huesped = _mapper.Map<Huesped>(huespedDTO);
            await _unitOfWork.Huespedes.AddAsync(huesped);
    await _unitOfWork.CommitAsync();

            TempData["Success"] = "Huésped registrado exitosamente.";
  return RedirectToAction(nameof(Details), new { id = huesped.Id });
        }
      catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error al registrar el huésped: {ex.Message}");
       return View(huespedDTO);
        }
    }

    // POST: Huespedes/CreateAjax (Para el modal en Reservas)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAjax([FromBody] HuespedDTO huespedDTO)
    {
      try
        {
      if (!ModelState.IsValid)
            {
         var errors = ModelState.Values.SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
        .ToList();
      return Json(new { success = false, message = string.Join(", ", errors) });
   }

     // Validar que el email no exista
          var huespedes = await _unitOfWork.Huespedes.GetAllAsync();
            if (huespedes.Any(h => h.Email.Equals(huespedDTO.Email, StringComparison.OrdinalIgnoreCase)))
            {
    return Json(new { success = false, message = "Ya existe un huésped con este email." });
   }

          // Validar que el documento no exista
            if (huespedes.Any(h => h.DocumentoIdentidad.Equals(huespedDTO.DocumentoIdentidad, StringComparison.OrdinalIgnoreCase)))
  {
       return Json(new { success = false, message = "Ya existe un huésped con este documento de identidad." });
       }

  var huesped = _mapper.Map<Huesped>(huespedDTO);
     await _unitOfWork.Huespedes.AddAsync(huesped);
            await _unitOfWork.CommitAsync();

       return Json(new
  {
       success = true,
              message = "Huésped registrado exitosamente.",
     huesped = new
   {
     id = huesped.Id,
   nombreCompleto = $"{huesped.Nombres} {huesped.Apellidos}"
      }
        });
        }
        catch (Exception ex)
        {
        return Json(new { success = false, message = $"Error al registrar el huésped: {ex.Message}" });
        }
    }

    // GET: Huespedes/Edit/5
    public async Task<IActionResult> Edit(int id)
{
    try
     {
            var huesped = await _unitOfWork.Huespedes.GetByIdAsync(id);

       if (huesped == null)
  {
       TempData["Error"] = "El huésped no fue encontrado.";
                return RedirectToAction(nameof(Index));
    }

  var huespedDTO = _mapper.Map<HuespedDTO>(huesped);
            return View(huespedDTO);
     }
        catch (Exception ex)
  {
        TempData["Error"] = $"Error al cargar el huésped: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
 }

    // POST: Huespedes/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, HuespedDTO huespedDTO)
    {
      if (id != huespedDTO.Id)
   {
       TempData["Error"] = "ID de huésped no válido.";
            return RedirectToAction(nameof(Index));
   }

        if (!ModelState.IsValid)
  {
          return View(huespedDTO);
        }

        try
        {
     // Validar que el email no exista (excluyendo el actual)
var huespedes = await _unitOfWork.Huespedes.GetAllAsync();
            if (huespedes.Any(h => h.Id != id && h.Email.Equals(huespedDTO.Email, StringComparison.OrdinalIgnoreCase)))
      {
        ModelState.AddModelError("Email", "Ya existe otro huésped con este email.");
      return View(huespedDTO);
 }

    // Validar que el documento no exista (excluyendo el actual)
            if (huespedes.Any(h => h.Id != id && h.DocumentoIdentidad.Equals(huespedDTO.DocumentoIdentidad, StringComparison.OrdinalIgnoreCase)))
            {
   ModelState.AddModelError("DocumentoIdentidad", "Ya existe otro huésped con este documento de identidad.");
       return View(huespedDTO);
       }

     var huesped = _mapper.Map<Huesped>(huespedDTO);
_unitOfWork.Huespedes.Update(huesped);
   await _unitOfWork.CommitAsync();

            TempData["Success"] = "Huésped actualizado exitosamente.";
return RedirectToAction(nameof(Details), new { id });
  }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error al actualizar el huésped: {ex.Message}");
            return View(huespedDTO);
      }
    }

 // GET: Huespedes/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        try
 {
 var huesped = await _unitOfWork.Huespedes
    .GetAllQueryable()
   .Include(h => h.Reservas)
      .FirstOrDefaultAsync(h => h.Id == id);

    if (huesped == null)
     {
      TempData["Error"] = "El huésped no fue encontrado.";
            return RedirectToAction(nameof(Index));
     }

    var huespedDTO = _mapper.Map<HuespedDTO>(huesped);
   ViewBag.TieneReservas = huesped.Reservas.Any();
    return View(huespedDTO);
        }
        catch (Exception ex)
        {
 TempData["Error"] = $"Error al cargar el huésped: {ex.Message}";
         return RedirectToAction(nameof(Index));
        }
    }

    // POST: Huespedes/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
    {
            var huesped = await _unitOfWork.Huespedes
       .GetAllQueryable()
           .Include(h => h.Reservas)
         .FirstOrDefaultAsync(h => h.Id == id);

if (huesped == null)
    {
            TempData["Error"] = "El huésped no fue encontrado.";
           return RedirectToAction(nameof(Index));
            }

            // Verificar que no tenga reservas activas
            if (huesped.Reservas.Any(r => r.Estado != "Cancelada"))
            {
     TempData["Error"] = "No se puede eliminar el huésped porque tiene reservas activas.";
              return RedirectToAction(nameof(Delete), new { id });
  }

            _unitOfWork.Huespedes.Delete(huesped);
            await _unitOfWork.CommitAsync();

      TempData["Success"] = "Huésped eliminado exitosamente.";
        return RedirectToAction(nameof(Index));
}
        catch (Exception ex)
        {
   TempData["Error"] = $"Error al eliminar el huésped: {ex.Message}";
  return RedirectToAction(nameof(Delete), new { id });
        }
    }
}
