# ?? HotelSuite API REST - Documentación

## ?? Descripción

API REST completa para el sistema de gestión hotelera HotelSuite HMS, diseñada para integrarse con aplicaciones móviles, kioscos de check-in automático y sistemas externos.

## ? Características Principales

- ? **Consulta de disponibilidad** de habitaciones en tiempo real
- ? **Gestión de reservas** (crear, consultar, cancelar)
- ? **Check-in/Check-out automático** para kioscos
- ? **Documentación Swagger** interactiva
- ? **CORS habilitado** para aplicaciones móviles
- ? **Respuestas estandarizadas** con `ApiResponse<T>`
- ? **Paginación** en listados
- ? **Validación de datos** con Data Annotations
- ? **Logging** completo de operaciones

## ?? Acceso a la API

### **Base URL:**
```
https://localhost:5001/api
```

### **Documentación Swagger:**
```
https://localhost:5001/api/docs
```

## ?? Endpoints Disponibles

### **1. Habitaciones (`/api/habitaciones`)**

#### **GET /api/habitaciones/disponibles**
Obtiene habitaciones disponibles con filtros opcionales.

**Query Parameters:**
```csharp
?fechaEntrada=2025-01-01
&fechaSalida=2025-01-05
&tipo=Suite
&precioMaximo=5000
&idHotel=1
&pagina=1
&registrosPorPagina=10
```

**Response 200 OK:**
```json
{
  "success": true,
  "message": "Habitaciones obtenidas exitosamente",
  "data": [
    {
      "id": 1,
      "numero": "101",
      "tipo": "Suite",
"precioPorNoche": 3500.00,
"estado": "Disponible",
      "hotel": {
        "id": 1,
        "nombre": "Hotel Plaza Grand",
        "direccion": "Av. Principal 123",
  "telefono": "555-0100",
        "categoria": 5
      },
 "caracteristicas": [
        "Cama King",
        "Sala de estar",
        "TV",
        "Baño con jacuzzi",
        "WiFi",
   "Minibar",
        "Vista panorámica"
      ]
    }
  ],
  "pagination": {
    "currentPage": 1,
    "pageSize": 10,
    "totalPages": 3,
    "totalCount": 25,
    "hasPrevious": false,
    "hasNext": true
  },
  "timestamp": "2025-01-10T10:30:00Z"
}
```

#### **GET /api/habitaciones/{id}**
Obtiene una habitación específica por ID.

**Response 200 OK:**
```json
{
  "success": true,
  "message": "Operación exitosa",
  "data": {
    "id": 1,
    "numero": "101",
    "tipo": "Suite",
    "precioPorNoche": 3500.00,
    "estado": "Disponible",
    "hotel": { ... },
    "caracteristicas": [ ... ]
  },
  "errors": [],
  "timestamp": "2025-01-10T10:30:00Z"
}
```

#### **GET /api/habitaciones/{id}/disponibilidad**
Verifica disponibilidad de una habitación en fechas específicas.

**Query Parameters:**
```
?fechaEntrada=2025-01-15&fechaSalida=2025-01-20
```

**Response 200 OK:**
```json
{
  "success": true,
  "message": "Habitación disponible para las fechas solicitadas",
  "data": true,
  "errors": [],
  "timestamp": "2025-01-10T10:30:00Z"
}
```

#### **GET /api/habitaciones/tipos**
Obtiene los tipos de habitación disponibles.

**Response 200 OK:**
```json
{
  "success": true,
  "message": "Operación exitosa",
  "data": [
    "Individual",
    "Doble",
    "Suite",
    "Presidencial",
 "Ejecutiva"
  ],
  "errors": [],
  "timestamp": "2025-01-10T10:30:00Z"
}
```

---

### **2. Reservas (`/api/reservas`)**

#### **GET /api/reservas/{id}**
Obtiene información completa de una reserva.

