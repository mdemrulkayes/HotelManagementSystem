using AutoMapper;
using Business.DataModels;
using DataAccess.Data;

namespace Business.Mapper
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            //Map Model from DTO
            CreateMap<ApplicationUser, UserDTO>()
                .ForMember(x => x.PhoneNo, opt => { opt.MapFrom(src => src.PhoneNumber); });
            CreateMap<HotelRoom, HotelRoomRequestDTO>();


            //Map DTO from Model
            CreateMap<UserRequestDTO, ApplicationUser>()
                .ForMember(x => x.Id,
                    opt => opt.Ignore())
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNo));

            CreateMap<UserDTO, ApplicationUser>()
                .ForMember(x => x.Id,
                    opt => opt.Ignore())
                .ForMember(x => x.Email,
                    opt => opt.Ignore())
                .ForMember(x => x.UserName,
                    opt => opt.Ignore())
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNo));
            
            CreateMap<HotelRoomRequestDTO, HotelRoom>().ForMember(x => x.Id, opt => opt.Ignore());
        }
    }
}
