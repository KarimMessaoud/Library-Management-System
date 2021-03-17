using LibraryData;
using LibraryData.Models;
using LibraryData.Models.Account;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryService
{
    public class LibraryBranchService : ILibraryBranch
    {
        private LibraryContext _context;
        public LibraryBranchService(LibraryContext context)
        {
            _context = context;
        }

        public async Task AddAsync(LibraryBranch newBranch)
        {
            _context.Add(newBranch);
            await _context.SaveChangesAsync();
        }

        public LibraryBranch GetBranchById(int branchId)
        {
            return GetAll()
                .FirstOrDefault(x => x.Id == branchId);
        }

        public LibraryBranch GetBranchByName(string branchName)
        {
            return GetAll()
                .FirstOrDefault(x => x.Name == branchName);
        }

        public IEnumerable<LibraryBranch> GetAll()
        {
            return _context.LibraryBranches
                .Include(x => x.Patrons)
                .Include(x => x.LibraryAssets);
        }

        public async Task<IEnumerable<LibraryAsset>> GetAssetsAsync(int branchId)
        {
            var branch = await _context.LibraryBranches
                .Include(x => x.LibraryAssets)
                .FirstOrDefaultAsync(x => x.Id == branchId);

            var branchAssets = branch.LibraryAssets;

            return branchAssets;
        }

        public IEnumerable<string> GetBranchHours(int branchId)
        {
            var hours = _context.BranchHours
                .Where(x => x.Branch.Id == branchId);

            return DataHelpers.HumanizeBizHours(hours);
        }

        public async Task<IEnumerable<User>> GetPatronsAsync(int branchId)
        {
            var branch = await _context.LibraryBranches
                .Include(x => x.Patrons)
                .FirstOrDefaultAsync(x => x.Id == branchId);

            var branchPatrons = branch.Patrons;

            return branchPatrons;
        }

        public bool IsBranchOpen(int branchId)
        {
            var currentTimeHour = DateTime.Now.Hour;
            var currentDayOfWeek = (int)DateTime.Now.DayOfWeek;

            var hours = _context.BranchHours
                .Where(x => x.Branch.Id == branchId);

            var daysHours = hours.FirstOrDefault(x => x.DayOfWeek == currentDayOfWeek);

            return currentTimeHour >= daysHours.OpenTime && currentTimeHour < daysHours.CloseTime;
        }
    }
}
