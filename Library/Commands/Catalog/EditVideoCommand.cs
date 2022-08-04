using Library.Models.Catalog;
using Library.Security;
using LibraryData;
using LibraryData.Models;
using MediatR;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Library.Commands.Catalog
{
    public class EditVideoCommand : UploadedAssetFileProcessor, IRequest<Video>
    {
        public AssetEditVideoViewModel Model { get; }

        public EditVideoCommand(AssetEditVideoViewModel model)
        {
            Model = model;
        }
    }

    public class EditVideoCommandHandler : IRequestHandler<EditVideoCommand, Video>
    {
        private readonly ILibraryAssetService _assetsService;
        private readonly IDataProtector protector;
        private readonly ILibraryBranch _branch;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EditVideoCommandHandler(ILibraryAssetService assetsService, 
                                      IDataProtectionProvider dataProtectionProvider, 
                                      DataProtectionPurposeStrings dataProtectionPurposeStrings, 
                                      ILibraryBranch branch, 
                                      IWebHostEnvironment webHostEnvironment)
        {
            _assetsService = assetsService;
            protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.AssetIdRouteValue);
            _branch = branch;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<Video> Handle(EditVideoCommand request, CancellationToken cancellationToken)
        {
            int decryptedId = Convert.ToInt32(protector.Unprotect(request.Model.Id));

            var video = await _assetsService.GetVideoByIdAsync(decryptedId);

            if (video == null)
            {
                return null;
            }

            video.Title = request.Model.Title;
            video.Director = request.Model.Director;
            video.Year = request.Model.Year;
            video.Cost = request.Model.Cost;
            video.NumberOfCopies = request.Model.NumberOfCopies;

            if (request.Model.Photo != null)
            {
                if (request.Model.ExistingPhotoPath != null && request.Model.ExistingPhotoPath != "/images/")
                {
                    string filePath = Path.Join(_webHostEnvironment.WebRootPath, request.Model.ExistingPhotoPath);
                    System.IO.File.Delete(filePath);
                }

                string uniqueFileName = request.ProcessUploadedAssetFile(request.Model, _webHostEnvironment);

                video.ImageUrl = "/images/" + uniqueFileName;
            }

            video.Location = _branch.GetBranchByName(request.Model.LibraryBranchName);

            await _assetsService.UpdateAsync(video);

            return video;
        }
    }
}
