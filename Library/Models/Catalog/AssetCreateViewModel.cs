using LibraryData.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Library.Models.Catalog
{
    public class AssetCreateViewModel
    {
        public string Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public int Year { get; set; }

        public Status Status { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [RegularExpression(@"^[0-9]{1,}\.[0-9]{2}$", ErrorMessage = "Enter decimal value of format 9.99")]
        public decimal Cost { get; set; }

        public IFormFile Photo { get; set; }

        [Required]
        [Display(Name = "Number of copies")]
        public int NumberOfCopies { get; set; }

        [Required]
        [Display(Name ="Library Branch Name")]
        public string LibraryBranchName { get; set; }
    }
}
