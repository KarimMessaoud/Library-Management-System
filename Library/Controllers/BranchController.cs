using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Library.Models.Branch;
using Library.Queries.Branch;
using LibraryData;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    [AllowAnonymous]
    public class BranchController : Controller
    {
        private readonly ILibraryBranch _branch;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        public BranchController(ILibraryBranch branch, IMapper mapper, IMediator mediator)
        {
            _branch = branch;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _mediator.Send(new GetAllBranchesQuery()));
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return View("NoIdFound");
            }

            var branch = _branch.GetBranchById(id.Value);

            if (branch == null)
            {
                Response.StatusCode = 404;
                return View("BranchNotFound", id);
            }

            var model = _mapper.Map<BranchDetailViewModel>(branch);
            model.HoursOpen = _branch.GetBranchHours(id.Value);

            var branchAssets = await _branch.GetAssetsAsync(id.Value);

            model.NumberOfAssets = branchAssets.Count();
            model.TotalAssetValue = branchAssets.Sum(x => x.Cost);
            model.NumberOfPatrons = (await _branch.GetPatronsAsync(id.Value)).Count();
            
            return View(model);
        }
    }
}