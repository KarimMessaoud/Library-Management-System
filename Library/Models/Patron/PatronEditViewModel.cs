using Library.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Library.Models.Patron
{
    public class PatronEditViewModel
    { 
        public string Id { get; set; }

        [Required] 
        [MinLength(2)] 
        [MaxLength(50)]
        [Display(Name ="First Name")]
        public string FirstName { get; set; }

        [Required, MinLength(2), MaxLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required] 
        [DataType(DataType.Date)]
        [Display(Name = "Date Of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [Display(Name = "Telephone")]
        public string Telephone { get; set; }

        [Required]
        [Display(Name ="Home Library Branch Name")]
        public string HomeLibraryBranchName { get; set; }
        public ViewResponse? PatronActionState { get; set; }
    }
}
