# Script de Diagnóstico Automático - HotelSuite HMS
# Detecta y soluciona problemas de conexión a base de datos

Write-Host "=================================" -ForegroundColor Cyan
Write-Host "  HotelSuite HMS - Diagnóstico  " -ForegroundColor Cyan
Write-Host "=================================" -ForegroundColor Cyan
Write-Host ""

# Función para verificar SQL Server LocalDB
function Test-SqlLocalDB {
    Write-Host "[1/6] Verificando SQL Server LocalDB..." -ForegroundColor Yellow
  
    try {
 $localDbInfo = sqllocaldb info 2>$null
        
      if ($LASTEXITCODE -eq 0 -and $localDbInfo) {
      Write-Host "  ? SQL Server LocalDB está instalado" -ForegroundColor Green
            Write-Host "  Instancias disponibles:" -ForegroundColor Gray
            sqllocaldb info | ForEach-Object { Write-Host "    - $_" -ForegroundColor Gray }
 
        # Verificar si mssqllocaldb está corriendo
 $status = sqllocaldb info mssqllocaldb 2>$null
         if ($status -match "State: Running") {
      Write-Host "  ? mssqllocaldb está corriendo" -ForegroundColor Green
                return "SqlServer"
  } else {
                Write-Host "  ? mssqllocaldb no está corriendo. Intentando iniciar..." -ForegroundColor Yellow
     sqllocaldb start mssqllocaldb 2>$null
          Start-Sleep -Seconds 2
      
   $status = sqllocaldb info mssqllocaldb 2>$null
  if ($status -match "State: Running") {
      Write-Host "  ? mssqllocaldb iniciado exitosamente" -ForegroundColor Green
       return "SqlServer"
     }
            }
     }
    } catch {
        Write-Host "  ? SQL Server LocalDB no está instalado" -ForegroundColor Red
    }
    
    return $null
}

# Función para verificar .NET
function Test-DotNet {
 Write-Host "[2/6] Verificando .NET SDK..." -ForegroundColor Yellow
    
    try {
     $dotnetVersion = dotnet --version 2>$null
        if ($dotnetVersion) {
            Write-Host "  ? .NET SDK $dotnetVersion instalado" -ForegroundColor Green
  return $true
        }
    } catch {
        Write-Host "  ? .NET SDK no encontrado" -ForegroundColor Red
        return $false
    }
}

# Función para verificar EF Core Tools
function Test-EFCore {
    Write-Host "[3/6] Verificando Entity Framework Core Tools..." -ForegroundColor Yellow
    
    try {
        $efVersion = dotnet ef --version 2>$null
        if ($efVersion) {
     Write-Host "  ? EF Core Tools instalado: $efVersion" -ForegroundColor Green
  return $true
   }
    } catch {}
    
    Write-Host "  ? EF Core Tools no encontrado. Instalando..." -ForegroundColor Yellow
    dotnet tool install --global dotnet-ef 2>$null
    Write-Host "  ? EF Core Tools instalado" -ForegroundColor Green
    return $true
}

# Función para actualizar appsettings
function Update-AppSettings {
    param([string]$provider)
    
    Write-Host "[4/6] Configurando base de datos..." -ForegroundColor Yellow
    
    $appSettingsPath = "HotelSuite\appsettings.Development.json"
    
    if (Test-Path $appSettingsPath) {
        $appSettings = Get-Content $appSettingsPath | ConvertFrom-Json
        
        if ($provider -eq "Sqlite") {
            $appSettings.ConnectionStrings.DefaultConnection = "Data Source=HotelSuite.db"
            $appSettings.DatabaseProvider = "Sqlite"
    Write-Host "  ? Configurado para usar SQLite" -ForegroundColor Green
 } else {
            $appSettings.ConnectionStrings.DefaultConnection = "Server=(localdb)\mssqllocaldb;Database=HotelSuiteDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
            $appSettings.DatabaseProvider = "SqlServer"
       Write-Host "  ? Configurado para usar SQL Server LocalDB" -ForegroundColor Green
}
        
        $appSettings | ConvertTo-Json -Depth 10 | Set-Content $appSettingsPath
     return $true
    }
    
    Write-Host "  ? No se encontró appsettings.Development.json" -ForegroundColor Red
    return $false
}

# Función para compilar proyecto
function Build-Project {
    Write-Host "[5/6] Compilando proyecto..." -ForegroundColor Yellow
    
    dotnet build HotelSuite.sln --verbosity quiet
    
    if ($LASTEXITCODE -eq 0) {
  Write-Host "  ? Compilación exitosa" -ForegroundColor Green
        return $true
 } else {
        Write-Host "  ? Error en la compilación" -ForegroundColor Red
        return $false
    }
}

