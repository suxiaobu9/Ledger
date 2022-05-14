using Ledger.Server;
using Ledger.Server.Middleware;
using Ledger.Server.Service.LIFF;
using Ledger.Shared.Model;
using Ledger.Shared.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddDbContext<BookkeepingContext>(option => option.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]));
builder.Services.Configure<LineBot>(builder.Configuration.GetSection("LineBot"));
builder.Services.Configure<LIFFInfo>(builder.Configuration.GetSection("LIFF"));

//每次Call Method都注入一個新的
//services.AddTransient

//每個LifeCycle注入一個新的
//services.AddScoped   

//只會在站台啟動時注入一個新的
//services.AddSingleton
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<BookkeepingService>();
builder.Services.AddScoped<DeleteAccountService>();
builder.Services.AddScoped<UserProfileService>();

builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseLIFFMiddleware();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
