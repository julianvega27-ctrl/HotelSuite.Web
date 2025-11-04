using AutoMapper;
using HotelSuite.Application.DTOs;
using HotelSuite.Domain.Entities;

namespace HotelSuite.Application.Mapping;

public class EmpleadoProfile : Profile
{
    public EmpleadoProfile()
    {
        CreateMap<Empleado, EmpleadoDTO>()
            .ForMember(dest => dest.NombreDepartamento, opt => opt.MapFrom(src => src.Departamento != null ? src.Departamento.Nombre : null));

        CreateMap<EmpleadoDTO, Empleado>()
         .ForMember(dest => dest.Departamento, opt => opt.Ignore())
            .ForMember(dest => dest.TareasDepartamento, opt => opt.Ignore());
    }
}
