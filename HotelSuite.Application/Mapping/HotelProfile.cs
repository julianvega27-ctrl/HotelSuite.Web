using AutoMapper;
using HotelSuite.Application.DTOs;
using HotelSuite.Domain.Entities;

namespace HotelSuite.Application.Mapping;

public class HotelProfile : Profile
{
    public HotelProfile()
    {
CreateMap<Hotel, HotelDTO>().ReverseMap();
    }
}
