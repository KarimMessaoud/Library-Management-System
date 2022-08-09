using Library.Models.Checkout;
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
    public class HoldLibraryAssetQuery : IRequest<CheckoutViewModel>
    {
        public string Id { get; }

        public HoldLibraryAssetQuery(string id)
        {
            Id = id;
        }
    }

    public class HoldLibraryAssetQueryHandler : IRequestHandler<HoldLibraryAssetQuery, CheckoutViewModel>
    {
        private readonly ILibraryAssetService _assetsService;
        private readonly IDataProtector protector;
        private readonly ICheckout _checkout;

        public HoldLibraryAssetQueryHandler(ILibraryAssetService assetsService,
                                            IDataProtectionProvider dataProtectionProvider,
                                            DataProtectionPurposeStrings dataProtectionPurposeStrings,
                                            ICheckout checkout)
        {
            _assetsService = assetsService;
            protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.AssetIdRouteValue);
            _checkout = checkout;
        }

        public async Task<CheckoutViewModel> Handle(HoldLibraryAssetQuery request, CancellationToken cancellationToken)
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

            var model = new CheckoutViewModel()
            {
                LibraryCardId = "",
                AssetId = request.Id,
                Title = asset.Title,
                ImageUrl = asset.ImageUrl,
                IsCheckedOut = await _checkout.IsCheckedOutAsync(decryptedId)
            };

            var currentholds = await _checkout.GetCurrentHoldsAsync(decryptedId);
            model.HoldCount = currentholds.Count();

            return model;
        }
    }
}
