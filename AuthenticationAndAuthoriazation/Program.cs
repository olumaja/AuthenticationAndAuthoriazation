using AuthenticationAndAuthoriazation.Data;
using AuthenticationAndAuthoriazation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;

// Begin of Nlog
var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    //Global authorization
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});
var dbConnections = builder.Configuration.GetConnectionString("DefaultConnections");
builder.Services.AddDbContextPool<ApplicationDBContext>(options => options.UseSqlServer(dbConnections));
builder.Services.AddIdentity<Employee, IdentityRole>(
        options =>
        {
            options.SignIn.RequireConfirmedEmail = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireDigit = true;
            options.Password.RequiredUniqueChars = 0;
        }
    ).AddEntityFrameworkStores<ApplicationDBContext>().AddDefaultTokenProviders();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanEditPolicy", policy => policy.RequireClaim("CanEdit")); //Add claims policy
    options.AddPolicy("AdminRolePolicy", policy => policy.RequireRole("Admin")); // Add role policy
    //This is a complex, user has admin or manager role and canedit claims
    options.AddPolicy("AdminOrManagerRoleAndCanEdit", policy => policy.RequireAssertion(c =>
            (c.User.IsInRole("Admin") || c.User.IsInRole("Manager")) && c.User.HasClaim("CanEdit", "true")
        ));
});

// NLog: Setup NLog for Dependency injection
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
builder.Host.UseNLog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseStatusCodePagesWithReExecute("/Error/{0}");
    app.UseExceptionHandler("/Error");
    //app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

Preseeder.Seeder(app.Services.CreateScope().ServiceProvider).Wait();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
