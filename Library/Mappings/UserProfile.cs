using AutoMapper;
using Library.Models.Account;
using Library.Models.Administration;
using Library.Models.Patron;
using LibraryData.Models.Account;

namespace Library.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<RegisterEmployeeViewModel, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Telephone));

            CreateMap<User, EditUserViewModel>()
                .ForMember(dest => dest.Roles, opt => opt.Ignore())
                .ForMember(dest => dest.Claims, opt => opt.Ignore());

            CreateMap<PatronCreateViewModel, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Telephone));

            CreateMap<User, PatronDetailModel>()
                .ForMember(dest => dest.HomeLibraryBranch, opt => opt.MapFrom(src => src.HomeLibraryBranch.Name))
                .ForMember(dest => dest.MemberSince, opt => opt.MapFrom(src => src.LibraryCard.Created))
                .ForMember(dest => dest.OverdueFees, opt => opt.MapFrom(src => src.LibraryCard.Fees))
                .ForMember(dest => dest.LibraryCardId, opt => opt.MapFrom(src => src.LibraryCard.Id))
                .ForMember(dest => dest.Telephone, opt => opt.MapFrom(src => src.PhoneNumber));

            CreateMap<User, PatronEditViewModel>()
                .ForMember(dest => dest.HomeLibraryBranchName, opt => opt.MapFrom(src => src.HomeLibraryBranch.Name))
                .ForMember(dest => dest.Telephone, opt => opt.MapFrom(src => src.PhoneNumber));
        }
    }
}
