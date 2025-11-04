# ?? Inicio Rápido - HotelSuite HMS

## ? Método 1: Diagnóstico Automático (RECOMENDADO)

### **Paso 1: Ejecutar script de diagnóstico**

```powershell
# En PowerShell (como Administrador)
.\diagnostico.ps1
```

El script automáticamente:
- ? Detecta si tienes SQL Server LocalDB
- ? Configura SQLite si no tienes SQL Server
- ? Instala herramientas necesarias
- ? Compila el proyecto
- ? Aplica migraciones
- ? Inicia la aplicación

### **Paso 2: Abrir el navegador**

```
http://localhost:5000
```

---

## ??? Método 2: Configuración Manual

### **Opción A: SQLite (Sin instalar SQL Server)**

#### **1. Configurar Base de Datos**

Edita `HotelSuite/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=HotelSuite.db"
  },
  "DatabaseProvider": "Sqlite"
}
```

#### **2. Ejecutar Aplicación**

```bash
cd HotelSuite
dotnet run
```

? **La base de datos se crea automáticamente**

---

### **Opción B: SQL Server LocalDB**

#### **1. Instalar SQL Server LocalDB**

**Visual Studio Installer:**
- Abrir **Visual Studio Installer**
- Click en **Modificar**
- Seleccionar: **Desarrollo de ASP.NET y web**
- En "Componentes individuales": **SQL Server Express LocalDB**
- Click en **Modificar**

**O descargar:**
- https://www.microsoft.com/es-es/sql-server/sql-server-downloads
- Seleccionar **Express** ? **LocalDB**

#### **2. Configurar Base de Datos**

Edita `HotelSuite/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=HotelSuiteDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  },
  "DatabaseProvider": "SqlServer"
}
```

#### **3. Iniciar LocalDB (si es necesario)**

```bash
sqllocaldb start mssqllocaldb
```

#### **4. Ejecutar Aplicación**

```bash
cd HotelSuite
dotnet run
```

---

## ?? Requisitos Previos

