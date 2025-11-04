# ?? HotelSuite HMS - Mejoras de Interfaz de Usuario

## ?? Descripción

Sistema completo de mejoras de UI/UX implementado con Bootstrap 5, diseño moderno con sidebar, validaciones visuales en español, componentes reutilizables y notificaciones con Toastr.

## ? Componentes Implementados

### ??? **Layout Principal (_Layout.cshtml)**

#### **Características:**
- ? Sidebar fijo con navegación vertical
- ? Navbar superior con perfil de usuario
- ? Diseño responsive (mobile-first)
- ? Sidebar colapsable con persistencia en localStorage
- ? Gradientes modernos y animaciones
- ? Footer profesional
- ? Breadcrumbs automáticos
- ? Integración con Toastr para mensajes
- ? Validaciones jQuery en español

#### **Sidebar:**
```html
- Logo y título del sistema
- Navegación por roles
- Indicador de página activa
- Collapse/Expand con animación
- Scroll personalizado
- Icono para cada menú
```

#### **Navbar Superior:**
```html
- Toggle del sidebar
- Avatar de usuario
- Nombre y rol del usuario
- Menú dropdown con opciones:
  - Mi Perfil
  - Cerrar Sesión
```

#### **Menú Condicional por Roles:**

| Rol | Menús Visibles |
|-----|----------------|
| **Administrador** | Dashboard, Hoteles, Habitaciones, Reservas, Pagos, Reportes, Departamentos, Tareas |
| **Gerente** | Dashboard, Hoteles, Habitaciones, Reservas, Pagos, Reportes, Departamentos, Tareas |
| **Recepcionista** | Dashboard, Habitaciones, Reservas, Pagos, Pagos Pendientes, Tareas |
| **Limpieza/Mantenimiento** | Dashboard, Tareas |
| **No autenticado** | Dashboard, Habitaciones |

### ?? **Partial Views Reutilizables**

#### **1. _HabitacionCard.cshtml**

**Uso:**
```razor
@await Html.PartialAsync("_HabitacionCard", habitacion)
```

**Características:**
- ? Tarjeta moderna con sombra y hover
- ? Badge de estado con colores
- ? Precio destacado
- ? Botones de acción condicionales
- ? Icono según estado
- ? Responsive

**Estados visuales:**
```csharp
Disponible ? Badge verde + Footer verde
Ocupada ? Badge rojo
Mantenimiento ? Badge amarillo
Reservada ? Badge azul
```

#### **2. _ReservaResumen.cshtml**

**Uso:**
```razor
@await Html.PartialAsync("_ReservaResumen", reserva)
```

**Características:**
- ? Resumen completo de reserva
- ? Información del huésped
- ? Detalles de habitación
- ? Fechas check-in/check-out
- ? Cálculo automático de noches
- ? Total a pagar
- ? Estado con badge
- ? Diseño en card con gradiente

#### **3. _Pagination.cshtml**

**Uso:**
```razor
@await Html.PartialAsync("_Pagination", Model)
```

**Características:**
- ? Paginación moderna con Bootstrap 5
- ? Botones Primera/Última
- ? Botones Anterior/Siguiente
- ? Números de página visibles (5 máximo)
- ? Información de registros mostrados
- ? Responsive (oculta texto en móvil)
- ? Estilos con gradientes

**Formato:**
```
[Primera] [Anterior] [1] [2] [3] [4] [5] [Siguiente] [Última]
Mostrando 1 - 10 de 95 registros
```

#### **4. _FilterCard.cshtml**

**Uso:**
```razor
@using (Html.BeginForm()) {
    @await Html.RenderPartialAsync("_FilterCard")
}
```

**Características:**
- ? Card para envolver filtros
- ? Botón Buscar
- ? Botón Limpiar (resetea filtros)
- ? Diseño consistente

### ?? **Páginas Mejoradas**

#### **Dashboard (Home/Index.cshtml)**

**Características:**
- ? Tarjetas de estadísticas (4)
  - Habitaciones totales
  - Reservas activas
  - Pagos pendientes
  - Ingresos del mes
