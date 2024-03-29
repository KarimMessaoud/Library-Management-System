﻿using AutoMapper;
using Library.Models.Administration;
using Library.Queries.Administration;
using LibraryData.Models.Account;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Library.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<AdministrationController> _logger;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public AdministrationController(RoleManager<IdentityRole> roleManager,
                                        UserManager<User> userManager,
                                        ILogger<AdministrationController> logger,
                                        IMapper mapper, 
                                        IMediator mediator)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
            _mediator = mediator;
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }


        [HttpGet]
        public IActionResult RolesList()
        {
            var roles = _roleManager.Roles;

            return View(roles);
        }


        [HttpGet]
        public async Task<IActionResult> UsersList()
        {
            var users = await _userManager.Users.ToListAsync();

            return View(users);
        }


        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = new IdentityRole
                {
                    Name = model.RoleName
                };

                var result = await _roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("RolesList", "Administration");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var result = await _mediator.Send(new EditRoleQuery(id));

            if (result == null) 
            {
                ViewBag.ErrorMessage = $"Role with id: {id} cannot be found.";
                return View("NotFound"); 
            }

            return View(result);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with id: {model.Id} cannot be found.";
                return View("NotFound");
            }

            role.Name = model.RoleName;
            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                return RedirectToAction("RolesList");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.RoleId = roleId;

            var result = await _mediator.Send(new EditUsersInRoleQuery(roleId));

            if(result == null)
            {
                ViewBag.ErrorMessage = $"Role with id: {roleId} cannot be found.";
                return View("NotFound");
            }

            return View(result);
        }


        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with id: {roleId} cannot be found.";
                return View("NotFound");
            }

            //Do not allow to remove any user from the patron role
            if (role.Name == "Patron" && model.Any(x => x.IsSelected == false))
            {
                ModelState.AddModelError("", "You cannot remove any user from the patron role!");
                return View(model);
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await _userManager.FindByIdAsync(model[i].UserId);

                IdentityResult result = null;

                if (model[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await _userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    if (i < model.Count - 1) continue;
                    else return RedirectToAction("EditRole", new { Id = role.Id });
                }
            }

            return RedirectToAction("EditRole", new { Id = role.Id });

        }


        [HttpPost]
        [Authorize(Policy = "DeleteRolePolicy")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with id: {id} cannot be found.";
                return View("NotFound");
            }

            try
            {
                var result = await _roleManager.DeleteAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("RolesList");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View("RolesList");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Exception occured: {ex}");
                ViewBag.ErrorTitle = $"{role.Name} role is in use.";
                ViewBag.ErrorMessage = $"{role.Name} role cannot be deleted." +
                    $" If you want to delete this role, please remove the users" +
                    $" from the role and then try to delete.";
                return View("Error");
            }
        }


        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with id: {id} cannot be found.";
                return View("NotFound");
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("UsersList");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View("UsersList");
        }


        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var result = await _mediator.Send(new EditUserQuery(id));

            if(result == null)
            {
                ViewBag.ErrorMessage = $"User with id: {id} cannot be found.";
                return View("NotFound");
            }

            return View(result);
        }


        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with id: {model.Id} cannot be found.";
                return View("NotFound");
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.UserName = model.UserName;
            user.Email = model.Email;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("UsersList", "Administration");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> ManageUserClaims(string userId)
        {
            var result = await _mediator.Send(new ManageUserClaimsQuery(userId));

            if(result == null)
            {
                ViewBag.ErrorMessage = $"User with id: {userId} cannot be found.";
                return View("NotFound");
            }

            return View(result);
        }


        [HttpPost]
        public async Task<IActionResult> ManageUserClaims(ManageUserClaimsViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with id: {model.UserId} cannot be found.";
                return View("NotFound");
            }

            var claims = await _userManager.GetClaimsAsync(user);

            var result = await _userManager.RemoveClaimsAsync(user, claims);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove existing user claims");
                return View(model);
            }

            result = await _userManager.AddClaimsAsync(user, model.Claims
                .Select(x => new Claim(x.ClaimType, x.IsSelected ? "true" : "false")));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected claims to the user");
                return View(model);
            }

            return RedirectToAction("EditUser", new { Id = model.UserId });
        }


        [HttpGet]
        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            ViewBag.UserId = userId;

            var result = await _mediator.Send(new ManageUserRolesQuery(userId));

            if(result == null)
            {
                ViewBag.ErrorMessage = $"User with id: {userId} cannot be found.";
                return View("NotFound");
            }

            return View(result);
        }


        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(List<ManageUserRolesViewModel> model, string userId)
        {
            //Do not allow uncheck the patron role
            var patronRoleItem = model.FirstOrDefault(x => x.RoleName == "Patron");

            if (patronRoleItem.IsSelected == false)
            {
                ModelState.AddModelError("", "You cannot remove the patron role!");
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with id: {userId} cannot be found.";
                return View("NotFound");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles.");
                return View(model);
            }

            result = await _userManager.AddToRolesAsync(user, model.Where(x => x.IsSelected).Select(x => x.RoleName));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to the user.");
                return View(model);
            }

            return RedirectToAction("EditUser", new { id = userId });
        }
    }
}
