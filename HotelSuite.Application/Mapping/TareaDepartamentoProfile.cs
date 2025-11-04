using AutoMapper;
using HotelSuite.Application.DTOs;
using HotelSuite.Domain.Entities;

namespace HotelSuite.Application.Mapping;

public class TareaDepartamentoProfile : Profile
{
    public TareaDepartamentoProfile()
    {
        CreateMap<TareaDepartamento, TareaDepartamentoDTO>()
 .ForMember(dest => dest.NombreDepartamento, opt => opt.MapFrom(src => src.Departamento != null ? src.Departamento.Nombre : null))
        .ForMember(dest => dest.NombreEmpleado, opt => opt.MapFrom(src => src.Empleado != null ? $"{src.Empleado.Nombres} {src.Empleado.Apellidos}" : null));
        
    CreateMap<TareaDepartamentoDTO, TareaDepartamento>()
   .ForMember(dest => dest.Departamento, opt => opt.Ignore())
    .ForMember(dest => dest.Empleado, opt => opt.Ignore());
    }
}
