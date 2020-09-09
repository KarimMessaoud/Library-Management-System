using LibraryData.Models;
using LibraryData.Models.Account;
using System.Collections.Generic;

namespace LibraryData
{
    public interface ILibraryBranch
    {
        IEnumerable<LibraryBranch> GetAll();
        IEnumerable<User> GetPatrons(int branchId);
        IEnumerable<LibraryAsset> GetAssets(int branchId);
        IEnumerable<string> GetBranchHours(int branchId);
        LibraryBranch GetBranchById(int branchId);
        LibraryBranch GetBranchByName(string branchName);
        void Add(LibraryBranch newBranch);
        bool IsBranchOpen(int branchId);
    }
}
