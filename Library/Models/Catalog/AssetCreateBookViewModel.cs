using LibraryData.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Library.Models.Catalog
{
    public class AssetCreateBookViewModel : AssetCreateViewModel
    {
        public string Author { get; set; }

        [Required]
        public string ISBN { get; set; }
    }
}
