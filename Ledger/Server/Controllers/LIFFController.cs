using Ledger.Shared.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Ledger.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LIFFController : Controller
    {
        private readonly LIFFInfo LIFFInfo;
        public LIFFController(IOptions<LIFFInfo> LIFFInfo)
        {
            this.LIFFInfo = LIFFInfo.Value;
        }

        [HttpGet("GetLIFFId")]
        public string? GetLIFFId()
        {
            return LIFFInfo.LiffId;
        }
    }
}
