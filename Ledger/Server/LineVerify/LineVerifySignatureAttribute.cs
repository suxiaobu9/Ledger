using Microsoft.AspNetCore.Mvc;

namespace Ledger.Server.LineVerify;

public class LineVerifySignatureAttribute : TypeFilterAttribute
{
    public LineVerifySignatureAttribute() : base(typeof(LineVerifySignatureFilter))
    {

    }
}
