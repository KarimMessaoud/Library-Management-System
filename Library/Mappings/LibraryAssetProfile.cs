using AutoMapper;
using Library.Models.Account;
using Library.Models.Administration;
using Library.Models.Catalog;
using LibraryData.Models;

namespace Library.Mappings
{
    public class LibraryAssetProfile : Profile
    {
        public LibraryAssetProfile()
        {
            CreateMap<AssetCreateBookViewModel, Book>();

            CreateMap<AssetCreateVideoViewModel, Video>();

            CreateMap<LibraryAsset, AssetDetailModel>()
                .ForMember(dest => dest.AssetId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Name))
                .ForMember(dest => dest.AssetId, opt => opt.Ignore());

            CreateMap<LibraryAsset, AssetEditBookViewModel>()
                .ForMember(dest => dest.LibraryBranchName, opt => opt.MapFrom(src => src.Location.Name))
                .ForMember(dest => dest.ExistingPhotoPath, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<LibraryAsset, AssetEditVideoViewModel>()
                .ForMember(dest => dest.LibraryBranchName, opt => opt.MapFrom(src => src.Location.Name))
                .ForMember(dest => dest.ExistingPhotoPath, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<AssetEditBookViewModel, Book>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
