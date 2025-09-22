using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using myHotel.Areas.SecureAccess.Models;
using myHotel.Infrastructure;
using myHotel.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register DbContext
builder.Services.AddDbContext<Hotel_Management_Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Hotel_Management_Connection")));

// Register Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<Hotel_Management_Context>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IAuditLogService, AuditLogService>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


// Run seeding here
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.InitializeAsync(services);
}

//app.Use(async (context, next) =>
//{
//    Console.WriteLine($"Incoming: {context.Request.Method} {context.Request.Path}");
//    await next();
//});


app.Run();
