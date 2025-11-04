# Script de Diagnóstico de Login/Register - HotelSuite HMS
# Verifica todos los componentes necesarios para que funcione el sistema de autenticación

Write-Host "????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "  HotelSuite HMS - Diagnóstico de Autenticación     " -ForegroundColor Cyan
Write-Host "????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

$ErrorCount = 0
$WarningCount = 0

# 1. Verificar librerías jQuery
Write-Host "[1/8] Verificando librerías JavaScript..." -ForegroundColor Yellow

$requiredLibs = @(
    "HotelSuite\wwwroot\lib\jquery\jquery.min.js",
    "HotelSuite\wwwroot\lib\jquery-validation\jquery.validate.min.js",
    "HotelSuite\wwwroot\lib\jquery-validation-unobtrusive\jquery.validate.unobtrusive.min.js",
    "HotelSuite\wwwroot\lib\bootstrap\js\bootstrap.bundle.min.js"
)

foreach ($lib in $requiredLibs) {
    if (Test-Path $lib) {
   Write-Host "  ? $(Split-Path $lib -Leaf)" -ForegroundColor Green
    } else {
        Write-Host "  ? $(Split-Path $lib -Leaf) - NO ENCONTRADO" -ForegroundColor Red
        $ErrorCount++
    }
}

# 2. Verificar configuración de base de datos
Write-Host ""
Write-Host "[2/8] Verificando configuración de base de datos..." -ForegroundColor Yellow

if (Test-Path "HotelSuite\appsettings.Development.json") {
    $config = Get-Content "HotelSuite\appsettings.Development.json" | ConvertFrom-Json
    
    if ($config.ConnectionStrings.DefaultConnection) {
        Write-Host "  ? Cadena de conexión configurada" -ForegroundColor Green

        $provider = $config.DatabaseProvider
        if ($provider) {
         Write-Host "  ? Proveedor de BD: $provider" -ForegroundColor Green
   } else {
          Write-Host "  ? Proveedor de BD no especificado (usando SqlServer por defecto)" -ForegroundColor Yellow
$WarningCount++
        }
    } else {
   Write-Host "  ? No se encontró cadena de conexión" -ForegroundColor Red
        $ErrorCount++
    }
} else {
    Write-Host "  ? appsettings.Development.json no encontrado" -ForegroundColor Red
    $ErrorCount++
}

# 3. Verificar archivo de base de datos SQLite (si aplica)
Write-Host ""
Write-Host "[3/8] Verificando base de datos..." -ForegroundColor Yellow

if ($config.DatabaseProvider -eq "Sqlite") {
    $dbPath = "HotelSuite\HotelSuite.db"
 if (Test-Path $dbPath) {
        $dbSize = (Get-Item $dbPath).Length / 1KB
        Write-Host "  ? Base de datos SQLite existe (${dbSize:N2} KB)" -ForegroundColor Green
} else {
 Write-Host "  ? Base de datos SQLite no existe (se creará al iniciar)" -ForegroundColor Yellow
        $WarningCount++
    }
} else {
    Write-Host "  ? Usando SQL Server - no se puede verificar sin conexión" -ForegroundColor Cyan
}

# 4. Verificar compilación del proyecto
Write-Host ""
Write-Host "[4/8] Verificando compilación..." -ForegroundColor Yellow

$buildOutput = dotnet build HotelSuite.sln --nologo --verbosity quiet 2>&1

if ($LASTEXITCODE -eq 0) {
    Write-Host "  ? Proyecto compila correctamente" -ForegroundColor Green
} else {
 Write-Host "  ? Errores de compilación detectados" -ForegroundColor Red
    $ErrorCount++
    Write-Host $buildOutput -ForegroundColor Red
}

# 5. Verificar archivos de vista
Write-Host ""
Write-Host "[5/8] Verificando archivos de vista..." -ForegroundColor Yellow

