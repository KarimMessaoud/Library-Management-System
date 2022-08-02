using LibraryData;
using LibraryData.Models.Account;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace Library.Commands.Patron
{
    public class DeletePatronCommand : IRequest<User>
    {
        public string Id { get; }

        public DeletePatronCommand(string id)
        {
            Id = id;
        }
    }

    public class DeleteConfirmedCommandHandler : IRequestHandler<DeletePatronCommand, User>
    {
        private readonly IPatron _patron;
        private readonly UserManager<User> _userManager;

        public DeleteConfirmedCommandHandler(UserManager<User> userManager, IPatron patron)
        {
            _userManager = userManager;
            _patron = patron;
        }

        public async Task<User> Handle(DeletePatronCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == null)
            {
                return null;
            }

            var patron = await _patron.GetAsync(request.Id);

            if (patron == null)
            {
                return null;
            }

            await _userManager.DeleteAsync(patron);

            return patron;
        }
    }
}
