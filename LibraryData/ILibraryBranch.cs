using LibraryData.Models;
using LibraryData.Models.Account;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryData
{
    public interface ILibraryBranch
    {
        IEnumerable<LibraryBranch> GetAll();
        IEnumerable<string> GetBranchHours(int branchId);

        Task<IEnumerable<User>> GetPatronsAsync(int branchId);
        Task<IEnumerable<LibraryAsset>> GetAssetsAsync(int branchId);
        Task AddAsync(LibraryBranch newBranch);

        LibraryBranch GetBranchById(int branchId);
        LibraryBranch GetBranchByName(string branchName);

        bool IsBranchOpen(int branchId);
    }
}
