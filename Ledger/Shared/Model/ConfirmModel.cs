using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ledger.Shared.Model;

public class ConfirmModel
{
    public int AccountId { get; set; }

    public string? EventName { get; set; }

    public  int Pay { get; set; }
}
