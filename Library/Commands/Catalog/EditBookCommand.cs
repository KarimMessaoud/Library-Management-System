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
    public class EditBookCommand : UploadedAssetFileProcessor, IRequest<Book>
    {
        public AssetEditBookViewModel Model { get; }

        public EditBookCommand(AssetEditBookViewModel model)
        {
            Model = model;
        }
    }

    public class EditBookCommandHandler : IRequestHandler<EditBookCommand, Book>
    {
        private readonly ILibraryAssetService _assetsService;
        private readonly IDataProtector protector;
        private readonly ILibraryBranch _branch;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EditBookCommandHandler(ILibraryAssetService assetsService, 
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

        public async Task<Book> Handle(EditBookCommand request, CancellationToken cancellationToken)
        {
            int decryptedId = Convert.ToInt32(protector.Unprotect(request.Model.Id));

            var book = await _assetsService.GetBookByIdAsync(decryptedId);

            if (book == null)
            {
                return null;
            }

            book.Title = request.Model.Title;
            book.Author = request.Model.Author;
            book.Year = request.Model.Year;
            book.ISBN = request.Model.ISBN;
            book.Cost = request.Model.Cost;
            book.NumberOfCopies = request.Model.NumberOfCopies;

            if (request.Model.Photo != null)
            {
                if (request.Model.ExistingPhotoPath != null && request.Model.ExistingPhotoPath != "/images/")
                {
                    string filePath = Path.Join(_webHostEnvironment.WebRootPath, request.Model.ExistingPhotoPath);
                    System.IO.File.Delete(filePath);
                }

                string uniqueFileName = request.ProcessUploadedAssetFile(request.Model, _webHostEnvironment);

                book.ImageUrl = "/images/" + uniqueFileName;
            }

            book.Location = _branch.GetBranchByName(request.Model.LibraryBranchName);

            await _assetsService.UpdateAsync(book);

            return book;
        }
    }
}
