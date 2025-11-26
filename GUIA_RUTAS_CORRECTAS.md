# ?? GUÍA - Rutas Correctas del Sistema HotelSuite

## ?? Problema Común: Acceder a la API en lugar de la Interfaz Web

### **Síntoma:**
Al intentar crear una reserva, ves **texto en formato JSON** en lugar de un formulario web.

### **Causa:**
Estás accediendo a la **ruta de la API REST** (`/api/Reservas`) en lugar de la **ruta de la interfaz web** (`/Reservas/Create`).

---

## ?? Rutas Correctas por Módulo

### **1. RESERVAS** ??

| Acción | ? **RUTA INCORRECTA (API - JSON)** | ? **RUTA CORRECTA (WEB - HTML)** |
|--------|-------------------------------------|-----------------------------------|
| **Ver listado** | `/api/Reservas` | `/Reservas` o `/Reservas/Index` |
| **Crear nueva** | `/api/Reservas` (POST JSON) | `/Reservas/Create` |
| **Ver detalles** | `/api/Reservas/{id}` | `/Reservas/Details/{id}` |
| **Editar** | `/api/Reservas/{id}` (PUT JSON) | `/Reservas/Edit/{id}` |
| **Cancelar** | `/api/Reservas/{id}/cancelar` (POST) | `/Reservas/Cancel/{id}` |

**Ejemplo Correcto:**
```
? PARA CREAR UNA RESERVA WEB:
https://localhost:5001/Reservas/Create

? NO uses:
https://localhost:5001/api/Reservas
```

---

### **2. PAGOS** ??

| Acción | ? **RUTA INCORRECTA (API)** | ? **RUTA CORRECTA (WEB)** |
|--------|------------------------------|----------------------------|
| **Ver listado** | `/api/Pagos` | `/Pagos` o `/Pagos/Index` |
| **Pendientes** | No disponible en API | `/Pagos/Pendientes` |
| **Ver detalles** | No disponible en API | `/Pagos/Details/{id}` |
| **Registrar pago** | No disponible en API | `/Pagos/Registrar/{id}` |
| **Generar PDF** | No disponible en API | `/Pagos/GenerarComprobante/{id}` |

**Ejemplo Correcto:**
```
? PARA VER PAGOS PENDIENTES:
https://localhost:5001/Pagos/Pendientes

? NO uses:
https://localhost:5001/api/Pagos
```

---

### **3. HABITACIONES** ??

| Acción | ? **RUTA INCORRECTA (API)** | ? **RUTA CORRECTA (WEB)** |
|--------|------------------------------|----------------------------|
| **Ver listado** | `/api/Habitaciones` | `/Habitaciones` o `/Habitaciones/Index` |
| **Crear nueva** | `/api/Habitaciones` (POST) | `/Habitaciones/Create` |
| **Ver detalles** | `/api/Habitaciones/{id}` | `/Habitaciones/Details/{id}` |
| **Editar** | `/api/Habitaciones/{id}` (PUT) | `/Habitaciones/Edit/{id}` |
| **Eliminar** | `/api/Habitaciones/{id}` (DELETE) | `/Habitaciones/Delete/{id}` |

---

### **4. HOTELES** ??

| Acción | ? **RUTA CORRECTA (WEB)** |
|--------|----------------------------|
| **Ver listado** | `/Hoteles` o `/Hoteles/Index` |
| **Crear nuevo** | `/Hoteles/Create` |
| **Ver detalles** | `/Hoteles/Details/{id}` |
| **Editar** | `/Hoteles/Edit/{id}` |
| **Eliminar** | `/Hoteles/Delete/{id}` |

---

### **5. HUÉSPEDES** ??

| Acción | ? **RUTA CORRECTA (WEB)** |
|--------|----------------------------|
| **Ver listado** | `/Huespedes` o `/Huespedes/Index` |
| **Crear nuevo** | `/Huespedes/Create` |
| **Ver detalles** | `/Huespedes/Details/{id}` |
| **Editar** | `/Huespedes/Edit/{id}` |
| **Eliminar** | `/Huespedes/Delete/{id}` |

---

## ?? Cómo Acceder Correctamente

### **Opción 1: Desde el Dashboard (Recomendado)**

