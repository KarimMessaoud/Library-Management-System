using LibraryData.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Library.Models.Catalog
{
    public class AssetCreateVideoViewModel : AssetCreateViewModel
    {
        [Required]
        public string Director { get; set; }
    }
}
