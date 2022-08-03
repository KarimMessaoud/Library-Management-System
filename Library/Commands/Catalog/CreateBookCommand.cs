using AutoMapper;
using Library.Models.Catalog;
using LibraryData;
using LibraryData.Models;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Library.Commands.Catalog
{
    public class CreateBookCommand : IRequest
    {
        public AssetCreateBookViewModel Model { get; }

        public CreateBookCommand(AssetCreateBookViewModel model)
        {
            Model = model;
        }
    }

    public class CreateBookCommandHandler : AsyncRequestHandler<CreateBookCommand>
    {
        private readonly IMapper _mapper;
        private readonly LibraryContext _context;
        private readonly ILibraryBranch _branch;
        private readonly ILibraryAssetService _assetsService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CreateBookCommandHandler(IMapper mapper, 
                                        LibraryContext context, 
                                        ILibraryBranch branch, 
                                        ILibraryAssetService assetsService, 
                                        IWebHostEnvironment webHostEnvironment)
        {
            _mapper = mapper;
            _context = context;
            _branch = branch;
            _assetsService = assetsService;
            _webHostEnvironment = webHostEnvironment;
        }

        protected async override Task Handle(CreateBookCommand request, CancellationToken cancellationToken)
        {
            string uniqueFileName = ProcessUploadedAssetFile(request.Model);

            var book = _mapper.Map<Book>(request.Model);

            book.Status = _context.Statuses.FirstOrDefault(x => x.Name == "Available");
            book.ImageUrl = "/images/" + uniqueFileName;
            book.Location = _branch.GetBranchByName(request.Model.LibraryBranchName);

            if (book.Author == null)
            {
                book.Author = "-";
            }

            await _assetsService.AddAsync(book);
        }


        private string ProcessUploadedAssetFile(AssetCreateViewModel model)
        {
            string uniqueFileName = null;

            if (model.Photo != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Photo.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }
    }
}