- ? Acciones rápidas (6 tarjetas)
  - Nueva Reserva
  - Registrar Pago
  - Nueva Habitación
  - Ver Reportes
  - Tablero de Tareas
  - Ver Habitaciones
- ? Panel de notificaciones
- ? Timeline de actividad reciente (solo Admin/Gerente)
- ? Vista pública para no autenticados

**Estadísticas:**
```html
[?? 95 Habitaciones] [? 24 Reservas] [? 8 Pendientes] [?? $45.2K Ingresos]
```

#### **Habitaciones/Index.cshtml**

**Características:**
- ? Header con gradiente
- ? Filtros dinámicos (Hotel, Tipo, Estado)
- ? Auto-submit en cambio de filtro
- ? Tarjetas de resumen (4)
  - Total
  - Disponibles
  - Ocupadas
  - En Mantenimiento
- ? Grid responsive (3 columnas en desktop)
- ? Uso de _HabitacionCard
- ? Paginación con _Pagination
- ? Mensaje cuando no hay resultados

#### **Habitaciones/Create.cshtml**

**Características:**
- ? Formulario con validaciones visuales
- ? Labels con iconos
- ? Campo requerido con asterisco (*)
- ? Preview en tiempo real
- ? Input groups ($)
- ? Validación jQuery
- ? Estados is-valid/is-invalid
- ? Formato automático de precio
- ? Panel de ayuda con consejos
- ? Botones con iconos

**Validaciones en español:**
```javascript
required: "Este campo es obligatorio."
email: "Introduzca una dirección de correo válida."
number: "Introduzca un número válido."
min: "Introduzca un valor mayor o igual a {0}."
max: "Introduzca un valor menor o igual a {0}."
```

#### **Reservas/Details.cshtml**

**Características:**
- ? Uso de _ReservaResumen
- ? Panel de acciones rápidas
- ? Cards de información organizada
- ? Historial de pagos en tabla
- ? Modal de confirmación para cancelar
- ? Estilos detail-group

### ?? **Estilos CSS (hotel-suite.css)**

#### **Variables CSS:**
```css
--primary-color: #667eea
--secondary-color: #764ba2
--accent-color: #f093fb
--success-color: #10b981
--danger-color: #ef4444
--warning-color: #f59e0b
--info-color: #3b82f6
```

#### **Gradientes:**
```css
bg-gradient-primary: #667eea ? #764ba2
bg-gradient-success: #10b981 ? #059669
bg-gradient-danger: #ef4444 ? #dc2626
bg-gradient-warning: #f59e0b ? #d97706
```

#### **Componentes Estilizados:**
- ? Cards con border-radius 12px
- ? Botones con hover y transform
- ? Forms con border 2px
- ? Tables con thead estilizado
- ? Badges con padding y border-radius
- ? Alerts con colores personalizados
- ? Modals con border-radius
- ? Sombras (sm, md, lg)

#### **Animaciones:**
```css
fadeInUp: opacity 0?1 + translateY 30px?0
Hover en cards: translateY(-5px)
Hover en botones: translateY(-2px) + shadow
```

### ?? **Sistema de Notificaciones (Toastr)**

#### **Configuración:**
```javascript
toastr.options = {
    closeButton: true,
progressBar: true,
    positionClass: "toast-top-right",
    timeOut: 5000,
    showDuration: 300,
    hideDuration: 1000
}
```

#### **Tipos de Mensajes:**

```csharp
// Controlador
TempData["Success"] = "Operación exitosa";
TempData["Error"] = "Ha ocurrido un error";
TempData["Warning"] = "Advertencia";
TempData["Info"] = "Información";
```

```javascript
// Vista automática en _Layout
toastr.success('Mensaje', 'Éxito');
toastr.error('Mensaje', 'Error');
toastr.warning('Mensaje', 'Advertencia');
toastr.info('Mensaje', 'Información');
```

### ? **Validaciones en Español**

