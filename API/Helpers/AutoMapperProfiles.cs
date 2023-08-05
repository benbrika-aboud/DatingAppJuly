using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles() 
        {
            CreateMap<AppUser, MemberDto>()
                .ForMember(dest=> dest.Age,
                opt => opt.MapFrom(u=> u.DateOfBirth.CalculateAge()))
                .ForMember(dest=> dest.PhotoUrl, 
                opt => opt.MapFrom(u => u.Photos.FirstOrDefault(p=> p.IsMain).Url));
            CreateMap<Photo, PhotoDto>();
            CreateMap<MemberUpdateDto, AppUser>();
        }
    }
}