```
1. Abre el navegador
2. Ve a: https://localhost:5001
3. Inicia sesión con:
   Usuario: admin@hotelsuite.com
   Contraseña: Admin123!
4. En la sección "Acciones Rápidas" haz click en:
   ?? "Nueva Reserva" ? Te lleva a /Reservas/Create ?
```

### **Opción 2: Desde el Menú Lateral**

```
1. Click en el menú lateral izquierdo (morado)
2. Click en "Reservas"
3. En la esquina superior derecha ? Botón "Nueva Reserva"
```

### **Opción 3: URL Directa**

```
Escribe en el navegador:
https://localhost:5001/Reservas/Create

?? ASEGÚRATE DE:
- NO escribir "/api/" en ninguna parte
- Escribir la primera letra en mayúscula: "Reservas" (no "reservas")
- Incluir "/Create" al final
```

---

## ??? Mapa de Rutas del Sistema

### **Interfaz Web (HTML - Para Usuarios)**

```
https://localhost:5001/
??? /      ? Dashboard (Home)
??? /Account
?   ??? /Login     ? Iniciar sesión
?   ??? /Register               ? Registrarse
?   ??? /Profile            ? Mi perfil
?   ??? /Logout        ? Cerrar sesión
??? /Hoteles
?   ??? /Index         ? Listado
?   ??? /Create          ? Crear
?   ??? /Details/{id}    ? Ver detalles
?   ??? /Edit/{id}    ? Editar
?   ??? /Delete/{id}            ? Eliminar
??? /Habitaciones
?   ??? /Index            ? Listado
???? /Create       ? Crear
?   ??? /Details/{id}    ? Ver detalles
?   ??? /Edit/{id}   ? Editar
?   ??? /Delete/{id}     ? Eliminar
??? /Reservas
?   ??? /Index           ? Listado
?   ??? /Create           ? AQUÍ CREAS RESERVAS
?   ??? /Details/{id}           ? Ver detalles
?   ??? /Edit/{id}        ? Editar
?   ??? /Cancel/{id}            ? Cancelar
??? /Pagos
?   ??? /Index  ? Listado
?   ??? /Pendientes    ? Pagos pendientes
?   ??? /Details/{id}           ? Ver detalles
?   ??? /Registrar/{id}         ? Registrar pago
?   ??? /GenerarComprobante/{id}? Descargar PDF
??? /Reportes
?   ??? /Index       ? Dashboard de reportes
?   ??? /Ocupacion ? Reporte de ocupación
?   ??? /Ingresos     ? Reporte de ingresos
?   ??? /HabitacionesPopulares  ? Habitaciones más reservadas
??? /TareasDepartamento
    ??? /TableroTareas       ? Tablero Kanban
```

### **API REST (JSON - Para Apps Móviles/Externas)**

```
https://localhost:5001/api/
??? /api/Habitaciones
?   ??? GET /disponibilidad     ? Consultar disponibilidad
? ??? GET /{id}  ? Obtener habitación
???? GET /  ? Listar habitaciones
?   ??? POST /            ? Crear habitación
?   ??? PUT /{id}               ? Actualizar habitación
?   ??? DELETE /{id}    ? Eliminar habitación
??? /api/Reservas
?   ??? GET /              ? Listar reservas (JSON)
?   ??? GET /{id}               ? Obtener reserva
?   ??? POST /       ? Crear reserva (JSON)
?   ??? POST /{id}/cancelar     ? Cancelar reserva
??? /api/Kiosco
 ??? POST /checkin           ? Check-in automático
    ??? POST /checkout        ? Check-out automático
```

**?? Documentación API:**
```
https://localhost:5001/api/docs
(Solo disponible en modo desarrollo)
```

---

## ?? Cómo Identificar si Estás en la Ruta Correcta

### **? Estás en la INTERFAZ WEB si ves:**

- ?? Diseño visual con colores, menú lateral morado, botones
- ?? Formularios con campos de texto, selectores, botones "Guardar"
- ??? Encabezados, tarjetas, tablas con diseño Bootstrap
- ?? Navbar superior con tu nombre de usuario

**Ejemplo de URL correcta:**
```
https://localhost:5001/Reservas/Create
```

