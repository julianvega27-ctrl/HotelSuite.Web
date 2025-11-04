using AutoMapper;
using HotelSuite.Application.DTOs;
using HotelSuite.Domain.Entities;
using HotelSuite.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelSuite.Controllers;

/// <summary>
/// Controlador de ejemplo que muestra cómo usar AutoMapper con el patrón Repository
/// </summary>
public class HotelesController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
  private readonly IMapper _mapper;

    public HotelesController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // GET: Hoteles
    public async Task<IActionResult> Index()
    {
        var hoteles = await _unitOfWork.Hoteles.GetAllAsync();
        var hotelesDTO = _mapper.Map<IEnumerable<HotelDTO>>(hoteles);
        return View(hotelesDTO);
    }

    // GET: Hoteles/Details/5
    public async Task<IActionResult> Details(int id)
    {
   var hotel = await _unitOfWork.Hoteles.GetByIdAsync(id);
        
        if (hotel == null)
        {
       return NotFound();
        }

        var hotelDTO = _mapper.Map<HotelDTO>(hotel);
     return View(hotelDTO);
  }

    // GET: Hoteles/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Hoteles/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(HotelDTO hotelDTO)
    {
        if (!ModelState.IsValid)
    {
  return View(hotelDTO);
    }

try
        {
            // Convertir DTO a Entidad
            var hotel = _mapper.Map<Hotel>(hotelDTO);
    
          // Guardar en la base de datos
            await _unitOfWork.Hoteles.AddAsync(hotel);
            await _unitOfWork.CommitAsync();

       TempData["Success"] = "Hotel creado exitosamente";
    return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
        ModelState.AddModelError("", $"Error al crear el hotel: {ex.Message}");
            return View(hotelDTO);
        }
    }

    // GET: Hoteles/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
 var hotel = await _unitOfWork.Hoteles.GetByIdAsync(id);
        
        if (hotel == null)
 {
       return NotFound();
        }

        var hotelDTO = _mapper.Map<HotelDTO>(hotel);
    return View(hotelDTO);
    }

    // POST: Hoteles/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, HotelDTO hotelDTO)
    {
      if (id != hotelDTO.Id)
   {
            return BadRequest();
}

 if (!ModelState.IsValid)
        {
            return View(hotelDTO);
        }

      try
        {
            // Convertir DTO a Entidad
            var hotel = _mapper.Map<Hotel>(hotelDTO);
     
          // Actualizar
        _unitOfWork.Hoteles.Update(hotel);
            await _unitOfWork.CommitAsync();

   TempData["Success"] = "Hotel actualizado exitosamente";
            return RedirectToAction(nameof(Index));
        }
     catch (Exception ex)
 {
            ModelState.AddModelError("", $"Error al actualizar el hotel: {ex.Message}");
            return View(hotelDTO);
        }
    }

    // GET: Hoteles/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
   var hotel = await _unitOfWork.Hoteles.GetByIdAsync(id);
        
  if (hotel == null)
        {
        return NotFound();
        }

      var hotelDTO = _mapper.Map<HotelDTO>(hotel);
        return View(hotelDTO);
    }

    // POST: Hoteles/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
   try
   {
            var hotel = await _unitOfWork.Hoteles.GetByIdAsync(id);
    
       if (hotel == null)
 {
           return NotFound();
            }

      _unitOfWork.Hoteles.Delete(hotel);
   await _unitOfWork.CommitAsync();

         TempData["Success"] = "Hotel eliminado exitosamente";
   return RedirectToAction(nameof(Index));
        }
 catch (Exception ex)
        {
            TempData["Error"] = $"Error al eliminar el hotel: {ex.Message}";
       return RedirectToAction(nameof(Delete), new { id });
    }
    }
}
