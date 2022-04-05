using System;
using System.Collections.Generic;

namespace Ledger.Server
{
    public partial class DeleteAccount
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public bool IsConfirm { get; set; }
        public DateTime Deadline { get; set; }

        public virtual Accounting Account { get; set; } = null!;
    }
}
