namespace HotelSuite.Application.DTOs.API;

/// <summary>
/// Respuesta estándar de la API
/// </summary>
/// <typeparam name="T">Tipo de datos</typeparam>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResponse<T> SuccessResponse(T data, string message = "Operación exitosa")
    {
 return new ApiResponse<T>
 {
     Success = true,
  Message = message,
          Data = data
        };
    }

    public static ApiResponse<T> ErrorResponse(string message, List<string>? errors = null)
    {
   return new ApiResponse<T>
        {
   Success = false,
   Message = message,
    Errors = errors ?? new List<string>()
   };
    }
}

/// <summary>
/// Respuesta paginada de la API
/// </summary>
public class PagedApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<T> Data { get; set; } = new();
    public PaginationMetadata Pagination { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class PaginationMetadata
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
  public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public bool HasPrevious { get; set; }
    public bool HasNext { get; set; }
}
