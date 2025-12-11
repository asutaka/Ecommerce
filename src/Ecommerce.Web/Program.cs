using Ecommerce.Infrastructure.Extensions;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllersWithViews();

// Add session support for cart
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(7); // Cart persists for 7 days
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register cart service
builder.Services.AddScoped<Ecommerce.Web.Services.ICartService, Ecommerce.Web.Services.CartService>();

// Register admin authentication service
builder.Services.AddScoped<Ecommerce.Web.Services.IAdminAuthService, Ecommerce.Web.Services.AdminAuthService>();

// Register customer authentication service
builder.Services.AddScoped<Ecommerce.Web.Services.ICustomerAuthService, Ecommerce.Web.Services.CustomerAuthService>();

// Configure dual authentication schemes (Admin + Customer)
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "CustomerAuth";
    options.DefaultChallengeScheme = "CustomerAuth";
})
.AddCookie("AdminAuth", options =>
{
    options.LoginPath = "/Admin/Account/Login";
    options.LogoutPath = "/Admin/Account/Logout";
    options.AccessDeniedPath = "/Admin/Account/Login";
    options.ExpireTimeSpan = TimeSpan.FromHours(12);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.Name = "Ecommerce.Admin.Auth";
})
.AddCookie("CustomerAuth", options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/Login";
    options.ExpireTimeSpan = TimeSpan.FromHours(12);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.Name = "Ecommerce.Customer.Auth";
})
.AddCookie("Cookies") // Temporary scheme for external login
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "";
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? "";
    options.SignInScheme = "Cookies";
})
.AddFacebook(options =>
{
    options.AppId = builder.Configuration["Authentication:Facebook:AppId"] ?? "";
    options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"] ?? "";
    options.SignInScheme = "Cookies";
})
.AddTwitter(options =>
{
    options.ConsumerKey = builder.Configuration["Authentication:Twitter:ConsumerKey"] ?? "";
    options.ConsumerSecret = builder.Configuration["Authentication:Twitter:ConsumerSecret"] ?? "";
    options.SignInScheme = "Cookies";
    options.RetrieveUserDetails = true;
});

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitSection = builder.Configuration.GetSection("RabbitMq");
        var host = rabbitSection.GetValue<string>("Host") ?? "localhost";
        var username = rabbitSection.GetValue<string>("Username") ?? "guest";
        var password = rabbitSection.GetValue<string>("Password") ?? "guest";

        cfg.Host(host, "/", h =>
        {
            h.Username(username);
            h.Password(password);
        });
    });
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession(); // Enable session for cart

// Configure cookie authentication for Admin area
app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Lax
});

app.UseAuthentication(); // Add authentication middleware
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
