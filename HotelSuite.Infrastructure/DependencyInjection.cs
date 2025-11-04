using HotelSuite.Domain.Interfaces;
using HotelSuite.Infrastructure.Data;
using HotelSuite.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotelSuite.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
this IServiceCollection services,
      IConfiguration configuration)
 {
        // Obtener el proveedor de base de datos
        var databaseProvider = configuration["DatabaseProvider"] ?? "SqlServer";
 var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
   {
  throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no está configurada.");
        }

        // Configurar DbContext según el proveedor
        services.AddDbContext<HotelDbContext>(options =>
  {
      if (databaseProvider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
   {
       options.UseSqlite(
       connectionString,
    sqliteOptions => sqliteOptions.MigrationsAssembly(typeof(HotelDbContext).Assembly.FullName));
          }
      else // SqlServer por defecto
 {
  options.UseSqlServer(
      connectionString,
            sqlServerOptions => sqlServerOptions.MigrationsAssembly(typeof(HotelDbContext).Assembly.FullName));
}

    // Configuración adicional
   options.EnableSensitiveDataLogging(true);
    options.EnableDetailedErrors(true);
    });

   // Registrar UnitOfWork y Repositories
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        return services;
    }
}
