using Library.Models.Patron;
using LibraryData;
using LibraryData.Models;
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
        public PatronController(IPatron patron)
        {
            _patron = patron;
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
    }
}
