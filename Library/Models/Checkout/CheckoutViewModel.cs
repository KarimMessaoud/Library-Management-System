using System.ComponentModel.DataAnnotations;

namespace Library.Models.Checkout
{
    public class CheckoutViewModel
    {
        public string AssetId { get; set; }
        public string ImageUrl { get; set; }
        public string Title { get; set; }

        [Required]
        [Display(Name = "Library Card Id")]
        public string LibraryCardId { get; set; }
        public bool IsCheckedOut { get; set; }
        public int HoldCount { get; set; }
    }
}
