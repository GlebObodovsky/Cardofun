using System.Linq;
using AutoMapper;
using Cardofun.Domain.Models;
using Cardofun.Interfaces.DTOs;
using Cardofun.Core.Helpers;
using System;

namespace Cardofun.API.Helpers
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            #region City
            CreateMap<City, CityDto>()
                .ForMember(dest => dest.Country, m => m.MapFrom(src => src.Country.Name));
            #endregion City

            #region Country
            CreateMap<Country, CountryDto>();
            #endregion City

            #region Language
            CreateMap<LanguageLevel, LanguageLevelDto>()
                .ForMember(dest => dest.Code, m => m.MapFrom(src => src.Language.Code))
                .ForMember(dest => dest.Name, m => m.MapFrom(src => src.Language.Name));

            CreateMap<LanguageLevelDto, LanguageLearningLevel>()
                .ForMember(dest => dest.LanguageCode, m => m.MapFrom(src => src.Code));

            CreateMap<LanguageLevelDto, LanguageSpeakingLevel>()
                .ForMember(dest => dest.LanguageCode, m => m.MapFrom(src => src.Code));

            CreateMap<Language, LanguageDto>().ReverseMap();
            #endregion Language

            #region Photo
            CreateMap<Photo, PhotoDto>().ReverseMap();

            CreateMap<PhotoForCreationDto, Photo>()
                .ForMember(dest => dest.DateAdded, m => m.MapFrom(src => DateTime.Now));

            CreateMap<Photo, PhotoForReturnDto>().ReverseMap();

            CreateMap<Photo, GlobalPhotoIdentifiersDto>().ReverseMap();
            #endregion Photo

            #region User
            CreateMap<UserForRegisterDto, User>();

            CreateMap<User, UserShortInfoDto>()
                .ForMember(dest => dest.Age, m => m.MapFrom(src => src.BirthDate.ToAges()))
                .ForMember(dest => dest.PhotoUrl, m => m.MapFrom(src => GetUserMaintPhotoUrl(src)));

            CreateMap<User, UserForListDto>()
                .ForMember(dest => dest.Age, m => m.MapFrom(src => src.BirthDate.ToAges()))
                .ForMember(dest => dest.PhotoUrl, m => m.MapFrom(src => GetUserMaintPhotoUrl(src)));

            CreateMap<User, UserForDetailedDto>()
                .ForMember(dest => dest.PhotoUrl, m => m.MapFrom(src => GetUserMaintPhotoUrl(src)))
                .ForMember(dest => dest.Age, m => m.MapFrom(src => src.BirthDate.ToAges()));
        
            CreateMap<UserForUpdateDto, User>()
                .ForMember(dest => dest.CityId, m => m.MapFrom(src => src.City.Id))
                .ForMember(dest => dest.City, m => m.Ignore());

         
            #endregion User
        }
        private String GetUserMaintPhotoUrl(User user)
        {
            if(user == null || user.Photos == null)
                return null;

            return user.Photos.FirstOrDefault(u => u.IsMain)?.Url;
        }
    }
}