**Response 200 OK:**
```json
{
  "success": true,
  "message": "Operación exitosa",
  "data": {
    "id": 123,
    "fechaReserva": "2025-01-01T14:30:00",
    "fechaEntrada": "2025-01-15T15:00:00",
    "fechaSalida": "2025-01-20T11:00:00",
    "estado": "Confirmada",
    "diasEstancia": 5,
    "montoTotal": 17500.00,
    "huesped": {
      "id": 45,
      "nombreCompleto": "Juan Pérez García",
   "email": "juan.perez@email.com",
      "telefono": "555-1234"
    },
    "habitacion": {
      "id": 1,
      "numero": "101",
      "tipo": "Suite",
      "precioPorNoche": 3500.00,
      "nombreHotel": "Hotel Plaza Grand"
    },
    "pagos": [
      {
        "id": 1,
        "monto": 17500.00,
        "fechaPago": "2025-01-01T14:30:00",
    "metodo": "Pendiente"
      }
    ]
  },
  "errors": [],
  "timestamp": "2025-01-10T10:30:00Z"
}
```

#### **POST /api/reservas**
Crea una nueva reserva.

**Request Body:**
```json
{
  "fechaEntrada": "2025-01-15",
  "fechaSalida": "2025-01-20",
  "idHabitacion": 1,
  "nombres": "Juan",
  "apellidos": "Pérez García",
  "email": "juan.perez@email.com",
  "telefono": "555-1234",
  "documentoIdentidad": "12345678"
}
```

**Response 201 Created:**
```json
{
  "success": true,
  "message": "Reserva creada exitosamente",
  "data": { ... },
  "errors": [],
  "timestamp": "2025-01-10T10:30:00Z"
}
```

**Response 400 Bad Request:**
```json
{
  "success": false,
  "message": "La habitación ya tiene reservas para las fechas seleccionadas",
  "data": null,
  "errors": [],
  "timestamp": "2025-01-10T10:30:00Z"
}
```

#### **POST /api/reservas/{id}/cancelar**
Cancela una reserva existente.

**Response 200 OK:**
```json
{
  "success": true,
  "message": "Reserva cancelada exitosamente",
  "data": { ... },
  "errors": [],
  "timestamp": "2025-01-10T10:30:00Z"
}
```

#### **GET /api/reservas/buscar**
Busca reservas por documento de identidad.

**Query Parameters:**
```
?documentoIdentidad=12345678
```

**Response 200 OK:**
```json
{
  "success": true,
  "message": "Se encontraron 2 reservas",
  "data": [ ... ],
  "errors": [],
  "timestamp": "2025-01-10T10:30:00Z"
}
```

---

### **3. Kiosco (`/api/kiosco`)**

#### **POST /api/kiosco/checkin**
Realiza check-in automático desde un kiosco.

**Request Body:**
```json
{
  "idReserva": 123,
  "documentoIdentidad": "12345678",
  "numeroConfirmacion": "ABC123"
}
```

**Response 200 OK:**
```json
{
  "success": true,
  "message": "Operación exitosa",
  "data": {
    "exitoso": true,
    "mensaje": "¡Bienvenido! Check-in completado exitosamente. Habitación: 101",
    "reserva": {
      "id": 123,
      "fechaEntrada": "2025-01-15T15:00:00",
"fechaSalida": "2025-01-20T11:00:00",
      "estado": "En curso",
      "diasEstancia": 5,
      "montoTotal": 17500.00,
 "huesped": { ... },
   "habitacion": { ... },
    "pagos": [ ... ]
    },
    "tarjetaAcceso": "R0123-H0001-T20250115"
  },
  "errors": [],
  "timestamp": "2025-01-15T14:30:00Z"
}
```

**Response 400 Bad Request:**
```json
{
  "success": false,
  "message": "Debe completar el pago antes del check-in. Diríjase a recepción.",
  "data": null,
  "errors": [],
  "timestamp": "2025-01-15T14:30:00Z"
}
```

#### **POST /api/kiosco/checkout**
Realiza check-out automático desde un kiosco.

**Request Body:**
```json
{
  "idReserva": 123,
  "documentoIdentidad": "12345678"
}
```

**Response 200 OK:**
```json
{
  "success": true,
  "message": "Operación exitosa",
  "data": "Check-out completado exitosamente. Habitación 101 liberada. ¡Gracias por su estadía!",
  "errors": [],
  "timestamp": "2025-01-20T11:00:00Z"
}
```

#### **GET /api/kiosco/reserva**
Obtiene información de reserva para el kiosco.

**Query Parameters:**
```
?documentoIdentidad=12345678&idReserva=123
```

**Response 200 OK:**
```json
{
  "success": true,
  "message": "Operación exitosa",
  "data": { ... },
  "errors": [],
  "timestamp": "2025-01-15T14:30:00Z"
}
```

---

## ?? Estructura de Respuestas

