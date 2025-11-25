using HotelSuite.Application;
using HotelSuite.Domain.Entities;
using HotelSuite.Infrastructure;
using HotelSuite.Infrastructure.Data;
using HotelSuite.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configurar API Controllers con JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// Configurar Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "HotelSuite API",
        Version = "v1",
        Description = "API REST para el sistema de gestión hotelera HotelSuite HMS. " +
    "Incluye endpoints para consultar disponibilidad de habitaciones, gestionar reservas y check-in/check-out automático.",
        Contact = new OpenApiContact
        {
            Name = "HotelSuite HMS",
         Email = "soporte@hotelsuite.com"
        },
        License = new OpenApiLicense
{
       Name = "MIT",
 }
    });

    // Incluir comentarios XML
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Configurar esquemas para evitar conflictos
    c.CustomSchemaIds(type => type.FullName);
});

// Configurar CORS para permitir acceso desde aplicaciones móviles
builder.Services.AddCors(options =>
{
    options.AddPolicy("ApiCorsPolicy", policy =>
 {
        policy.AllowAnyOrigin()
     .AllowAnyMethod()
    .AllowAnyHeader();
    });
});

// Agregar servicios de Application (AutoMapper)
builder.Services.AddApplication();

// Agregar HttpClientFactory para consumir la API interna
builder.Services.AddHttpClient("HotelSuiteApi", client =>
{
    // La URL base de tu propia API. Asegúrate que el puerto sea el correcto.
    // En tu caso, parece ser 5001.
    client.BaseAddress = new Uri("https://localhost:5001/api/");
});

// Agregar servicios de Infrastructure (DbContext, UnitOfWork, Repositories)
builder.Services.AddInfrastructure(builder.Configuration);

// Configurar Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Configuración de contraseñas
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;

    // Configuración de usuario
    options.User.RequireUniqueEmail = true;

    // Configuración de bloqueo
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // Configuración de SignIn
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<HotelDbContext>()
.AddDefaultTokenProviders();

// Configurar cookies de autenticación
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest;
});

// Registrar servicio de comprobantes PDF
builder.Services.AddScoped<ComprobanteService>();

// Registrar Seeders
builder.Services.AddScoped<DataSeeder>();
builder.Services.AddScoped<IdentitySeeder>();

var app = builder.Build();

