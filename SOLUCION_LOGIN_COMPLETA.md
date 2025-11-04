# ?? Solución Completa - Problemas de Login y Registro

## ? Problema: No es posible iniciar sesión ni registrarse

### **Síntomas:**
- ? No se puede hacer login con usuarios existentes
- ? No se puede registrar nuevos usuarios
- ? Los formularios no muestran errores de validación
- ? La página se queda en blanco o no responde

---

## ? SOLUCIÓN COMPLETA APLICADA

### **?? Problema 1: Archivos JSON con Comentarios Inválidos**

**Error encontrado:**
```json
{
  "ConnectionStrings": {
    // Este comentario causa error en JSON
    "DefaultConnection": "..."
  }
}
```

**? Solución:**
Los archivos JSON no soportan comentarios. Se han limpiado ambos archivos de configuración.

**Archivos corregidos:**
- ? `appsettings.json`
- ? `appsettings.Development.json`

---

### **?? Problema 2: Configuración Inconsistente de Base de Datos**

**Error encontrado:**
- DatabaseProvider configurado como "SqlServer"
- Cadena de conexión configurada para SQLite
- ? CONFLICTO: El sistema no sabía qué base de datos usar

**? Solución:**

#### **appsettings.Development.json** (Desarrollo - SQLite)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=HotelSuite.db"
  },
  "DatabaseProvider": "Sqlite",
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Debug",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information",
      "Microsoft.EntityFrameworkCore.Migrations": "Information"
    }
},
  "ApplicationSettings": {
    "AutoMigrate": true,
  "SeedData": true,
    "EnableSwagger": true
  }
}
```

#### **appsettings.json** (Producción - SQL Server)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=HotelSuiteDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  },
  "DatabaseProvider": "SqlServer",
  "Logging": {
    "LogLevel": {
 "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ApplicationSettings": {
    "AutoMigrate": true,
    "SeedData": true,
    "EnableSwagger": true
  }
}
```

---

### **?? Problema 3: Librerías jQuery Validation**

**? Ya corregido anteriormente:**
- ? Instaladas todas las librerías necesarias
- ? Rutas actualizadas en todas las vistas
- ? Validaciones del lado del cliente funcionando

---

## ?? CÓMO INICIAR LA APLICACIÓN

### **Opción 1: Script de Diagnóstico (RECOMENDADO)**

```powershell
# Ejecutar diagnóstico completo
.\diagnostico-login.ps1
```

Este script verifica:
- ? Librerías JavaScript instaladas
- ? Configuración de base de datos
- ? Base de datos creada
- ? Compilación del proyecto
- ? Archivos de vista
- ? Controladores
- ? ViewModels
- ? Rutas de scripts

### **Opción 2: Inicio Manual**

```powershell
# 1. Limpiar y compilar
cd HotelSuite
dotnet clean
dotnet build

# 2. Ejecutar
dotnet run
```

### **Opción 3: Visual Studio**

1. Abrir `HotelSuite.sln`
2. Presionar `F5` o click en "Start"

---

## ?? VERIFICACIÓN PASO A PASO

### **1. Verificar que la aplicación inicia correctamente**

```powershell
cd HotelSuite
dotnet run
```

**Logs esperados:**
```
=== Iniciando configuración de base de datos ===
Aplicando migraciones pendientes...
? Migraciones aplicadas exitosamente.
Verificando datos iniciales...
? Datos iniciales verificados/creados.
Verificando roles y usuarios...
? Roles y usuarios verificados/creados.
=== Configuración de base de datos completada exitosamente ===

????????????????????????????????????????????????????????
?? HotelSuite HMS - Sistema de Gestión Hotelera
????????????????????????????????????????????????????????
?? Aplicación Web:  https://localhost:5001
?? API REST:    https://localhost:5001/api
?? Documentación:   https://localhost:5001/api/docs
????????????????????????????????????????????????????????
?? Usuario Admin:   admin@hotelsuite.com
?? Contraseña:      Admin123!
????????????????????????????????????????????????????????
```

### **2. Abrir el navegador**

```
https://localhost:5001
```

### **3. Ir a Login**

```
https://localhost:5001/Account/Login
```

### **4. Probar con usuario de prueba**

