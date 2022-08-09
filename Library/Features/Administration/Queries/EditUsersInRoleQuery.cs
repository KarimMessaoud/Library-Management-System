using Library.Models.Administration;
using LibraryData.Models.Account;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Library.Queries.Administration
{
    public class EditUsersInRoleQuery : IRequest<List<UserRoleViewModel>>
    {
        public string RoleId { get; }

        public EditUsersInRoleQuery(string roleId)
        {
            RoleId = roleId;
        }
    }

    public class EditUsersInRoleQueryHandler : IRequestHandler<EditUsersInRoleQuery, List<UserRoleViewModel>>
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;

        public EditUsersInRoleQueryHandler(RoleManager<IdentityRole> roleManager,
                                           UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<List<UserRoleViewModel>> Handle(EditUsersInRoleQuery request, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(request.RoleId);

            if (role == null)
            {
                return null;
            }

            var model = new List<UserRoleViewModel>();

            foreach (var user in _userManager.Users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }
                model.Add(userRoleViewModel);
            }

            return model;
        }
    }
}