# Función para aplicar migraciones
function Apply-Migrations {
    Write-Host "[6/6] Aplicando migraciones..." -ForegroundColor Yellow
    
    # Eliminar base de datos existente si hay problemas
    if (Test-Path "HotelSuite\HotelSuite.db") {
        Write-Host "  ? Eliminando base de datos SQLite anterior..." -ForegroundColor Yellow
        Remove-Item "HotelSuite\HotelSuite.db" -Force
    }
    
    # Aplicar migraciones
    dotnet ef database update --project HotelSuite.Infrastructure --startup-project HotelSuite 2>$null
    
    if ($LASTEXITCODE -eq 0) {
   Write-Host "  ? Migraciones aplicadas exitosamente" -ForegroundColor Green
        return $true
    } else {
        Write-Host "  ? No se pudieron aplicar migraciones (se aplicarán automáticamente al iniciar)" -ForegroundColor Yellow
        return $true
    }
}

# Ejecución principal
Write-Host ""
$dotnetOk = Test-DotNet

if (-not $dotnetOk) {
    Write-Host ""
    Write-Host "=================================" -ForegroundColor Red
    Write-Host "  ERROR: .NET SDK no encontrado  " -ForegroundColor Red
    Write-Host "=================================" -ForegroundColor Red
    Write-Host ""
    Write-Host "Por favor instala .NET 9 SDK desde:" -ForegroundColor Yellow
    Write-Host "https://dotnet.microsoft.com/download/dotnet/9.0" -ForegroundColor Cyan
    exit 1
}

$efOk = Test-EFCore
$sqlServerAvailable = Test-SqlLocalDB

# Decidir qué proveedor usar
$provider = "Sqlite"
if ($sqlServerAvailable -eq "SqlServer") {
    Write-Host ""
  Write-Host "¿Qué base de datos deseas usar?" -ForegroundColor Cyan
    Write-Host "1. SQLite (recomendado para desarrollo rápido)" -ForegroundColor White
    Write-Host "2. SQL Server LocalDB (más características)" -ForegroundColor White
    Write-Host ""
    
    $choice = Read-Host "Selecciona una opción (1 o 2) [Por defecto: 1]"
    
    if ($choice -eq "2") {
        $provider = "SqlServer"
    }
} else {
    Write-Host ""
    Write-Host "Usando SQLite (SQL Server LocalDB no disponible)" -ForegroundColor Yellow
}

$configOk = Update-AppSettings -provider $provider
$buildOk = Build-Project

if ($buildOk) {
    Apply-Migrations
}

Write-Host ""
Write-Host "=================================" -ForegroundColor Cyan
Write-Host "       Diagnóstico Completo      " -ForegroundColor Cyan
Write-Host "=================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Estado de Componentes:" -ForegroundColor White
Write-Host "  .NET SDK: $(if ($dotnetOk) { '?' } else { '?' })" -ForegroundColor $(if ($dotnetOk) { 'Green' } else { 'Red' })
Write-Host "  EF Core Tools:   $(if ($efOk) { '?' } else { '?' })" -ForegroundColor $(if ($efOk) { 'Green' } else { 'Red' })
Write-Host "  Configuración:   $(if ($configOk) { '?' } else { '?' })" -ForegroundColor $(if ($configOk) { 'Green' } else { 'Red' })
Write-Host "  Compilación:     $(if ($buildOk) { '?' } else { '?' })" -ForegroundColor $(if ($buildOk) { 'Green' } else { 'Red' })
Write-Host ""

if ($buildOk -and $configOk) {
    Write-Host "? Sistema listo para ejecutar" -ForegroundColor Green
    Write-Host ""
    Write-Host "Base de datos configurada: $provider" -ForegroundColor Cyan
    Write-Host ""
 Write-Host "Para iniciar la aplicación, ejecuta:" -ForegroundColor Yellow
    Write-Host "  cd HotelSuite" -ForegroundColor White
  Write-Host "  dotnet run" -ForegroundColor White
    Write-Host ""
    Write-Host "URLs de acceso:" -ForegroundColor Yellow
    Write-Host "  Web:     http://localhost:5000" -ForegroundColor Cyan
    Write-Host "  API:   http://localhost:5000/api/docs" -ForegroundColor Cyan
    Write-Host "  Swagger: http://localhost:5000/api/docs" -ForegroundColor Cyan
    Write-Host ""
    
 # Preguntar si desea iniciar automáticamente
  $autoStart = Read-Host "¿Deseas iniciar la aplicación ahora? (S/N) [Por defecto: S]"
    
 if ($autoStart -eq "" -or $autoStart -eq "S" -or $autoStart -eq "s") {
   Write-Host ""
        Write-Host "Iniciando HotelSuite HMS..." -ForegroundColor Green
Write-Host ""
        Set-Location HotelSuite
        dotnet run
    }
} else {
    Write-Host "? Hay problemas que deben resolverse" -ForegroundColor Red
    Write-Host ""
    Write-Host "Revisa el archivo SOLUCION_PROBLEMAS_BD.md para más información" -ForegroundColor Yellow
}
