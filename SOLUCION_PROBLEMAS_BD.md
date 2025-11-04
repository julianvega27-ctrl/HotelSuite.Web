# ?? Guía de Solución de Problemas - HotelSuite HMS

## ? Error: "Could not open a connection to SQL Server"

### **Síntomas:**
```
Microsoft.Data.SqlClient.SqlException (0x80131904): A network-related or instance-specific error occurred while establishing a connection to SQL Server.
```

---

## ? Error: "Pending Model Changes" ? **NUEVO**

### **Síntomas:**
```
Microsoft.EntityFrameworkCore.Migrations.PendingModelChangesWarning: 
The model for context 'HotelDbContext' has pending changes. 
Add a new migration before updating the database.
```

### **Causa:**
Modificaste las entidades del dominio pero no creaste una migración para reflejar esos cambios.

### **? Solución Rápida:**

#### **Opción 1: Script automático (RECOMENDADO)**
```powershell
.\migraciones.ps1 add SolucionarCambiosPendientes
```

#### **Opción 2: Manual**
```bash
# Crear nueva migración
dotnet ef migrations add UpdateModel --project HotelSuite.Infrastructure --startup-project HotelSuite

# Aplicar migración
dotnet ef database update --project HotelSuite.Infrastructure --startup-project HotelSuite
```

#### **Opción 3: Desde la aplicación**
```
La aplicación ahora maneja este error automáticamente en modo desarrollo
y mostrará instrucciones claras en los logs.
```

---

## ?? **Gestión de Migraciones con Script**

### **Comandos Disponibles:**

```powershell
# Ver ayuda completa
.\migraciones.ps1 help

# Crear nueva migración
.\migraciones.ps1 add NombreDeLaMigracion

# Ver lista de migraciones
.\migraciones.ps1 list

# Aplicar migraciones pendientes
.\migraciones.ps1 update

# Revertir a migración específica
.\migraciones.ps1 update NombreMigracionAnterior

# Eliminar última migración
.\migraciones.ps1 remove

# Eliminar base de datos completa
.\migraciones.ps1 drop

# Generar script SQL
.\migraciones.ps1 script
```

### **Ejemplos Prácticos:**

#### **Agregar un nuevo campo a una entidad:**
```powershell
# 1. Edita la entidad en Domain/Entities/
# 2. Crea la migración
.\migraciones.ps1 add AgregarCampoNuevo

# 3. Aplica los cambios
.\migraciones.ps1 update
```

#### **Deshacer una migración:**
```powershell
# Si NO ha sido aplicada:
.\migraciones.ps1 remove

# Si YA fue aplicada:
.\migraciones.ps1 list          # Ver migración anterior
.\migraciones.ps1 update PreviousMigration  # Revertir
.\migraciones.ps1 remove         # Eliminar archivo
```

#### **Empezar desde cero:**
```powershell
.\migraciones.ps1 drop    # Eliminar BD
.\migraciones.ps1 update  # Recrear con todas las migraciones
```

---

## ? **SOLUCIONES para Conexión SQL Server**

### **?? Solución 1: Usar SQLite (RECOMENDADO - Sin instalar SQL Server)**

Esta es la solución más rápida y no requiere instalar SQL Server.

#### **Paso 1: Configurar para usar SQLite**

Edita `appsettings.Development.json`:

```json
{
"ConnectionStrings": {
    "DefaultConnection": "Data Source=HotelSuite.db"
  },
  "DatabaseProvider": "Sqlite",
  "ApplicationSettings": {
    "AutoMigrate": true,
    "SeedData": true
  }
}
```

#### **Paso 2: Ejecutar la aplicación**

```bash
dotnet run --project HotelSuite
```

? **La base de datos SQLite se creará automáticamente** en `HotelSuite.db`

---

### **?? Solución 2: Instalar SQL Server LocalDB**

Si prefieres usar SQL Server:

#### **Paso 1: Instalar SQL Server LocalDB**

**Opción A: Visual Studio Installer**
1. Abrir **Visual Studio Installer**
2. Click en **Modificar**
3. En "Cargas de trabajo", seleccionar:
   - ? **Desarrollo de ASP.NET y web**
4. En "Componentes individuales", buscar y seleccionar:
   - ? **SQL Server Express LocalDB**
5. Click en **Modificar** e instalar

