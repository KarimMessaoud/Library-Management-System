namespace Library.Models.Catalog
{
    public class AssetEditBookViewModel : AssetCreateBookViewModel
    {
        public int? DecryptedId { get; set; }
        public string ExistingPhotoPath { get; set; }
    }
}
