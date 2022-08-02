using AutoMapper;
using Library.Models.Patron;
using LibraryData;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Library.Queries.Patron
{
    public class EditPatronQuery : IRequest<PatronEditViewModel>
    {
        public string Id { get; }

        public EditPatronQuery(string id)
        {
            Id = id;
        }
    }

    public class EditPatronQueryHandler : IRequestHandler<EditPatronQuery, PatronEditViewModel>
    {
        private readonly IPatron _patron;
        private readonly IMapper _mapper;

        public EditPatronQueryHandler(IPatron patron, IMapper mapper)
        {
            _patron = patron;
            _mapper = mapper;
        }

        public async Task<PatronEditViewModel> Handle(EditPatronQuery request, CancellationToken cancellationToken)
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

            var model = _mapper.Map<PatronEditViewModel>(patron);

            return model;
        }
    }
}
