using AutoMapper;
using HotelSuite.Application.DTOs;
using HotelSuite.Domain.Entities;

namespace HotelSuite.Application.Mapping;

public class HuespedProfile : Profile
{
    public HuespedProfile()
    {
    CreateMap<Huesped, HuespedDTO>()
     .ReverseMap()
       .ForMember(dest => dest.Reservas, opt => opt.Ignore());
    }
}
