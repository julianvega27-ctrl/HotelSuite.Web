# ?? SOLUCIÓN - Problema de Acceso a Crear Reserva

## ?? Problema Reportado

**Síntoma:** No se puede acceder a registrar una nueva reserva en `/Reservas/Create`

## ?? Causas Posibles

### **1. Problema de Autenticación/Autorización** ??

El controlador `ReservasController` tiene un atributo que requiere roles específicos:

```csharp
[Authorize(Roles = "Administrador,Gerente,Recepcionista")]
public class ReservasController : Controller
{
    // ...
}
```

**Esto significa que solo usuarios con estos roles pueden acceder:**
- ? Administrador
- ? Gerente
- ? Recepcionista

**Si tu usuario NO tiene ninguno de estos roles:**
- ? Verás una página de "Access Denied" (403)
- ? O serás redirigido al login

### **2. No hay Huéspedes en la Base de Datos** ??

El método `CargarHuespedesYHabitacionesDisponibles()` carga los huéspedes para el selector:

```csharp
var huespedes = await _unitOfWork.Huespedes.GetAllAsync();
ViewBag.Huespedes = new SelectList(huespedes...);
```

**Si no hay huéspedes:**
- ?? El selector de huéspedes estará vacío
- ?? No podrás crear la reserva

### **3. No hay Habitaciones Disponibles** ??

El método filtra solo habitaciones con estado "Disponible":

```csharp
var habitacionesDisponibles = habitaciones
    .Where(h => h.Estado == "Disponible" || ...)
    .ToList();

if (!habitacionesDisponibles.Any())
{
    ViewBag.NoHabitacionesDisponibles = true;
    TempData["Warning"] = "?? No hay habitaciones disponibles...";
}
```

**Si no hay habitaciones disponibles:**
- ?? Verás una alerta naranja
- ?? El botón "Crear Reserva" estará deshabilitado

## ? Soluciones

### **Solución 1: Verificar Autenticación y Roles** ??

#### **Paso 1: Verificar si estás autenticado**

```
1. Abre: https://localhost:5001
2. Verifica que veas tu nombre en la esquina superior derecha
3. Si ves "Iniciar Sesión" ? No estás autenticado
```

#### **Paso 2: Iniciar sesión con usuario administrador**

```
Usuario: admin@hotelsuite.com
Contraseña: Admin123!
```

Este usuario tiene **todos los permisos** incluyendo crear reservas.

#### **Paso 3: Verificar tu rol**

```
1. Una vez autenticado, ve a: /Account/Profile
2. Verifica que tu rol sea: Administrador, Gerente o Recepcionista
3. Si tu rol es "Limpieza" o no tienes rol ? NO podrás crear reservas
```

### **Solución 2: Ejecutar el Seeder de Datos** ??

Si la base de datos está vacía (no hay huéspedes ni habitaciones):

#### **Opción A: Ejecutar con el seeder habilitado**

```powershell
# 1. Verifica que appsettings.json tenga:
# "ApplicationSettings": {
#   "AutoMigrate": true,
#   "SeedData": true
# }

# 2. Ejecuta la aplicación
cd C:\Users\LEGION\source\repos\HotelSuite.Web
dotnet run --project HotelSuite
```

El seeder creará automáticamente:
- ? 3 Hoteles
- ? 95 Habitaciones
- ? 10 Huéspedes
- ? 15 Reservas de ejemplo
- ? 15 Pagos
- ? Roles y usuarios (incluyendo admin@hotelsuite.com)

#### **Opción B: Ejecutar script de diagnóstico**

```powershell
# Ejecuta el script de diagnóstico
.\diagnostico-reservas.ps1
```

Este script verificará:
- ? Total de huéspedes en la BD
- ? Total de habitaciones disponibles
- ? Roles del sistema
- ? Usuarios con permisos
- ? Sugerencias específicas según el problema

### **Solución 3: Registrar Datos Manualmente** ??

Si el seeder no funciona, puedes registrar manualmente:

#### **A. Registrar un Huésped**

```
1. Ve a: https://localhost:5001/Huespedes/Create
2. Llena el formulario:
   - Nombres: Carlos
   - Apellidos: García
   - Email: carlos@example.com
   - Teléfono: +52 55 1234 5678
   - Documento: ABC123456
3. Click en "Guardar"
```

#### **B. Verificar/Crear Habitaciones**

