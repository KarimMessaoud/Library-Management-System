using AutoMapper;
using Library.Models.Catalog;
using Library.Security;
using LibraryData;
using MediatR;
using Microsoft.AspNetCore.DataProtection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Library.Queries.Catalog
{
    public class EditVideoQuery : IRequest<AssetEditVideoViewModel>
    {
        public string Id { get; }

        public EditVideoQuery(string id)
        {
            Id = id;
        }
    }

    public class EditVideoQueryHandler : IRequestHandler<EditVideoQuery, AssetEditVideoViewModel>
    {
        private readonly ILibraryAssetService _assetsService;
        private readonly IDataProtector protector;
        private readonly IMapper _mapper;

        public EditVideoQueryHandler(ILibraryAssetService assetsService,
                                    IDataProtectionProvider dataProtectionProvider,
                                    DataProtectionPurposeStrings dataProtectionPurposeStrings,
                                    IMapper mapper)
        {
            _assetsService = assetsService;
            protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.AssetIdRouteValue);
            _mapper = mapper;
        }


        public async Task<AssetEditVideoViewModel> Handle(EditVideoQuery request, CancellationToken cancellationToken)
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

            var model = _mapper.Map<AssetEditVideoViewModel>(asset);

            model.Id = request.Id;
            model.Director = await _assetsService.GetAuthorOrDirectorAsync(decryptedId);

            return model;
        }
    }
}