#### **Mensajes Personalizados:**
```javascript
$.validator.messages = {
    required: "Este campo es obligatorio.",
 remote: "Por favor, corrija este campo.",
    email: "Por favor, introduzca una dirección de correo válida.",
    url: "Por favor, introduzca una URL válida.",
    date: "Por favor, introduzca una fecha válida.",
    number: "Por favor, introduzca un número válido.",
    digits: "Por favor, introduzca solo dígitos.",
    maxlength: "No introduzca más de {0} caracteres.",
    minlength: "Introduzca al menos {0} caracteres.",
    range: "Introduzca un valor entre {0} y {1}.",
    max: "Introduzca un valor menor o igual a {0}.",
    min: "Introduzca un valor mayor o igual a {0}."
}
```

#### **Clases de Validación:**
```javascript
errorClass: 'is-invalid'
validClass: 'is-valid'
errorElement: 'div'
errorPlacement: dentro del .mb-3 más cercano
```

#### **Estados Visuales:**
```css
.is-valid ? border verde
.is-invalid ? border rojo
.valid-feedback ? texto verde
.invalid-feedback ? texto rojo
```

### ?? **Responsive Design**

#### **Breakpoints:**
```css
xs: < 576px   ? Sidebar colapsado, 1 columna
sm: ? 576px   ? 2 columnas en grid
md: ? 768px   ? Sidebar visible, 2 columnas
lg: ? 992px   ? 3 columnas en grid
xl: ? 1200px  ? 4 columnas en stats
```

#### **Adaptaciones Móviles:**
- ? Sidebar en overlay (no fijo)
- ? Navbar compacto
- ? Grid a 1 columna
- ? Botones más pequeños
- ? Padding reducido
- ? Texto oculto en paginación

### ?? **Funcionalidades JavaScript**

#### **Sidebar Toggle:**
```javascript
- Click en toggle ? addClass('collapsed')
- Persistencia en localStorage
- Animación suave (transition: 0.3s)
```

#### **Auto-submit Filtros:**
```javascript
$('#hotelId, #tipo, #estado').change(function() {
    $('#filtrosForm').submit();
});
```

#### **Preview en Tiempo Real:**
```javascript
// Create Habitación
$('#Numero, #Tipo, #PrecioPorNoche').on('input', updatePreview);
// Muestra: "Habitación 101 - Individual - $1200/noche"
```

#### **Limpiar Filtros:**
```javascript
function limpiarFiltros() {
    $('form').find('input, select').val('');
    $('form').submit();
}
```

### ?? **Paleta de Colores**

#### **Colores Principales:**
```
Primario: #667eea (Azul violeta)
Secundario: #764ba2 (Púrpura)
Acento: #f093fb (Rosa claro)

Éxito: #10b981 (Verde)
Peligro: #ef4444 (Rojo)
Advertencia: #f59e0b (Amarillo)
Información: #3b82f6 (Azul)

Gris claro: #f8f9fa
Gris oscuro: #333
```

#### **Gradientes Usados:**
```css
Header: linear-gradient(135deg, #667eea 0%, #764ba2 100%)
Botón primario: linear-gradient(135deg, #667eea 0%, #764ba2 100%)
Sidebar: linear-gradient(180deg, #667eea 0%, #764ba2 100%)
```

### ?? **Componentes Bootstrap 5 Utilizados**

- ? **Cards** - Tarjetas de contenido
- ? **Navbar** - Barra de navegación
- ? **Dropdown** - Menús desplegables
- ? **Forms** - Formularios con validación
- ? **Buttons** - Botones estilizados
- ? **Badges** - Etiquetas de estado
- ? **Alerts** - Mensajes de alerta
- ? **Modals** - Ventanas modales
- ? **Tables** - Tablas responsive
- ? **Grid** - Sistema de rejilla
- ? **Utilities** - Clases de utilidad

### ?? **Librerías Externas**

#### **Font Awesome 6.4.0:**
```html
<link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" />
```

