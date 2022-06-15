using LibraryData.Models.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryData.Models
{
    public class LibraryCard
    {
        public int Id { get; set; }
        public decimal Fees { get; set; }
        public DateTime Created { get; set; }
        public virtual User Patron { get; set; }
        public string PatronId { get; set; }
        public virtual IEnumerable<Checkout> Checkouts { get; set; }
    }
}