$requiredViews = @(
    "HotelSuite\Views\Account\Login.cshtml",
    "HotelSuite\Views\Account\Register.cshtml",
    "HotelSuite\Views\Shared\_Layout.cshtml",
    "HotelSuite\Views\Shared\_ValidationScriptsPartial.cshtml"
)

foreach ($view in $requiredViews) {
    if (Test-Path $view) {
        Write-Host "  ? $(Split-Path $view -Leaf)" -ForegroundColor Green
    } else {
        Write-Host "  ? $(Split-Path $view -Leaf) - NO ENCONTRADO" -ForegroundColor Red
        $ErrorCount++
    }
}

# 6. Verificar controlador de Account
Write-Host ""
Write-Host "[6/8] Verificando AccountController..." -ForegroundColor Yellow

if (Test-Path "HotelSuite\Controllers\AccountController.cs") {
    $controllerContent = Get-Content "HotelSuite\Controllers\AccountController.cs" -Raw
    
    if ($controllerContent -match "public async Task<IActionResult> Login\(LoginViewModel model\)") {
        Write-Host "  ? Método Login encontrado" -ForegroundColor Green
    } else {
        Write-Host "  ? Método Login no encontrado" -ForegroundColor Red
        $ErrorCount++
  }
    
    if ($controllerContent -match "public async Task<IActionResult> Register\(RegisterViewModel model\)") {
  Write-Host "  ? Método Register encontrado" -ForegroundColor Green
    } else {
        Write-Host "  ? Método Register no encontrado" -ForegroundColor Red
        $ErrorCount++
    }
} else {
    Write-Host "  ? AccountController.cs no encontrado" -ForegroundColor Red
    $ErrorCount++
}

# 7. Verificar ViewModels
Write-Host ""
Write-Host "[7/8] Verificando ViewModels..." -ForegroundColor Yellow

$requiredModels = @(
    "HotelSuite\Models\LoginViewModel.cs",
    "HotelSuite\Models\RegisterViewModel.cs"
)

foreach ($model in $requiredModels) {
    if (Test-Path $model) {
        Write-Host "  ? $(Split-Path $model -Leaf)" -ForegroundColor Green
    } else {
        Write-Host "  ? $(Split-Path $model -Leaf) - NO ENCONTRADO" -ForegroundColor Red
    $ErrorCount++
    }
}

# 8. Verificar rutas en Login.cshtml
Write-Host ""
Write-Host "[8/8] Verificando rutas de scripts en Login.cshtml..." -ForegroundColor Yellow

if (Test-Path "HotelSuite\Views\Account\Login.cshtml") {
    $loginContent = Get-Content "HotelSuite\Views\Account\Login.cshtml" -Raw
    
    $correctPaths = @{
        "jquery" = '~/lib/jquery/jquery.min.js'
        "validate" = '~/lib/jquery-validation/jquery.validate.min.js'
        "unobtrusive" = '~/lib/jquery-validation-unobtrusive/jquery.validate.unobtrusive.min.js'
        "bootstrap" = '~/lib/bootstrap/js/bootstrap.bundle.min.js'
    }
    
    $allCorrect = $true
    foreach ($key in $correctPaths.Keys) {
        $path = $correctPaths[$key]
   if ($loginContent -match [regex]::Escape($path)) {
            Write-Host "  ? Ruta correcta: $key" -ForegroundColor Green
        } else {
            Write-Host "  ? Ruta incorrecta: $key" -ForegroundColor Red
        $ErrorCount++
     $allCorrect = $false
        }
  }
    
    if ($allCorrect) {
  Write-Host "  ? Todas las rutas son correctas" -ForegroundColor Green
    }
} else {
    Write-Host "  ? Login.cshtml no encontrado" -ForegroundColor Red
    $ErrorCount++
}

# Resumen
Write-Host ""
Write-Host "????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "   RESUMEN     " -ForegroundColor Cyan
Write-Host "????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