// Ejecutar migraciones y seeding automáticamente
await using (var scope = app.Services.CreateAsyncScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var configuration = services.GetRequiredService<IConfiguration>();
    
// Verificar si el auto-migrate está habilitado
    var autoMigrate = configuration.GetValue<bool>("ApplicationSettings:AutoMigrate", true);
    var seedData = configuration.GetValue<bool>("ApplicationSettings:SeedData", true);
    
    if (autoMigrate)
    {
        try
        {
  logger.LogInformation("=== Iniciando configuración de base de datos ===");
            
        // Obtener el contexto de base de datos
            var context = services.GetRequiredService<HotelDbContext>();
        
     // Verificar si la base de datos existe
            var canConnect = await context.Database.CanConnectAsync();
      
      if (!canConnect)
         {
                logger.LogInformation("Base de datos no existe. Creando...");
   }
            
            // Ejecutar migraciones pendientes
            logger.LogInformation("Aplicando migraciones pendientes...");
    await context.Database.MigrateAsync();
      logger.LogInformation("? Migraciones aplicadas exitosamente.");
            
          // Ejecutar seeding solo si está habilitado
            if (seedData)
            {
     // Ejecutar seeding de datos iniciales
      logger.LogInformation("Verificando datos iniciales...");
                var seeder = services.GetRequiredService<DataSeeder>();
     await seeder.SeedAsync();
       logger.LogInformation("? Datos iniciales verificados/creados.");

   // Ejecutar seeding de Identity (roles y usuarios)
       logger.LogInformation("Verificando roles y usuarios...");
           var identitySeeder = services.GetRequiredService<IdentitySeeder>();
          await identitySeeder.SeedAsync();
        logger.LogInformation("? Roles y usuarios verificados/creados.");
}
       
            logger.LogInformation("=== Configuración de base de datos completada exitosamente ===");
 }
        catch (InvalidOperationException ex) when (ex.Message.Contains("pending changes"))
        {
            logger.LogError(ex, "? ERROR: El modelo tiene cambios pendientes que requieren una migración.");
            logger.LogError("???????????????????????????????????????????????????");
            logger.LogError("SOLUCIÓN:");
            logger.LogError("1. Detén la aplicación (Ctrl+C)");
            logger.LogError("2. Ejecuta el siguiente comando:");
     logger.LogError("   dotnet ef migrations add NombreDeLaMigracion --project HotelSuite.Infrastructure --startup-project HotelSuite");
    logger.LogError("3. Reinicia la aplicación");
   logger.LogError("???????????????????????????????????????????????????");
      
 // No lanzar excepción para permitir que la app continúe
       logger.LogWarning("? La aplicación continuará sin aplicar migraciones.");
          logger.LogWarning("? Algunas funcionalidades pueden no funcionar correctamente.");
    }
        catch (Exception ex)
 {
    logger.LogError(ex, "? Error durante la configuración de base de datos.");
  logger.LogError("???????????????????????????????????????????????????");
    logger.LogError("TIPO DE ERROR: {ErrorType}", ex.GetType().Name);
logger.LogError("MENSAJE: {Message}", ex.Message);
        
  if (ex.InnerException != null)
  {
  logger.LogError("CAUSA: {InnerMessage}", ex.InnerException.Message);
    }
      
  logger.LogError("???????????????????????????????????????????????????");
     logger.LogError("POSIBLES SOLUCIONES:");
     logger.LogError("1. Verifica la cadena de conexión en appsettings.json");
     logger.LogError("2. Asegúrate de que SQL Server/SQLite esté disponible");
       logger.LogError("3. Ejecuta: .\\diagnostico.ps1 (para diagnóstico automático)");
       logger.LogError("4. Consulta: SOLUCION_PROBLEMAS_BD.md");
      logger.LogError("???????????????????????????????????????????????????");
       
      // Solo lanzar excepción en producción
            if (!app.Environment.IsDevelopment())
         {
   throw;
        }
     
logger.LogWarning("? Modo desarrollo: La aplicación continuará a pesar del error.");
    }
    }
    else
    {
   logger.LogInformation("Auto-migrate deshabilitado. Saltando configuración de base de datos.");
logger.LogInformation("Para habilitar, configura 'ApplicationSettings:AutoMigrate: true' en appsettings.json");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    // Habilitar Swagger solo en desarrollo
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HotelSuite API v1");
      c.RoutePrefix = "api/docs"; // Acceder en /api/docs
        c.DocumentTitle = "HotelSuite API - Documentación";
        c.DisplayRequestDuration();
    });
}

app.UseHttpsRedirection();
app.UseRouting();

// Habilitar CORS
app.UseCors("ApiCorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Mapear controladores de API
app.MapControllers();

// Mensaje de inicio
var startupLogger = app.Services.GetRequiredService<ILogger<Program>>();
startupLogger.LogInformation("???????????????????????????????????????????????????");
startupLogger.LogInformation("?? HotelSuite HMS - Sistema de Gestión Hotelera");
startupLogger.LogInformation("???????????????????????????????????????????????????");
startupLogger.LogInformation("?? Aplicación Web:  https://localhost:5001");
startupLogger.LogInformation("?? API REST:    https://localhost:5001/api");
startupLogger.LogInformation("?? Documentación:   https://localhost:5001/api/docs");
startupLogger.LogInformation("???????????????????????????????????????????????????");
startupLogger.LogInformation("?? Usuario Admin:   admin@hotelsuite.com");
startupLogger.LogInformation("?? Contraseña:    Admin123!");
startupLogger.LogInformation("???????????????????????????????????????????????????");

app.Run();
