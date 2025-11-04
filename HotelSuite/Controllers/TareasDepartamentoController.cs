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

[Authorize] // Requiere autenticación
public class TareasDepartamentoController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private const int PageSize = 15;

    public TareasDepartamentoController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // GET: TareasDepartamento
    public async Task<IActionResult> Index(int? idDepartamento, string? filtroEstado, int? pagina)
 {
        try
        {
var tareas = await _unitOfWork.TareasDepartamento.GetAllAsync();

      // Filtrar por departamento
        if (idDepartamento.HasValue && idDepartamento.Value > 0)
 {
     tareas = tareas.Where(t => t.IdDepartamento == idDepartamento.Value);
      ViewBag.IdDepartamento = idDepartamento.Value;
            }

      // Filtrar por estado
     if (!string.IsNullOrWhiteSpace(filtroEstado))
    {
        tareas = tareas.Where(t => t.Estado == filtroEstado);
            ViewBag.FiltroEstado = filtroEstado;
   }

       // Ordenar por fecha de asignación descendente
      tareas = tareas.OrderByDescending(t => t.FechaAsignacion);

  // Convertir a DTO
          var tareasDTO = _mapper.Map<IEnumerable<TareaDepartamentoDTO>>(tareas);

         // Aplicar paginación
            int numeroPagina = pagina ?? 1;
    var tareasPaginadas = tareasDTO.ToPagedList(numeroPagina, PageSize);

     // Cargar departamentos para el filtro
await CargarDepartamentos();

    return View(tareasPaginadas);
        }
   catch (Exception ex)
   {
       TempData["Error"] = $"Error al cargar las tareas: {ex.Message}";
return View(new List<TareaDepartamentoDTO>().ToPagedList(1, PageSize));
        }
 }

    // GET: TareasDepartamento/TableroTareas (Vista con AJAX)
 public async Task<IActionResult> TableroTareas(int? idDepartamento)
    {
        try
    {
    var tareas = await _unitOfWork.TareasDepartamento.GetAllAsync();

     if (idDepartamento.HasValue && idDepartamento.Value > 0)
   {
      tareas = tareas.Where(t => t.IdDepartamento == idDepartamento.Value);
      ViewBag.IdDepartamento = idDepartamento.Value;
            }

   tareas = tareas.OrderByDescending(t => t.FechaAsignacion);
       var tareasDTO = _mapper.Map<IEnumerable<TareaDepartamentoDTO>>(tareas);

            // Agrupar por estado
         ViewBag.TareasPendientes = tareasDTO.Where(t => t.Estado == "Pendiente").ToList();
     ViewBag.TareasEnProceso = tareasDTO.Where(t => t.Estado == "En proceso").ToList();
      ViewBag.TareasCompletadas = tareasDTO.Where(t => t.Estado == "Completada").ToList();

            await CargarDepartamentos();

     return View();
   }
        catch (Exception ex)
        {
      TempData["Error"] = $"Error al cargar el tablero de tareas: {ex.Message}";
          return RedirectToAction(nameof(Index));
      }
    }

    // GET: TareasDepartamento/Details/5
    public async Task<IActionResult> Details(int id)
    {
     try
        {
     var tarea = await _unitOfWork.TareasDepartamento.GetByIdAsync(id);

   if (tarea == null)
            {
           TempData["Error"] = "La tarea no fue encontrada.";
              return RedirectToAction(nameof(Index));
            }

            var tareaDTO = _mapper.Map<TareaDepartamentoDTO>(tarea);

     // Obtener información del departamento
  var departamento = await _unitOfWork.Departamentos.GetByIdAsync(tarea.IdDepartamento);
            if (departamento != null)
            {
   ViewBag.Departamento = _mapper.Map<DepartamentoDTO>(departamento);
       }

  // Obtener información del empleado asignado si existe
            if (tarea.IdEmpleadoAsignado.HasValue)
   {
       var empleado = await _unitOfWork.Empleados.GetByIdAsync(tarea.IdEmpleadoAsignado.Value);
                if (empleado != null)
   {
ViewBag.Empleado = _mapper.Map<EmpleadoDTO>(empleado);
        }
        }

            return View(tareaDTO);
        }
        catch (Exception ex)
    {
            TempData["Error"] = $"Error al cargar los detalles de la tarea: {ex.Message}";
        return RedirectToAction(nameof(Index));
 }
    }

    // GET: TareasDepartamento/Create
    public async Task<IActionResult> Create()
    {
        await CargarDepartamentosYEmpleados();
   
        var tareaDTO = new TareaDepartamentoDTO
        {
    FechaAsignacion = DateTime.Now,
            Estado = "Pendiente"
   };

        return View(tareaDTO);
    }

    // POST: TareasDepartamento/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador,Gerente")] // Solo admin y gerente crean tareas
    public async Task<IActionResult> Create(TareaDepartamentoDTO tareaDto)
    {
        if (!ModelState.IsValid)
        {
   await CargarDepartamentosYEmpleados();
         return View(tareaDto);
        }

        try
        {
// Convertir DTO a Entidad
   var tarea = _mapper.Map<TareaDepartamento>(tareaDto);
            tarea.FechaAsignacion = DateTime.Now;
       tarea.Estado = "Pendiente";

      // Guardar
   await _unitOfWork.TareasDepartamento.AddAsync(tarea);
            await _unitOfWork.CommitAsync();

 TempData["Success"] = "Tarea creada exitosamente.";
            return RedirectToAction(nameof(TableroTareas));
        }
        catch (Exception ex)
        {
       ModelState.AddModelError("", $"Error al crear la tarea: {ex.Message}");
   await CargarDepartamentosYEmpleados();
    return View(tareaDto);
  }
    }

    // GET: TareasDepartamento/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
      var tarea = await _unitOfWork.TareasDepartamento.GetByIdAsync(id);

   if (tarea == null)
{
   TempData["Error"] = "La tarea no fue encontrada.";
       return RedirectToAction(nameof(Index));
            }

            var tareaDTO = _mapper.Map<TareaDepartamentoDTO>(tarea);
    await CargarDepartamentosYEmpleados(tarea.IdDepartamento);

            return View(tareaDTO);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error al cargar la tarea: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: TareasDepartamento/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TareaDepartamentoDTO tareaDTO)
    {
        if (id != tareaDTO.Id)
        {
         TempData["Error"] = "ID de tarea no válido.";
   return RedirectToAction(nameof(Index));
  }

        if (!ModelState.IsValid)
        {
   await CargarDepartamentosYEmpleados(tareaDTO.IdDepartamento);
       return View(tareaDTO);
     }

        try
      {
   var tarea = _mapper.Map<TareaDepartamento>(tareaDTO);

   // Si se completa la tarea, actualizar fecha de finalización
       if (tarea.Estado == "Completada" && !tarea.FechaFinalizacion.HasValue)
       {
            tarea.FechaFinalizacion = DateTime.Now;
            }

  _unitOfWork.TareasDepartamento.Update(tarea);
       await _unitOfWork.CommitAsync();

   TempData["Success"] = "Tarea actualizada exitosamente.";
    return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
   {
            ModelState.AddModelError("", $"Error al actualizar la tarea: {ex.Message}");
      await CargarDepartamentosYEmpleados(tareaDTO.IdDepartamento);
            return View(tareaDTO);
        }
    }

    // POST: TareasDepartamento/ActualizarEstado (AJAX)
    [HttpPost]
    public async Task<IActionResult> ActualizarEstado([FromBody] ActualizarEstadoRequest request)
    {
        try
        {
     var tarea = await _unitOfWork.TareasDepartamento.GetByIdAsync(request.Id);

if (tarea == null)
          {
   return Json(new { success = false, message = "Tarea no encontrada" });
         }

            // Actualizar estado
       tarea.Estado = request.NuevoEstado;

            // Si se completa, actualizar fecha de finalización
            if (request.NuevoEstado == "Completada" && !tarea.FechaFinalizacion.HasValue)
            {
              tarea.FechaFinalizacion = DateTime.Now;
            }

_unitOfWork.TareasDepartamento.Update(tarea);
  await _unitOfWork.CommitAsync();

  // Obtener información actualizada
  var tareaDTO = _mapper.Map<TareaDepartamentoDTO>(tarea);

          return Json(new
         {
           success = true,
  message = "Estado actualizado correctamente",
                tarea = new
       {
       tareaDTO.Id,
           tareaDTO.Titulo,
         tareaDTO.Estado,
  tareaDTO.Prioridad,
      tareaDTO.NombreDepartamento,
                  tareaDTO.NombreEmpleado,
   FechaAsignacion = tareaDTO.FechaAsignacion.ToString("dd/MM/yyyy"),
           FechaFinalizacion = tareaDTO.FechaFinalizacion?.ToString("dd/MM/yyyy")
     }
     });
        }
 catch (Exception ex)
      {
            return Json(new { success = false, message = $"Error: {ex.Message}" });
     }
    }

    // GET: TareasDepartamento/ObtenerTarea/5 (AJAX)
    [HttpGet]
    public async Task<IActionResult> ObtenerTarea(int id)
    {
  try
        {
     var tarea = await _unitOfWork.TareasDepartamento.GetByIdAsync(id);

            if (tarea == null)
            {
      return Json(new { success = false, message = "Tarea no encontrada" });
        }

            var tareaDTO = _mapper.Map<TareaDepartamentoDTO>(tarea);

            return Json(new
            {
            success = true,
       tarea = new
   {
        tareaDTO.Id,
   tareaDTO.Titulo,
   tareaDTO.Descripcion,
           tareaDTO.Estado,
          tareaDTO.Prioridad,
            tareaDTO.NombreDepartamento,
           tareaDTO.NombreEmpleado,
     FechaAsignacion = tareaDTO.FechaAsignacion.ToString("dd/MM/yyyy HH:mm"),
            FechaFinalizacion = tareaDTO.FechaFinalizacion?.ToString("dd/MM/yyyy HH:mm")
   }
  });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
     }
    }

    // GET: TareasDepartamento/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
    try
        {
            var tarea = await _unitOfWork.TareasDepartamento.GetByIdAsync(id);

       if (tarea == null)
     {
      TempData["Error"] = "La tarea no fue encontrada.";
     return RedirectToAction(nameof(Index));
            }

            var tareaDTO = _mapper.Map<TareaDepartamentoDTO>(tarea);
       return View(tareaDTO);
     }
        catch (Exception ex)
        {
         TempData["Error"] = $"Error al cargar la tarea: {ex.Message}";
     return RedirectToAction(nameof(Index));
        }
    }

    // POST: Tareas Departamento/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
 {
     var tarea = await _unitOfWork.TareasDepartamento.GetByIdAsync(id);

  if (tarea == null)
 {
              TempData["Error"] = "La tarea no fue encontrada.";
        return RedirectToAction(nameof(Index));
            }

_unitOfWork.TareasDepartamento.Delete(tarea);
            await _unitOfWork.CommitAsync();

            TempData["Success"] = "Tarea eliminada exitosamente.";
      return RedirectToAction(nameof(Index));
   }
        catch (Exception ex)
        {
        TempData["Error"] = $"Error al eliminar la tarea: {ex.Message}";
            return RedirectToAction(nameof(Delete), new { id });
        }
    }

    // Métodos auxiliares
    private async Task CargarDepartamentos()
    {
        var departamentos = await _unitOfWork.Departamentos.GetAllAsync();
        ViewBag.Departamentos = new SelectList(departamentos.OrderBy(d => d.Nombre), "Id", "Nombre");
    }

    private async Task CargarDepartamentosYEmpleados(int? idDepartamento = null)
    {
   var departamentos = await _unitOfWork.Departamentos.GetAllAsync();
  ViewBag.Departamentos = new SelectList(departamentos.OrderBy(d => d.Nombre), "Id", "Nombre");

        var empleados = await _unitOfWork.Empleados.GetAllAsync();
        
   // Si hay un departamento seleccionado, filtrar empleados
        if (idDepartamento.HasValue)
        {
            empleados = empleados.Where(e => e.IdDepartamento == idDepartamento.Value);
        }

        ViewBag.Empleados = new SelectList(
            empleados.OrderBy(e => e.Apellidos).ThenBy(e => e.Nombres),
     "Id",
      "Nombres"
        );
    }

    // Clase auxiliar para la solicitud AJAX
    public class ActualizarEstadoRequest
    {
        public int Id { get; set; }
        public string NuevoEstado { get; set; } = string.Empty;
    }
}
