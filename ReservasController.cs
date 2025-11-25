using AutoMapper;
using HotelSuite.Application.DTOs.API; // Asegúrate de tener este using
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using X.PagedList;

namespace HotelSuite.Controllers;

[Authorize(Roles = "Administrador,Gerente,Recepcionista")]
public class ReservasController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMapper _mapper;
    private const int PageSize = 10;

    // Inyectamos IHttpClientFactory en lugar de IUnitOfWork
    public ReservasController(IHttpClientFactory httpClientFactory, IMapper mapper)
    {
        _httpClientFactory = httpClientFactory;
        _mapper = mapper;
    }

    // GET: Reservas
    public async Task<IActionResult> Index(string? busquedaEstado, int? pagina)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("HotelSuiteApi");
            int numeroPagina = pagina ?? 1;

            // Construir la URL con los parámetros de consulta para la API
            var url = $"Reservas?pagina={numeroPagina}&tamanoPagina={PageSize}";
            if (!string.IsNullOrWhiteSpace(busquedaEstado))
            {
                url += $"&estado={busquedaEstado}";
                ViewBag.BusquedaEstado = busquedaEstado;
            }

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = $"Error al comunicarse con la API: {response.ReasonPhrase}";
                // Devuelve una lista vacía paginada en caso de error
                return View(new StaticPagedList<ReservaInfoDTO>(new List<ReservaInfoDTO>(), 1, PageSize, 0));
            }

            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var apiResponse = JsonSerializer.Deserialize<PagedApiResponse<ReservaInfoDTO>>(content, options);

            if (apiResponse == null || !apiResponse.Success)
            {
                TempData["Error"] = apiResponse?.Message ?? "Error al procesar la respuesta de la API.";
                return View(new StaticPagedList<ReservaInfoDTO>(new List<ReservaInfoDTO>(), 1, PageSize, 0));
            }

            // Creamos una lista paginada estática para que la vista la entienda
            var pagedList = new StaticPagedList<ReservaInfoDTO>(
                apiResponse.Data,
                apiResponse.Pagination.CurrentPage,
                apiResponse.Pagination.PageSize,
                apiResponse.Pagination.TotalCount
            );

            return View(pagedList);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error al cargar las reservas: {ex.Message}";
            return View(new StaticPagedList<ReservaInfoDTO>(new List<ReservaInfoDTO>(), 1, PageSize, 0));
        }
    }

    // ... El resto de tus métodos (Details, Create, Edit, etc.) pueden permanecer igual por ahora.
    // Idealmente, también deberían usar la API en el futuro para mantener la consistencia.
}
@model X.PagedList.IPagedList<HotelSuite.Application.DTOs.API.ReservaInfoDTO>
@using X.PagedList.Mvc.Core
@using X.PagedList.Web.Common

@{
    ViewData["Title"] = "Gestión de Reservas";
}

<div class="container-fluid py-4">
    <!-- Encabezado y Filtros (sin cambios) -->
    <!-- ... -->

    <!-- Tabla de reservas -->
    <div class="card border-0 shadow-sm">
        <div class="card-body p-0">
            @if (Model != null && Model.Any())
            {
                <div class="table-responsive">
                    <table class="table table-hover mb-0">
                        <thead class="table-light">
                            <tr>
                                <th class="text-center">ID</th>
                                <th>Huésped</th>
                                <th>Habitación</th>
                                <th class="text-center">Check-in</th>
                                <th class="text-center">Check-out</th>
                                <th class="text-center">Noches</th>
                                <th class="text-center">Estado</th>
                                <th class="text-center">Acciones</th>
                            </tr>
                        </thead>
                        <tbody>
                            @foreach (var reserva in Model)
                            {
                                <tr>
                                    <td class="text-center fw-bold text-primary">#@reserva.Id</td>
                                    <td>@reserva.Huesped?.NombreCompleto</td>
                                    <td>
                                        <span class="badge bg-info text-dark">
                                            #@reserva.Habitacion?.Numero - @reserva.Habitacion?.Tipo
                                        </span>
                                    </td>
                                    <td class="text-center">
                                        <small>@reserva.FechaEntrada.ToString("dd/MM/yyyy")</small>
                                    </td>
                                    <td class="text-center">
                                        <small>@reserva.FechaSalida.ToString("dd/MM/yyyy")</small>
                                    </td>
                                    <td class="text-center">
                                        <span class="badge bg-secondary">@reserva.DiasEstancia</span>
                                    </td>
                                    <td class="text-center">
                                        @{
                                            var estadoBadge = reserva.Estado?.ToLower() switch
                                            {
                                                var e when e.Contains("confirmada") => "success",
                                                var e when e.Contains("cancelada") => "danger",
                                                var e when e.Contains("finalizada") => "primary",
                                                var e when e.Contains("en curso") => "info",
                                                _ => "secondary"
                                            };
                                        }
                                        <span class="badge bg-@estadoBadge">@reserva.Estado</span>
                                    </td>
                                    <td class="text-center">
                                        <div class="btn-group btn-group-sm" role="group">
                                            <a asp-action="Details" asp-route-id="@reserva.Id" class="btn btn-outline-info" title="Ver Detalles">
                                                <i class="fas fa-eye"></i>
                                            </a>
                                            <a asp-action="Edit" asp-route-id="@reserva.Id" class="btn btn-outline-warning" title="Editar">
                                                <i class="fas fa-edit"></i>
                                            </a>
                                            @if (reserva.Estado != "Cancelada" && reserva.Estado != "Finalizada")
                                            {
                                                <a asp-action="Cancel" asp-route-id="@reserva.Id" class="btn btn-outline-danger" title="Cancelar">
                                                    <i class="fas fa-ban"></i>
                                                </a>
                                            }
                                        </div>
                                    </td>
                                </tr>
                            }
                        </tbody>
                    </table>
                </div>

                <!-- Paginación (sin cambios) -->
                <!-- ... -->
            }
            else
            {
                <!-- Mensaje de "No se encontraron reservas" (sin cambios) -->
                <!-- ... -->
            }
        </div>
    </div>
</div>