using AutoMapper;
using Library.Models.Catalog;
using Library.Security;
using LibraryData;
using MediatR;
using Microsoft.AspNetCore.DataProtection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Library.Queries.Catalog
{
    public class GetLibraryAssetQuery : IRequest<AssetDetailModel>
    {
        public string Id { get; }

        public GetLibraryAssetQuery(string id)
        {
            Id = id;
        }
    }

    public class GetLibraryAssetQueryHandler : IRequestHandler<GetLibraryAssetQuery, AssetDetailModel>
    {
        private readonly ILibraryAssetService _assetsService;
        private readonly IDataProtector protector;
        private readonly ICheckout _checkout;
        private readonly IMapper _mapper;

        public GetLibraryAssetQueryHandler(ILibraryAssetService assetsService, 
                                           IDataProtectionProvider dataProtectionProvider, 
                                           DataProtectionPurposeStrings dataProtectionPurposeStrings, 
                                           ICheckout checkout, 
                                           IMapper mapper)
        {
            _assetsService = assetsService;
            protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.AssetIdRouteValue);
            _checkout = checkout;
            _mapper = mapper;
        }

        public async Task<AssetDetailModel> Handle(GetLibraryAssetQuery request, CancellationToken cancellationToken)
        {
            if (request.Id == null)
            {
                return null;
            }

            int decryptedId = Convert.ToInt32(protector.Unprotect(request.Id));
            var asset = await _assetsService.GetByIdAsync(decryptedId);

            if (asset == null)
            {
                return null;
            }

            var currentHolds = await _checkout.GetCurrentHoldsAsync(decryptedId);

            var assetHoldModelCurrentHolds = currentHolds
                .Select(x => new AssetHoldModel
                {
                    PatronName = _checkout.GetCurrentHoldPatronName(x.Id),
                    HoldPlaced = _checkout.GetCurrentHoldPlaced(x.Id)
                });

            var model = _mapper.Map<AssetDetailModel>(asset);

            model.AssetId = request.Id;
            model.AuthorOrDirector = await _assetsService.GetAuthorOrDirectorAsync(decryptedId);
            model.Type = _assetsService.GetType(decryptedId);
            model.ISBN = await _assetsService.GetIsbnAsync(decryptedId);
            model.CurrentLocation = await _assetsService.GetCurrentLocationNameAsync(decryptedId);
            model.LatestCheckout = await _checkout.GetLatestCheckoutAsync(decryptedId);
            model.PatronName = await _checkout.GetCurrentCheckoutPatronAsync(decryptedId);
            model.CheckoutHistory = await _checkout.GetCheckoutHistoryAsync(decryptedId);
            model.CurrentHolds = assetHoldModelCurrentHolds;

            return model;
        }
    }
}
