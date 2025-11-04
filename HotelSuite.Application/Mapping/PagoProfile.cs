using AutoMapper;
using HotelSuite.Application.DTOs;
using HotelSuite.Domain.Entities;

namespace HotelSuite.Application.Mapping;

public class PagoProfile : Profile
{
    public PagoProfile()
    {
        CreateMap<Pago, PagoDTO>()
      .ForMember(dest => dest.NombreHuesped,
    opt => opt.MapFrom(src => src.Reserva != null && src.Reserva.Huesped != null 
    ? $"{src.Reserva.Huesped.Nombres} {src.Reserva.Huesped.Apellidos}" 
   : null))
 .ForMember(dest => dest.NumeroHabitacion,
       opt => opt.MapFrom(src => src.Reserva != null && src.Reserva.Habitacion != null 
       ? src.Reserva.Habitacion.Numero 
             : null))
        .ReverseMap()
   .ForMember(dest => dest.Reserva, opt => opt.Ignore());
    }
}