**Iconos usados:**
```
fa-hotel, fa-home, fa-door-open, fa-calendar-check,
fa-dollar-sign, fa-chart-line, fa-briefcase, fa-tasks,
fa-user, fa-building, fa-clock, fa-check-circle, etc.
```

#### **Toastr 2.1.4:**
```html
<link href="https://cdnjs.cloudflare.com/ajax/libs/toastr.js/latest/toastr.min.css" />
<script src="https://cdnjs.cloudflare.com/ajax/libs/toastr.js/latest/toastr.min.js"></script>
```

#### **jQuery Validation:**
```html
<script src="~/lib/jquery-validation/dist/jquery.validate.min.js"></script>
<script src="~/lib/jquery-validation-unobtrusive/jquery.validate.unobtrusive.min.js"></script>
```

### ?? **Mejoras de UX**

#### **1. Feedback Visual Inmediato:**
- ? Hover en cards
- ? Hover en botones
- ? Estados de validación en tiempo real
- ? Spinners de carga
- ? Transiciones suaves

#### **2. Navegación Intuitiva:**
- ? Breadcrumbs
- ? Menú con iconos
- ? Indicador de página activa
- ? Botones "Volver" en todas las páginas

#### **3. Información Clara:**
- ? Mensajes de éxito/error con Toastr
- ? Estados con badges de colores
- ? Tooltips informativos
- ? Íconos descriptivos

#### **4. Accesibilidad:**
- ? ARIA labels
- ? Roles ARIA
- ? Contraste de colores
- ? Teclado navegable

### ?? **Ejemplo de Uso Completo**

#### **Vista con todos los componentes:**

```razor
@model X.PagedList.IPagedList<HabitacionDTO>

<!-- Header con gradiente -->
<div class="card border-0 shadow-sm">
    <div class="card-body bg-gradient text-white">
    <h2><i class="fas fa-door-open me-2"></i>Habitaciones</h2>
    </div>
</div>

<!-- Filtros -->
<form asp-action="Index">
    <div class="row g-3">
    <div class="col-md-4">
   <select name="tipo" class="form-select">...</select>
    </div>
      <div class="col-md-4">
            <button type="submit" class="btn btn-primary">
    <i class="fas fa-search me-1"></i>Buscar
       </button>
      </div>
    </div>
</form>

<!-- Tarjetas de resumen -->
<div class="row g-3">
 <div class="col-md-3">
        <div class="card shadow-sm">
            <div class="card-body text-center">
        <i class="fas fa-door-open fa-3x text-primary"></i>
           <h3>95</h3>
         <p>Total</p>
     </div>
        </div>
    </div>
</div>

<!-- Grid de habitaciones -->
<div class="row g-4">
    @foreach (var habitacion in Model)
    {
        <div class="col-lg-4">
            @await Html.PartialAsync("_HabitacionCard", habitacion)
    </div>
    }
</div>

<!-- Paginación -->
@await Html.PartialAsync("_Pagination", Model)
```

### ? **Checklist de Implementación**

- [x] Layout con sidebar y navbar moderno
- [x] Partial view _HabitacionCard
- [x] Partial view _ReservaResumen
- [x] Partial view _Pagination
- [x] Partial view _FilterCard
- [x] Dashboard con estadísticas
- [x] Index de Habitaciones con filtros
- [x] Create de Habitaciones con validaciones
- [x] Details de Reservas mejorado
- [x] CSS personalizado (hotel-suite.css)
- [x] Toastr para notificaciones
- [x] Validaciones jQuery en español
- [x] Responsive design
- [x] Gradientes y animaciones
- [x] Iconos Font Awesome
- [x] Mensajes TempData automáticos
- [x] Navegación por roles
- [x] Sidebar colapsable
- [x] Preview en tiempo real
- [x] Auto-submit en filtros

---

**Versión**: 2.0.0  
**Última actualización**: 2025  
**Autor**: Sistema HotelSuite  
**Tecnologías**: Bootstrap 5, jQuery, Font Awesome 6, Toastr, ASP.NET Core 9
