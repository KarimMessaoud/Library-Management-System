using Library.Enums;
using LibraryData;
using LibraryData.Models.Account;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace Library.Commands.Patron
{
    public class DeletePatronCommand : IRequest<ViewResponse>
    {
        public string Id { get; }

        public DeletePatronCommand(string id)
        {
            Id = id;
        }
    }

    public class DeleteConfirmedCommandHandler : IRequestHandler<DeletePatronCommand, ViewResponse>
    {
        private readonly IPatron _patron;
        private readonly UserManager<User> _userManager;

        public DeleteConfirmedCommandHandler(UserManager<User> userManager, IPatron patron)
        {
            _userManager = userManager;
            _patron = patron;
        }

        public async Task<ViewResponse> Handle(DeletePatronCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == null)
            {
                return ViewResponse.NotFound;
            }

            var patron = await _patron.GetAsync(request.Id);

            if (patron == null)
            {
                return ViewResponse.NotFound;
            }

            await _userManager.DeleteAsync(patron);

            return ViewResponse.OK;
        }
    }
}
