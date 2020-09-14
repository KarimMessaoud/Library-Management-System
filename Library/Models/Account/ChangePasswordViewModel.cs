using System.ComponentModel.DataAnnotations;

namespace Library.Models.Account
{
    public class ChangePasswordViewModel
    {

        [Required]  
        [MinLength(8)] 
        [MaxLength(50)]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string CurrentPassword { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(50)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The confirmation password does not match the new password")]
        public string ConfirmPassword { get; set; }
    }
}
