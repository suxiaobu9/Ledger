using Ledger.Server;
using Ledger.Shared.Model;

namespace Ledger.Shared.Service.Bookkeeping;

public interface IBookkeepingService
{
    /// <summary>
    /// 記帳
    /// </summary>
    /// <param name="lineEvent"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public Task<(bool isFlex, string message)> Accounting(string message, User user);

    /// <summary>
    /// 刪除帳務
    /// </summary>
    /// <param name="model"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public Task<string> DeleteAccounting(ConfirmModel model, User user);
}
