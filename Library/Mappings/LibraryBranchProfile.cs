using AutoMapper;
using Library.Models.Branch;
using LibraryData.Models;


namespace Library.Mappings
{
    public class LibraryBranchProfile : Profile
    {
        public LibraryBranchProfile()
        {
            CreateMap<LibraryBranch, BranchDetailViewModel>()
                .ForMember(dest => dest.OpenDate, opt => opt.MapFrom(src => src.OpenDate.ToString("yyyy-MM-dd")));
        }
    }
}
