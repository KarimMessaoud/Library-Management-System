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
        IEnumerable<CheckoutHistory> GetCheckoutHistory(int id);
        IEnumerable<Hold> GetCurrentHolds(int id);

        Checkout GetById(int checkoutId);
        Checkout GetLatestCheckout(int assetId);

        string GetCurrentCheckoutPatron(int assetId);
        string GetCurrentHoldPatronName(int id);
        DateTime GetCurrentHoldPlaced(int id);
        Task<bool> IsCheckedOutAsync(int assetId);

        Task CheckOutItemAsync(int assetId, int libraryCardId);
        Task CheckInItemAsync(int assetId);
        bool PlaceHold(int assetId, int libraryCardId);
        Task MarkLostAsync(int assetId);
        Task MarkFoundAsync(int assetId);
        void ChargeOverdueFees(string patronId);
        void ResetOverdueFees(string patronId);
    }
}
