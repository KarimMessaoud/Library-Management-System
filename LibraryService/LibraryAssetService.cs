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
            await _context.LibraryAssets.AddAsync(newAsset);
            await _context.SaveChangesAsync();
        }

        public void Update(LibraryAsset newAsset)
        {
            _context.LibraryAssets.Update(newAsset);
            _context.SaveChanges();
        }

        public void Delete(LibraryAsset newAsset)
        {
            _context.LibraryAssets.Remove(newAsset);
            _context.SaveChanges();
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

        public LibraryAsset GetById(int id)
        {
            return GetAll().FirstOrDefault(x => x.Id == id);
        }

        public Book GetBookById(int id)
        {
            return _context.Books
                .Include(x => x.Status)
                .Include(x => x.Location)
                .FirstOrDefault(x => x.Id == id);
        }

        public Video GetVideoById(int id)
        {
            return _context.Videos
                .Include(x => x.Status)
                .Include(x => x.Location)
                .FirstOrDefault(x => x.Id == id);
        }

        public LibraryBranch GetCurrentLocation(int id)
        {
            return _context.LibraryAssets
                .FirstOrDefault(x => x.Id == id).Location;
        }


        public string GetIsbn(int id)
        {
            if (_context.Books.Any(x => x.Id == id))
                return _context.Books.FirstOrDefault(x => x.Id == id).ISBN;
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