**Opción B: Descarga directa**
- Descargar: [SQL Server Express](https://www.microsoft.com/es-es/sql-server/sql-server-downloads)
- Seleccionar: **Descarga gratuita** ? **Express**
- Durante instalación, elegir: **LocalDB**

#### **Paso 2: Configurar para SQL Server LocalDB**

Edita `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=HotelSuiteDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  },
  "DatabaseProvider": "SqlServer",
  "ApplicationSettings": {
    "AutoMigrate": true,
    "SeedData": true
  }
}
```

#### **Paso 3: Verificar que LocalDB está corriendo**

```bash
# Ver instancias de LocalDB
sqllocaldb info

# Iniciar LocalDB si no está corriendo
sqllocaldb start mssqllocaldb

# Ver información de la instancia
sqllocaldb info mssqllocaldb
```

#### **Paso 4: Ejecutar la aplicación**

```bash
dotnet run --project HotelSuite
```

---

### **?? Solución 3: Usar SQL Server Express con instancia nombrada**

#### **Paso 1: Instalar SQL Server Express**

1. Descargar: [SQL Server Express](https://www.microsoft.com/es-es/sql-server/sql-server-downloads)
2. Durante instalación:
   - Nombre de instancia: **SQLEXPRESS** (o personalizado)
   - Modo de autenticación: **Mixto** (Windows + SQL)
   - Contraseña SA: **YourPassword123!**

#### **Paso 2: Habilitar TCP/IP**

1. Abrir **SQL Server Configuration Manager**
2. Ir a: **SQL Server Network Configuration** ? **Protocols for SQLEXPRESS**
3. Click derecho en **TCP/IP** ? **Habilitar**
4. Reiniciar servicio: **SQL Server (SQLEXPRESS)**

#### **Paso 3: Configurar cadena de conexión**

Edita `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    // Opción 1: Autenticación Windows
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=HotelSuiteDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true",
    
    // Opción 2: Autenticación SQL
    // "DefaultConnection": "Server=.\\SQLEXPRESS;Database=HotelSuiteDb;User Id=sa;Password=YourPassword123!;TrustServerCertificate=True;MultipleActiveResultSets=true"
  },
  "DatabaseProvider": "SqlServer"
}
```

---

## ?? **Cambiar entre SQLite y SQL Server**

### **Para usar SQLite:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=HotelSuite.db"
  },
  "DatabaseProvider": "Sqlite"
}
```

### **Para usar SQL Server LocalDB:**
```json
{
  "ConnectionStrings": {
 "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=HotelSuiteDb;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "DatabaseProvider": "SqlServer"
}
```

---

## ??? **Gestión de Migraciones**

### **Usando el script (RECOMENDADO):**

```powershell
# Ver todas las migraciones
.\migraciones.ps1 list

# Crear migración
.\migraciones.ps1 add InitialCreate

# Aplicar migraciones
.\migraciones.ps1 update

# Ver ayuda completa
.\migraciones.ps1 help
```

### **Comandos manuales:**

```bash
# Crear una nueva migración (si cambias el provider)
dotnet ef migrations add InitialCreate --project HotelSuite.Infrastructure --startup-project HotelSuite

# Aplicar migración
dotnet ef database update --project HotelSuite.Infrastructure --startup-project HotelSuite

# Ver estado de migraciones
dotnet ef migrations list --project HotelSuite.Infrastructure --startup-project HotelSuite

# Eliminar última migración
dotnet ef migrations remove --project HotelSuite.Infrastructure --startup-project HotelSuite

# Eliminar base de datos
dotnet ef database drop --project HotelSuite.Infrastructure --startup-project HotelSuite --force

# SQLite: eliminar archivo directamente
del HotelSuite\HotelSuite.db
```

---

## ?? **Otros Problemas Comunes**

### **Error: "Login failed for user"**

**Causa:** Credenciales incorrectas

**Solución:**
1. Verificar usuario y contraseña en la cadena de conexión
2. Para autenticación Windows, usar: `Trusted_Connection=True`
3. Para autenticación SQL, usar: `User Id=sa;Password=TuPassword`

### **Error: "Cannot attach the file as database"**

**Causa:** Base de datos en uso o bloqueada

**Solución:**
```bash
# Detener la aplicación
# Eliminar archivo de base de datos
del HotelSuite\HotelSuite.db

# O para SQL Server
.\migraciones.ps1 drop
```

### **Error: "Pending model changes"** ?

**Causa:** Cambios en el modelo que requieren migración

**Solución:**
```powershell
# Opción 1: Script automático
.\migraciones.ps1 add UpdateModel

# Opción 2: Manual
dotnet ef migrations add UpdateModel --project HotelSuite.Infrastructure --startup-project HotelSuite
```

### **Error: "The model backing the context has changed"**

**Causa:** Modelo desincronizado con la base de datos

**Solución:**
```powershell
# Opción 1: Aplicar migraciones pendientes
.\migraciones.ps1 update

# Opción 2: Empezar desde cero
.\migraciones.ps1 drop
.\migraciones.ps1 update
```

### **Error: "No such table" (SQLite)**

**Causa:** Migraciones no aplicadas

**Solución:**
```bash
# La aplicación las aplica automáticamente, pero si falla:
dotnet ef database update --project HotelSuite.Infrastructure --startup-project HotelSuite
```

---

## ?? **Verificar que todo funciona**

### **1. Verificar configuración:**

```bash
# Ver configuración activa
dotnet run --project HotelSuite --no-build --no-launch-profile -- --urls="http://localhost:5000"
```

### **2. Probar conexión:**

Crear archivo temporal `TestConnection.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using HotelSuite.Infrastructure.Data;

var connectionString = "Data Source=HotelSuite.db";
var options = new DbContextOptionsBuilder<HotelDbContext>()
    .UseSqlite(connectionString)
    .Options;

using var context = new HotelDbContext(options);
var canConnect = await context.Database.CanConnectAsync();
Console.WriteLine($"Conexión: {(canConnect ? "? Exitosa" : "? Fallida")}");
```

### **3. Ver logs detallados:**

Edita `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information",
      "Microsoft.EntityFrameworkCore.Migrations": "Information"
    }
  }
}
```

---

## ?? **Comparación: SQLite vs SQL Server**

| Característica | SQLite | SQL Server LocalDB | SQL Server Express |
|----------------|--------|-------------------|-------------------|
| **Instalación** | ? No requiere | ?? Visual Studio | ?? Instalación separada |
| **Rendimiento** | ?? Bueno | ? Excelente | ? Excelente |
| **Tamaño DB** | ?? Hasta 281 TB | ? Sin límite | ?? 10 GB |
| **Usuarios simultáneos** | ?? 1 escritor | ? Múltiples | ? Múltiples |
| **Desarrollo** | ? **Ideal** | ? Muy bueno | ?? Overkill |
| **Producción** | ?? Solo para apps pequeñas | ? No | ? Recomendado |

### **Recomendación:**

- **Desarrollo rápido:** ? **SQLite**
- **Desarrollo profesional:** ? **SQL Server LocalDB**
- **Producción:** ? **SQL Server Express/Standard**

---

## ?? **Inicio Rápido - Solo 3 Pasos**

### **Opción A: SQLite (Más rápido)**

```bash
# 1. Configurar SQLite
# Editar appsettings.Development.json ? DatabaseProvider: "Sqlite"

