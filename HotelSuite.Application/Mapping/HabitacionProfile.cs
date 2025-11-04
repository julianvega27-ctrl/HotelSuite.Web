using AutoMapper;
using HotelSuite.Application.DTOs;
using HotelSuite.Domain.Entities;

namespace HotelSuite.Application.Mapping;

public class HabitacionProfile : Profile
{
    public HabitacionProfile()
    {
    CreateMap<Habitacion, HabitacionDTO>()
            .ForMember(dest => dest.NombreHotel, opt => opt.MapFrom(src => src.Hotel != null ? src.Hotel.Nombre : string.Empty))
  .ReverseMap()
       .ForMember(dest => dest.Hotel, opt => opt.Ignore())
         .ForMember(dest => dest.Reservas, opt => opt.Ignore());
  }
}
