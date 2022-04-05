using Ledger.Server;
using Ledger.Shared.Model;
using Ledger.Shared.Service.EventService;
using Ledger.Shared.StaticCode;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ledger.Shared.Service.Bookkeeping;

public class BookkeepingService : IBookkeepingService
{
    private readonly BookkeepingContext _db;
    private readonly IEventService _eventService;

    public BookkeepingService(BookkeepingContext db,
                                IEventService eventService)
    {
        _db = db;
        _eventService = eventService;
    }

    /// <summary>
    /// 記帳
    /// </summary>
    /// <param name="lineEvent"></param>
    /// <returns></returns>
    public async Task<(bool isFlex, string message)> Accounting(string message, User user)
    {

        if (string.IsNullOrWhiteSpace(message))
            return (false, $"沒有輸入資料");

        var messsageSplit = message.Split(Environment.NewLine.ToCharArray())
                                    .Where(x => !string.IsNullOrWhiteSpace(x))
                                    .ToArray();

        if (messsageSplit == null || messsageSplit.Length == 0)
            return (false, $"沒有輸入資料");

        if (messsageSplit.Length > 2)
            return (false, $"格式錯誤 !{Environment.NewLine}金額{Environment.NewLine}說明");

        var utcNow = DateTime.UtcNow;

        var eventName = "其他";
        int amount = 0;
        Event payEvent;
        switch (messsageSplit.Length)
        {
            case 1:

                if (string.IsNullOrWhiteSpace(messsageSplit[0]))
                    return (false, "請輸入金額 !");

                // 正負整數
                if (Regex.Match(messsageSplit[0], @"^-?\d+$").Success)
                {
                    payEvent = await _eventService.CreateAndGetEvent(eventName, user.Id);
                    amount = Convert.ToInt32(messsageSplit[0]);
                    break;
                }

                var regexParamList = new List<string>
                    {
                        // 數字開頭，帶文字 ex. 1000吃大餐
                        @"^-?\d+",

                        // 文字開頭，帶數字 ex. 吃大餐1000
                        @"-?\d+$\n*",
                    };

                foreach (var regexGetInt in regexParamList)
                {
                    var regexMatch = Regex.Match(messsageSplit[0], regexGetInt);

                    if (!regexMatch.Success)
                        continue;

                    messsageSplit = new string[]
                    {
                            messsageSplit[0].Substring(regexMatch.Index, regexMatch.Length),
                            messsageSplit[0].Remove(regexMatch.Index,regexMatch.Length)
                    };

                    goto case 2;
                }

                return (false, "請輸入金額 !");
            case 2:
                if (!int.TryParse(messsageSplit[0], out amount))
                {
                    if (!int.TryParse(messsageSplit[1], out amount))
                        return (false, $"格式錯誤 !{Environment.NewLine}金額{Environment.NewLine}說明");

                    eventName = messsageSplit[0];
                }
                else
                {
                    eventName = messsageSplit[1];
                }

                payEvent = await _eventService.CreateAndGetEvent(eventName, user.Id);

                break;
            default:
                return (false, "訊息長度異常 !");
        }

        var accounting = new Accounting
        {
            AccountDate = utcNow,
            CreateDate = utcNow,
            Amount = amount,
            UserId = user.Id,
            EventId = payEvent.Id,
        };
        _db.Accountings.Add(accounting);

        await _db.SaveChangesAsync();

        var twNow = utcNow.AddHours(8);

        DateTime startDate = new(twNow.Year, twNow.Month, 1),
            endDate = startDate.AddMonths(1).AddMilliseconds(-1);

        startDate = startDate.ToUniversalTime();
        endDate = endDate.ToUniversalTime();

        var monthlyAccountings = await _db.Accountings.AsNoTracking()
            .Where(x => startDate <= x.AccountDate &&
                                    x.AccountDate <= endDate &&
                                    x.UserId == user.Id)
            .ToArrayAsync();

        var flexMessageModel = new AccountingFlexMessageModel
        {
            AccountId = accounting.Id,
            MonthlyPay = monthlyAccountings.Where(x => x.Amount > 0).Sum(x => x.Amount),
            MonthlyIncome = monthlyAccountings.Where(x => x.Amount < 0).Sum(x => x.Amount) * -1,
            EventName = payEvent.Name,
            Pay = amount,
            CreateDate = utcNow.AddHours(8),
            IsConfirm = false
        };

        return (true, LineFlexTemplate.AccountingFlexMessageTemplate(flexMessageModel));
    }

    /// <summary>
    /// 刪除帳務
    /// </summary>
    /// <param name="model"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public async Task<string> DeleteAccounting(ConfirmModel model, User user)
    {
        var accounting = await _db.Accountings.FirstOrDefaultAsync(x => x.UserId == user.Id && x.Id == model.AccountId);

        if (accounting == null)
            return "刪除成功";

        var deleteEvent = await _db.DeleteAccounts.Where(x => x.AccountId == model.AccountId).ToListAsync();

        _db.DeleteAccounts.RemoveRange(deleteEvent);
        _db.Accountings.Remove(accounting);

        await _db.SaveChangesAsync();

        return "刪除成功";
    }
}
