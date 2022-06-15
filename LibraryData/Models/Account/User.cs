using Microsoft.AspNetCore.Identity;
using System;

namespace LibraryData.Models.Account
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public virtual LibraryCard LibraryCard { get; set; }
        public int LibraryCardId { get; set; }
        public virtual LibraryBranch HomeLibraryBranch { get; set; }
    }
}
