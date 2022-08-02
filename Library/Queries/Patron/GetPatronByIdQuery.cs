using AutoMapper;
using Library.Models.Patron;
using Library.Security;
using LibraryData;
using LibraryData.Models;
using MediatR;
using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Library.Queries.Patron
{
    public class GetPatronByIdQuery : IRequest<PatronDetailModel>
    {
        public string Id { get; }
        public GetPatronByIdQuery(string id)
        {
            Id = id;
        }
    }

    public class GetPatronByIdQueryHandler : IRequestHandler<GetPatronByIdQuery, PatronDetailModel>
    {
        private readonly IPatron _patron;
        private readonly IMapper _mapper;
        private readonly IDataProtector protector;

        public GetPatronByIdQueryHandler(IPatron patron, 
                                         IMapper mapper, 
                                         IDataProtectionProvider dataProtectionProvider,
                                         DataProtectionPurposeStrings dataProtectionPurposeStrings)
        {
            _patron = patron;
            _mapper = mapper;
            protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.AssetIdRouteValue);
        }

        public async Task<PatronDetailModel> Handle(GetPatronByIdQuery request, CancellationToken cancellationToken)
        {
            if(request.Id == null)
            {
                return null;
            }

            var patron = await _patron.GetAsync(request.Id);

            if(patron == null)
            {
                return null;
            }

            var model = _mapper.Map<PatronDetailModel>(patron);

            var assetsCheckedOut = await _patron.GetCheckoutsAsync(request.Id);
            if (assetsCheckedOut != null) model.AssetsCheckedOut = assetsCheckedOut;
            else model.AssetsCheckedOut = new List<Checkout>();

            model.CheckoutHistory = await _patron.GetCheckoutHistoryAsync(request.Id);
            model.Holds = await _patron.GetHoldsAsync(request.Id);

            //Encrypt Library Assets' Ids in order to be able to get to details of the checkout items
            //from Patron's detail view  
            foreach (var item in model.AssetsCheckedOut)
            {
                item.LibraryAsset.EncryptedId = protector.Protect(item.LibraryAsset.Id.ToString());
            }

            return model;
        }
    }

}
