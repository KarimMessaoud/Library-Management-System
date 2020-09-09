using System;

namespace LibraryData.Models
{
    public class Hold
    {
        public int Id { get; set; }
        public LibraryAsset LibraryAsset { get; set; }
        public LibraryCard LibraryCard { get; set; }
        public DateTime HoldPlaced { get; set; }
        public bool FirstHold { get; set; } = false;


    }
}