### **Esenciales:**
- ? [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- ? Editor de código (Visual Studio 2022, VS Code, o Rider)

### **Opcionales:**
- ?? SQL Server LocalDB (si no usas SQLite)
- ?? Git (para clonar el repositorio)

---

## ?? Usuarios y Contraseñas Predeterminados

El sistema crea automáticamente usuarios de prueba:

| Rol | Usuario | Contraseña |
|-----|---------|------------|
| **Administrador** | admin@hotelsuite.com | Admin123! |
| **Gerente** | gerente@hotelsuite.com | Gerente123! |
| **Recepcionista** | recepcionista@hotelsuite.com | Recep123! |
| **Limpieza** | limpieza@hotelsuite.com | Limpieza123! |

---

## ?? URLs de Acceso

| Servicio | URL | Descripción |
|----------|-----|-------------|
| **Web App** | http://localhost:5000 | Aplicación principal |
| **API REST** | http://localhost:5000/api | Endpoints de API |
| **Swagger** | http://localhost:5000/api/docs | Documentación API |
| **HTTPS** | https://localhost:5001 | Versión segura |

---

## ?? Datos de Prueba

El sistema incluye datos de ejemplo:

- **2 Hoteles**
  - Hotel Plaza Grand (5 estrellas)
  - Hotel Vista Mar (4 estrellas)

- **10 Habitaciones**
  - Varios tipos: Individual, Doble, Suite, Presidencial

- **5 Huéspedes** de ejemplo

- **8 Reservas** de prueba

- **3 Departamentos**
  - Recepción
  - Limpieza
  - Mantenimiento

- **5 Empleados** distribuidos por departamento

---

## ?? Comandos Útiles

### **Gestión de Base de Datos:**

```bash
# Ver migraciones disponibles
dotnet ef migrations list --project HotelSuite.Infrastructure --startup-project HotelSuite

# Crear nueva migración
dotnet ef migrations add MigracionNueva --project HotelSuite.Infrastructure --startup-project HotelSuite

# Aplicar migraciones
dotnet ef database update --project HotelSuite.Infrastructure --startup-project HotelSuite

# Eliminar base de datos
dotnet ef database drop --project HotelSuite.Infrastructure --startup-project HotelSuite --force

# SQLite: eliminar archivo directamente
del HotelSuite\HotelSuite.db
```

### **Compilación y Ejecución:**

```bash
# Restaurar paquetes
dotnet restore

# Compilar
dotnet build

# Ejecutar
dotnet run --project HotelSuite

# Ejecutar con hot reload
dotnet watch run --project HotelSuite

# Compilar para producción
dotnet publish -c Release
```

### **Limpieza:**

```bash
# Limpiar archivos de compilación
dotnet clean

# Limpiar todo (incluyendo paquetes)
dotnet clean
dotnet nuget locals all --clear
```

---

## ?? Solución de Problemas

### **Error: "Could not open a connection to SQL Server"**

**Solución rápida:**
```bash
# Ejecutar script de diagnóstico
.\diagnostico.ps1
```

**O ver guía completa:**
- ?? `SOLUCION_PROBLEMAS_BD.md`

### **Error: "Login failed"**

Cambiar a SQLite en `appsettings.Development.json`:
```json
{
  "DatabaseProvider": "Sqlite",
  "ConnectionStrings": {
  "DefaultConnection": "Data Source=HotelSuite.db"
  }
}
```

### **Error: "dotnet ef not found"**

```bash
# Instalar EF Core Tools
dotnet tool install --global dotnet-ef

# Verificar instalación
dotnet ef --version
```

### **Error: "Port 5000 is already in use"**

```bash
# Usar puerto diferente
dotnet run --project HotelSuite --urls="http://localhost:5555"
```

---

## ?? Estructura del Proyecto

```
HotelSuite.Web/
??? HotelSuite/  # Proyecto Web (UI + API)
?   ??? Controllers/     # Controladores MVC y API
?   ??? Views/          # Vistas Razor
?   ??? API/Controllers/         # Controladores API REST
?   ??? wwwroot/                 # Archivos estáticos
?   ??? Program.cs       # Punto de entrada
?
??? HotelSuite.Application/    # Capa de Aplicación
?   ??? DTOs/   # Data Transfer Objects
?   ??? Mapping/        # AutoMapper Profiles
?
??? HotelSuite.Infrastructure/   # Capa de Infraestructura
?   ??? Data/            # DbContext y Seeders
?   ??? Repositories/         # Repositorios
?
??? HotelSuite.Domain/    # Capa de Dominio
    ??? Entities/    # Entidades del negocio
    ??? Interfaces/              # Interfaces de repositorios
```

---

## ?? Características Principales

### **? Gestión de Hoteles**
- CRUD completo de hoteles
- Categorización por estrellas
- Dirección y contacto

### **? Gestión de Habitaciones**
- Múltiples tipos (Individual, Doble, Suite, Presidencial)
- Estados (Disponible, Ocupada, Mantenimiento)
- Precios dinámicos
- Filtros avanzados

### **? Sistema de Reservas**
- Creación y gestión de reservas
- Validación de disponibilidad
- Edición y cancelación
- Estados (Confirmada, En curso, Finalizada, Cancelada)

### **? Gestión de Pagos**
- Registro de pagos
- Múltiples métodos (Efectivo, Tarjeta, Transferencia)
- Pagos pendientes
- Generación de comprobantes PDF
- Historial completo

### **? Reportes**
- Ocupación por período
- Ingresos totales
- Habitaciones más populares
- Gráficos interactivos

### **? Sistema de Tareas**
- Gestión de departamentos
- Asignación de tareas
- Tablero Kanban
- Estados (Pendiente, En Proceso, Completada)

### **? Autenticación y Autorización**
- Sistema de roles (Admin, Gerente, Recepcionista, Limpieza)
- Registro de usuarios
- Login/Logout
- Protección de rutas

### **? API REST**
- Endpoints públicos
- Documentación Swagger
- Consulta de disponibilidad
- Gestión de reservas
- Check-in/Check-out automático (Kioscos)

### **? UI Moderna**
- Bootstrap 5
- Sidebar colapsable
- Responsive design
- Notificaciones Toastr
- Validaciones visuales en español
- Componentes reutilizables

---

## ?? Seguridad

### **Configuración de Contraseñas:**
- Mínimo 8 caracteres
- Requiere mayúscula
- Requiere minúscula
- Requiere dígito
- Requiere carácter especial

### **Protección de Rutas:**
```csharp
[Authorize(Roles = "Administrador")]      // Solo admin
[Authorize(Roles = "Administrador,Gerente")] // Admin o Gerente
```

### **CORS Habilitado:**
```csharp
// Para integraciones con apps móviles
policy.AllowAnyOrigin()
      .AllowAnyMethod()
      .AllowAnyHeader();
```

---

## ?? Integración con Apps Móviles

### **Endpoints Disponibles:**

```
GET  /api/habitaciones/disponibles
GET  /api/habitaciones/{id}
GET  /api/habitaciones/{id}/disponibilidad
POST /api/reservas
GET  /api/reservas/{id}
POST /api/reservas/{id}/cancelar
POST /api/kiosco/checkin
POST /api/kiosco/checkout
```

### **Ejemplo de Uso (JavaScript):**

```javascript
// Consultar habitaciones disponibles
const response = await fetch('http://localhost:5000/api/habitaciones/disponibles?tipo=Suite');
const data = await response.json();

// Crear reserva
const reserva = {
  fechaEntrada: "2025-01-15",
  fechaSalida: "2025-01-20",
  idHabitacion: 1,
  nombres: "Juan",
  apellidos: "Pérez",
  email: "juan@email.com",
  telefono: "555-1234",
  documentoIdentidad: "12345678"
};

const result = await fetch('http://localhost:5000/api/reservas', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify(reserva)
});
```

---

## ?? Documentación Adicional

| Archivo | Descripción |
|---------|-------------|
| `SOLUCION_PROBLEMAS_BD.md` | Solución de problemas de base de datos |
| `API/README_API.md` | Documentación completa de API REST |
| `Views/README_UI_Improvements.md` | Mejoras de UI implementadas |
| `Controllers/README_*.md` | Documentación de controladores |
| `Infrastructure/README_DataSeeder.md` | Información sobre datos de prueba |

---

## ?? Primeros Pasos Recomendados

### **1. Explorar la Aplicación:**
- Login con usuario administrador
- Revisar el dashboard
- Crear una habitación
- Hacer una reserva de prueba
- Registrar un pago

### **2. Probar la API:**
- Acceder a Swagger: `http://localhost:5000/api/docs`
- Probar endpoint de habitaciones disponibles
- Crear una reserva desde la API
- Realizar check-in desde kiosco

### **3. Personalizar:**
- Cambiar nombre del hotel en seeders
- Agregar nuevos tipos de habitación
- Personalizar precios
- Modificar roles y permisos

---

## ?? Tips y Trucos

### **Desarrollo Rápido:**
```bash
# Hot reload automático
dotnet watch run --project HotelSuite
```

### **Ver Logs Detallados:**
```json
// En appsettings.Development.json
{
  "Logging": {
    "LogLevel": {
   "Default": "Debug",
  "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

### **Cambiar Puerto:**
```bash
dotnet run --project HotelSuite --urls="http://localhost:8080"
```

### **Acceso desde Red Local:**
```bash
dotnet run --project HotelSuite --urls="http://0.0.0.0:5000"
# Acceder desde: http://<IP-LOCAL>:5000
```

---

## ?? Despliegue en Producción

### **1. Configurar Base de Datos:**
```json
// appsettings.Production.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=<SERVER>;Database=HotelSuiteDb;User Id=<USER>;Password=<PASSWORD>;TrustServerCertificate=True"
  },
  "DatabaseProvider": "SqlServer"
}
```

### **2. Compilar para Producción:**
```bash
dotnet publish -c Release -o ./publish
```

### **3. Configurar HTTPS:**
```bash
# Generar certificado de desarrollo
dotnet dev-certs https --trust
```

### **4. Variables de Entorno:**
```bash
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS="https://+:443;http://+:80"
```

---

## ?? Soporte

### **¿Problemas?**
1. Revisa `SOLUCION_PROBLEMAS_BD.md`
2. Ejecuta `.\diagnostico.ps1`
3. Verifica los logs en consola

### **¿Preguntas?**
- Documentación completa en carpetas `/Controllers/README_*.md`
- API documentation en Swagger
- Código bien documentado con comentarios

---

**¡Listo para comenzar! ??**

```bash
# Inicio rápido en 3 pasos:
.\diagnostico.ps1         # 1. Ejecutar diagnóstico
cd HotelSuite             # 2. Ir al proyecto
dotnet run   # 3. Iniciar aplicación
```

**Versión:** 2.0.0  
**Última actualización:** 2025  
**Sistema:** HotelSuite HMS  
**Tecnologías:** ASP.NET Core 9, Entity Framework Core 9, Bootstrap 5, SQLite/SQL Server
