using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryData.Models
{
    public abstract class LibraryAsset
    {
        public int Id { get; set; }
        public string EncryptedId { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public virtual Status Status { get; set; }
        public decimal Cost { get; set; }
        public string ImageUrl { get; set; }
        public int NumberOfCopies { get; set; }
        public virtual LibraryBranch Location { get; set; }
        public virtual Checkout Checkout { get; set; }
    }
}
