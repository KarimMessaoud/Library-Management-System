using Library.Models.Administration;
using LibraryData.Models.Account;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace Library.Queries.Administration
{
    public class EditRoleQuery : IRequest<EditRoleViewModel>
    {
        public string Id { get; }

        public EditRoleQuery(string id)
        {
            Id = id;
        }
    }

    public class EditRoleQueryHandler : IRequestHandler<EditRoleQuery, EditRoleViewModel>
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;

        public EditRoleQueryHandler(RoleManager<IdentityRole> roleManager, 
                                      UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<EditRoleViewModel> Handle(EditRoleQuery request, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(request.Id);

            if (role == null)
            {
                return null;
            }

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            foreach (var user in _userManager.Users)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

            return model;
        }
    }
}
