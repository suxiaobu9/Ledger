using Ledger.Shared.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;

namespace Ledger.Server.Service.LIFF;

public class UserProfileService
{
    public UserProfile? UserProfile { get; private set; }
    private readonly HttpClient http;
    private readonly LIFFInfo liffInfo;
    private readonly BookkeepingContext db;

    public UserProfileService(HttpClient http,
        BookkeepingContext db,
        IOptions<LIFFInfo> liffInfo)
    {
        this.http = http;
        this.liffInfo = liffInfo.Value;
        this.db = db;
    }

    public async Task SetUserProfile(HttpContext context)
    {
        var authorization = context.Request.Headers["Authorization"];

        if (authorization.Count == 0)
            return;

        var token = authorization[0][7..authorization[0].Length];

        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(liffInfo.ChannelId))
            return;

        var dict = new Dictionary<string, string>
        {
            { "id_token", token },
            { "client_id", liffInfo.ChannelId }
        };

        var responseMsg = await http.PostAsync("https://api.line.me/oauth2/v2.1/verify", new FormUrlEncodedContent(dict));

        if (!responseMsg.IsSuccessStatusCode)
            return;

        UserProfile = await responseMsg.Content.ReadFromJsonAsync<UserProfile>();

        return;
    }

    /// <summary>
    /// 取得月花費
    /// </summary>
    /// <param name="month"></param>
    /// <returns></returns>
    public async Task<MonthlyAccountingVm?> GetUserMonthlyAccounting(int? month = null)
    {
        if (UserProfile == null)
            return null;

        var twNow = DateTime.UtcNow.AddHours(8);
        DateTime startDate = new(twNow.Year, month ?? twNow.Month, 1),
            endDate = new(twNow.Year, (month == null ? twNow.Month + 1 : month.Value + 1), 1);

        startDate = startDate.ToUniversalTime();
        endDate = endDate.ToUniversalTime();

        var tmp = await db.Accountings.AsNoTracking()
            .Where(x => x.User.LineUserId == UserProfile.Sub &&
                startDate <= x.AccountDate && x.AccountDate <= endDate)
            .OrderBy(x => x.AccountDate)
            .ToArrayAsync();

        var result = new MonthlyAccountingVm
        {
            Month = month ?? twNow.Month,
            Income = tmp.Where(x => x.Amount < 0).Select(x => new MonthlyAccountingVm.EventDetail
            {
                Amount = x.Amount * -1,
                Date = x.AccountDate.AddHours(8),
                Event = x.Event,
            }).ToArray(),
            Outlay = tmp.Where(x => x.Amount > 0).Select(x => new MonthlyAccountingVm.EventDetail
            {
                Amount = x.Amount,
                Date = x.AccountDate.AddHours(8),
                Event = x.Event,
            }).ToArray(),
        };

        return result;
    }

}
