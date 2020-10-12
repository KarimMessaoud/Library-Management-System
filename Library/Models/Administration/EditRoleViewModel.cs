using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Library.Models.Administration
{
    public class EditRoleViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage= "Role name is required")]
        [Display(Name="Role name")]
        [MinLength(2)]
        public string RoleName { get; set; }
        public List<string> Users { get; set; } = new List<string>();
    }
}
