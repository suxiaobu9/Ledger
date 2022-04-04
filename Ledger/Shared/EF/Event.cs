using System;
using System.Collections.Generic;

namespace Ledger.Server
{
    public partial class Event
    {
        public Event()
        {
            Accountings = new HashSet<Accounting>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int UserId { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual ICollection<Accounting> Accountings { get; set; }
    }
}
