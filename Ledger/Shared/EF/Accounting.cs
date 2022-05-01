using System;
using System.Collections.Generic;

namespace Ledger.Server
{
    public partial class Accounting
    {
        public Accounting()
        {
            DeleteAccounts = new HashSet<DeleteAccount>();
        }

        public int Id { get; set; }
        public int Amount { get; set; }
        public string Event { get; set; } = null!;
        public int UserId { get; set; }
        public DateTime AccountDate { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual ICollection<DeleteAccount> DeleteAccounts { get; set; }
    }
}
