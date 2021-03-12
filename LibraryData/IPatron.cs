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
        IQueryable<User> GetAll();
        Task<IEnumerable<CheckoutHistory>> GetCheckoutHistory(string patronId);
        Task<IEnumerable<Hold>> GetHolds(string patronId);
        Task<IEnumerable<Checkout>> GetCheckouts(string patronId);
    }
}
