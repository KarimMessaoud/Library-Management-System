using Library.Models.Administration;
using LibraryData.Models.Account;
using LibraryData.Models.Administration;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Library.Queries.Administration
{
    public class ManageUserClaimsQuery : IRequest<ManageUserClaimsViewModel>
    {
        public string UserId { get; }

        public ManageUserClaimsQuery(string userId)
        {
            UserId = userId;
        }
    }

    public class ManageUserClaimsQueryHandler : IRequestHandler<ManageUserClaimsQuery, ManageUserClaimsViewModel>
    {
        private readonly UserManager<User> _userManager;

        public ManageUserClaimsQueryHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ManageUserClaimsViewModel> Handle(ManageUserClaimsQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user == null)
            {
                return null;
            }

            var existingUserClaims = await _userManager.GetClaimsAsync(user);

            var model = new ManageUserClaimsViewModel
            {
                UserId = request.UserId
            };

            foreach (var claim in ClaimsStore.AllClaims)
            {
                var userClaim = new UserClaim
                {
                    ClaimType = claim.Type
                };

                if (existingUserClaims.Any(x => x.Type == claim.Type && x.Value == "true"))
                {
                    userClaim.IsSelected = true;
                }

                model.Claims.Add(userClaim);
            }

            return model;
        }
    }
}
