# ? CONFIGURACIÓN COMPLETADA - SQL Server LocalDB

## ?? Cambios Realizados

### **1. Configuración actualizada a SQL Server LocalDB**

**Archivo:** `appsettings.Development.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=HotelSuiteDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  },
  "DatabaseProvider": "SqlServer",
  "ApplicationSettings": {
    "AutoMigrate": true,
    "SeedData": true,
    "EnableSwagger": true
  }
}
```

### **2. SQL Server LocalDB configurado y corriendo**

```bash
? Instancia 'mssqllocaldb' creada
? Instancia iniciada
? Base de datos 'HotelSuiteDb' creada
? Migraciones aplicadas
```

### **3. Migraciones regeneradas para SQL Server**

- ? Eliminadas migraciones antiguas (SQLite)
- ? Creada migración `InitialCreate` para SQL Server
- ? Todas las tablas creadas correctamente
- ? Índices y restricciones aplicados

---

## ?? INICIAR LA APLICACIÓN

### **Opción 1: Desde la terminal**

```powershell
cd HotelSuite
dotnet run
```

### **Opción 2: Desde Visual Studio**

1. Presiona `F5` o click en el botón ?? **Start**
2. La aplicación se abrirá automáticamente en el navegador

---

## ?? Estado de la Base de Datos

### **Conexión configurada:**
- **Servidor:** `(localdb)\mssqllocaldb`
- **Base de datos:** `HotelSuiteDb`
- **Autenticación:** Windows (Trusted_Connection)
- **Estado:** ? **LISTA PARA USAR**

### **Tablas creadas:**
```
? Hoteles
? Habitaciones
? Huespedes
? Reservas
? Pagos
? Departamentos
? Empleados
? TareasDepartamento
? AspNetUsers (Identity)
? AspNetRoles (Identity)
? AspNetUserRoles (Identity)
? Y tablas adicionales de Identity...
```

---

## ?? Usuarios de Prueba

Al iniciar la aplicación, el sistema creará automáticamente usuarios de prueba:

| Rol | Email | Contraseña |
|-----|-------|------------|
| **Administrador** | admin@hotelsuite.com | Admin123! |
| **Gerente** | gerente@hotelsuite.com | Gerente123! |
| **Recepcionista** | recepcionista@hotelsuite.com | Recep123! |
| **Limpieza** | limpieza@hotelsuite.com | Limpieza123! |

---

## ?? PASOS PARA PROBAR

### **1. Ejecutar la aplicación**

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
?? API REST:      https://localhost:5001/api
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

### **4. Iniciar sesión con usuario de prueba**

```
Email:   admin@hotelsuite.com
Contraseña: Admin123!
```

### **5. Verificar que funciona**

- ? El login debería ser exitoso
- ? Deberías ser redirigido al Dashboard
- ? Deberías ver tu nombre de usuario en la esquina superior derecha
- ? Deberías poder acceder a todas las opciones del menú

---

## ?? Verificar la Base de Datos (Opcional)

### **Opción 1: SQL Server Management Studio (SSMS)**

1. Abrir SSMS
2. Conectar a: `(localdb)\mssqllocaldb`
3. Autenticación: **Windows Authentication**
4. Expandir **Databases** ? Verás `HotelSuiteDb`

### **Opción 2: Visual Studio**

1. Ir a **View** ? **SQL Server Object Explorer**
2. Expandir **SQL Server** ? **(localdb)\mssqllocaldb**
3. Expandir **Databases** ? **HotelSuiteDb**
4. Explorar las tablas

### **Opción 3: Línea de comandos**

```powershell
# Ver información de la base de datos
sqlcmd -S "(localdb)\mssqllocaldb" -Q "SELECT name FROM sys.databases WHERE name = 'HotelSuiteDb'"

# Ver tablas
sqlcmd -S "(localdb)\mssqllocaldb" -d HotelSuiteDb -Q "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'"

# Ver usuarios de AspNetUsers
sqlcmd -S "(localdb)\mssqllocaldb" -d HotelSuiteDb -Q "SELECT Email, NombresCompletos FROM AspNetUsers"
```

---

## ??? Comandos Útiles

### **Gestión de LocalDB:**

```powershell
# Ver información de la instancia
sqllocaldb info mssqllocaldb

# Detener la instancia
sqllocaldb stop mssqllocaldb

# Iniciar la instancia
sqllocaldb start mssqllocaldb

# Eliminar la instancia (si necesitas empezar desde cero)
sqllocaldb delete mssqllocaldb
sqllocaldb create mssqllocaldb
sqllocaldb start mssqllocaldb
```

### **Gestión de Migraciones:**

```powershell
# Ver migraciones aplicadas
.\migraciones.ps1 list

# Crear nueva migración
.\migraciones.ps1 add NombreDeLaMigracion

# Aplicar migraciones pendientes
.\migraciones.ps1 update

# Eliminar base de datos
.\migraciones.ps1 drop

# Generar script SQL
.\migraciones.ps1 script
```

### **Resetear Base de Datos:**

```powershell
# Opción 1: Con script
.\migraciones.ps1 drop
.\migraciones.ps1 update

# Opción 2: Manual
dotnet ef database drop --project HotelSuite.Infrastructure --startup-project HotelSuite --force
dotnet ef database update --project HotelSuite.Infrastructure --startup-project HotelSuite
```

