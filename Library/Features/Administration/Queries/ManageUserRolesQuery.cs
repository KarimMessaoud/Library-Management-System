using Library.Models.Administration;
using LibraryData.Models.Account;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Library.Queries.Administration
{
    public class ManageUserRolesQuery : IRequest<List<ManageUserRolesViewModel>>
    {
        public string UserId { get; }

        public ManageUserRolesQuery(string userId)
        {
            UserId = userId;
        }
    }

    public class ManageUserRolesQueryHandler : IRequestHandler<ManageUserRolesQuery, List<ManageUserRolesViewModel>>
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ManageUserRolesQueryHandler(UserManager<User> userManager, 
                                           RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<ManageUserRolesViewModel>> Handle(ManageUserRolesQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user == null)
            {
                return null;
            }

            var model = new List<ManageUserRolesViewModel>();

            foreach (var role in _roleManager.Roles)
            {
                var manageUserRolesViewModel = new ManageUserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    manageUserRolesViewModel.IsSelected = true;
                }
                else
                {
                    manageUserRolesViewModel.IsSelected = false;
                }
                model.Add(manageUserRolesViewModel);
            }

            return model;
        }
    }
}
