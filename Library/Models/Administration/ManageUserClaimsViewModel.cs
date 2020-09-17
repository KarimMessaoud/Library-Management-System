using System.Collections.Generic;

namespace Library.Models.Administration
{
    public class ManageUserClaimsViewModel
    {
        public string UserId { get; set; }
        public List<UserClaim> Claims { get; set; } = new List<UserClaim>();
    }
}
