using System.Collections.Generic;

namespace Library.Models.Catalog
{
    public class AssetIndexViewModel
    {
        public IEnumerable<AssetIndexListingViewModel> Assets { get; set; }
    }
}