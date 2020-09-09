using LibraryData;
using LibraryData.Models;
using LibraryData.Models.Account;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryService
{
    public class LibraryBranchService : ILibraryBranch
    {
        private LibraryContext _context;
        public LibraryBranchService(LibraryContext context)
        {
            _context = context;
        }

        public void Add(LibraryBranch newBranch)
        {
            _context.Add(newBranch);
            _context.SaveChanges();
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

        public IEnumerable<LibraryAsset> GetAssets(int branchId)
        {
            return _context.LibraryBranches
                .Include(x => x.LibraryAssets)
                .FirstOrDefault(x => x.Id == branchId)
                .LibraryAssets;
        }

        public IEnumerable<string> GetBranchHours(int branchId)
        {
            var hours = _context.BranchHours
                .Where(x => x.Branch.Id == branchId);

            return DataHelpers.HumanizeBizHours(hours);
        }

        public IEnumerable<User> GetPatrons(int branchId)
        {
            return _context.LibraryBranches
                .Include(x => x.Patrons)
                .FirstOrDefault(x => x.Id == branchId)
                .Patrons;
        }

        public bool IsBranchOpen(int branchId)
        {
            var currentTimeHour = DateTime.Now.Hour;
            var currentDayOfWeek = (int)DateTime.Now.DayOfWeek;

            var hours = _context.BranchHours
                .Where(x => x.Branch.Id == branchId);

            var daysHours = hours.FirstOrDefault(x => x.DayOfWeek == currentDayOfWeek);

            return currentTimeHour < daysHours.OpenTime && currentTimeHour > daysHours.CloseTime;
        }
    }
}
