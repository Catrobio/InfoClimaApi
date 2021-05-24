using AutoMapper;
using InfoClimaApi.Models;
using InfoClimaApi.Models.DTOs;
using System.Linq;

namespace InfoClimaApi.Mappers
{
    
    public class ClimasMapping : Profile
    {
        public ClimasMapping()
        {
            CreateMap<Clima, InfoClimasDTO>()
                .ForMember(dest => dest.Ciudad,
                    opts => opts.MapFrom(
                        src => src.IdCiudadesNavigation.Descripcion
                ))
                .ForMember(dest => dest.Pais,
                    opts => opts.MapFrom(
                        src => src.IdCiudadesNavigation.IdPaisesNavigation.Descripcion
                ))
                .ForMember(dest => dest.Clima,
                    opts => opts.MapFrom(
                        src => src.Clima1
                ))                
            .ReverseMap();

             CreateMap<Ciudades, CiudadesDTO>()
                .ForMember(dest => dest.Ciudad,
                    opts => opts.MapFrom(
                        src => src.Descripcion
                ))
                .ForMember(dest => dest.Pais,
                    opts => opts.MapFrom(
                        src => src.IdPaisesNavigation.Descripcion
                ))                           
            .ReverseMap();
        }
    }
}

