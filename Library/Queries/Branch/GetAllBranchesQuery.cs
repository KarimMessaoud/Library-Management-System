using Library.Models.Branch;
using LibraryData;
using MediatR;
using System.Linq;

namespace Library.Queries.Branch
{
    public class GetAllBranchesQuery : IRequest<BranchIndexViewModel>{}

    public class  GetAllBranchesQueryHandler : RequestHandler<GetAllBranchesQuery, BranchIndexViewModel>
    {
        private readonly ILibraryBranch _branch;

        public GetAllBranchesQueryHandler(ILibraryBranch branch)
        {
            _branch = branch;
        }

        protected override BranchIndexViewModel Handle(GetAllBranchesQuery request)
        {
            var branches = _branch.GetAll().Select(x => new BranchDetailViewModel()
            {
                Id = x.Id,
                Name = x.Name,
                IsOpen = _branch.IsBranchOpen(x.Id),
                //Asynchronous operations used in a blocking manner below
                NumberOfAssets = _branch.GetAssetsAsync(x.Id).Result.Count(),
                NumberOfPatrons = _branch.GetPatronsAsync(x.Id).Result.Count()
            });

            var model = new BranchIndexViewModel()
            {
                Branches = branches
            };

            return model;
        }
    }
}
