namespace Library.Models.Catalog
{
    public class AssetIndexListingViewModel
    {
        public string Id { get; set; }
        public string ImageUrl { get; set; }
        public string Title { get; set; }
        public string AuthorOrDirector { get; set; }
        public string Type { get; set; }
        public int NumberOfCopies { get; set; }
    }
}