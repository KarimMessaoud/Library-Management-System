using Library.Models.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Library.Controllers
{
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        
        [HttpGet]
        public IActionResult RolesList()
        {
            var roles = _roleManager.Roles;

            return View(roles);
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
    }
}
