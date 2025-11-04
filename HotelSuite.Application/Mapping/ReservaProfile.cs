using AutoMapper;
using HotelSuite.Application.DTOs;
using HotelSuite.Domain.Entities;

namespace HotelSuite.Application.Mapping;

public class ReservaProfile : Profile
{
    public ReservaProfile()
    {
        CreateMap<Reserva, ReservaDTO>()
       .ForMember(dest => dest.NombreHuesped, 
                opt => opt.MapFrom(src => src.Huesped != null ? $"{src.Huesped.Nombres} {src.Huesped.Apellidos}" : string.Empty))
  .ForMember(dest => dest.NumeroHabitacion, 
   opt => opt.MapFrom(src => src.Habitacion != null ? src.Habitacion.Numero : string.Empty))
  .ForMember(dest => dest.TipoHabitacion, 
opt => opt.MapFrom(src => src.Habitacion != null ? src.Habitacion.Tipo : string.Empty))
  .ForMember(dest => dest.NombreHotel,
   opt => opt.MapFrom(src => src.Habitacion != null && src.Habitacion.Hotel != null ? src.Habitacion.Hotel.Nombre : string.Empty))
  .ForMember(dest => dest.PrecioPorNoche,
         opt => opt.MapFrom(src => src.Habitacion != null ? src.Habitacion.PrecioPorNoche : 0))
     .ForMember(dest => dest.PrecioHabitacion,
     opt => opt.MapFrom(src => src.Habitacion != null ? src.Habitacion.PrecioPorNoche : (decimal?)null))
  .ForMember(dest => dest.MontoTotal,
opt => opt.MapFrom(src => src.Habitacion != null && src.FechaSalida > src.FechaEntrada 
    ? src.Habitacion.PrecioPorNoche * (src.FechaSalida - src.FechaEntrada).Days 
   : (decimal?)null))
     .ForMember(dest => dest.Pagos,
 opt => opt.MapFrom(src => src.Pagos));

        CreateMap<ReservaDTO, Reserva>()
         .ForMember(dest => dest.Huesped, opt => opt.Ignore())
  .ForMember(dest => dest.Habitacion, opt => opt.Ignore())
     .ForMember(dest => dest.Pagos, opt => opt.Ignore());
    }
}
