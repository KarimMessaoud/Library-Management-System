using System.Collections.Generic;

namespace Library.Models.Branch
{
    public class BranchDetailViewModel
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public string OpenDate { get; set; }
        public string Telephone { get; set; }
        public bool IsOpen { get; set; }
        public string Description { get; set; }
        public int NumberOfPatrons { get; set; }
        public int NumberOfAssets { get; set; }
        public decimal TotalAssetValue { get; set; }
        public string ImageUrl { get; set; }
        public IEnumerable<string> HoursOpen { get; set; }
    }
}
