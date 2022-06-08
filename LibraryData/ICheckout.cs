using LibraryData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryData
{
    public interface ICheckout
    {
        Task<Checkout> GetByIdAsync(int checkoutId);
        Task<Checkout> GetLatestCheckoutAsync(int assetId);

        Task<string> GetCurrentCheckoutPatronAsync(int assetId);

        Task<bool> PlaceHoldAsync(int assetId, int libraryCardId);
        Task<bool> IsCheckedOutAsync(int assetId);

        Task AddAsync(Checkout newCheckout);
        Task CheckOutItemAsync(int assetId, int libraryCardId);
        Task CheckInItemAsync(int assetId);
        Task MarkLostAsync(int assetId);
        Task MarkFoundAsync(int assetId);
        Task ChargeOverdueFeesAsync(string patronId);
        Task ResetOverdueFeesAsync(string patronId);

        Task<IEnumerable<CheckoutHistory>> GetCheckoutHistoryAsync(int id);
        Task<IQueryable<Hold>> GetCurrentHoldsAsync(int id);
        Task<IEnumerable<Checkout>> GetAllAsync();

        string GetCurrentHoldPatronName(int id);
        DateTime GetCurrentHoldPlaced(int id);
    }
}