if ($ErrorCount -eq 0 -and $WarningCount -eq 0) {
    Write-Host "? Todos los componentes están correctos" -ForegroundColor Green
    Write-Host ""
    Write-Host "El sistema debería funcionar correctamente." -ForegroundColor Green
    Write-Host ""
    Write-Host "Si aún tienes problemas:" -ForegroundColor Yellow
    Write-Host "1. Limpia la caché del navegador (Ctrl+Shift+Delete)" -ForegroundColor White
    Write-Host "2. Abre las Developer Tools (F12) y revisa la consola" -ForegroundColor White
    Write-Host "3. Verifica la pestaña Network para ver si hay errores 404" -ForegroundColor White
    Write-Host "4. Asegúrate de que la base de datos se haya creado correctamente" -ForegroundColor White
    Write-Host ""
    Write-Host "Para iniciar la aplicación:" -ForegroundColor Yellow
    Write-Host "  cd HotelSuite" -ForegroundColor White
    Write-Host "  dotnet run" -ForegroundColor White
} else {
    Write-Host "? Se encontraron problemas:" -ForegroundColor Red
  Write-Host "  Errores: $ErrorCount" -ForegroundColor Red
    Write-Host "  Advertencias: $WarningCount" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "ACCIONES RECOMENDADAS:" -ForegroundColor Yellow
    Write-Host ""
    
    if ($ErrorCount -gt 0) {
     Write-Host "1. Restaurar librerías JavaScript:" -ForegroundColor White
    Write-Host "   libman restore" -ForegroundColor Cyan
      Write-Host ""
        
        Write-Host "2. Recompilar el proyecto:" -ForegroundColor White
        Write-Host "   dotnet clean" -ForegroundColor Cyan
        Write-Host "   dotnet build" -ForegroundColor Cyan
        Write-Host ""
      
        Write-Host "3. Aplicar migraciones:" -ForegroundColor White
        Write-Host "   .\migraciones.ps1 update" -ForegroundColor Cyan
        Write-Host ""
    }
    
    Write-Host "4. Consulta la documentación:" -ForegroundColor White
    Write-Host "   SOLUCION_VALIDACION_JQUERY.md" -ForegroundColor Cyan
    Write-Host "   SOLUCION_PROBLEMAS_BD.md" -ForegroundColor Cyan
}

Write-Host ""
Write-Host "????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

# Preguntar si desea ver los logs detallados
$showLogs = Read-Host "¿Deseas ver información detallada de la base de datos? (S/N) [N]"

if ($showLogs -eq "S" -or $showLogs -eq "s") {
    Write-Host ""
    Write-Host "????????????????????????????????????????????????????????" -ForegroundColor Cyan
    Write-Host "     INFORMACIÓN DE BASE DE DATOS            " -ForegroundColor Cyan
    Write-Host "????????????????????????????????????????????????????????" -ForegroundColor Cyan
    Write-Host ""
    
    if ($config.DatabaseProvider -eq "Sqlite" -and (Test-Path "HotelSuite\HotelSuite.db")) {
        Write-Host "Base de datos SQLite:" -ForegroundColor Yellow
        $db = Get-Item "HotelSuite\HotelSuite.db"
        Write-Host "  Ruta: $($db.FullName)" -ForegroundColor White
        Write-Host "  Tamaño: $([math]::Round($db.Length / 1KB, 2)) KB" -ForegroundColor White
     Write-Host "  Última modificación: $($db.LastWriteTime)" -ForegroundColor White
  Write-Host ""
     Write-Host "Para verificar usuarios en la BD, ejecuta la aplicación y revisa los logs." -ForegroundColor Cyan
    }
}

# Preguntar si desea iniciar la aplicación
Write-Host ""
$startApp = Read-Host "¿Deseas iniciar la aplicación ahora? (S/N) [N]"

if ($startApp -eq "S" -or $startApp -eq "s") {
    Write-Host ""
    Write-Host "Iniciando HotelSuite HMS..." -ForegroundColor Green
    Write-Host "Presiona Ctrl+C para detener la aplicación" -ForegroundColor Yellow
    Write-Host ""
    Set-Location HotelSuite
    dotnet run
}
