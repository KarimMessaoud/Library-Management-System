using Hangfire;
using LibraryData;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace LibraryService.Tasks
{
    public class WaitingHoldsProcessingTask : IWaitingHoldsProcessingTask
    {
        private readonly LibraryContext _context;
        public WaitingHoldsProcessingTask(LibraryContext context)
        {
            _context = context;
        }

        public void Execute()
        {
            var holds = _context.Holds
                .Include(x => x.LibraryAsset)
                .Include(x => x.LibraryCard)
                .OrderBy(x => x.HoldPlaced);

            if (!holds.Any()) return;

            //Search for holds that did not culminate in checkouts
            foreach (var hold in holds)
            {
                if (DateTime.Now > hold.HoldPlaced.AddHours(24) && hold.FirstHold == true)
                {
                    //Check whether the asset was checked out in 24 hours time since hold was placed on it
                    var checkout = _context.Checkouts
                    .Include(x => x.LibraryAsset)
                    .Include(x => x.LibraryCard)
                    .FirstOrDefault(x => x.LibraryAsset.Id == hold.LibraryAsset.Id
                            && x.LibraryCard.Id == hold.LibraryCard.Id
                            && x.Since >= hold.HoldPlaced && x.Since <= hold.HoldPlaced.AddHours(24));
                    
                    //If the asset was not checked out, send email to the patron
                    if (checkout == null)
                    {
                        var asset = _context.LibraryAssets
                            .FirstOrDefault(x => x.Id == hold.LibraryAsset.Id);

                        var patron = _context.Users
                            .FirstOrDefault(x => x.LibraryCard.Id == hold.LibraryCard.Id);

                        BackgroundJob.Enqueue<IEmailService>(x => x.SendEmailAsync(patron.FirstName, patron.Email,
                            $"Library asset is not available.",
                            $"The asset: '{asset.Title}' on which you have placed hold is not available. " +
                            $"The time in which you were to borrow the item has left."));

                        _context.Remove(hold);


                        //If there are not any more holds on the asset change its status to available
                        var holdsOnAsset = holds
                             .Where(x => x.LibraryAsset.Id == asset.Id && x.Id != hold.Id);

                        if (!holdsOnAsset.Any())
                        {
                            _context.Update(asset);
                            asset.Status = _context.Statuses.FirstOrDefault(x => x.Name == "Available");
                        }
                        //If there are more holds on the asset send email to the next patron waiting for it 
                        else
                        {
                            var earliestHold = holds
                            .FirstOrDefault(x => x.Id != hold.Id
                                             && x.LibraryAsset.Id == hold.LibraryAsset.Id);

                            _context.Update(earliestHold);
                            earliestHold.FirstHold = true;
                            earliestHold.HoldPlaced = DateTime.Now;

                            var nextPatron = _context.Users
                                .FirstOrDefault(x => x.LibraryCard.Id == earliestHold.LibraryCard.Id);

                            BackgroundJob.Enqueue<IEmailService>(x => x.SendEmailAsync(nextPatron.FirstName, nextPatron.Email, "Library asset is available",
                                $"The asset: '{asset.Title}' on which you have placed hold is now available. " +
                                "Now you have to come to us and take the item in 24 hours time. " +
                                "If you will not take the item up to this time you will not be able to borrow it."));
                        }
                    }
                }
            }

            _context.SaveChanges();
        }

    }

    public interface IWaitingHoldsProcessingTask
    {
        void Execute();
    }
}
