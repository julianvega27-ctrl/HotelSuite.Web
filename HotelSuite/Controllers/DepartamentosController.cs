using AutoMapper;
using HotelSuite.Application.DTOs;
using HotelSuite.Domain.Entities;
using HotelSuite.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace HotelSuite.Controllers;

[Authorize]
public class DepartamentosController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private const int PageSize = 10;

    public DepartamentosController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // GET: Departamentos
    public async Task<IActionResult> Index(string? busqueda, int? pagina)
    {
    try
      {
    var departamentos = await _unitOfWork.Departamentos.GetAllAsync();

      // Aplicar búsqueda
 if (!string.IsNullOrWhiteSpace(busqueda))
  {
        departamentos = departamentos.Where(d => 
   d.Nombre.Contains(busqueda, StringComparison.OrdinalIgnoreCase) ||
          (d.Descripcion != null && d.Descripcion.Contains(busqueda, StringComparison.OrdinalIgnoreCase))
             );
         ViewBag.Busqueda = busqueda;
  }

      // Ordenar alfabéticamente
 departamentos = departamentos.OrderBy(d => d.Nombre);

    // Convertir a DTO
          var departamentosDTO = _mapper.Map<IEnumerable<DepartamentoDTO>>(departamentos);

            // Aplicar paginación
  int numeroPagina = pagina ?? 1;
            var departamentosPaginados = departamentosDTO.ToPagedList(numeroPagina, PageSize);

          return View(departamentosPaginados);
        }
   catch (Exception ex)
        {
            TempData["Error"] = $"Error al cargar los departamentos: {ex.Message}";
        return View(new List<DepartamentoDTO>().ToPagedList(1, PageSize));
        }
 }

    // GET: Departamentos/Details/5
    public async Task<IActionResult> Details(int id)
    {
        try
        {
    var departamento = await _unitOfWork.Departamentos.GetByIdAsync(id);

          if (departamento == null)
     {
         TempData["Error"] = "El departamento no fue encontrado.";
      return RedirectToAction(nameof(Index));
            }

            var departamentoDTO = _mapper.Map<DepartamentoDTO>(departamento);

    // Obtener estadísticas de empleados
            var empleados = await _unitOfWork.Empleados.GetAllAsync();
       var empleadosDepartamento = empleados.Where(e => e.IdDepartamento == id);
          ViewBag.TotalEmpleados = empleadosDepartamento.Count();

            // Obtener estadísticas de tareas
     var tareas = await _unitOfWork.TareasDepartamento.GetAllAsync();
            var tareasDepartamento = tareas.Where(t => t.IdDepartamento == id);
  ViewBag.TotalTareas = tareasDepartamento.Count();
     ViewBag.TareasPendientes = tareasDepartamento.Count(t => t.Estado == "Pendiente");
            ViewBag.TareasEnProceso = tareasDepartamento.Count(t => t.Estado == "En proceso");
          ViewBag.TareasCompletadas = tareasDepartamento.Count(t => t.Estado == "Completada");

   return View(departamentoDTO);
        }
        catch (Exception ex)
        {
   TempData["Error"] = $"Error al cargar los detalles del departamento: {ex.Message}";
 return RedirectToAction(nameof(Index));
        }
    }

    // GET: Departamentos/Create
    public IActionResult Create()
    {
     return View();
    }

    // POST: Departamentos/Create
    [HttpPost]
 [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DepartamentoDTO departamentoDTO)
    {
        if (!ModelState.IsValid)
        {
   return View(departamentoDTO);
        }

        try
        {
// Validar que el nombre sea único
  var departamentos = await _unitOfWork.Departamentos.GetAllAsync();
     if (departamentos.Any(d => d.Nombre.Equals(departamentoDTO.Nombre, StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError("Nombre", "Ya existe un departamento con este nombre.");
     return View(departamentoDTO);
     }

            // Convertir DTO a Entidad
var departamento = _mapper.Map<Departamento>(departamentoDTO);

      // Guardar
        await _unitOfWork.Departamentos.AddAsync(departamento);
          await _unitOfWork.CommitAsync();

       TempData["Success"] = "Departamento creado exitosamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error al crear el departamento: {ex.Message}");
            return View(departamentoDTO);
        }
    }

    // GET: Departamentos/Edit/5
 public async Task<IActionResult> Edit(int id)
    {
  try
        {
            var departamento = await _unitOfWork.Departamentos.GetByIdAsync(id);

      if (departamento == null)
            {
TempData["Error"] = "El departamento no fue encontrado.";
     return RedirectToAction(nameof(Index));
       }

        var departamentoDTO = _mapper.Map<DepartamentoDTO>(departamento);
            return View(departamentoDTO);
     }
  catch (Exception ex)
        {
        TempData["Error"] = $"Error al cargar el departamento: {ex.Message}";
            return RedirectToAction(nameof(Index));
    }
    }

    // POST: Departamentos/Edit/5
  [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, DepartamentoDTO departamentoDTO)
    {
        if (id != departamentoDTO.Id)
        {
            TempData["Error"] = "ID de departamento no válido.";
return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            return View(departamentoDTO);
        }

        try
        {
    // Validar que el nombre sea único (excepto para este departamento)
  var departamentos = await _unitOfWork.Departamentos.GetAllAsync();
       if (departamentos.Any(d => d.Nombre.Equals(departamentoDTO.Nombre, StringComparison.OrdinalIgnoreCase) && d.Id != id))
  {
     ModelState.AddModelError("Nombre", "Ya existe un departamento con este nombre.");
  return View(departamentoDTO);
     }

   // Convertir DTO a Entidad
            var departamento = _mapper.Map<Departamento>(departamentoDTO);

      // Actualizar
     _unitOfWork.Departamentos.Update(departamento);
await _unitOfWork.CommitAsync();

            TempData["Success"] = "Departamento actualizado exitosamente.";
     return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
   {
            ModelState.AddModelError("", $"Error al actualizar el departamento: {ex.Message}");
            return View(departamentoDTO);
    }
    }

    // GET: Departamentos/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
      var departamento = await _unitOfWork.Departamentos.GetByIdAsync(id);

            if (departamento == null)
        {
       TempData["Error"] = "El departamento no fue encontrado.";
    return RedirectToAction(nameof(Index));
   }

   var departamentoDTO = _mapper.Map<DepartamentoDTO>(departamento);

            // Obtener empleados y tareas relacionadas
        var empleados = await _unitOfWork.Empleados.GetAllAsync();
            var tareas = await _unitOfWork.TareasDepartamento.GetAllAsync();
         ViewBag.TotalEmpleados = empleados.Count(e => e.IdDepartamento == id);
            ViewBag.TotalTareas = tareas.Count(t => t.IdDepartamento == id);

 return View(departamentoDTO);
        }
        catch (Exception ex)
  {
            TempData["Error"] = $"Error al cargar el departamento: {ex.Message}";
      return RedirectToAction(nameof(Index));
}
    }

    // POST: Departamentos/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      try
        {
            var departamento = await _unitOfWork.Departamentos.GetByIdAsync(id);

  if (departamento == null)
{
  TempData["Error"] = "El departamento no fue encontrado.";
      return RedirectToAction(nameof(Index));
   }

            // Verificar que no tenga empleados o tareas asociadas
            var empleados = await _unitOfWork.Empleados.GetAllAsync();
     var tareas = await _unitOfWork.TareasDepartamento.GetAllAsync();

         if (empleados.Any(e => e.IdDepartamento == id))
   {
       TempData["Error"] = "No se puede eliminar el departamento porque tiene empleados asociados.";
              return RedirectToAction(nameof(Delete), new { id });
       }

         if (tareas.Any(t => t.IdDepartamento == id))
      {
    TempData["Error"] = "No se puede eliminar el departamento porque tiene tareas asociadas.";
            return RedirectToAction(nameof(Delete), new { id });
            }

            _unitOfWork.Departamentos.Delete(departamento);
        await _unitOfWork.CommitAsync();

  TempData["Success"] = "Departamento eliminado exitosamente.";
            return RedirectToAction(nameof(Index));
     }
    catch (Exception ex)
    {
            TempData["Error"] = $"Error al eliminar el departamento: {ex.Message}";
      return RedirectToAction(nameof(Delete), new { id });
    }
    }
}
