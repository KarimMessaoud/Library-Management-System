using System.Linq;
using System.Threading.Tasks;
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
                //Asynchronous operations used in a blocking manner below
                NumberOfAssets = _branch.GetAssetsAsync(x.Id).Result.Count(),
                NumberOfPatrons = _branch.GetPatronsAsync(x.Id).Result.Count()
            });

            var model = new BranchIndexViewModel()
            {
                Branches = branches
            };

            return View(model);
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

            var model = new BranchDetailViewModel()
            {
                Id = branch.Id,
                Name = branch.Name,
                Address = branch.Address,
                Telephone = branch.Telephone,
                Description = branch.Description,
                OpenDate = branch.OpenDate.ToString("yyyy-MM-dd"),
                ImageUrl = branch.ImageUrl,
                HoursOpen = _branch.GetBranchHours(id.Value)
            };

            var branchAssets = await _branch.GetAssetsAsync(id.Value);

            model.NumberOfAssets = branchAssets.Count();
            model.TotalAssetValue = branchAssets.Sum(x => x.Cost);
            model.NumberOfPatrons = (await _branch.GetPatronsAsync(id.Value)).Count();
            
            return View(model);
        }
    }
}