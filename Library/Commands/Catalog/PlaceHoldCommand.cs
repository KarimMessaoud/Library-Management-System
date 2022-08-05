using Library.Enums;
using Library.Security;
using LibraryData;
using MediatR;
using Microsoft.AspNetCore.DataProtection;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Hangfire;

namespace Library.Commands.Catalog
{
    public class PlaceHoldCommand : IRequest<ViewResponse>
    {
        public string AssetId { get; }
        public int LibraryCardId { get; }
        public PlaceHoldCommand(string assetId, int libraryCardId)
        {
            AssetId = assetId;
            LibraryCardId = libraryCardId;
        }
    }

    public class PlaceHoldCommandHandler : IRequestHandler<PlaceHoldCommand, ViewResponse>
    {
        private readonly IDataProtector protector;
        private readonly ICheckout _checkout;
        private readonly LibraryContext _context;

        public PlaceHoldCommandHandler(IDataProtectionProvider dataProtectionProvider,
                                       DataProtectionPurposeStrings dataProtectionPurposeStrings,
                                       ICheckout checkout, 
                                       LibraryContext context)
        {
            protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.AssetIdRouteValue);
            _checkout = checkout;
            _context = context;
        }

        public async Task<ViewResponse> Handle(PlaceHoldCommand request, CancellationToken cancellationToken)
        {
            int decryptedId = Convert.ToInt32(protector.Unprotect(request.AssetId));

            if (!await _checkout.PlaceHoldAsync(decryptedId, request.LibraryCardId))
            {
                return ViewResponse.NotAllowed;
            }
            
            var patron = await _context.Users
                .FirstOrDefaultAsync(x => x.LibraryCard.Id == request.LibraryCardId);

            var hold = await _context.Holds
                .FirstOrDefaultAsync(x => x.LibraryCard.Id == request.LibraryCardId && x.LibraryAsset.Id == decryptedId);


            if (hold.FirstHold == true)
            {
                BackgroundJob.Enqueue<IEmailService>(x => x.SendEmailAsync(patron.FirstName, patron.Email, "Place hold on the book",
                $"You have placed hold on the asset: '{hold.LibraryAsset.Title}' from our library. " +
                "Now you have to come to us and take the item in 24 hours time. " +
                "If you will not take the item up to this time you will not be able to borrow it."));
            }

            return ViewResponse.OK;
        }
    }
}
