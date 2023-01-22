using Microsoft.EntityFrameworkCore;
using UniProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("UniProjectContextConnection") ?? throw new InvalidOperationException("Connection string 'UniProjectContextConnection' not found.");

// Add services to the container.
builder.Services.AddControllersWithViews();

//DB context with specified connection string
builder.Services.AddDbContext<BooksDB>(
dbContextOptions => dbContextOptions.UseMySql("server=127.0.0.1;port=3306;username=root;password=;database=booksdb;", ServerVersion.AutoDetect("server=127.0.0.1;port=3306;username=root;password=;database=booksdb;"))
.LogTo(Console.WriteLine, LogLevel.Information)
.EnableSensitiveDataLogging()
.EnableDetailedErrors()
);

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<BooksDB>();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


//Auth cookie, naming and path
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Login";
        options.Cookie.Name = "BooksCookie";
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}"); //by default its login

app.Run();
