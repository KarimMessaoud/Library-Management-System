using AutoMapper;
using Library.Models.Catalog;
using LibraryData;
using LibraryData.Models;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Library.Commands.Catalog
{
    public class CreateVideoCommand : UploadedAssetFileProcessor, IRequest
    {
        public AssetCreateVideoViewModel Model { get; }

        public CreateVideoCommand(AssetCreateVideoViewModel model)
        {
            Model = model;
        }
    }

    public class CreateVideoCommandHandler : AsyncRequestHandler<CreateVideoCommand>
    {
        private readonly IMapper _mapper;
        private readonly LibraryContext _context;
        private readonly ILibraryBranch _branch;
        private readonly ILibraryAssetService _assetsService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CreateVideoCommandHandler(IMapper mapper, 
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

        protected async override Task Handle(CreateVideoCommand request, CancellationToken cancellationToken)
        {
            string uniqueFileName = request.ProcessUploadedAssetFile(request.Model, _webHostEnvironment);

            var video = _mapper.Map<Video>(request.Model);

            video.Status = _context.Statuses.FirstOrDefault(x => x.Name == "Available");
            video.ImageUrl = "/images/" + uniqueFileName;
            video.Location = _branch.GetBranchByName(request.Model.LibraryBranchName);

            if (video.Director == null)
            {
                video.Director = "-";
            }

            await _assetsService.AddAsync(video);
        }
    }
}