**Captura visual esperada:**
```
???????????????????????????????????????????????
? ?? Registrar Nueva Reserva          ?
???????????????????????????????????????????????
? Información del Huésped     ?
? [Selector de huésped ?]   ?
?           ?
? Información de la Habitación      ?
? [Selector de habitación ?]        ?
?   ?
? Fechas de la Reserva      ?
? Fecha Entrada: [__/__/____]     ?
? Fecha Salida:  [__/__/____]          ?
?      ?
? [Cancelar]  [Crear Reserva]         ?
???????????????????????????????????????????????
```

### **? Estás en la API REST si ves:**

- ?? Texto plano en formato JSON
- `{ "data": [...], "success": true, ... }`
- Sin diseño visual, solo texto estructurado
- Fondo blanco plano sin estilos

**Ejemplo de URL INCORRECTA (API):**
```
https://localhost:5001/api/Reservas
```

**Captura visual de error:**
```
{
  "data": [
    {
      "id": 1,
      "fechaEntrada": "2025-01-05T00:00:00",
    "fechaSalida": "2025-01-10T00:00:00",
      "huesped": {
   "nombres": "Carlos",
      "apellidos": "García"
      }
    }
  ],
  "success": true,
  "message": null,
  "pagination": {
    "currentPage": 1,
    "pageSize": 10
  }
}
```

---

## ?? Solución Rápida

### **Si ves JSON en lugar de formulario:**

```
1. ? Cierra la pestaña actual
2. ? Abre una nueva pestaña
3. ? Escribe EXACTAMENTE:
   https://localhost:5001/Reservas/Create
   
4. ? Presiona Enter
5. ? Deberías ver el formulario web completo
```

### **Si aún ves JSON:**

Verifica que estés autenticado:
```
1. Ve a: https://localhost:5001
2. Si no ves tu nombre arriba a la derecha ? No estás autenticado
3. Click en "Iniciar Sesión"
4. Usuario: admin@hotelsuite.com
5. Contraseña: Admin123!
6. Después ve a: https://localhost:5001/Reservas/Create
```

---

## ?? Checklist de Verificación

Antes de reportar un problema, verifica:

- [ ] ? La URL NO contiene "/api/"
- [ ] ? La URL comienza con "/Reservas/Create" (no "/api/Reservas")
- [ ] ? Estás autenticado (ves tu nombre arriba a la derecha)
- [ ] ? Tu rol es Administrador, Gerente o Recepcionista
- [ ] ? El navegador muestra un formulario HTML (no JSON)

---

## ?? Atajos del Teclado (Recomendado)

### **Opción 1: Crear Reserva Rápido**
```
1. Presiona Ctrl + L (selecciona la barra de URL)
2. Escribe: localhost:5001/Reservas/Create
3. Presiona Enter
```

### **Opción 2: Desde el Dashboard**
```
1. Presiona Ctrl + L
2. Escribe: localhost:5001
3. Presiona Enter
4. Click en tarjeta "Nueva Reserva"
```

---

## ?? Comparación de Rutas

| Necesitas | ? NO uses | ? Usa |
|-----------|------------|--------|
| Crear reserva (formulario web) | `/api/Reservas` | `/Reservas/Create` |
| Ver listado web | `/api/Reservas` | `/Reservas` |
| Ver detalles web | `/api/Reservas/15` | `/Reservas/Details/15` |
| Descargar PDF | N/A | `/Pagos/GenerarComprobante/15` |
| Ver pagos pendientes | N/A | `/Pagos/Pendientes` |

---

## ?? Estado Esperado

```
? URL correcta: https://localhost:5001/Reservas/Create
? Autenticado: Sí (ves tu nombre)
? Rol correcto: Administrador/Gerente/Recepcionista
? Vista: Formulario web completo (NO JSON)
? Selectores: Huésped y Habitación con datos
? Botón: "Crear Reserva" visible y habilitado

?? LISTO PARA CREAR RESERVAS
```

---

**Versión:** 5.0.0  
**Fecha:** 2025-01-04  
**Estado:** ? **GUÍA COMPLETA**  
**Problema:** Confusión entre rutas API y Web

---

## ?? Tip Final

**Recuerda:**
- `/api/Reservas` = JSON (para programas/apps)
- `/Reservas/Create` = HTML (para navegador web)

**¡Siempre usa las rutas SIN "/api/" para la interfaz web!** ??
