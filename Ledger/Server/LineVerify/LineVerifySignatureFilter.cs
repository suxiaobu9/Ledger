using Ledger.Shared.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace Ledger.Server.LineVerify;

public class LineVerifySignatureFilter : IAuthorizationFilter
{
    private readonly LineBot _lineBot;
    public LineVerifySignatureFilter(IOptions<LineBot> linebot)
    {
        _lineBot = linebot.Value;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (string.IsNullOrWhiteSpace(_lineBot.ChannelSecret))
        {
            context.Result = new ForbidResult();
            return;
        }

        context.HttpContext.Request.EnableBuffering();
        string requestBody = new StreamReader(context.HttpContext.Request.Body).ReadToEndAsync().Result;
        context.HttpContext.Request.Body.Position = 0;

        var xLineSignature = context.HttpContext.Request.Headers["X-Line-Signature"].ToString();
        var channelSecret = Encoding.UTF8.GetBytes(_lineBot.ChannelSecret);
        var body = Encoding.UTF8.GetBytes(requestBody);

        using var hmac = new HMACSHA256(channelSecret);

        var hash = hmac.ComputeHash(body, 0, body.Length);
        var xLineBytes = Convert.FromBase64String(xLineSignature);
        if (!SlowEquals(xLineBytes, hash))
        {
            context.Result = new ForbidResult();
        }
    }

    private static bool SlowEquals(byte[] a, byte[] b)
    {
        uint diff = (uint)a.Length ^ (uint)b.Length;
        for (int i = 0; i < a.Length && i < b.Length; i++)
            diff |= (uint)(a[i] ^ b[i]);
        return diff == 0;
    }
}
