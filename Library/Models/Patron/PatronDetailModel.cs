using LibraryData.Models;
using System;
using System.Collections.Generic;

namespace Library.Models.Patron
{
    public class PatronDetailModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName 
        { 
            get
            {
                return FirstName + " " + LastName;
            }
        }

        public int LibraryCardId { get; set; }
        public string Address { get; set; }
        public DateTime MemberSince { get; set; }
        public string Telephone { get; set; }
        public string HomeLibraryBranch { get; set; }
        public decimal OverdueFees { get; set; }
        public IEnumerable<LibraryData.Models.Checkout> AssetsCheckedOut { get; set; }
        public IEnumerable<CheckoutHistory> CheckoutHistory { get; set; }
        public IEnumerable<Hold> Holds { get; set; }
    }
}
