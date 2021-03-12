using Hangfire;
using Library.Models.Patron;
using Library.Security;
using LibraryData;
using LibraryData.Models;
using LibraryData.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly ICheckout _checkout;
        private readonly IDataProtector protector;
        public PatronController(IPatron patron,
            ILibraryBranch branch,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<AccountController> logger,
            ICheckout checkout,
            IDataProtectionProvider dataProtectionProvider,
            DataProtectionPurposeStrings dataProtectionPurposeStrings)
        {
            _patron = patron;
            _branch = branch;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _checkout = checkout;
            protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.AssetIdRouteValue);
        }

        [Authorize(Roles = "Admin, Employee")]
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
        [AllowAnonymous]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PatronCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.Email,
                    Email = model.Email,
                    Address = model.Address,
                    DateOfBirth = model.DateOfBirth,
                    PhoneNumber = model.Telephone,
                    LibraryCard = new LibraryCard
                    {
                        Fees = 0,
                        Created = DateTime.Now
                    },
                    HomeLibraryBranch = _branch.GetBranchByName(model.HomeLibraryBranchName)
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var confirmationLink = Url.Action("ConfirmEmail", "Account",
                        new { userId = user.Id, token = token }, Request.Scheme);

                    _logger.Log(LogLevel.Warning, confirmationLink);

                    var addToRoleResult = await _userManager.AddToRoleAsync(user, "Patron");
                    if (!addToRoleResult.Succeeded)
                    {
                        ModelState.AddModelError("", "Cannot add user to the Patron role.");
                        return View(model);
                    }

                    BackgroundJob.Enqueue<IEmailService>(x => x.SendEmailAsync(user.FirstName, user.Email, "Email confirmation",
                        $"Congratulations! You are registered. </br> This is your libraryCardId: {user.LibraryCard.Id} </br>" +
                        $"<a href= \"{confirmationLink}\">Please confirm your email address " +
                        $"by clicking this text</a>"));

                    if (_signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        return RedirectToAction("UsersList", "Administration");
                    }

                    ViewBag.ErrorTitle = "Registration successful";
                    ViewBag.ErrorMessage = $"This is your libraryCardId: {user.LibraryCard.Id}. Before you can log in," +
                    $" please confirm your email, by clicking on the confirmation link " +
                    $"we have emailed you.";

                    return View("~/Views/Account/RegistrationSuccessful.cshtml");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> ChargeFeesAsync(string patronId)
        {
            await _checkout.ChargeOverdueFeesAsync(patronId);
            return RedirectToAction("Detail", new { id = patronId });
        }

        [Authorize(Roles = "Admin, Employee")]
        public IActionResult ResetFees(string patronId)
        {
            _checkout.ResetOverdueFees(patronId);
            return RedirectToAction("Detail", new { id = patronId });
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Employee, Patron")]
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
   
            // Logged in patron can see only his own profile
            if (User.IsInRole("Patron") 
                && !User.IsInRole("Employee")
                && !User.IsInRole("Admin")
                && _userManager.GetUserId(User) != id)
            {
                return View("~/Views/Administration/AccessDenied.cshtml");
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

            //Encrypt Library Assets' Ids in order to be able to get to details of the checkout items
            //from Patron's detail view  
            foreach (var item in model.AssetsCheckedOut)
            {
                item.LibraryAsset.EncryptedId = protector.Protect(item.LibraryAsset.Id.ToString());
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Employee, Patron")]
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

            // Logged in patron can see only his own profile
            if (User.IsInRole("Patron")
                && !User.IsInRole("Employee")
                && !User.IsInRole("Admin")
                && _userManager.GetUserId(User) != id)
            {
                return View("~/Views/Administration/AccessDenied.cshtml");
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
        [Authorize(Roles = "Admin, Employee, Patron")]
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
        [Authorize(Roles = "Admin, Employee")]
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

            //Check if there are any items that were checked out by the patron and not turned back. 
            // If so do not allow to delete this patron.
            var checkouts = _patron.GetCheckouts(id);

            var holds = _patron.GetHolds(id);

            if (checkouts.Result.Any() || holds.Result.Any())
            {
                return View("DeletingForbidden", id);
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
        [Authorize(Roles = "Admin, Employee")]
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
