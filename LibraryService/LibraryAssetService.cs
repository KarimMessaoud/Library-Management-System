using LibraryData;
using LibraryData.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LibraryService
{
    public class LibraryAssetService : ILibraryAssetService
    {

        private LibraryContext _context;
        public LibraryAssetService(LibraryContext context)
        {
            _context = context;
        }

        public async Task AddAsync(LibraryAsset newAsset)
        {
            _context.LibraryAssets.Add(newAsset);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(LibraryAsset newAsset)
        {
            _context.LibraryAssets.Update(newAsset);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(LibraryAsset newAsset)
        {
            _context.LibraryAssets.Remove(newAsset);
            await _context.SaveChangesAsync();
        }

        public IEnumerable<LibraryAsset> GetAll()
        {
            return _context.LibraryAssets
                .Include(x => x.Status)
                .Include(x => x.Location);
        }

        public string GetAuthorOrDirector(int id)
        {
            var isBook = _context.Books.Any(x => x.Id == id);
            if (isBook) return _context.Books.FirstOrDefault(x => x.Id == id).Author;
            else return _context.Videos.FirstOrDefault(x => x.Id == id).Director;
        }

        public async Task<LibraryAsset> GetByIdAsync(int id)
        {
            var assets = _context.LibraryAssets
                    .Include(x => x.Status)
                    .Include(x => x.Location);

            return await assets.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Book> GetBookByIdAsync(int id)
        {
            return await _context.Books
                .Include(x => x.Status)
                .Include(x => x.Location)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Video> GetVideoByIdAsync(int id)
        {
            return await _context.Videos
                .Include(x => x.Status)
                .Include(x => x.Location)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<string> GetCurrentLocationNameAsync(int id)
        {
            var asset = await _context.LibraryAssets
                .FirstOrDefaultAsync(x => x.Id == id);

            return asset.Location.Name;
        }


        public async Task<string> GetIsbnAsync(int id)
        {
            if (await _context.Books.AnyAsync(x => x.Id == id))
            {
                var book = await _context.Books.FirstOrDefaultAsync(x => x.Id == id);
                return book.ISBN;
            }
                 
            else return "";
        }

        public string GetTitle(int id)
        {
            return _context.LibraryAssets.FirstOrDefault(x => x.Id == id).Title;
        }

        public string GetType(int id)
        {
            var isBook = _context.LibraryAssets.OfType<Book>().Where(x => x.Id == id);
            return isBook.Any() ? "Book" : "Video";
        }
    }
}
