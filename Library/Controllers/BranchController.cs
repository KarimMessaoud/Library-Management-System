using System.Linq;
using Library.Models.Branch;
using LibraryData;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    public class BranchController : Controller
    {
        private readonly ILibraryBranch _branch;
        public BranchController(ILibraryBranch branch)
        {
            _branch = branch;
        }

        public IActionResult Index()
        {
            var branches = _branch.GetAll().Select(x => new BranchDetailViewModel()
            {
                Id = x.Id,
                Name = x.Name,
                IsOpen = _branch.IsBranchOpen(x.Id),
                NumberOfAssets = _branch.GetAssets(x.Id).Count(),
                NumberOfPatrons = _branch.GetPatrons(x.Id).Count()
            });

            var model = new BranchIndexViewModel()
            {
                Branches = branches
            };

            return View(model);
        }
    }
}