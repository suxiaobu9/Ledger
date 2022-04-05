using Ledger.Server;
using Ledger.Shared.Model;
using Ledger.Shared.Service.Bookkeeping;
using Ledger.Shared.Service.EventService;
using Ledger.Shared.Service.Member;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddDbContext<BookkeepingContext>(option => option.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]));
builder.Services.Configure<LineBot>(builder.Configuration.GetSection("LineBot"));

//�C��Call Method���`�J�@�ӷs��
//services.AddTransient

//�C��LifeCycle�`�J�@�ӷs��
//services.AddScoped   

//�u�|�b���x�Ұʮɪ`�J�@�ӷs��
//services.AddSingleton
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IBookkeepingService, BookkeepingService>();

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


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
