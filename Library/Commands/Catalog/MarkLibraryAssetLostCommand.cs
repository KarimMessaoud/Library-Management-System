using Library.Enums;
using Library.Security;
using LibraryData;
using MediatR;
using Microsoft.AspNetCore.DataProtection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Library.Commands.Catalog
{
    public class MarkLibraryAssetLostCommand : IRequest<ViewResponse>
    {
        public string AssetId;

        public MarkLibraryAssetLostCommand(string assetId)
        {
            AssetId = assetId;
        }
    }

    public class MarkLibraryAssetLostCommandHandler : IRequestHandler<MarkLibraryAssetLostCommand, ViewResponse>
    {
        private readonly IDataProtector protector;
        private readonly ICheckout _checkout;

        public MarkLibraryAssetLostCommandHandler(IDataProtectionProvider dataProtectionProvider,
                                           DataProtectionPurposeStrings dataProtectionPurposeStrings,
                                           ICheckout checkout)
        {
            protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.AssetIdRouteValue);
            _checkout = checkout;
        }

        public async Task<ViewResponse> Handle(MarkLibraryAssetLostCommand request, CancellationToken cancellationToken)
        {
            if (request.AssetId == null)
            {
                return ViewResponse.NotFound;
            }

            int decryptedId = Convert.ToInt32(protector.Unprotect(request.AssetId));

            await _checkout.MarkLostAsync(decryptedId);

            return ViewResponse.OK;
        }
    }
}
