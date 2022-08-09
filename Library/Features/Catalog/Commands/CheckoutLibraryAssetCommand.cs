using Library.Security;
using LibraryData;
using MediatR;
using Microsoft.AspNetCore.DataProtection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Library.Commands.Catalog
{
    public class CheckoutLibraryAssetCommand : IRequest
    {
        public string AssetId { get; }
        public int LibraryCardId { get; }
        public CheckoutLibraryAssetCommand(string assetId, int libraryCardId)
        {
            AssetId = assetId;
            LibraryCardId = libraryCardId;
        }
    }

    public class CheckoutLibraryAssetCommandHandler : AsyncRequestHandler<CheckoutLibraryAssetCommand>
    {
        private readonly IDataProtector protector;
        private readonly ICheckout _checkout;

        public CheckoutLibraryAssetCommandHandler(ICheckout checkout,
                                                  IDataProtectionProvider dataProtectionProvider,
                                                  DataProtectionPurposeStrings dataProtectionPurposeStrings)
        {
            _checkout = checkout;
            protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.AssetIdRouteValue);
        }

        protected async override Task Handle(CheckoutLibraryAssetCommand request, CancellationToken cancellationToken)
        {
            int decryptedId = Convert.ToInt32(protector.Unprotect(request.AssetId));

            await _checkout.CheckOutItemAsync(decryptedId, request.LibraryCardId);
        }
    }
}
