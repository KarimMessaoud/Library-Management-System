using LibraryData;
using LibraryData.Models;
using LibraryData.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryService
{
    public class PatronService : IPatron
    {
        private LibraryContext _context;
        private UserManager<User> _userManager;
        public PatronService(LibraryContext context,
            UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<User> GetAsync(string id)
        {
            var patron = await (await GetAllAsync()).FirstOrDefaultAsync(x => x.Id == id);

            return patron;
        }

        public async Task<IQueryable<User>> GetAllAsync()
        {
            var patronRoleId = _context.Roles
                .FirstOrDefault(x => x.Name == "Patron").Id;

            var allPatronsIds = await _context.UserRoles
                .Where(x => x.RoleId == patronRoleId)
                .Select(x => x.UserId).ToListAsync();

            return _context.Users
                .Include(x => x.LibraryCard)
                .Include(x => x.HomeLibraryBranch)
                .Where(x => allPatronsIds.Any(y => y == x.Id));
        }

        public async Task<IEnumerable<CheckoutHistory>> GetCheckoutHistoryAsync(string patronId)
        {
            var patron = await _userManager.FindByIdAsync(patronId);
            var cardId = patron.LibraryCard.Id;

            return _context.CheckoutHistories
                .Include(x => x.LibraryAsset)
                .Include(x => x.LibraryCard)
                .Where(x => x.LibraryCard.Id == cardId)
                .OrderByDescending(x => x.CheckedOut);
        }

        public async Task<IQueryable<Checkout>> GetCheckoutsAsync(string patronId)
        {
            var patron = await _userManager.FindByIdAsync(patronId);
            var cardId = patron.LibraryCard.Id;

            return _context.Checkouts
                .Include(x => x.LibraryAsset)
                .Where(x => x.LibraryCard.Id == cardId);
        }

        public async Task<IQueryable<Hold>> GetHoldsAsync(string patronId)
        {
            var patron = await _userManager.FindByIdAsync(patronId);
            var cardId = patron.LibraryCard.Id;

            return _context.Holds
                .Include(x => x.LibraryAsset)
                .Where(x => x.LibraryCard.Id == cardId)
                .OrderByDescending(x => x.HoldPlaced);
        }

    }
}
