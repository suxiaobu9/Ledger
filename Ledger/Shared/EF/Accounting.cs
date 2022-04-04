using System;
using System.Collections.Generic;

namespace Ledger.Server
{
    public partial class Accounting
    {
        public int Id { get; set; }
        public int Amount { get; set; }
        public int EventId { get; set; }
        public int UserId { get; set; }
        public DateTime AccountDate { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual Event Event { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