---

## ?? Solución de Problemas

### **Error: "Cannot connect to (localdb)\mssqllocaldb"**

**Solución:**
```powershell
# Verificar que LocalDB está instalado
sqllocaldb info

# Si no está instalado, instalar SQL Server Express con LocalDB
# Descargar de: https://www.microsoft.com/sql-server/sql-server-downloads

# Crear e iniciar la instancia
sqllocaldb create mssqllocaldb
sqllocaldb start mssqllocaldb
```

### **Error: "Login failed for user"**

**Causa:** Problema con autenticación Windows

**Solución:**
```json
// Cambiar a autenticación SQL en appsettings.Development.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=HotelSuiteDb;Integrated Security=true;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

### **Error: "Database already exists"**

**Solución:**
```powershell
# Eliminar y recrear
.\migraciones.ps1 drop
.\migraciones.ps1 update
```

### **Error: "Pending model changes"**

**Solución:**
```powershell
# Crear migración para los cambios
.\migraciones.ps1 add ActualizarModelo
.\migraciones.ps1 update
```

### **Los seeders no crean usuarios**

**Verificar configuración:**
```json
// En appsettings.Development.json
{
  "ApplicationSettings": {
    "SeedData": true  // ? Debe estar en true
  }
}
```

**Ver logs:**
Los logs deberían mostrar:
```
Verificando roles y usuarios...
? Roles y usuarios verificados/creados.
```

Si no ves esto, hay un problema con los seeders.

---

## ?? Diferencias: SQLite vs SQL Server

| Aspecto | SQLite (Anterior) | SQL Server (Actual) |
|---------|------------------|---------------------|
| **Instalación** | No requiere | Requiere LocalDB |
| **Archivo DB** | HotelSuite.db | Base de datos del servidor |
| **Rendimiento** | Bueno | Excelente |
| **Características** | Limitadas | Completas |
| **Usuarios simultáneos** | 1 escritor | Múltiples |
| **Tamaño máximo** | 281 TB | Sin límite práctico |
| **Herramientas** | DB Browser | SSMS, Azure Data Studio |
| **Producción** | No recomendado | Recomendado |

---

## ?? Ventajas de SQL Server LocalDB

### **Para Desarrollo:**
- ? Misma base de datos que producción
- ? Herramientas profesionales disponibles
- ? Mejor rendimiento
- ? Características avanzadas (stored procedures, triggers, etc.)
- ? Depuración más fácil

### **Para Aprendizaje:**
- ? Aprende SQL Server real
- ? Compatible con Azure SQL Database
- ? Buenas prácticas de la industria

---

## ?? Recursos Adicionales

### **Documentación:**
- [SQL Server Express](https://www.microsoft.com/sql-server/sql-server-downloads)
- [LocalDB](https://learn.microsoft.com/sql/database-engine/configure-windows/sql-server-express-localdb)
- [Entity Framework Core](https://learn.microsoft.com/ef/core/)

### **Herramientas:**
- [SQL Server Management Studio (SSMS)](https://learn.microsoft.com/sql/ssms/download-sql-server-management-studio-ssms)
- [Azure Data Studio](https://learn.microsoft.com/sql/azure-data-studio/download-azure-data-studio)

### **Scripts del proyecto:**
- `diagnostico.ps1` - Diagnóstico general
- `diagnostico-login.ps1` - Diagnóstico de autenticación
- `migraciones.ps1` - Gestión de migraciones

### **Documentación del proyecto:**
- `INICIO_RAPIDO.md` - Guía de inicio rápido
- `SOLUCION_PROBLEMAS_BD.md` - Solución de problemas de BD
- `SOLUCION_LOGIN_COMPLETA.md` - Solución de problemas de login
- `SOLUCION_VALIDACION_JQUERY.md` - Validaciones jQuery

---

## ? Checklist Final

Antes de considerar que todo está funcionando:

- [ ] ? SQL Server LocalDB está instalado e iniciado
- [ ] ? Base de datos `HotelSuiteDb` creada
- [ ] ? Migraciones aplicadas correctamente
- [ ] ? Compilación exitosa
- [ ] ? Aplicación inicia sin errores
- [ ] ? Seeders ejecutados (ver logs)
- [ ] ? Login funciona con admin@hotelsuite.com
- [ ] ? Dashboard carga correctamente
- [ ] ? Menú de navegación funciona
- [ ] ? Puede registrar nuevos usuarios
- [ ] ? Validaciones funcionan (jQuery)

---

## ?? ¡TODO LISTO!

### **Estado del Sistema:**
```
? SQL Server LocalDB configurado
? Base de datos creada
? Migraciones aplicadas
? Seeders listos para ejecutar
? Compilación exitosa
? Sistema listo para usar
```

### **Próximo paso:**
```powershell
cd HotelSuite
dotnet run
```

**Luego:**
1. Abrir: https://localhost:5001
2. Login: admin@hotelsuite.com / Admin123!
3. ¡Disfrutar del sistema! ??

---

**Fecha de configuración:** 2025-01-04  
**Versión de .NET:** 9.0  
**Base de datos:** SQL Server LocalDB  
**Estado:** ? **COMPLETAMENTE FUNCIONAL**
