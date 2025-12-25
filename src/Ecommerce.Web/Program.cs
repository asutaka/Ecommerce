using Microsoft.AspNetCore.Authentication;
using Ecommerce.Infrastructure.Extensions;
using Ecommerce.Web.Jobs;
using Hangfire;
using Hangfire.PostgreSql;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllersWithViews();

// Add performance optimizations
builder.Services.AddResponseCaching();
builder.Services.AddMemoryCache();
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

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

// Register order service
builder.Services.AddScoped<Ecommerce.Web.Services.IOrderService, Ecommerce.Web.Services.OrderService>();

// Register admin authentication service
builder.Services.AddScoped<Ecommerce.Web.Services.IAdminAuthService, Ecommerce.Web.Services.AdminAuthService>();

// Register customer authentication service
builder.Services.AddScoped<Ecommerce.Web.Services.ICustomerAuthService, Ecommerce.Web.Services.CustomerAuthService>();

// Register permission service
builder.Services.AddScoped<Ecommerce.Web.Services.IPermissionService, Ecommerce.Web.Services.PermissionService>();
builder.Services.AddHttpContextAccessor(); // Required for PermissionService to get current user

// Register banner analytics service
builder.Services.AddScoped<Ecommerce.Web.Services.IBannerAnalyticsService, Ecommerce.Web.Services.BannerAnalyticsService>();

// Configure MoMo Payment
builder.Services.Configure<Ecommerce.Web.Models.MoMoPaymentOptions>(
    builder.Configuration.GetSection(Ecommerce.Web.Models.MoMoPaymentOptions.SectionName));

// Register MoMo payment service (use mock by default)
var useMockMoMo = builder.Configuration.GetValue<bool>("MoMoPayment:UseMockService", true);
if (useMockMoMo)
{
    builder.Services.AddScoped<Ecommerce.Web.Services.IMoMoPaymentService, Ecommerce.Web.Services.MoMoMockPaymentService>();
}
else
{
    builder.Services.AddHttpClient<Ecommerce.Web.Services.IMoMoPaymentService, Ecommerce.Web.Services.MoMoPaymentService>();
}

// Register ZaloPay payment service (mock only for now)
builder.Services.AddScoped<Ecommerce.Web.Services.IZaloPayPaymentService, Ecommerce.Web.Services.ZaloPayMockPaymentService>();

// Register VNPay payment service (mock only for now)
builder.Services.AddScoped<Ecommerce.Web.Services.IVNPayPaymentService, Ecommerce.Web.Services.VNPayMockPaymentService>();

// Register Apple Pay payment service (mock only for now)
builder.Services.AddScoped<Ecommerce.Web.Services.IApplePayPaymentService, Ecommerce.Web.Services.ApplePayMockPaymentService>();

// Configure Hangfire
var connectionString = builder.Configuration.GetConnectionString("Postgres");
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(c => c.UseNpgsqlConnection(connectionString)));

// Optimize Hangfire for development
builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = 5; // Reduce from default 20 to 5
});

// Register cleanup job
builder.Services.AddScoped<CleanupExpiredOrdersJob>();

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

// Add SignalR
builder.Services.AddSignalR();

