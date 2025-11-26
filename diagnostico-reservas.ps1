# Script de diagnóstico para problema de reservas
Write-Host "=== DIAGNÓSTICO: Problema de Acceso a Crear Reserva ===" -ForegroundColor Cyan
Write-Host ""

# 1. Verificar si hay huéspedes en la BD
Write-Host "1. Verificando huéspedes en la base de datos..." -ForegroundColor Yellow
$queryHuespedes = "SELECT COUNT(*) AS Total FROM Huespedes"
try {
    $totalHuespedes = sqlcmd -S "(localdb)\mssqllocaldb" -d HotelSuiteDb -Q $queryHuespedes -h -1 2>&1
    Write-Host "   ? Total de huéspedes: $totalHuespedes" -ForegroundColor Green
    
    if ([int]$totalHuespedes -eq 0) {
        Write-Host "   ?? WARNING: No hay huéspedes registrados en la base de datos" -ForegroundColor Red
        Write-Host "   Solución: Ejecuta el seeder o registra huéspedes manualmente" -ForegroundColor Yellow
    }
} catch {
    Write-Host "   ? Error al consultar huéspedes: $_" -ForegroundColor Red
}

Write-Host ""

# 2. Verificar si hay habitaciones disponibles
Write-Host "2. Verificando habitaciones disponibles..." -ForegroundColor Yellow
$queryHabitaciones = "SELECT COUNT(*) AS Total FROM Habitaciones WHERE Estado = 'Disponible'"
try {
    $totalDisponibles = sqlcmd -S "(localdb)\mssqllocaldb" -d HotelSuiteDb -Q $queryHabitaciones -h -1 2>&1
    Write-Host "   ? Habitaciones disponibles: $totalDisponibles" -ForegroundColor Green
    
    if ([int]$totalDisponibles -eq 0) {
        Write-Host "?? WARNING: No hay habitaciones disponibles" -ForegroundColor Red
        Write-Host "   Solución: Cancela alguna reserva o cambia estado de habitaciones" -ForegroundColor Yellow
        
   # Mostrar estados actuales
        Write-Host "   Estados de habitaciones:" -ForegroundColor Yellow
$queryEstados = "SELECT Estado, COUNT(*) AS Total FROM Habitaciones GROUP BY Estado"
 sqlcmd -S "(localdb)\mssqllocaldb" -d HotelSuiteDb -Q $queryEstados 2>&1
    }
} catch {
    Write-Host "   ? Error al consultar habitaciones: $_" -ForegroundColor Red
}

Write-Host ""

# 3. Verificar roles de usuario
Write-Host "3. Verificando roles del sistema..." -ForegroundColor Yellow
$queryRoles = "SELECT Name FROM AspNetRoles ORDER BY Name"
try {
    Write-Host "   Roles disponibles:" -ForegroundColor Green
    sqlcmd -S "(localdb)\mssqllocaldb" -d HotelSuiteDb -Q $queryRoles 2>&1
} catch {
    Write-Host "   ? Error al consultar roles: $_" -ForegroundColor Red
}

Write-Host ""

# 4. Verificar usuarios con roles apropiados
Write-Host "4. Verificando usuarios con permisos para crear reservas..." -ForegroundColor Yellow
$queryUsuariosConRol = @"
SELECT 
    u.UserName,
    r.Name AS Rol
FROM AspNetUsers u
INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE r.Name IN ('Administrador', 'Gerente', 'Recepcionista')
ORDER BY r.Name, u.UserName
"@
try {
    Write-Host "   Usuarios con permisos:" -ForegroundColor Green
  sqlcmd -S "(localdb)\mssqllocaldb" -d HotelSuiteDb -Q $queryUsuariosConRol 2>&1
} catch {
    Write-Host "   ? Error al consultar usuarios: $_" -ForegroundColor Red
}

Write-Host ""

# 5. Probar la ruta directamente
Write-Host "5. Verificando configuración de la aplicación..." -ForegroundColor Yellow
$appsettingsPath = "HotelSuite\appsettings.json"
if (Test-Path $appsettingsPath) {
    $appsettings = Get-Content $appsettingsPath | ConvertFrom-Json
    Write-Host "   ? Archivo appsettings.json encontrado" -ForegroundColor Green
    Write-Host "   AutoMigrate: $($appsettings.ApplicationSettings.AutoMigrate)" -ForegroundColor Cyan
    Write-Host "   SeedData: $($appsettings.ApplicationSettings.SeedData)" -ForegroundColor Cyan
} else {
    Write-Host "   ? No se encontró appsettings.json" -ForegroundColor Red
}

Write-Host ""

# 6. Soluciones sugeridas
Write-Host "=== SOLUCIONES SUGERIDAS ===" -ForegroundColor Cyan
Write-Host ""

Write-Host "Si no puedes acceder a /Reservas/Create, verifica:" -ForegroundColor Yellow
Write-Host "1. ? Estás autenticado (logged in)" -ForegroundColor White
Write-Host "2. ? Tu usuario tiene rol: Administrador, Gerente o Recepcionista" -ForegroundColor White
Write-Host "3. ? Hay al menos 1 huésped registrado" -ForegroundColor White
Write-Host "4. ? Hay al menos 1 habitación con estado 'Disponible'" -ForegroundColor White
Write-Host ""

Write-Host "ACCIONES RÁPIDAS:" -ForegroundColor Cyan
Write-Host ""

Write-Host "A. Si no hay huéspedes:" -ForegroundColor Yellow
Write-Host "   Opción 1: Ejecuta el seeder" -ForegroundColor White
Write-Host "   dotnet run --project HotelSuite" -ForegroundColor Gray
Write-Host ""
Write-Host "   Opción 2: Registra un huésped manualmente" -ForegroundColor White
Write-Host "   Ve a: https://localhost:5001/Huespedes/Create" -ForegroundColor Gray
Write-Host ""

Write-Host "B. Si no hay habitaciones disponibles:" -ForegroundColor Yellow
Write-Host "   Opción 1: Cancela una reserva existente" -ForegroundColor White
Write-Host "   Ve a: https://localhost:5001/Reservas" -ForegroundColor Gray
Write-Host ""
Write-Host "   Opción 2: Cambia estado de habitación a 'Disponible'" -ForegroundColor White
Write-Host "   Ve a: https://localhost:5001/Habitaciones" -ForegroundColor Gray
Write-Host ""

Write-Host "C. Si no tienes los roles correctos:" -ForegroundColor Yellow
Write-Host "   Inicia sesión con el usuario administrador:" -ForegroundColor White
Write-Host "   Usuario: admin@hotelsuite.com" -ForegroundColor Gray
Write-Host "   Contraseña: Admin123!" -ForegroundColor Gray
Write-Host ""

Write-Host "=== FIN DEL DIAGNÓSTICO ===" -ForegroundColor Cyan
