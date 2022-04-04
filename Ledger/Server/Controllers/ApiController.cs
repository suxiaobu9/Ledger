using isRock.LineBot;
using Ledger.Server.LineVerify;
using Microsoft.AspNetCore.Mvc;

namespace Ledger.Server.Controllers;

/// <summary>
/// ngrok http 5099 -host-header="localhost:5099"
/// </summary>
[Route("api")]
public class ApiController : LineWebHookControllerBase
{
    [HttpPost]
    [Route("Accounting")]
    [LineVerifySignature]
    public IActionResult Index()
    {
        return Ok();
    }
}
