using Microsoft.Extensions.DependencyInjection;

namespace HotelSuite.Application;

public static class DependencyInjection
{
 public static IServiceCollection AddApplication(this IServiceCollection services)
    {
     // Registrar AutoMapper con todos los perfiles del ensamblado
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);

        return services;
    }
}
