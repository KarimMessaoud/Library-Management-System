using Hangfire;
using LibraryData;
using LibraryData.Models;
using LibraryData.Models.Account;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LibraryService
{
    public class CheckoutService : ICheckout
    {
        private LibraryContext _context;
        private UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILibraryAssetService _assetService;
        public CheckoutService(LibraryContext context,
            UserManager<User> userManager,
            IEmailService emailService,
            IHttpContextAccessor httpContextAccessor,
            ILibraryAssetService assetService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
            _assetService = assetService;
        }

        public async Task AddAsync(Checkout newCheckout)
        {
            await _context.AddAsync(newCheckout);
            await _context.SaveChangesAsync();
        }

        public IEnumerable<Checkout> GetAll()
        {
            return _context.Checkouts;
        }

        public Checkout GetById(int checkoutId)
        {
            return GetAll().FirstOrDefault(x => x.Id == checkoutId);
        }

        public IEnumerable<CheckoutHistory> GetCheckoutHistory(int id)
        {
            return _context.CheckoutHistories
                .Include(x => x.LibraryAsset)
                .Include(x => x.LibraryCard)
                .Where(x => x.LibraryAsset.Id == id);
        }

        public IEnumerable<Hold> GetCurrentHolds(int id)
        {
            return _context.Holds
                .Include(x => x.LibraryAsset)
                .Where(x => x.LibraryAsset.Id == id);
        }

        public Checkout GetLatestCheckout(int assetId)
        {
            return _context.Checkouts
                .Where(x => x.LibraryAsset.Id == assetId)
                .OrderByDescending(x => x.Since)
                .FirstOrDefault();
        }
        public async Task MarkFoundAsync(int assetId)
        {
            var now = DateTime.Now;

            UpdateAssetStatus(assetId, "Available");

            // remove an existing checkout on the item
            RemoveExistingCheckouts(assetId);

            // close any existing checkout history
            CloseExistingCheckouts(assetId, now);

            await _context.SaveChangesAsync();
        }

        private void UpdateAssetStatus(int assetId, string newStatus)
        {
            var item = _context.LibraryAssets.FirstOrDefault(x => x.Id == assetId);
            _context.Update(item);
            item.Status = _context.Statuses.FirstOrDefault(x => x.Name == newStatus);
        }

        private void CloseExistingCheckouts(int assetId, DateTime now)
        {
            var history = _context.CheckoutHistories
                .FirstOrDefault(x => x.LibraryAsset.Id == assetId && x.CheckedIn == null);

            if (history != null)
            {
                _context.Update(history);
                history.CheckedIn = now;
            }
        }

        private void RemoveExistingCheckouts(int assetId)
        {
            var checkout = _context.Checkouts
                .FirstOrDefault(x => x.LibraryAsset.Id == assetId);

            if (checkout != null)
            {
                _context.Remove(checkout);
            }
        }

        public async Task MarkLostAsync(int assetId)
        {
            UpdateAssetStatus(assetId, "Lost");
            await _context.SaveChangesAsync();
        }

        public async Task CheckInItemAsync(int assetId)
        {
            var now = DateTime.Now;
            var item = _context.LibraryAssets.FirstOrDefault(x => x.Id == assetId);

            // remove any existing checkouts on the item
            RemoveExistingCheckouts(assetId);

            // close any existing checkout history
            CloseExistingCheckouts(assetId, now);

            // look for existing holds on the item
            var currentHolds = _context.Holds
                .Include(x => x.LibraryAsset)
                .Include(x => x.LibraryCard)
                .Where(x => x.LibraryAsset.Id == assetId);

            // if there are any holds on the item 
            //send email to the patron with the earliest hold

            if (await currentHolds.AnyAsync())
            {
                SendEmailToEarliestHoldPatron(assetId, currentHolds);

                UpdateAssetStatus(assetId, "On Hold");

                await _context.SaveChangesAsync();

                return;
            }

            // otherwise, update the item status to available
                UpdateAssetStatus(assetId, "Available");
            
            _context.SaveChanges();
        }

        private void SendEmailToEarliestHoldPatron(int assetId, IQueryable<Hold> currentHolds)
        {
            var asset = _assetService.GetById(assetId);
            var earliestHold = currentHolds.OrderBy(x => x.HoldPlaced).FirstOrDefault();

            // Change FirstHold property in order for the patron who has placed hold
            // to receive email if he will not take the item in 24 hours time
            _context.Update(earliestHold);
            earliestHold.FirstHold = true;
            
            var cardId = earliestHold.LibraryCard.Id;
            var patron = _context.Users.FirstOrDefault(x => x.LibraryCard.Id == cardId);
            BackgroundJob.Enqueue<IEmailService>(x => x.SendEmailAsync(patron.FirstName, patron.Email, "Library item is free to borrow",
                $"The asset: '{asset.Title}' on which you have placed hold is now available. " +
                "You have to come to us and take this item in 24 hours time. " +
                "If you will not take the item up to this time you will not be able to borrow it."));
        }

        public void CheckOutItem(int assetId, int libraryCardId)
        {
           if(IsCheckedOut(assetId))
            {
                return;
            }

            var item = _context.LibraryAssets.FirstOrDefault(x => x.Id == assetId);

            UpdateAssetStatus(assetId, "Checked Out");

            var libraryCard = _context.LibraryCards
                .Include(x => x.Checkouts)
                .FirstOrDefault(x => x.Id == libraryCardId);

            if (libraryCard == null)
            {
                return;
            }

            //In case of libraryCard exists but patron has been deleted,
            //do not allow to checkout the item
            var patron = _context.Users.FirstOrDefault(x => x.LibraryCard.Id == libraryCardId);

            if(patron == null)
            {
                return;
            }

            var now = DateTime.Now;

            var checkout = new Checkout()
            {
                LibraryAsset = item,
                LibraryCard = libraryCard,
                Since = now,
                Until = GetDefaultCheckoutTime(now)
            };

            _context.Add(checkout);

            var checkoutHistory = new CheckoutHistory()
            {
                LibraryAsset = item,
                LibraryCard = libraryCard,
                CheckedOut = now
            };

            _context.Add(checkoutHistory);

            //Remove patron's hold on the item
            var hold = _context.Holds
                .Include(x => x.LibraryAsset)
                .Include(x => x.LibraryCard)
                .FirstOrDefault(x => x.LibraryCard.Id == libraryCardId && x.LibraryAsset.Id == assetId);

            if(hold != null)
            {
                _context.Remove(hold);
            }

            _context.SaveChanges();
        }

        private DateTime GetDefaultCheckoutTime(DateTime now)
        {
            return now.AddDays(14);
        }

        public bool IsCheckedOut(int assetId)
        {
            return _context.Checkouts
                .Where(x => x.LibraryAsset.Id == assetId)
                .Any();
        }

        public bool PlaceHold(int id, int libraryCardId)
        {
            var now = DateTime.Now;

            var asset = _context.LibraryAssets
                .Include(x => x.Status)
                .First(x => x.Id == id);

            var card = _context.LibraryCards
                .FirstOrDefault(x => x.Id == libraryCardId);

            //Do not allow user enter libraryCardId that does not exist
            if(card == null)
            {
                return false;
            }

            //In case of libraryCard exists but patron has been deleted,
            //do not allow to place hold on the item
            var patron = _context.Users.FirstOrDefault(x => x.LibraryCard.Id == libraryCardId);

            if (patron == null)
            {
                return false;
            }

            var signedInUser = _httpContextAccessor.HttpContext.User;

            //  If the user who is currently logged in is only a patron, he can place hold on an item only for himself
            if (signedInUser.IsInRole("Patron") && !signedInUser.IsInRole("Employee") && !signedInUser.IsInRole("Admin"))
            {
                var userId = signedInUser.FindFirst(ClaimTypes.NameIdentifier).Value;

                var userLibraryCardId = _context.Users
                    .Include(x => x.LibraryCard)
                    .FirstOrDefault(x => x.Id == userId).LibraryCard?.Id;

                if (userLibraryCardId != libraryCardId)
                {
                    return false;
                }
            }

            //If the user already has checked the asset out, do not allow him to do this again

            var currentCheckouts = _context.Checkouts
                .Include(x => x.LibraryAsset)
                .Include(x => x.LibraryCard)
                .Where(x => x.Until >= now);

            var isCheckoutByUser = currentCheckouts.FirstOrDefault(x => x.LibraryAsset.Id == id && x.LibraryCard.Id == libraryCardId && x.Until >= now);
           
            if (isCheckoutByUser != null)
            {
                return false;
            }

            //If the user already has placed hold on the asset, do not allow him to do this again
            var currentHolds = _context.Holds
                .Include(x => x.LibraryAsset)
                .Include(x => x.LibraryCard)
                .Where(x => x.LibraryAsset.Id == id);

            var currentUserHolds = currentHolds
                .FirstOrDefault(x => x.LibraryCard.Id == libraryCardId);

            if (currentUserHolds != null) return false;

            var hold = new Hold()
            {
                LibraryAsset = asset,
                LibraryCard = card,
                HoldPlaced = now,
                FirstHold = false
            };

            _context.Update(asset);

            if (asset.Status.Name == "Available")
            {
                hold.FirstHold = true;
                asset.Status = _context.Statuses.FirstOrDefault(x => x.Name == "On Hold");
            }

            _context.Add(hold);
            _context.SaveChanges();

            return true;
        }

        
        public string GetCurrentHoldPatronName(int holdId)
        {
            var hold = _context.Holds
                .Include(x => x.LibraryAsset)
                .Include(x => x.LibraryCard)
                .FirstOrDefault(x => x.Id == holdId);

            var cardId = hold?.LibraryCard.Id;

            var patron = _context.Users
                .FirstOrDefault(x => x.LibraryCard.Id == cardId);

            return patron?.FirstName + " " + patron?.LastName;
        }


        public DateTime GetCurrentHoldPlaced(int holdId)
        {
            return _context.Holds
                .Include(x => x.LibraryAsset)
                .Include(x => x.LibraryCard)
                .FirstOrDefault(x => x.Id == holdId)
                .HoldPlaced;
        }

        public string GetCurrentCheckoutPatron(int assetId)
        {
            var checkout = GetCheckoutByAssetId(assetId);

            if(checkout == null)
            {
                return "";
            }
            
            var cardId = checkout.LibraryCard.Id;

            var patron = _context.Users
                .FirstOrDefault(x => x.LibraryCard.Id == cardId);

            return patron.FirstName + " " + patron.LastName;
        }

        private Checkout GetCheckoutByAssetId(int assetId)
        {
            return _context.Checkouts
                .Include(x => x.LibraryAsset)
                .Include(x => x.LibraryCard)
                 .FirstOrDefault(x => x.LibraryAsset.Id == assetId);
        }

        public void ChargeOverdueFees(string patronId)
        {
            //User who is only a patron cannot charge fees in somebody's behalf
            var signedInUser = _httpContextAccessor.HttpContext.User;
            var signedInUserId = signedInUser.FindFirstValue(ClaimTypes.NameIdentifier);

            if(signedInUser.IsInRole("Patron") 
                && !signedInUser.IsInRole("Employee") 
                && !signedInUser.IsInRole("Admin")
                && signedInUserId != patronId)
            {
                return;
            }

            var patron = _context.Users
                .Include(x => x.LibraryCard)
                .FirstOrDefault(x => x.Id == patronId);

            if(patron == null)
            {
                return;
            }

            var libraryCard = _context.LibraryCards
                .FirstOrDefault(x => x.Id == patron.LibraryCard.Id);

            var checkouts = _context.Checkouts
                .Where(x => x.LibraryCard.Id == libraryCard.Id);

            var now = DateTime.Now;

            _context.Update(libraryCard);
            libraryCard.Fees = 0;

            foreach (var checkout in checkouts)
            {
                var redundantDays = (now - checkout.Until).Days;
                if (redundantDays >= 1)
                {
                    libraryCard.Fees += redundantDays * 2;
                }
            }
            _context.SaveChanges();
        }

        public void ResetOverdueFees(string patronId)
        {
            var patron = _context.Users
                .Include(x => x.LibraryCard)
                .FirstOrDefault(x => x.Id == patronId);

            if (patron == null)
            {
                return;
            }

            var libraryCard = _context.LibraryCards
                .FirstOrDefault(x => x.Id == patron.LibraryCard.Id);

            if (libraryCard.Fees > 0)
            {
                _context.Update(libraryCard);
                libraryCard.Fees = 0;
                _context.SaveChanges();
            }
            else return;
        }
    }
}
