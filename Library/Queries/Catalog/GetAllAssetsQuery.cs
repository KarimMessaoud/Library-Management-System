using Library.Models.Catalog;
using Library.Security;
using LibraryData;
using MediatR;
using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Library.Queries.Catalog
{
    public class GetAllAssetsQuery : IRequest<List<AssetIndexListingViewModel>>
    {
        public string SearchString { get; set; }
        public string CurrentFilter { get; }
        public int? PageNumber { get; set; }

        public GetAllAssetsQuery(string searchString, string currentFilter, int? pageNumber)
        {
            SearchString = searchString;
            CurrentFilter = currentFilter;
            PageNumber = pageNumber;
        }
    }

    public class GetAllAssetsQueryHandler : IRequestHandler<GetAllAssetsQuery, List<AssetIndexListingViewModel>>
    {
        private readonly ILibraryAssetService _assetsService;
        private readonly IDataProtector protector;

        public GetAllAssetsQueryHandler(ILibraryAssetService assetsService,
                                    IDataProtectionProvider dataProtectionProvider,
                        DataProtectionPurposeStrings dataProtectionPurposeStrings)
        {
            _assetsService = assetsService;
            protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.AssetIdRouteValue);
        }

        public async Task<List<AssetIndexListingViewModel>> Handle(GetAllAssetsQuery request, CancellationToken cancellationToken)
        {
            if (request.SearchString != null)
            {
                request.PageNumber = 1;
            }
            else
            {
                request.SearchString = request.CurrentFilter;
            }

            var assets = await _assetsService.GetAllAsync();

            var encryptedIdAssets = assets.Select(x =>
            {
                x.EncryptedId = protector.Protect(x.Id.ToString());
                return x;
            }).OrderBy(x => x.Title);


            var listingResult = encryptedIdAssets
                .Select(x => new AssetIndexListingViewModel
                {
                    Id = x.EncryptedId,
                    ImageUrl = x.ImageUrl,
                    AuthorOrDirector = _assetsService.GetAuthorOrDirector(x.Id),
                    Title = _assetsService.GetTitle(x.Id),
                    Type = _assetsService.GetType(x.Id),
                }).ToList();


            if (!String.IsNullOrEmpty(request.SearchString))
            {
                listingResult = listingResult.Where(x => x.Title.ToUpper().Contains(request.SearchString.ToUpper()) 
                                                    || x.AuthorOrDirector.ToUpper().Contains(request.SearchString.ToUpper())).ToList();
            }

            listingResult = listingResult.OrderBy(x => x.Title).ToList();

            return listingResult;
        }
    }
}
