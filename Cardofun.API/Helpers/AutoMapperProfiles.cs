using System.Linq;
using AutoMapper;
using Cardofun.Domain.Models;
using Cardofun.Interfaces.DTOs;
using Cardofun.Core.Helpers;
using System;
using Cardofun.Core.Enumerables;
using System.Collections.Generic;

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

            CreateMap<Language, LanguageDto>()
                .ReverseMap();
            #endregion Language

            #region Photo
            CreateMap<UserPhoto, UserPhotoForReturnDto>()
                .ForMember(dest => dest.Url, m => m.MapFrom(src => src.Photo.Url))
                .ForMember(dest => dest.PublicId, m => m.MapFrom(src => src.Photo.PublicId))
                .ReverseMap();

            CreateMap<Photo, GlobalPhotoIdentifiersDto>()
                .ReverseMap();

            CreateMap<UserPhoto, UserPhotoDto>()
                .ForMember(dest => dest.Url, m => m.MapFrom(src => src.Photo.Url))
                .ReverseMap();

            CreateMap<Photo, UserPhotoForCreationDto>()
                .ReverseMap();

            CreateMap<UserPhotoForCreationDto, UserPhoto>()
                .ForMember(dest => dest.Photo, m => m.MapFrom(src => src))
                .ForMember(dest => dest.DateAdded, m => m.MapFrom(src => DateTime.Now))
                .ReverseMap();
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

            CreateMap<User, UserForMessageListDto>()
                .ForMember(dest => dest.PhotoUrl, m => m.MapFrom(src => GetUserMaintPhotoUrl(src)));
                            
            CreateMap<User, UserForAdminPanelDto>()
                .ForMember(dest => dest.Roles, m => m.MapFrom(src => src.UserRoles.Select(ur => ur.Role.Name)));
            #endregion User

            #region Roles
            CreateMap<Role, RoleForList>();
            #endregion Roles

            #region Messages
            CreateMap<Message, MessageForReturnDto>()
                .ForMember(dest => dest.PhotoUrl, m => m.MapFrom(src => src.Photo.Url));

            CreateMap<Message, MessageExtendedDto>()
                .ForMember(dest => dest.SenderName, m => m.MapFrom(src => src.Sender.Name))
                .ForMember(dest => dest.RecipientName, m => m.MapFrom(src => src.Recipient.Name))
                .ForMember(dest => dest.SenderPhotoUrl, m => m.MapFrom(src => GetUserMaintPhotoUrl(src.Sender)))
                .ForMember(dest => dest.RecipientPhotoUrl, m => m.MapFrom(src => GetUserMaintPhotoUrl(src.Recipient)));

            CreateMap<MessageForCreationDto, Message>();

            CreateMap<PagedList<Message>, MessageListDto>()
                .ForMember(dest => dest.Messages, m => m.MapFrom(src => src))
                .ForMember(dest => dest.Users, m => m.MapFrom(src => GetUsersFromMessages(src)));
            #endregion Messages
        }

        private String GetUserMaintPhotoUrl(User user)
        {
            if(user == null || user.Photos == null)
                return null;

            return user.Photos.FirstOrDefault(u => u.IsMain)?.Photo?.Url;
        }

        private IEnumerable<User> GetUsersFromMessages(PagedList<Message> src)
            => src.Select(m => m.Sender).Union(src.Select(m => m.Recipient));
    }
}