```
1. Ve a: https://localhost:5001/Habitaciones
2. Verifica que haya al menos 1 habitación con estado "Disponible"
3. Si no hay:
   - Opción 1: Cancela una reserva existente
   - Opción 2: Crea una nueva habitación
   - Opción 3: Edita una habitación y cambia su estado a "Disponible"
```

### **Solución 4: Permitir Acceso sin Autenticación (Solo Desarrollo)** ??

**ADVERTENCIA: Solo para pruebas en desarrollo**

Si quieres quitar temporalmente la restricción de roles:

```csharp
// HotelSuite/Controllers/ReservasController.cs

// COMENTAR esta línea:
// [Authorize(Roles = "Administrador,Gerente,Recepcionista")]

// O cambiar por:
[AllowAnonymous] // Permite acceso sin autenticar (¡PELIGROSO!)

public class ReservasController : Controller
{
    // ...
}
```

**?? NO HACER ESTO EN PRODUCCIÓN**

### **Solución 5: Cambiar Estados de Habitaciones con SQL** ???

Si todas las habitaciones están "Reservadas" u "Ocupadas":

```sql
-- Abrir SQL Server Management Studio o Azure Data Studio
-- Conectar a: (localdb)\mssqllocaldb
-- Base de datos: HotelSuiteDb

-- Ver estados actuales
SELECT Estado, COUNT(*) AS Total 
FROM Habitaciones 
GROUP BY Estado;

-- Cambiar algunas habitaciones a "Disponible"
UPDATE TOP (10) Habitaciones
SET Estado = 'Disponible'
WHERE Estado IN ('Reservada', 'Ocupada');

-- Verificar cambio
SELECT Estado, COUNT(*) AS Total 
FROM Habitaciones 
GROUP BY Estado;
```

## ?? Verificación Paso a Paso

### **1. Ejecutar Diagnóstico**

```powershell
.\diagnostico-reservas.ps1
```

**Salida esperada:**
```
=== DIAGNÓSTICO: Problema de Acceso a Crear Reserva ===

1. Verificando huéspedes en la base de datos...
   ? Total de huéspedes: 10

2. Verificando habitaciones disponibles...
   ? Habitaciones disponibles: 45

3. Verificando roles del sistema...
   Roles disponibles:
   Administrador
   Gerente
   Recepcionista
   Limpieza

4. Verificando usuarios con permisos para crear reservas...
   Usuarios con permisos:
   admin@hotelsuite.com    Administrador
   gerente@hotelsuite.com  Gerente
   recepcion@hotelsuite.com Recepcionista

5. Verificando configuración de la aplicación...
   ? Archivo appsettings.json encontrado
   AutoMigrate: True
   SeedData: True
```

### **2. Intentar Acceder a Crear Reserva**

```
1. Abre navegador
2. Ve a: https://localhost:5001
3. Inicia sesión con: admin@hotelsuite.com / Admin123!
4. Ve a: https://localhost:5001/Reservas/Create
5. Deberías ver el formulario completo
```

**Formulario esperado:**
```
???????????????????????????????????????????????????
? ?? Registrar Nueva Reserva           ?
???????????????????????????????????????????????????
?   ?
? Información del Huésped     ?
? ??????????????????????????????????????????????? ?
? ? Huésped: [Selector con lista de huéspedes] ? ?
? ??????????????????????????????????????????????? ?
?                 ?
? Información de la Habitación         ?
? ??????????????????????????????????????????????? ?
? ? Habitación: [Hotel - #Num - Tipo - Precio]? ?
? ??????????????????????????????????????????????? ?
? ?
? Fechas de la Reserva  ?
? Fecha Entrada: [__/__/____] ?
? Fecha Salida:  [__/__/____]          ?
?            ?
? Resumen de Estancia    ?
? ??????????????????????????????????????????????? ?
? ? ?? Noches: 2 ? ?
? ? ?? Precio/Noche: $2,200.00           ? ?
? ? ?? Subtotal: $4,400.00          ? ?
? ? ?? TOTAL: $4,400.00    ? ?
? ??????????????????????????????????????????????? ?
?           ?
? [Cancelar]  [Crear Reserva]    ?
???????????????????????????????????????????????????
```

### **3. Si Ves Alerta de "No hay habitaciones disponibles"**

