namespace LibraryData.Models
{
    public class Book : LibraryAsset
    {
        public string ISBN { get; set; }
        public string Author { get; set; }
    }
}
