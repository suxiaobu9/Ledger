using isRock.LineBot;
using Ledger.Server.LineVerify;
using Ledger.Shared.Model;
using Ledger.Shared.Service;
using Ledger.Shared.StaticCode;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Ledger.Server.Controllers;

/// <summary>
/// ngrok http 5099 --host-header="localhost:5099"
/// https://developers.line.biz/console/channel/1657106184/liff/1657106184-R4A80Nrm
/// </summary>
[Route("api")]
public class ApiController : LineWebHookControllerBase
{
    private readonly UserService userService;
    private readonly BookkeepingService bookkeepingService;
    private readonly DeleteAccountService deleteAccountService;
    private readonly LIFFInfo LIFFInfo;

    public ApiController(UserService userService,
                        BookkeepingService bookkeepingService,
                        DeleteAccountService deleteAccountService,
                        IOptions<LineBot> linebot,
                        IOptions<LIFFInfo> LIFFInfo)
    {
        this.userService = userService;
        this.bookkeepingService = bookkeepingService;
        this.deleteAccountService = deleteAccountService;
        this.ChannelAccessToken = linebot.Value.ChannelAccessToken;
        this.LIFFInfo = LIFFInfo.Value;
    }

    [HttpPost]
    [Route("Accounting")]
    [LineVerifySignature]
    public async Task<IActionResult> Accounting()
    {
        if (this.ReceivedMessage == null)
            return Ok();

        try
        {
            var lineEvents = this.ReceivedMessage.events
                  .Where(x => x != null &&
                              (x.type.ToLower() == "postback" ||
                              (x.type.ToLower() == "message" &&
                              x.message.type == "text")))
                  .ToArray();

            var lineSourceUser = lineEvents.Select(x => x.source.userId).Distinct().ToArray();

            var users = await userService.GetUsers(lineSourceUser);

            foreach (var item in lineEvents)
            {
                var user = users.FirstOrDefault(x => x.LineUserId == item.source.userId);
                if (user == null)
                    continue;

                if (item.type.ToLower() == "postback")
                {
                    await PostBack(item, user);
                    continue;
                }

                var (isFlex, message) = await bookkeepingService.Accounting(item.message.text, user);

                if (isFlex)
                    this.ReplyMessageWithJSON(item.replyToken, message);
                else
                    this.ReplyMessage(item.replyToken, message);
            }
        }
        catch (Exception ex)
        {
            var admin = await userService.GetAdmin();

            if (admin != null)
                this.PushMessage(admin.LineUserId, ex.Message);
        }

        return Ok();
    }

    /// <summary>
    /// 刪除的 Flex Message PostBack
    /// </summary>
    /// <param name="lineEvent"></param>
    /// <returns></returns>
    private async Task PostBack(isRock.LineBot.Event lineEvent, User user)
    {
        var convertSuccess = int.TryParse(lineEvent.postback.data, out int accountId);

        if (!convertSuccess)
            return;

        var (isConfirm, confirmModel) = await deleteAccountService.IsConfirm(accountId);

        if (confirmModel == null)
            return;

        if (!isConfirm)
        {
            ReplyMessageWithJSON(lineEvent.replyToken, LineFlexTemplate.DeleteAccountingComfirm(confirmModel));
            return;
        }

        var message = await bookkeepingService.DeleteAccounting(confirmModel, user);

        ReplyMessage(lineEvent.replyToken, message);
    }

    [HttpGet("GetLIFFId")]
    public string? GetLIFFId()
    {
        return LIFFInfo.LiffId;
    }
}