### **ApiResponse<T>**

Todas las respuestas de la API siguen este formato estándar:

```csharp
{
  "success": bool,        // Indica si la operación fue exitosa
  "message": string,    // Mensaje descriptivo
  "data": T, // Datos de respuesta (tipo genérico)
  "errors": string[],     // Lista de errores (si los hay)
  "timestamp": DateTime   // Marca de tiempo UTC
}
```

### **PagedApiResponse<T>**

Para endpoints con paginación:

```csharp
{
  "success": bool,
  "message": string,
  "data": T[],
  "pagination": {
 "currentPage": int,
    "pageSize": int,
    "totalPages": int,
    "totalCount": int,
    "hasPrevious": bool,
    "hasNext": bool
  },
  "timestamp": DateTime
}
```

---

## ?? Seguridad y Autenticación

### **Versión Actual (Pública)**
- ? Todos los endpoints son **públicos** (sin autenticación)
- ? Validación de datos con Data Annotations
- ? Logging de todas las operaciones

### **Versión Futura (Autenticada)**
```csharp
// Agregar autenticación JWT
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

// Header requerido:
Authorization: Bearer {token}
```

---

## ?? CORS (Cross-Origin Resource Sharing)

La API está configurada para aceptar solicitudes desde cualquier origen:

```csharp
Policy: ApiCorsPolicy
AllowAnyOrigin()
AllowAnyMethod()
AllowAnyHeader()
```

**Producción:** Configurar orígenes específicos.

---

## ?? Ejemplos de Uso

### **JavaScript (Fetch API)**

```javascript
// Consultar habitaciones disponibles
const consultarHabitaciones = async () => {
  const response = await fetch('https://api.hotelsuite.com/api/habitaciones/disponibles?tipo=Suite&pagina=1');
  const data = await response.json();
  
  if (data.success) {
    console.log('Habitaciones:', data.data);
    console.log('Total:', data.pagination.totalCount);
  }
};

// Crear reserva
const crearReserva = async (reservaData) => {
  const response = await fetch('https://api.hotelsuite.com/api/reservas', {
    method: 'POST',
    headers: {
   'Content-Type': 'application/json'
    },
    body: JSON.stringify(reservaData)
  });
  
  const result = await response.json();
  
  if (result.success) {
    console.log('Reserva creada:', result.data.id);
  } else {
    console.error('Error:', result.message);
  }
};

// Check-in desde kiosco
const realizarCheckIn = async (idReserva, documento) => {
  const response = await fetch('https://api.hotelsuite.com/api/kiosco/checkin', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
    idReserva: idReserva,
  documentoIdentidad: documento
    })
  });
  
  const result = await response.json();
  
  if (result.success && result.data.exitoso) {
    console.log('Check-in exitoso!');
    console.log('Tarjeta de acceso:', result.data.tarjetaAcceso);
  }
};
```

### **React Native / Mobile**

```typescript
import axios from 'axios';

const API_BASE_URL = 'https://api.hotelsuite.com/api';

// Servicio de habitaciones
export const HabitacionesService = {
  async obtenerDisponibles(filtros: any) {
    const response = await axios.get(`${API_BASE_URL}/habitaciones/disponibles`, {
      params: filtros
    });
 return response.data;
  },
  
  async verificarDisponibilidad(id: number, fechaEntrada: string, fechaSalida: string) {
    const response = await axios.get(
      `${API_BASE_URL}/habitaciones/${id}/disponibilidad`,
      {
        params: { fechaEntrada, fechaSalida }
      }
    );
    return response.data;
  }
};

// Servicio de reservas
export const ReservasService = {
  async crear(reserva: any) {
    const response = await axios.post(`${API_BASE_URL}/reservas`, reserva);
    return response.data;
  },
  
  async obtener(id: number) {
    const response = await axios.get(`${API_BASE_URL}/reservas/${id}`);
    return response.data;
  },
  
  async cancelar(id: number) {
    const response = await axios.post(`${API_BASE_URL}/reservas/${id}/cancelar`);
    return response.data;
  }
};

// Servicio de kiosco
export const KioscoService = {
  async checkIn(idReserva: number, documentoIdentidad: string) {
    const response = await axios.post(`${API_BASE_URL}/kiosco/checkin`, {
  idReserva,
      documentoIdentidad
    });
  return response.data;
  },
  
  async checkOut(idReserva: number, documentoIdentidad: string) {
    const response = await axios.post(`${API_BASE_URL}/kiosco/checkout`, {
    idReserva,
      documentoIdentidad
    });
    return response.data;
  }
};
```

