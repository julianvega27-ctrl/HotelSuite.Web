# Script de Gestión de Migraciones - HotelSuite HMS
# Ayuda a crear, aplicar y gestionar migraciones de Entity Framework Core

param(
    [Parameter(Position = 0)]
    [ValidateSet("add", "remove", "list", "update", "drop", "script", "help")]
    [string]$Action = "help",
    
    [Parameter(Position = 1)]
    [string]$Name = ""
)

$InfraProject = "HotelSuite.Infrastructure"
$StartupProject = "HotelSuite"

function Write-Header {
    param([string]$Title)
    Write-Host ""
    Write-Host "???????????????????????????????????????" -ForegroundColor Cyan
    Write-Host "  $Title" -ForegroundColor Cyan
    Write-Host "???????????????????????????????????????" -ForegroundColor Cyan
    Write-Host ""
}

function Write-Success {
    param([string]$Message)
    Write-Host "? $Message" -ForegroundColor Green
}

function Write-Info {
    param([string]$Message)
    Write-Host "? $Message" -ForegroundColor Cyan
}

function Write-Warning {
    param([string]$Message)
    Write-Host "? $Message" -ForegroundColor Yellow
}

function Write-Error {
    param([string]$Message)
    Write-Host "? $Message" -ForegroundColor Red
}

function Test-EFCoreTools {
  try {
   $version = dotnet ef --version 2>$null
        if ($version) {
 Write-Success "Entity Framework Core Tools instalado"
            return $true
        }
    } catch {}
    
 Write-Warning "Entity Framework Core Tools no encontrado"
    Write-Info "Instalando EF Core Tools..."
    
    dotnet tool install --global dotnet-ef
    
  if ($LASTEXITCODE -eq 0) {
        Write-Success "EF Core Tools instalado correctamente"
        return $true
 }
    
    Write-Error "No se pudo instalar EF Core Tools"
    return $false
}

function Add-Migration {
    param([string]$MigrationName)
 
    if ([string]::IsNullOrWhiteSpace($MigrationName)) {
        Write-Error "Debe proporcionar un nombre para la migración"
Write-Info "Uso: .\migraciones.ps1 add NombreDeLaMigracion"
        return
    }
    
    Write-Header "Creando Nueva Migración: $MigrationName"
    
    Write-Info "Verificando herramientas..."
    if (-not (Test-EFCoreTools)) {
        return
    }
    
    Write-Info "Analizando cambios en el modelo..."
    dotnet ef migrations add $MigrationName --project $InfraProject --startup-project $StartupProject
    
    if ($LASTEXITCODE -eq 0) {
        Write-Success "Migración '$MigrationName' creada exitosamente"
      Write-Info "Archivo de migración creado en: $InfraProject\Migrations\"
Write-Info ""
        Write-Info "Próximos pasos:"
        Write-Info "1. Revisa el archivo de migración generado"
        Write-Info "2. Ejecuta: .\migraciones.ps1 update (para aplicar la migración)"
    } else {
 Write-Error "Error al crear la migración"
    }
}

function Remove-LastMigration {
    Write-Header "Eliminando Última Migración"
    
    Write-Warning "Esto eliminará la última migración creada"
$confirm = Read-Host "¿Estás seguro? (S/N)"
    
    if ($confirm -ne "S" -and $confirm -ne "s") {
        Write-Info "Operación cancelada"
        return
    }
    
    Write-Info "Eliminando migración..."
    dotnet ef migrations remove --project $InfraProject --startup-project $StartupProject --force
    
    if ($LASTEXITCODE -eq 0) {
    Write-Success "Migración eliminada exitosamente"
    } else {
 Write-Error "Error al eliminar la migración"
    Write-Info "Tip: Si la migración ya fue aplicada, primero debes revertirla"
        Write-Info "Ejecuta: .\migraciones.ps1 update PreviousMigrationName"
    }
}

function Get-MigrationList {
    Write-Header "Lista de Migraciones"
    
    Write-Info "Obteniendo lista de migraciones..."
    dotnet ef migrations list --project $InfraProject --startup-project $StartupProject
    
  if ($LASTEXITCODE -eq 0) {
        Write-Info ""
    Write-Info "Leyenda:"
      Write-Info "  [Applied]   - Migración aplicada a la base de datos"
 Write-Info "  [Pending]   - Migración pendiente de aplicar"
    }
}

function Update-Database {
    param([string]$TargetMigration = "")
    
    Write-Header "Actualizando Base de Datos"
    
 if ([string]::IsNullOrWhiteSpace($TargetMigration)) {
        Write-Info "Aplicando todas las migraciones pendientes..."
   dotnet ef database update --project $InfraProject --startup-project $StartupProject
    } else {
        Write-Info "Migrando a: $TargetMigration"
        dotnet ef database update $TargetMigration --project $InfraProject --startup-project $StartupProject
    }
    
    if ($LASTEXITCODE -eq 0) {
  Write-Success "Base de datos actualizada exitosamente"
    Write-Info "Todas las migraciones han sido aplicadas"
    } else {
    Write-Error "Error al actualizar la base de datos"
    }
}

