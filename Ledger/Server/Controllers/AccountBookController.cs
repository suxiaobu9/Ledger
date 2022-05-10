using Ledger.Server.Service.LIFF;
using Ledger.Shared.Model;
using Microsoft.AspNetCore.Mvc;

namespace Ledger.Server.Controllers;

[Route("[controller]")]
public class AccountBookController : Controller
{
    private readonly UserProfileService userProfileService;
    public AccountBookController(UserProfileService userProfileService)
    {
        this.userProfileService = userProfileService;
    }

    [HttpGet("MonthlyAccounting")]
    public async Task<MonthlyAccountingVm?> MonthlyAccounting()
    {
        return await userProfileService.GetUserMonthlyAccounting();
    }

    [HttpGet("UserProfile")]
    public UserProfile? UserProfile()
    {
        return userProfileService.UserProfile;
    }

}
