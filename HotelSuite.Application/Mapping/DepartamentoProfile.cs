using AutoMapper;
using HotelSuite.Application.DTOs;
using HotelSuite.Domain.Entities;

namespace HotelSuite.Application.Mapping;

public class DepartamentoProfile : Profile
{
    public DepartamentoProfile()
    {
        CreateMap<Departamento, DepartamentoDTO>()
       .ReverseMap()
     .ForMember(dest => dest.TareasDepartamento, opt => opt.Ignore());
    }
}
