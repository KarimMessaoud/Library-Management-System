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
    public class MarkLibraryAssetFoundCommand : IRequest<ViewResponse>
    {
        public string AssetId { get; }

        public MarkLibraryAssetFoundCommand(string assetId)
        {
            AssetId = assetId;
        }
    }

    public class MarkLibraryAssetFoundCommandHandler : IRequestHandler<MarkLibraryAssetFoundCommand, ViewResponse>
    {
        private readonly IDataProtector protector;
        private readonly ICheckout _checkout;

        public MarkLibraryAssetFoundCommandHandler(IDataProtectionProvider dataProtectionProvider,
                                           DataProtectionPurposeStrings dataProtectionPurposeStrings,
                                           ICheckout checkout)
        {
            protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.AssetIdRouteValue);
            _checkout = checkout;
        }

        public async Task<ViewResponse> Handle(MarkLibraryAssetFoundCommand request, CancellationToken cancellationToken)
        {
            if (request.AssetId == null)
            {
                return ViewResponse.NotFound;
            }

            int decryptedId = Convert.ToInt32(protector.Unprotect(request.AssetId));

            await _checkout.MarkFoundAsync(decryptedId);

            return ViewResponse.OK;
        }
    }
}
