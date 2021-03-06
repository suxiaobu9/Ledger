using System;
using System.Collections.Generic;

namespace Ledger.Server
{
    public partial class User
    {
        public User()
        {
            Accountings = new HashSet<Accounting>();
        }

        public int Id { get; set; }
        public string LineUserId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public bool IsAdmin { get; set; }

        public virtual ICollection<Accounting> Accountings { get; set; }
    }
}
