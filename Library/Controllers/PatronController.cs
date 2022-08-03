using AutoMapper;
using Hangfire;
using Library.Commands.Patron;
using Library.Enums;
using Library.Models.Patron;
using Library.Queries.Patron;
using LibraryData;
using LibraryData.Models;
using LibraryData.Models.Account;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Library.Controllers
{
    public class PatronController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILibraryBranch _branch;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly ICheckout _checkout;
        private readonly IMapper _mapper;

        public PatronController(IMediator mediator,
                                ILibraryBranch branch,
                                UserManager<User> userManager,
                                SignInManager<User> signInManager,
                                ILogger<AccountController> logger,
                                ICheckout checkout,
                                IMapper mapper)
        {
            _mediator = mediator;
            _branch = branch;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _checkout = checkout;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> Index(string searchString)
        {
            return View(await _mediator.Send(new GetAllPatronsQuery(searchString)));
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
                var user = _mapper.Map<User>(model);

                user.LibraryCard = new LibraryCard
                {
                    Fees = 0,
                    Created = DateTime.Now
                };

                user.HomeLibraryBranch = _branch.GetBranchByName(model.HomeLibraryBranchName);

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


        public async Task<IActionResult> ChargeFees(string patronId)
        {
            await _checkout.ChargeOverdueFeesAsync(patronId);
            return RedirectToAction("Detail", new { id = patronId });
        }


        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> ResetFees(string patronId)
        {
            await _checkout.ResetOverdueFeesAsync(patronId);
            return RedirectToAction("Detail", new { id = patronId });
        }


        [HttpGet]
        [Authorize(Roles = "Admin, Employee, Patron")]
        public async Task<IActionResult> Detail(string id)
        {
            var model = await _mediator.Send(new GetPatronByIdQuery(id));

            if(model == null) return View("PatronNotFound", id);


            // Logged in patron can see only his own profile
            if (User.IsInRole("Patron") && !User.IsInRole("Employee") && !User.IsInRole("Admin")
                && _userManager.GetUserId(User) != id)
            {
                return View("~/Views/Administration/AccessDenied.cshtml");
            }

            return View(model);
        }


        [HttpGet]
        [Authorize(Roles = "Admin, Employee, Patron")]
        public async Task<IActionResult> Edit(string id)
        {
            var model = await _mediator.Send(new EditPatronQuery(id));

            if(model == null) return View("PatronNotFound", id);

            // Logged in patron can see only his own profile
            if (User.IsInRole("Patron") && !User.IsInRole("Employee") && !User.IsInRole("Admin")
                && _userManager.GetUserId(User) != id)
            {
                return View("~/Views/Administration/AccessDenied.cshtml");
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Employee, Patron")]
        public async Task<IActionResult> Edit(PatronEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var patron = await _mediator.Send(new EditPatronCommand(model));

                if(patron == null) return View("PatronNotFound", model.Id);
                else return RedirectToAction("Index", "Patron", new { id = patron.Id });
            }

            return View(model);
        }


        [HttpGet]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> Delete(string id)
        {
            var model = await _mediator.Send(new DeletePatronQuery(id));

            if(model == null) return View("PatronNotFound", id);

            //If patron has checked out some items (and did not turn them back)
            //or has placed hold on them one cannot delete this patron
            if (model.PatronActionState == ViewResponse.DeletingForbidden)
            {
                return View("DeletingForbidden", id);
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var patron = await _mediator.Send(new DeletePatronCommand(id));

            if (patron == null) return View("PatronNotFound", id);

            return RedirectToAction("Index", "Patron");
        }
    }
}
