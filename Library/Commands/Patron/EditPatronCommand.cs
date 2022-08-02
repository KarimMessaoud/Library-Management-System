using Library.Models.Patron;
using LibraryData;
using LibraryData.Models.Account;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Library.Commands.Patron
{
    public class EditPatronCommand : IRequest<User>
    {
        public PatronEditViewModel Model { get; set; }

        public EditPatronCommand(PatronEditViewModel model)
        {
            Model = model;
        }
    }

    public class EditPatronCommandHandler : IRequestHandler<EditPatronCommand, User>
    {
        private readonly IPatron _patron;
        private readonly ILibraryBranch _branch;
        private readonly UserManager<User> _userManager;

        public EditPatronCommandHandler(IPatron patron, ILibraryBranch branch, UserManager<User> userManager)
        {
            _patron = patron;
            _branch = branch;
            _userManager = userManager;
        }

        public async Task<User> Handle(EditPatronCommand request, CancellationToken cancellationToken)
        {
            var patron = await _patron.GetAsync(request.Model.Id);

            if (patron == null)
            {
                return null;
            }

            patron.FirstName = request.Model.FirstName;
            patron.LastName = request.Model.LastName;
            patron.Address = request.Model.Address;
            patron.DateOfBirth = request.Model.DateOfBirth;
            patron.PhoneNumber = request.Model.Telephone;
            patron.HomeLibraryBranch = _branch.GetBranchByName(request.Model.HomeLibraryBranchName);

            await _userManager.UpdateAsync(patron);

            return patron;
        }
    }
}
