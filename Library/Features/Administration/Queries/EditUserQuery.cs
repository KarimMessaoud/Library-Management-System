using AutoMapper;
using Library.Models.Administration;
using LibraryData.Models.Account;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Library.Queries.Administration
{
    public class EditUserQuery : IRequest<EditUserViewModel>
    {
        public string Id { get; }

        public EditUserQuery(string id)
        {
            Id = id;
        }
    }

    public class EditUserQueryHandler : IRequestHandler<EditUserQuery, EditUserViewModel>
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public EditUserQueryHandler(UserManager<User> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<EditUserViewModel> Handle(EditUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id);

            if (user == null)
            {
                return null;
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var userClaims = await _userManager.GetClaimsAsync(user);

            var model = _mapper.Map<EditUserViewModel>(user);
            model.Roles = userRoles;

            List<AuxiliaryClaim> AuxiliaryClaims = new List<AuxiliaryClaim>();

            foreach (var claim in userClaims)
            {
                if (claim.Value == "true") AuxiliaryClaims.Add(new AuxiliaryClaim { Type = claim.Type, Value = "Yes" });
                else AuxiliaryClaims.Add(new AuxiliaryClaim { Type = claim.Type, Value = "No" });
            }

            model.Claims = AuxiliaryClaims.Select(x => x.Type + " : " + x.Value).ToList();

            return model;
        }
    }
}
