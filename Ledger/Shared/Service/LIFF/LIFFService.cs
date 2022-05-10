using Microsoft.JSInterop;

namespace Ledger.Shared.Service.LIFF;

public class LIFFService
{
    private readonly IJSRuntime js;
    protected bool IsInit { get; private set; }

    public LIFFService(IJSRuntime js)
    {
        this.js = js;
        IsInit = false;
    }

    public async Task InitAsync(string liffId)
    {
        if(IsInit)
            return;

        var param = new { liffId };

        await js.InvokeVoidAsync("liff.init", param);
        IsInit = true;
    }

    public ValueTask<string> GetAccessTokendAsync()
        => js.InvokeAsync<string>("liff.getAccessToken");

    public ValueTask<string> GetIDTokenAsync()
        => js.InvokeAsync<string>("liff.getIDToken");

    public ValueTask<bool> IsLoggedInAsync()
        => js.InvokeAsync<bool>("liff.isLoggedIn");

    public ValueTask LoginAsync()
        => js.InvokeVoidAsync("liff.login");
}