# 2. Ejecutar
dotnet run --project HotelSuite

# 3. Abrir navegador
# http://localhost:5000
```

### **Opción B: SQL Server LocalDB**

```bash
# 1. Instalar LocalDB (desde Visual Studio Installer)

# 2. Configurar SQL Server
# Editar appsettings.Development.json ? DatabaseProvider: "SqlServer"

# 3. Ejecutar
dotnet run --project HotelSuite
```

---

## ?? **¿Necesitas Ayuda?**

### **Logs de error:**
```bash
# Ver logs en tiempo real
dotnet run --project HotelSuite --verbosity detailed
```

### **Información del sistema:**
```bash
# Versión de .NET
dotnet --version

# Versiones de SQL Server instaladas
sqllocaldb info

# Entity Framework Core CLI
dotnet ef --version
```

### **Limpiar y reconstruir:**
```bash
# Limpiar
dotnet clean

# Restaurar paquetes
dotnet restore

# Reconstruir
dotnet build

# Ejecutar
dotnet run --project HotelSuite
```

---

## ? **Checklist de Verificación**

Antes de reportar un problema, verifica:

- [ ] ¿Está configurado `DatabaseProvider` en `appsettings.Development.json`?
- [ ] ¿La cadena de conexión es correcta?
- [ ] ¿SQL Server está instalado y corriendo? (si usas SQL Server)
- [ ] ¿Tienes permisos para crear bases de datos?
- [ ] ¿Los paquetes NuGet están restaurados?
- [ ] ¿La compilación es exitosa? (`dotnet build`)
- [ ] ¿Los logs muestran información útil?

---

**Versión:** 1.0.0  
**Última actualización:** 2025  
**Sistema:** HotelSuite HMS  
**Tecnologías:** .NET 9, Entity Framework Core 9, SQLite/SQL Server
