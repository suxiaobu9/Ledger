using Ledger.Server;
using Microsoft.EntityFrameworkCore;

namespace Ledger.Shared.Service;

public class UserService
{
    private readonly BookkeepingContext _db;
    public UserService(BookkeepingContext db)
    {
        _db = db;
    }
    /// <summary>
    /// 取得管理者
    /// </summary>
    /// <returns></returns>
    public async Task<User?> GetAdmin()
    {
        return await _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.IsAdmin);
    }

    /// <summary>
    /// 取得使用者
    /// </summary>
    /// <param name="lineUserId"></param>
    /// <returns></returns>
    public async Task<User?> GetUser(string lineUserId)
    {
        return await _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.LineUserId == lineUserId);
    }

    /// <summary>
    /// 取得使用者
    /// </summary>
    /// <param name="lineUserIds"></param>
    /// <returns></returns>
    public async Task<User[]> GetUsers(IEnumerable<string> lineUserIds)
    {
        return await _db.Users.AsNoTracking().Where(x => lineUserIds.Contains(x.LineUserId)).ToArrayAsync();
    }
}
