﻿using LibraryData.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryData
{
    public interface ILibraryAssetService
    {
        IEnumerable<LibraryAsset> GetAll();

        Task<LibraryAsset> GetByIdAsync(int id);
        Task<Book> GetBookByIdAsync(int id);
        Task<Video> GetVideoByIdAsync(int id);

        Task AddAsync(LibraryAsset newAsset);
        Task UpdateAsync(LibraryAsset newAsset);
        Task DeleteAsync(LibraryAsset newAsset);

        Task<string> GetIsbnAsync(int id);
        Task<string> GetCurrentLocationNameAsync(int id);

        string GetAuthorOrDirector(int id);
        string GetType(int id);
        string GetTitle(int id);
    }
}
