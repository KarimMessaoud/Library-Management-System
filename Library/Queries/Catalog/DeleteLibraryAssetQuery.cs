using AutoMapper;
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
    public class DeleteLibraryAssetQuery : IRequest<AssetEditBookViewModel>
    {
        public string Id { get; }

        public DeleteLibraryAssetQuery(string id)
        {
            Id = id;
        }
    }

    public class DeleteLibraryAssetQueryHandler : IRequestHandler<DeleteLibraryAssetQuery, AssetEditBookViewModel>
    {
        private readonly ILibraryAssetService _assetsService;
        private readonly IDataProtector protector;
        private readonly IMapper _mapper;

        public DeleteLibraryAssetQueryHandler(ILibraryAssetService assetsService,
                                              IDataProtectionProvider dataProtectionProvider,
                                              DataProtectionPurposeStrings dataProtectionPurposeStrings,
                                              IMapper mapper)
        {
            _assetsService = assetsService;
            protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.AssetIdRouteValue);
            _mapper = mapper;
        }

        public async Task<AssetEditBookViewModel> Handle(DeleteLibraryAssetQuery request, CancellationToken cancellationToken)
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

            var model = _mapper.Map<AssetEditBookViewModel>(asset);

            model.Id = request.Id;
            model.DecryptedId = decryptedId;
            model.Author = await _assetsService.GetAuthorOrDirectorAsync(decryptedId);
            model.ISBN = await _assetsService.GetIsbnAsync(decryptedId);

            return model;
        }
    }
}
