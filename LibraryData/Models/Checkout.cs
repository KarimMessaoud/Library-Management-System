using System;

namespace LibraryData.Models
{
    public class Checkout
    {
        public int Id { get; set; }
        public virtual LibraryAsset LibraryAsset { get; set; }
        public int LibraryAssetId { get; set; }
        public virtual LibraryCard LibraryCard { get; set; }
        public DateTime Since { get; set; }
        public DateTime Until { get; set; }
    }
}
