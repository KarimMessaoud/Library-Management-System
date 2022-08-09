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
    public class EditBookQuery : IRequest<AssetEditBookViewModel>
    {
        public string Id { get; }

        public EditBookQuery(string id)
        {
            Id = id;
        }
    }

    public class EditBookQueryHandler : IRequestHandler<EditBookQuery, AssetEditBookViewModel>
    {
        private readonly ILibraryAssetService _assetsService;
        private readonly IDataProtector protector;
        private readonly IMapper _mapper;

        public EditBookQueryHandler(ILibraryAssetService assetsService,
                                    IDataProtectionProvider dataProtectionProvider,
                                    DataProtectionPurposeStrings dataProtectionPurposeStrings,
                                    IMapper mapper)
        {
            _assetsService = assetsService;
            protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.AssetIdRouteValue);
            _mapper = mapper;
        }


        public async Task<AssetEditBookViewModel> Handle(EditBookQuery request, CancellationToken cancellationToken)
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
            model.Author = await _assetsService.GetAuthorOrDirectorAsync(decryptedId);
            model.ISBN = await _assetsService.GetIsbnAsync(decryptedId);

            return model;
        }
    }
}
