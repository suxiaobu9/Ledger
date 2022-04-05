using Ledger.Shared.Model;

namespace Ledger.Shared.Service.Delete;

public interface IDeleteAccountService
{
    /// <summary>
    /// 是否確定
    /// </summary>
    /// <param name="accountId"></param>
    /// <returns></returns>
    public Task<(bool isConfirm, ConfirmModel? confirmModel)> IsConfirm(int accountId);
}
