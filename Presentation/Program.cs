using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Presentation.Data;
using DataAccess.DataContext;
using DataAccess.Repositories;
using Domain.Interfaces;
using Presentation.ActionFilters;

var builder = WebApplication.CreateBuilder(args);
var useFileRepo = builder.Configuration.GetValue<bool>("UseFileRepository");

// Add services to the container.
//string connectionString = "Data Source=SQL1003.site4now.net;Initial Catalog=db_ab745e_polldb;MultipleActiveResultSets=true;User Id=db_ab745e_polldb_admin;Password=#w6nta*fE3r9diF";
string connectionString = "Server=LAPTOP-D3AORRD8\\SQLEXPRESS;Database=PollDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddScoped<OneVoteOnlyFilter>();


if (useFileRepo)
{
    builder.Services.AddSingleton<IPollRepository, PollFileRepository>();
}
else
{
    builder.Services.AddScoped<IPollRepository, PollRepository>();
    builder.Services.AddDbContext<PollDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
