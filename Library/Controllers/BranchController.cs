using System.Linq;
using Library.Models.Branch;
using LibraryData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    [AllowAnonymous]
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

        public IActionResult Detail(int? id)
        {
            if (id == null)
            {
                return View("NoIdFound");
            }

            var branch = _branch.GetBranchById((int)id);

            if (branch == null)
            {
                Response.StatusCode = 404;
                return View("BranchNotFound", id);
            }

            var model = new BranchDetailViewModel()
            {
                Id = branch.Id,
                Name = branch.Name,
                Address = branch.Address,
                Telephone = branch.Telephone,
                Description = branch.Description,
                OpenDate = branch.OpenDate.ToString("yyyy-MM-dd"),
                NumberOfAssets = _branch.GetAssets((int)id).Count(),
                NumberOfPatrons = _branch.GetPatrons((int)id).Count(),
                TotalAssetValue = _branch.GetAssets((int)id).Sum(x => x.Cost),
                ImageUrl = branch.ImageUrl,
                HoursOpen = _branch.GetBranchHours((int)id)
            };

            return View(model);
        }
    }
}