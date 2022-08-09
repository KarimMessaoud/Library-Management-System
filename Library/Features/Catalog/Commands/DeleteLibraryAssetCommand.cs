using System;
using System.Threading;
using System.Threading.Tasks;
using Library.Enums;
using Library.Security;
using LibraryData;
using MediatR;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Library.Commands.Catalog
{
    public class DeleteLibraryAssetCommand : IRequest<ViewResponse>
    {
        public string Id { get; }

        public DeleteLibraryAssetCommand(string id)
        {
            Id = id;
        }
    }

    public class DeleteLibraryAssetCommandHandler : IRequestHandler<DeleteLibraryAssetCommand, ViewResponse>
    {
        private readonly ILibraryAssetService _assetsService;
        private readonly IDataProtector protector;
        private readonly ILogger<DeleteLibraryAssetCommandHandler> _logger;

        public DeleteLibraryAssetCommandHandler(ILibraryAssetService assetsService,
                                                IDataProtectionProvider dataProtectionProvider,
                                                DataProtectionPurposeStrings dataProtectionPurposeStrings,
                                                ILogger<DeleteLibraryAssetCommandHandler> logger)
        {
            _assetsService = assetsService;
            protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.AssetIdRouteValue);
            _logger = logger;
        }

        public async Task<ViewResponse> Handle(DeleteLibraryAssetCommand request, CancellationToken cancellationToken)
        {
            int decryptedId = Convert.ToInt32(protector.Unprotect(request.Id));

            var book = await _assetsService.GetByIdAsync(decryptedId);

            if (book == null)
            {
                return ViewResponse.NotFound;
            }

            try
            {
                await _assetsService.DeleteAsync(book);
                return ViewResponse.OK;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex.Message);
                return ViewResponse.Error;
            }
        }
    }
}
