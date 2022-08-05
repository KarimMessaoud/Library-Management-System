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
    public class CheckInLibraryAssetCommand : IRequest<ViewResponse>
    {
        public string Id { get; }

        public CheckInLibraryAssetCommand(string id)
        {
            Id = id;
        }
    }

    public class CheckInLibraryAssetCommandHandler : IRequestHandler<CheckInLibraryAssetCommand, ViewResponse>
    {
        private readonly IDataProtector protector;
        private readonly ICheckout _checkout;

        public CheckInLibraryAssetCommandHandler(IDataProtectionProvider dataProtectionProvider,
                                                 DataProtectionPurposeStrings dataProtectionPurposeStrings,
                                                 ICheckout checkout)
        {
            protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.AssetIdRouteValue);
            _checkout = checkout;
        }

        public async Task<ViewResponse> Handle(CheckInLibraryAssetCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == null)
            {
                return ViewResponse.NotFound;
            }

            int decryptedId = Convert.ToInt32(protector.Unprotect(request.Id));

            await _checkout.CheckInItemAsync(decryptedId);

            return ViewResponse.OK;
        }
    }
}