function Drop-Database {
    Write-Header "Eliminando Base de Datos"
    
    Write-Warning "??? ADVERTENCIA ???"
    Write-Warning "Esto eliminará TODA la base de datos y sus datos"
    Write-Warning "Esta operación NO se puede deshacer"
    Write-Host ""
    
    $confirm = Read-Host "¿Estás COMPLETAMENTE seguro? Escribe 'ELIMINAR' para confirmar"
 
    if ($confirm -ne "ELIMINAR") {
        Write-Info "Operación cancelada"
      return
    }
    
    Write-Info "Eliminando base de datos..."
    dotnet ef database drop --project $InfraProject --startup-project $StartupProject --force
    
    if ($LASTEXITCODE -eq 0) {
      Write-Success "Base de datos eliminada"
        Write-Info "Para recrear la base de datos, ejecuta: .\migraciones.ps1 update"
    } else {
      Write-Error "Error al eliminar la base de datos"
}
}

function Generate-SqlScript {
    param([string]$MigrationName)
    
    Write-Header "Generando Script SQL"
    
    if ([string]::IsNullOrWhiteSpace($MigrationName)) {
        Write-Info "Generando script para todas las migraciones..."
    dotnet ef migrations script --project $InfraProject --startup-project $StartupProject --output "migration-script.sql"
    } else {
     Write-Info "Generando script hasta: $MigrationName"
 dotnet ef migrations script 0 $MigrationName --project $InfraProject --startup-project $StartupProject --output "migration-script.sql"
    }
    
    if ($LASTEXITCODE -eq 0) {
        Write-Success "Script SQL generado: migration-script.sql"
        Write-Info "Puedes ejecutar este script directamente en tu base de datos"
    } else {
        Write-Error "Error al generar el script SQL"
    }
}

function Show-Help {
    Write-Header "HotelSuite HMS - Gestión de Migraciones"
    
    Write-Host "COMANDOS DISPONIBLES:" -ForegroundColor Yellow
    Write-Host ""
    
    Write-Host "  add <nombre>" -ForegroundColor Green
    Write-Host "    Crea una nueva migración con los cambios del modelo"
    Write-Host "    Ejemplo: .\migraciones.ps1 add AgregarNuevosCampos"
    Write-Host ""
  
    Write-Host "  remove" -ForegroundColor Green
    Write-Host "    Elimina la última migración (si no ha sido aplicada)"
    Write-Host "    Ejemplo: .\migraciones.ps1 remove"
    Write-Host ""
    
    Write-Host "  list" -ForegroundColor Green
    Write-Host "    Muestra todas las migraciones y su estado"
    Write-Host "    Ejemplo: .\migraciones.ps1 list"
  Write-Host ""
    
    Write-Host "  update [migración]" -ForegroundColor Green
    Write-Host "    Aplica migraciones pendientes a la base de datos"
    Write-Host "    Ejemplo: .\migraciones.ps1 update"
 Write-Host "    Ejemplo: .\migraciones.ps1 update InitialCreate (revertir)"
    Write-Host ""
    
    Write-Host "  drop" -ForegroundColor Green
    Write-Host "    Elimina completamente la base de datos"
    Write-Host "    Ejemplo: .\migraciones.ps1 drop"
    Write-Host ""
    
    Write-Host "  script [migración]" -ForegroundColor Green
    Write-Host "    Genera un script SQL con las migraciones"
    Write-Host "    Ejemplo: .\migraciones.ps1 script"
    Write-Host ""
    
    Write-Host "ESCENARIOS COMUNES:" -ForegroundColor Yellow
    Write-Host ""
    
    Write-Host "  ?? Agregar una nueva columna a una tabla:" -ForegroundColor Cyan
    Write-Host "    1. Modifica la entidad en Domain/Entities/"
    Write-Host "    2. .\migraciones.ps1 add AgregarNuevaColumna"
    Write-Host "    3. .\migraciones.ps1 update"
    Write-Host ""
    
    Write-Host "  ?? Deshacer la última migración:" -ForegroundColor Cyan
    Write-Host "    1. .\migraciones.ps1 remove"
  Write-Host "    O si ya fue aplicada:"
    Write-Host "    1. .\migraciones.ps1 list (para ver el nombre de la migración anterior)"
Write-Host " 2. .\migraciones.ps1 update NombreMigracionAnterior"
    Write-Host "    3. .\migraciones.ps1 remove"
 Write-Host ""
    
 Write-Host "  ??? Empezar desde cero:" -ForegroundColor Cyan
    Write-Host "    1. .\migraciones.ps1 drop"
  Write-Host "    2. .\migraciones.ps1 update"
    Write-Host ""
    
    Write-Host "  ?? Error 'pending changes':" -ForegroundColor Cyan
    Write-Host "    1. .\migraciones.ps1 add SolucionarCambiosPendientes"
    Write-Host "    2. .\migraciones.ps1 update"
    Write-Host ""
    
    Write-Host "TIPS:" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "  • Usa nombres descriptivos para las migraciones"
    Write-Host "  • Revisa el archivo de migración antes de aplicarlo"
    Write-Host "  • Haz backup antes de aplicar migraciones en producción"
    Write-Host "  • Si algo sale mal, siempre puedes revertir a una migración anterior"
    Write-Host ""
}

# Ejecutar acción
switch ($Action.ToLower()) {
    "add" { Add-Migration -MigrationName $Name }
    "remove" { Remove-LastMigration }
    "list" { Get-MigrationList }
  "update" { Update-Database -TargetMigration $Name }
    "drop" { Drop-Database }
    "script" { Generate-SqlScript -MigrationName $Name }
    "help" { Show-Help }
    default { Show-Help }
}

Write-Host ""
