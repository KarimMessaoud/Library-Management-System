using LibraryData.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryData
{
    public interface ICheckout
    {
        Task AddAsync(Checkout newCheckout);
        IEnumerable<Checkout> GetAll();
        Task<IEnumerable<CheckoutHistory>> GetCheckoutHistoryAsync(int id);
        IEnumerable<Hold> GetCurrentHolds(int id);

        Task<Checkout> GetByIdAsync(int checkoutId);
        Task<Checkout> GetLatestCheckoutAsync(int assetId);

        Task<string> GetCurrentCheckoutPatronAsync(int assetId);
        string GetCurrentHoldPatronName(int id);
        DateTime GetCurrentHoldPlaced(int id);
        Task<bool> IsCheckedOutAsync(int assetId);

        Task CheckOutItemAsync(int assetId, int libraryCardId);
        Task CheckInItemAsync(int assetId);
        Task<bool> PlaceHoldAsync(int assetId, int libraryCardId);
        Task MarkLostAsync(int assetId);
        Task MarkFoundAsync(int assetId);
        Task ChargeOverdueFeesAsync(string patronId);
        Task ResetOverdueFeesAsync(string patronId);
    }
}