| Campo | Valor |
|-------|-------|
| **Email** | admin@hotelsuite.com |
| **Contraseña** | Admin123! |

### **5. Verificar validaciones**

1. **Dejar campos vacíos** ? Deberías ver mensajes de error en rojo
2. **Email inválido** ? Mensaje: "Email inválido"
3. **Contraseña incorrecta** ? Mensaje: "Email o contraseña incorrectos"

---

## ?? DIAGNÓSTICO DE PROBLEMAS

### **Si aún no funciona:**

#### **1. Abrir Developer Tools (F12)**

**Pestaña Console:**
```
? No deberías ver errores JavaScript
? No deberías ver mensajes "Uncaught ReferenceError"
? No deberías ver "$(...).validate is not a function"
```

**Pestaña Network:**
```
? jquery.min.js ? 200 OK
? jquery.validate.min.js ? 200 OK
? jquery.validate.unobtrusive.min.js ? 200 OK
? bootstrap.bundle.min.js ? 200 OK

? Si ves 404 ? Las rutas están mal configuradas
```

#### **2. Limpiar caché del navegador**

```
Ctrl + Shift + Delete

Seleccionar:
? Cookies y otros datos de sitios
? Imágenes y archivos almacenados en caché

Período de tiempo: Desde siempre

Click en "Borrar datos"
```

#### **3. Verificar base de datos**

```powershell
# Ver si existe
Test-Path "HotelSuite\HotelSuite.db"

# Ver tamaño (debería ser > 20 KB)
(Get-Item "HotelSuite\HotelSuite.db").Length / 1KB

# Si está vacía o corrupta, eliminar y recrear
Remove-Item "HotelSuite\HotelSuite.db" -Force
cd HotelSuite
dotnet run
```

#### **4. Verificar migraciones**

```powershell
# Ver migraciones aplicadas
.\migraciones.ps1 list

# Aplicar migraciones pendientes
.\migraciones.ps1 update

# Si hay problemas, empezar desde cero
.\migraciones.ps1 drop
.\migraciones.ps1 update
```

#### **5. Verificar que los seeders funcionan**

En los logs de inicio, deberías ver:
```
Verificando roles y usuarios...
? Roles y usuarios verificados/creados.
```

Si no ves estos mensajes:
```json
// Verificar en appsettings.Development.json
{
  "ApplicationSettings": {
    "AutoMigrate": true,
    "SeedData": true  // ? Debe estar en true
  }
}
```

---

## ?? USUARIOS DE PRUEBA

Después de ejecutar la aplicación, estos usuarios estarán disponibles:

| Rol | Email | Contraseña |
|-----|-------|------------|
| **Administrador** | admin@hotelsuite.com | Admin123! |
| **Gerente** | gerente@hotelsuite.com | Gerente123! |
| **Recepcionista** | recepcionista@hotelsuite.com | Recep123! |
| **Limpieza** | limpieza@hotelsuite.com | Limpieza123! |

---

## ??? COMANDOS ÚTILES

### **Limpiar todo y empezar desde cero:**

```powershell
# 1. Detener aplicación (Ctrl+C si está corriendo)

# 2. Limpiar proyecto
cd HotelSuite
dotnet clean

# 3. Eliminar base de datos
Remove-Item HotelSuite.db -Force -ErrorAction SilentlyContinue

# 4. Limpiar librerías y reinstalar
libman clean
libman restore

# 5. Compilar
dotnet build

# 6. Ejecutar (creará BD y datos)
dotnet run
```

### **Ver logs detallados:**

```powershell
# En appsettings.Development.json, cambiar:
{
  "Logging": {
  "LogLevel": {
      "Default": "Debug",  // ? De Information a Debug
 "Microsoft.AspNetCore": "Debug",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information",
      "Microsoft.EntityFrameworkCore.Migrations": "Information"
    }
  }
}
```

### **Probar conexión a BD:**

```powershell
# SQLite
dotnet ef database update --project HotelSuite.Infrastructure --startup-project HotelSuite

# Si funciona, la conexión es correcta
```

---

## ?? ERRORES COMUNES Y SOLUCIONES

### **Error: "System.Text.Json.JsonException"**

**Causa:** Comentarios en archivos JSON

**Solución:**
```powershell
# Ya corregido en appsettings.json y appsettings.Development.json
# Si modificaste los archivos, asegúrate de NO tener comentarios
```

