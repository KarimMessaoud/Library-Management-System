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

        public User Get(string id)
        {
            return GetAll()
                .FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<User> GetAll()
        {
            var patronRoleId = _context.Roles
                .FirstOrDefault(x => x.Name == "Patron").Id;

            var allPatronsIds = _context.UserRoles
                .Where(x => x.RoleId == patronRoleId)
                .Select(x => x.UserId);

            return _context.Users
                .Include(x => x.LibraryCard)
                .Include(x => x.HomeLibraryBranch)
                .Where(x => allPatronsIds.Any(y => y == x.Id));
        }

        public async Task<IEnumerable<CheckoutHistory>> GetCheckoutHistory(string patronId)
        {
            var patron = await _userManager.FindByIdAsync(patronId);
            var cardId = patron.LibraryCard.Id;

            return _context.CheckoutHistories
                .Include(x => x.LibraryAsset)
                .Include(x => x.LibraryCard)
                .Where(x => x.LibraryCard.Id == cardId)
                .OrderByDescending(x => x.CheckedOut);
        }

        public async Task<IEnumerable<Checkout>> GetCheckouts(string patronId)
        {
            var patron = await _userManager.FindByIdAsync(patronId);
            var cardId = patron.LibraryCard.Id;

            return _context.Checkouts
                .Include(x => x.LibraryAsset)
                .Include(x => x.LibraryCard)
                .Where(x => x.LibraryCard.Id == cardId);
        }

        public async Task<IEnumerable<Hold>> GetHolds(string patronId)
        {
            var patron = await _userManager.FindByIdAsync(patronId);
            var cardId = patron.LibraryCard.Id;

            return _context.Holds
                .Include(x => x.LibraryAsset)
                .Include(x => x.LibraryCard)
                .Where(x => x.LibraryCard.Id == cardId)
                .OrderByDescending(x => x.HoldPlaced);
        }

    }
}
