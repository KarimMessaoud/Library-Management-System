using System.ComponentModel.DataAnnotations;

namespace Library.Models.Administration
{
    public class CreateRoleViewModel
    {
        [Required]
        [MinLength(2)]
        [Display(Name="Role name")]
        public string RoleName { get; set; }
    }
}
