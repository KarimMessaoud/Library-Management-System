using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Library.Models.Administration
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required]
        [Display(Name="First name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required] [EmailAddress]
        public string Email { get; set; }

        public IList<string> Roles { get; set; } = new List<string>();
        public List<string> Claims { get; set; } = new List<string>();
    }
}
