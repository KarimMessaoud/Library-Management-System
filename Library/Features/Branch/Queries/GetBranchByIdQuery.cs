using AutoMapper;
using Library.Models.Branch;
using LibraryData;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Library.Queries.Branch
{
    public class GetBranchByIdQuery : IRequest<BranchDetailViewModel>
    {
        public int? Id { get; }

        public GetBranchByIdQuery(int id)
        {
            Id = id;
        }
    }

    public class GetBranchByIdQueryHandler : IRequestHandler<GetBranchByIdQuery, BranchDetailViewModel>
    {
        private readonly ILibraryBranch _branch;
        private readonly IMapper _mapper;

        public GetBranchByIdQueryHandler(ILibraryBranch branch, IMapper mapper)
        {
            _branch = branch;
            _mapper = mapper;
        }

        public async Task<BranchDetailViewModel> Handle(GetBranchByIdQuery request, CancellationToken cancellationToken)
        {
            if (request.Id == null)
            {
                return null;
            }

            var branch = _branch.GetBranchById(request.Id.Value);

            if (branch == null)
            {
                return null;
            }

            var model = _mapper.Map<BranchDetailViewModel>(branch);
            model.HoursOpen = _branch.GetBranchHours(request.Id.Value);

            var branchAssets = await _branch.GetAssetsAsync(request.Id.Value);

            model.NumberOfAssets = branchAssets.Count();
            model.TotalAssetValue = branchAssets.Sum(x => x.Cost);
            model.NumberOfPatrons = (await _branch.GetPatronsAsync(request.Id.Value)).Count();

            return model;
        }
    }
}