### **C# / Xamarin / MAUI**

```csharp
using System.Net.Http.Json;

public class HotelSuiteApiClient
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://api.hotelsuite.com/api";

    public HotelSuiteApiClient()
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
    }

    // Consultar habitaciones
    public async Task<PagedApiResponse<HabitacionDisponibleDTO>> ObtenerHabitacionesDisponibles(
   DisponibilidadQueryDTO query)
    {
 var response = await _httpClient.GetFromJsonAsync<PagedApiResponse<HabitacionDisponibleDTO>>(
   $"habitaciones/disponibles?tipo={query.Tipo}&precioMaximo={query.PrecioMaximo}");
     return response;
    }

    // Crear reserva
    public async Task<ApiResponse<ReservaInfoDTO>> CrearReserva(CrearReservaDTO reserva)
    {
        var response = await _httpClient.PostAsJsonAsync("reservas", reserva);
        return await response.Content.ReadFromJsonAsync<ApiResponse<ReservaInfoDTO>>();
    }

    // Check-in
    public async Task<ApiResponse<CheckInResponseDTO>> RealizarCheckIn(CheckInDTO checkIn)
    {
        var response = await _httpClient.PostAsJsonAsync("kiosco/checkin", checkIn);
        return await response.Content.ReadFromJsonAsync<ApiResponse<CheckInResponseDTO>>();
    }
}
```

---

## ?? Testing con Swagger

### **Acceso a Swagger UI:**
```
https://localhost:5001/api/docs
```

### **Características de Swagger:**
- ? Documentación interactiva
- ? Probar endpoints directamente
- ? Ver modelos de datos
- ? Ejemplos de request/response
- ? Códigos de respuesta HTTP

---

## ?? Códigos de Estado HTTP

| Código | Descripción |
|--------|-------------|
| **200 OK** | Solicitud exitosa |
| **201 Created** | Recurso creado exitosamente |
| **400 Bad Request** | Error en la solicitud (validación fallida) |
| **404 Not Found** | Recurso no encontrado |
| **500 Internal Server Error** | Error del servidor |

---

## ?? Casos de Uso

### **1. Aplicación Móvil para Clientes**
```
1. Cliente busca habitaciones disponibles
2. Selecciona tipo y fechas
3. Crea reserva con sus datos
4. Recibe confirmación por email
5. Realiza check-in desde el móvil
```

### **2. Kiosco de Auto Check-in**
```
1. Cliente escanea QR de confirmación
2. Ingresa documento de identidad
3. Sistema valida reserva y pagos
4. Genera tarjeta de acceso
5. Imprime código de habitación
```

### **3. Integración con OTAs (Booking, Expedia)**
```
1. OTA consulta disponibilidad
2. Crea reserva automáticamente
3. Sincroniza estados
4. Notifica cambios
```

---

## ?? Notas Importantes

### **Validaciones:**
- Fechas: `FechaEntrada < FechaSalida`
- Fechas: `FechaEntrada >= Hoy`
- Email: Formato válido
- Teléfono: Formato válido
- Documento: Obligatorio y único

### **Estados de Reserva:**
- `Confirmada`: Reserva creada, sin check-in
- `En curso`: Check-in realizado
- `Finalizada`: Check-out realizado
- `Cancelada`: Reserva cancelada

### **Limitaciones Actuales:**
- Sin autenticación (todos los endpoints públicos)
- Sin rate limiting
- Sin versionado de API

### **Próximas Mejoras:**
- [ ] Autenticación JWT
- [ ] Versionado de API (v1, v2)
- [ ] Rate limiting
- [ ] Webhooks para notificaciones
- [ ] Soporte para múltiples idiomas
- [ ] GraphQL endpoint

---

## ?? URLs de Referencia

- **API Base:** `https://localhost:5001/api`
- **Swagger UI:** `https://localhost:5001/api/docs`
- **Health Check:** `https://localhost:5001/api/health` (próximamente)

---

**Versión:** 1.0.0  
**Última actualización:** 2025  
**Autor:** HotelSuite HMS  
**Tecnologías:** ASP.NET Core 9, Entity Framework Core, Swagger/OpenAPI

