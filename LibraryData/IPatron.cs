using LibraryData.Models;
using LibraryData.Models.Account;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryData
{
    public interface IPatron
    {
        Task<User> GetAsync(string id);

        Task<IQueryable<User>> GetAllAsync();
        Task<IEnumerable<CheckoutHistory>> GetCheckoutHistoryAsync(string patronId);
        Task<IEnumerable<Hold>> GetHoldsAsync(string patronId);
        Task<IEnumerable<Checkout>> GetCheckoutsAsync(string patronId);
    }
}
