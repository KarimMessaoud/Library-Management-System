using Library.Models.Patron;
using LibraryData;
using LibraryData.Models;
using LibraryData.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Controllers
{
    public class PatronController : Controller
    {
        private readonly IPatron _patron;
        private readonly ILibraryBranch _branch;
        private readonly UserManager<User> _userManager;
        public PatronController(IPatron patron,
            ILibraryBranch branch,
            UserManager<User> userManager)
        {
            _patron = patron;
            _branch = branch;
            _userManager = userManager;
        }

        public IActionResult Index(string searchString)
        {
            var allPatrons = _patron.GetAll();

            if (!String.IsNullOrEmpty(searchString))
            {
                allPatrons = allPatrons.Where(x => x.LastName.Contains(searchString));
            }

            var patronModels = allPatrons.Select(x => new PatronDetailModel()
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                LibraryCardId = x.LibraryCard.Id,
                OverdueFees = x.LibraryCard.Fees,
                HomeLibraryBranch = x.HomeLibraryBranch.Name
            })
                .OrderBy(x => x.LastName)
                .ToList();

            var model = new PatronIndexModel()
            {
                Patrons = patronModels
            };


            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(string id)
        {
            if (id == null)
            {
                return View("NoIdFound");
            }

            var patron = _patron.Get(id);

            if (patron == null)
            {
                Response.StatusCode = 404;
                return View("PatronNotFound", id);
            }

            var model = new PatronDetailModel()
            {
                Id = patron.Id,
                LastName = patron.LastName,
                FirstName = patron.FirstName,
                Address = patron.Address,
                HomeLibraryBranch = patron.HomeLibraryBranch.Name,
                MemberSince = patron.LibraryCard.Created,
                OverdueFees = patron.LibraryCard.Fees,
                LibraryCardId = patron.LibraryCard.Id,
                Telephone = patron.PhoneNumber,
                AssetsCheckedOut = await _patron.GetCheckouts(id) ?? new List<Checkout>(),
                CheckoutHistory = await _patron.GetCheckoutHistory(id),
                Holds = await _patron.GetHolds(id)
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            if (id == null)
            {
                return View("NoIdFound");
            }

            var patron = _patron.Get(id);

            if (patron == null)
            {
                Response.StatusCode = 404;
                return View("PatronNotFound", id);
            }

            var model = new PatronEditViewModel
            {
                Id = patron.Id,
                FirstName = patron.FirstName,
                LastName = patron.LastName,
                Address = patron.Address,
                DateOfBirth = patron.DateOfBirth,
                HomeLibraryBranchName = patron.HomeLibraryBranch.Name,
                Telephone = patron.PhoneNumber
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PatronEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var patron = _patron.Get(model.Id);

                if (patron == null)
                {
                    Response.StatusCode = 404;
                    return View("PatronNotFound", model.Id);
                }

                patron.FirstName = model.FirstName;
                patron.LastName = model.LastName;
                patron.Address = model.Address;
                patron.DateOfBirth = model.DateOfBirth;
                patron.PhoneNumber = model.Telephone;
                patron.HomeLibraryBranch = _branch.GetBranchByName(model.HomeLibraryBranchName);

                await _userManager.UpdateAsync(patron);

                return RedirectToAction("Index", "Patron", new { id = patron.Id });
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Delete(string id)
        {
            if (id == null)
            {
                return View("NoIdFound");
            }

            var patron = _patron.Get(id);

            if (patron == null)
            {
                Response.StatusCode = 404;
                return View("PatronNotFound", id);
            }

            var model = new PatronEditViewModel
            {
                Id = patron.Id,
                FirstName = patron.FirstName,
                LastName = patron.LastName,
                Address = patron.Address,
                DateOfBirth = patron.DateOfBirth,
                HomeLibraryBranchName = patron.HomeLibraryBranch.Name,
                Telephone = patron.PhoneNumber
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (id == null)
            {
                return View("NoIdFound");
            }

            var patron = _patron.Get(id);

            if (patron == null)
            {
                return NotFound();
            }

            await _userManager.DeleteAsync(patron);

            return RedirectToAction("Index", "Patron");
        }
    }
}