// Add HttpClient for AI providers
builder.Services.AddHttpClient("GroqAI", client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Add Chat Services
builder.Services.AddScoped<Ecommerce.Web.Services.IChatService, Ecommerce.Web.Services.ChatService>();

// Add AI Provider (Groq - FREE)
var enableAI = builder.Configuration.GetValue<bool>("ChatSettings:EnableAI", true);
if (enableAI)
{
    builder.Services.AddScoped<Ecommerce.Web.Services.AI.IAIProvider, Ecommerce.Web.Services.AI.GroqProvider>();
}

// Register Social Media Services
builder.Services.AddScoped<Ecommerce.Web.Services.Social.ISocialService, Ecommerce.Web.Services.Social.SocialService>();

// Register Social Platforms (Mock for Phase 1)
builder.Services.AddScoped<Ecommerce.Web.Services.Social.ISocialPlatform, Ecommerce.Web.Services.Social.FacebookPlatform>();
builder.Services.AddScoped<Ecommerce.Web.Services.Social.ISocialPlatform, Ecommerce.Web.Services.Social.ShopeePlatform>();
builder.Services.AddScoped<Ecommerce.Web.Services.Social.ISocialPlatform, Ecommerce.Web.Services.Social.ZaloPlatform>();
builder.Services.AddScoped<Ecommerce.Web.Services.Social.ISocialPlatform, Ecommerce.Web.Services.Social.TikTokPlatform>();
builder.Services.AddScoped<Ecommerce.Web.Services.Social.ISocialPlatform, Ecommerce.Web.Services.Social.YouTubePlatform>();

builder.Services.AddMassTransit(x =>
{
    // Register consumers
    x.AddConsumer<Ecommerce.Workers.Consumers.PaymentConsumer>();
    x.AddConsumer<Ecommerce.Workers.Consumers.PaymentSuccessNotificationConsumer>();
    x.AddConsumer<Ecommerce.Workers.Consumers.PaymentFailedNotificationConsumer>();
    
    x.SetKebabCaseEndpointNameFormatter();
    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitSection = builder.Configuration.GetSection("RabbitMq");
        
        // Support both URI and individual config
         var connectionUri = rabbitSection.GetValue<string>("ConnectionUri");
        
        if (!string.IsNullOrEmpty(connectionUri))
        {
            // Use URI-based configuration (CloudAMQP, etc.)
            cfg.Host(new Uri(connectionUri));
        }
        else
        {
            // Use individual configuration (localhost, etc.)
            var host = rabbitSection.GetValue<string>("Host") ?? "localhost";
            var username = rabbitSection.GetValue<string>("Username") ?? "guest";
            var password = rabbitSection.GetValue<string>("Password") ?? "guest";

            cfg.Host(host, "/", h =>
            {
                h.Username(username);
                h.Password(password);
            });
        }
        
        // Configure endpoints
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseResponseCompression();
app.UseResponseCaching();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        // Cache static files for 7 days
        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=604800");
    }
});
app.UseRouting();

app.UseSession(); // Enable session for cart

// Configure cookie authentication for Admin area
app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Lax
});

app.UseAuthentication(); // Add authentication middleware

// Middleware to ensure AdminAuth is checked for SignalR Hub
app.Use(async (context, next) =>
{
    var path = context.Request.Path;
    if (path.StartsWithSegments("/chathub"))
    {
        // Try to authenticate with AdminAuth scheme
        var result = await context.AuthenticateAsync("AdminAuth");
        if (result.Succeeded && result.Principal != null)
        {
            if (context.User.Identity?.IsAuthenticated != true)
            {
                context.User = result.Principal;
            }
            else
            {
                // Merge identities if already authenticated (e.g. as Customer)
                context.User.AddIdentities(result.Principal.Identities);
            }
        }
    }
    await next();
});

app.UseAuthorization();

// Configure Hangfire Dashboard
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() }
});

// Schedule recurring jobs
RecurringJob.AddOrUpdate<CleanupExpiredOrdersJob>(
    "cleanup-expired-orders",
    job => job.Execute(),
    Cron.Daily(2)); // Run daily at 2:00 AM

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

// SEO-friendly URLs
app.MapControllerRoute(
    name: "login",
    pattern: "login",
    defaults: new { controller = "Account", action = "Login" });

app.MapControllerRoute(
    name: "cart",
    pattern: "gio-hang",
    defaults: new { controller = "Cart", action = "Index" });

app.MapControllerRoute(
    name: "profile",
    pattern: "profile",
    defaults: new { controller = "Account", action = "Profile" });

app.MapControllerRoute(
    name: "product-details",
    pattern: "san-pham/{slug}",
    defaults: new { controller = "Product", action = "Details" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map SignalR Hub
app.MapHub<Ecommerce.Web.Hubs.ChatHub>("/chathub");

app.Run();
