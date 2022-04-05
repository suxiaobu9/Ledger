using Ledger.Server;

namespace Ledger.Shared.Service.Member;

public interface IUserService
{
    /// <summary>
    /// 取得管理者
    /// </summary>
    /// <returns></returns>
    public Task<User?> GetAdmin();

    /// <summary>
    /// 取得使用者
    /// </summary>
    /// <param name="lineUserId"></param>
    /// <returns></returns>
    public Task<User?> GetUser(string lineUserId);

    /// <summary>
    /// 取得使用者
    /// </summary>
    /// <param name="lineUserIds"></param>
    /// <returns></returns>
    public Task<User[]> GetUsers(IEnumerable<string> lineUserIds);
}