### **Error: "No DbContext was found"**

**Causa:** Configuración de base de datos incorrecta

**Solución:**
```powershell
# Verificar que DatabaseProvider coincida con la cadena de conexión
# SQLite ? DatabaseProvider: "Sqlite"
# SQL Server ? DatabaseProvider: "SqlServer"
```

### **Error: "User not found" al hacer login**

**Causa:** Datos de prueba no se crearon

**Solución:**
```powershell
# 1. Verificar que SeedData esté en true
# 2. Eliminar y recrear BD
Remove-Item HotelSuite\HotelSuite.db -Force
cd HotelSuite
dotnet run
```

### **Error: "$ is not defined" en la consola**

**Causa:** jQuery no se está cargando

**Solución:**
```powershell
# Reinstalar librerías
libman clean
libman restore

# Verificar que existe
Test-Path "HotelSuite\wwwroot\lib\jquery\jquery.min.js"
```

### **Error: "validate is not a function"**

**Causa:** jQuery Validation no se está cargando

**Solución:**
```html
<!-- Verificar orden en Login.cshtml -->
<script src="~/lib/jquery/jquery.min.js"></script>  <!-- 1º -->
<script src="~/lib/jquery-validation/jquery.validate.min.js"></script>  <!-- 2º -->
<script src="~/lib/jquery-validation-unobtrusive/jquery.validate.unobtrusive.min.js"></script>  <!-- 3º -->
```

---

## ? CHECKLIST DE VERIFICACIÓN

Antes de reportar un problema, verifica:

- [ ] ? Los archivos JSON no tienen comentarios
- [ ] ? DatabaseProvider coincide con la cadena de conexión
- [ ] ? La compilación es exitosa (`dotnet build`)
- [ ] ? Las librerías JavaScript están instaladas (`libman restore`)
- [ ] ? La base de datos existe y no está vacía
- [ ] ? Los seeders se ejecutaron (revisar logs)
- [ ] ? La caché del navegador está limpia
- [ ] ? No hay errores en la consola del navegador (F12)
- [ ] ? Los archivos de vista tienen las rutas correctas
- [ ] ? AutoMigrate y SeedData están en `true`

---

## ?? SI NADA FUNCIONA

### **Último recurso - Reinstalación completa:**

```powershell
# 1. Hacer backup de cambios personales
# 2. Limpiar TODO
git clean -fdx  # ?? CUIDADO: Elimina archivos no rastreados

# 3. Restaurar
dotnet restore
libman restore

# 4. Compilar
dotnet build

# 5. Ejecutar
cd HotelSuite
dotnet run
```

### **Reportar el problema:**

Si sigues teniendo problemas, proporciona:

1. **Logs completos** de la consola al ejecutar `dotnet run`
2. **Errores de la consola del navegador** (F12 ? Console)
3. **Errores de Network** (F12 ? Network ? Filtrar por errores 404/500)
4. **Contenido de** `appsettings.Development.json`
5. **Versión de .NET:** `dotnet --version`
6. **Sistema operativo:** Windows/Linux/Mac

---

## ?? RESUMEN

### **Problemas corregidos:**
1. ? Archivos JSON con comentarios inválidos
2. ? Configuración inconsistente de base de datos
3. ? Librerías jQuery Validation instaladas
4. ? Rutas de scripts corregidas en todas las vistas
5. ? AutoMigrate y SeedData habilitados
6. ? Script de diagnóstico creado

### **Estado actual:**
- ? Compilación exitosa
- ? Base de datos configurada (SQLite en desarrollo)
- ? Usuarios de prueba disponibles
- ? Validaciones funcionando
- ? Sistema listo para usar

### **Para iniciar:**
```powershell
cd HotelSuite
dotnet run
```

**Luego abrir:** https://localhost:5001/Account/Login  
**Usuario:** admin@hotelsuite.com  
**Contraseña:** Admin123!

---

**¡El sistema debería funcionar perfectamente ahora!** ??

**Versión:** 2.0.0  
**Última actualización:** 2025  
**Sistema:** HotelSuite HMS  
**Tecnologías:** ASP.NET Core 9, Identity, SQLite/SQL Server, jQuery Validation
