using AutoMapper;
using Library.Enums;
using Library.Models.Patron;
using LibraryData;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Library.Queries.Patron
{
    public class DeletePatronQuery : IRequest<PatronEditViewModel>
    {
        public string Id { get; }
        public DeletePatronQuery(string id)
        {
            Id = id;
        }
    }

    public class DeletePatronCommandHandler : IRequestHandler<DeletePatronQuery, PatronEditViewModel>
    {
        private readonly IPatron _patron;
        private readonly IMapper _mapper;

        public DeletePatronCommandHandler(IPatron patron, IMapper mapper)
        {
            _patron = patron;
            _mapper = mapper;
        }

        public async Task<PatronEditViewModel> Handle(DeletePatronQuery request, CancellationToken cancellationToken)
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

            //Check if there are any items that were checked out by the patron and not turned back 
            //or if the patron has placed hold on them. 
            // If so do not allow to delete this patron.
            var checkouts = await _patron.GetCheckoutsAsync(request.Id);

            var holds = await _patron.GetHoldsAsync(request.Id);

            var model = _mapper.Map<PatronEditViewModel>(patron);

            if (checkouts.Any() || holds.Any())
            {
                model.PatronActionState = ViewResponse.DeletingForbidden;
                return model;
            }

            return model;
        }
    }
}
