using System.Linq;
using AutoMapper;
using Cardofun.Domain.Models;
using Cardofun.Interfaces.DTOs;
using Cardofun.Core.Helpers;

namespace Cardofun.API.Helpers
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<City, CityDto>()
                .ForMember(dest => dest.Country, m => m.MapFrom(src => src.Country.Name));

            CreateMap<LanguageLevel, LanguageLevelDto>()
                .ForMember(dest => dest.Code, m => m.MapFrom(src => src.Language.Code))
                .ForMember(dest => dest.Name, m => m.MapFrom(src => src.Language.Name));

            CreateMap<Language, LanguageDto>().ReverseMap();

            CreateMap<Photo, PhotoDto>().ReverseMap();

            CreateMap<User, UserForListDto>()
                .ForMember(dest => dest.Age, m => m.MapFrom(src => src.BirthDate.ToAges()))
                .ForMember(dest => dest.PhotoUrl, m => m.MapFrom(src => src.Photos.First(p => p.IsMain).Url));

            CreateMap<User, UserForDetailedDto>()
                .ForMember(dest => dest.Age, m => m.MapFrom(src => src.BirthDate.ToAges()))
                .ForMember(dest => dest.PhotoUrl, m => m.MapFrom(src => src.Photos.First(p => p.IsMain).Url));
        }
    }
}