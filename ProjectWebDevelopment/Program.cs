using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectWebDevelopment.Data;
using ProjectWebDevelopment.Data.Entities;
using ProjectWebDevelopment.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<AuctionUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

DotNetEnv.Env.Load();

builder.Services.AddScoped<IAuctionImageProcessor, Base64AuctionImageProcessor>();
builder.Services.AddScoped<IAuctionRepository, AuctionRepositoryEF>();
builder.Services.AddScoped<IAuctionMailer>(provider =>
{
    var smtpHost = Environment.GetEnvironmentVariable("MAILER_HOST");
    var smtpUser = Environment.GetEnvironmentVariable("MAILER_USERNAME");
    var smtpPass = Environment.GetEnvironmentVariable("MAILER_PASSWORD");

    return new AuctionMailer(smtpHost, smtpUser, smtpPass);
});
builder.Services.AddScoped<AuctionService, AuctionService>();

builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.MapHub<AuctionHub>("/auctionhub");

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
