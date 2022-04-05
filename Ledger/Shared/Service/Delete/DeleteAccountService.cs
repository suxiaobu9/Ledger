using Ledger.Server;
using Ledger.Shared.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ledger.Shared.Service.Delete;

public class DeleteAccountService : IDeleteAccountService
{
    private readonly BookkeepingContext _db;
    public DeleteAccountService(BookkeepingContext db)
    {
        _db = db;
    }

    /// <summary>
    /// 是否確定
    /// </summary>
    /// <param name="accountId"></param>
    /// <returns></returns>
    public async Task<(bool isConfirm, ConfirmModel? confirmModel)> IsConfirm(int accountId)
    {
        var deleteEvent = await _db.DeleteAccounts.FirstOrDefaultAsync(x => x.AccountId == accountId);
        var utcNow = DateTime.UtcNow;

        var account = await _db.Accountings
          .Include(x => x.Event)
          .FirstOrDefaultAsync(x => x.Id == accountId);

        if (account == null)
            return (false, null);

        var result = new ConfirmModel
        {
            AccountId = accountId,
            EventName = account.Event.Name,
            Pay = account.Amount
        };

        // 沒有確定事件
        if (deleteEvent == null)
        {
            await _db.DeleteAccounts.AddAsync(new DeleteAccount
            {
                AccountId = accountId,
                Deadline = DateTime.UtcNow.AddMinutes(2),
                IsConfirm = true
            });
            await _db.SaveChangesAsync();
            return (false, result);
        }

        // 過期或未同意
        if (deleteEvent.Deadline < utcNow || !deleteEvent.IsConfirm)
        {
            deleteEvent.IsConfirm = true;
            deleteEvent.Deadline = utcNow.AddMinutes(2);
            await _db.SaveChangesAsync();
            return (false, result);
        }

        return (true, result);
    }

}
