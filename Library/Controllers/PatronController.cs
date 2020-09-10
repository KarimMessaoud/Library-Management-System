using Library.Models.Patron;
using LibraryData;
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
    }
}
