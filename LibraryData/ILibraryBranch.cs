using LibraryData.Models;
using LibraryData.Models.Account;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryData
{
    public interface ILibraryBranch
    {
        IEnumerable<LibraryBranch> GetAll();
        Task<IEnumerable<User>> GetPatronsAsync(int branchId);
        Task<IEnumerable<LibraryAsset>> GetAssetsAsync(int branchId);
        IEnumerable<string> GetBranchHours(int branchId);

        LibraryBranch GetBranchById(int branchId);
        LibraryBranch GetBranchByName(string branchName);

        Task AddAsync(LibraryBranch newBranch);

        bool IsBranchOpen(int branchId);
    }
}
