using Library.Models.Patron;
using LibraryData;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Library.Queries
{
    public class GetAllPatronsQuery : IRequest<PatronIndexModel> 
    {
        public string SearchString { get; } 
        public GetAllPatronsQuery(string searchString)
        {
            SearchString = searchString;
        }
    }

    public class GetAllPatronsQueryHandler : IRequestHandler<GetAllPatronsQuery, PatronIndexModel>
    {
        private readonly IPatron _patron;

        public GetAllPatronsQueryHandler(IPatron patron)
        {
            _patron = patron;
        }

        public async Task<PatronIndexModel> Handle(GetAllPatronsQuery request, CancellationToken cancellationToken)
        {   
            var allPatrons = await _patron.GetAllAsync();

            if (!String.IsNullOrEmpty(request.SearchString))
            {
                allPatrons = allPatrons.Where(x => x.LastName.Contains(request.SearchString));
            }

            var patronModels = allPatrons.Select(x => new PatronDetailModel()
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                LibraryCardId = x.LibraryCard.Id,
                OverdueFees = x.LibraryCard.Fees,
                HomeLibraryBranch = x.HomeLibraryBranch.Name
            })
            .OrderBy(x => x.LastName)
            .ToList();

            var model = new PatronIndexModel()
            {
                Patrons = patronModels
            };

            return model;
        }
    }
}

