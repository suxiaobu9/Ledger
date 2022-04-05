using Ledger.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ledger.Shared.Service.EventService;

public interface IEventService
{
    /// <summary>
    /// 取得花費項目
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public Task<Event> CreateAndGetEvent(string eventName, int userId);
}