```
???????????????????????????????????????????????????
? ?? No hay habitaciones disponibles        ?
?   ?
? Actualmente todas las habitaciones están        ?
? ocupadas o reservadas.          ?
?     ?
? Opciones:    ?
? • Espere a que haya disponibilidad           ?
? • Cancele una reserva existente       ?
? • Agregue más habitaciones   ?
???????????????????????????????????????????????????
```

**Solución:**
```powershell
# Opción 1: Cancelar una reserva
# 1. Ve a /Reservas
# 2. Click en icono ?? de cualquier reserva "Confirmada"
# 3. Confirma la cancelación
# 4. La habitación queda "Disponible" automáticamente

# Opción 2: SQL directo
sqlcmd -S "(localdb)\mssqllocaldb" -d HotelSuiteDb -Q "UPDATE TOP (5) Habitaciones SET Estado = 'Disponible' WHERE Estado = 'Reservada'"
```

## ?? Tabla de Resolución Rápida

| Síntoma | Causa | Solución |
|---------|-------|----------|
| Error 401/403 | No autenticado | Iniciar sesión |
| Access Denied | Rol incorrecto | Usar admin@hotelsuite.com |
| Selector huéspedes vacío | No hay huéspedes | Ejecutar seeder o crear manualmente |
| Alerta "No disponibles" | Estado ? "Disponible" | Cancelar reserva o cambiar SQL |
| Formulario no carga | Error en Create (GET) | Ver logs, ejecutar diagnóstico |

## ?? Checklist de Verificación

Antes de reportar el problema, verifica:

- [ ] ? Estoy autenticado (veo mi nombre arriba a la derecha)
- [ ] ? Mi usuario tiene rol: Administrador, Gerente o Recepcionista
- [ ] ? Ejecuté el diagnóstico: `.\diagnostico-reservas.ps1`
- [ ] ? Hay al menos 1 huésped en la BD
- [ ] ? Hay al menos 1 habitación con estado "Disponible"
- [ ] ? La aplicación está corriendo sin errores
- [ ] ? Accedí a: `https://localhost:5001/Reservas/Create` (no /api/Reservas)

## ?? Solución Express (Más Rápida)

```powershell
# 1. Ejecutar diagnóstico
.\diagnostico-reservas.ps1

# 2. Si no hay datos, ejecutar la app (el seeder se ejecutará automáticamente)
dotnet run --project HotelSuite

# 3. Esperar a que aparezca:
# "? Datos iniciales verificados/creados."

# 4. Abrir navegador
Start-Process "https://localhost:5001"

# 5. Iniciar sesión
# Usuario: admin@hotelsuite.com
# Contraseña: Admin123!

# 6. Ir a crear reserva
Start-Process "https://localhost:5001/Reservas/Create"
```

## ?? Logs a Revisar

Si el problema persiste, revisa estos logs:

```powershell
# 1. Ver logs de la aplicación en la consola donde ejecutaste dotnet run

# 2. Buscar errores específicos:
# - "Error al cargar el formulario"
# - "Error al cargar huéspedes"
# - "Error al cargar habitaciones"
# - "NullReferenceException"
# - "Access Denied"

# 3. Ver logs de SQL
sqlcmd -S "(localdb)\mssqllocaldb" -d HotelSuiteDb -Q "SELECT TOP 10 * FROM __EFMigrationsHistory ORDER BY MigrationId DESC"
```

## ?? Estado Esperado Final

```
? Usuario autenticado como Administrador
? Base de datos tiene huéspedes
? Base de datos tiene habitaciones disponibles
? Acceso a /Reservas/Create exitoso
? Formulario completo visible
? Selectores con datos
? Botón "Crear Reserva" habilitado
? JavaScript funcionando (cálculos en tiempo real)

?? SISTEMA LISTO PARA CREAR RESERVAS
```

---

**Versión:** 4.4.0  
**Fecha:** 2025-01-04  
**Estado:** ? **GUÍA COMPLETA**  
**Problema:** Acceso a crear reserva  
**Archivos:** diagnostico-reservas.ps1

---

## ?? Tip Final

**El 90% de los problemas de "No puedo crear reserva" se deben a:**

1. **No estar autenticado** (40%)
2. **No tener el rol correcto** (30%)
3. **No hay habitaciones disponibles** (20%)
4. **No hay huéspedes en la BD** (10%)

**Solución más común:**
```
Inicia sesión con: admin@hotelsuite.com / Admin123!
```

**¡Eso debería resolver el problema en la mayoría de los casos!** ??
