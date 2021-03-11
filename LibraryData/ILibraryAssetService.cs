using LibraryData.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryData
{
    public interface ILibraryAssetService
    {
        IEnumerable<LibraryAsset> GetAll();
        Task<LibraryAsset> GetByIdAsync(int id);
        Task<Book> GetBookByIdAsync(int id);
        Video GetVideoById(int id);

        Task AddAsync(LibraryAsset newAsset);
        Task UpdateAsync(LibraryAsset newAsset);
        Task DeleteAsync(LibraryAsset newAsset);
        string GetAuthorOrDirector(int id);
        string GetType(int id);
        string GetTitle(int id);
        Task<string> GetIsbnAsync(int id);

        LibraryBranch GetCurrentLocation(int id);

    }
}
