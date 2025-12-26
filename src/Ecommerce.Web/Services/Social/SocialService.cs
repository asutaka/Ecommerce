using Microsoft.Extensions.Logging;
using Ecommerce.Infrastructure.Entities;

namespace Ecommerce.Web.Services.Social;

public interface ISocialService
{
    IEnumerable<ISocialPlatform> GetAvailablePlatforms();
    Task<List<SocialPostResult>> PublishToPlatformsAsync(Guid productId, List<string> platformIds, string message);
}

public class SocialService : ISocialService
{
    private readonly IEnumerable<ISocialPlatform> _platforms;
    private readonly Ecommerce.Infrastructure.Persistence.EcommerceDbContext _db;
    private readonly ILogger<SocialService> _logger;

    public SocialService(
        IEnumerable<ISocialPlatform> platforms,
        Ecommerce.Infrastructure.Persistence.EcommerceDbContext db,
        ILogger<SocialService> logger)
    {
        _platforms = platforms;
        _db = db;
        _logger = logger;
    }

    public IEnumerable<ISocialPlatform> GetAvailablePlatforms()
    {
        return _platforms;
    }

    public async Task<List<SocialPostResult>> PublishToPlatformsAsync(Guid productId, List<string> platformIds, string message)
    {
        var product = await _db.Products.FindAsync(productId);
        if (product == null)
        {
            throw new Exception("Sản phẩm không tồn tại");
        }

        var results = new List<SocialPostResult>();
        var options = new SocialPostOptions 
        { 
            IncludePrice = true, 
            IncludeLink = true, 
            IncludeHashtags = true 
        };

        foreach (var platformId in platformIds)
        {
            var platform = _platforms.FirstOrDefault(p => p.Id == platformId);
            if (platform == null) continue;

            try
            {
                var result = await platform.PublishProductAsync(product, message, options);
                results.Add(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error pushing to {Platform}", platform.Name);
                results.Add(new SocialPostResult 
                { 
                    Success = false, 
                    Message = $"Lỗi: {ex.Message}", 
                    PlatformId = platform.Id 
                });
            }
        }

        return results;
    }
}

// Mock Implementations for Phase 1
public class FacebookPlatform : ISocialPlatform
{
    public string Id => "facebook";
    public string Name => "Facebook";
    public string IconClass => "fab fa-facebook-f";
    public string ColorClass => "#3b5998";
    public bool IsConfigured => true; // Simulated

    public async Task<SocialPostResult> PublishProductAsync(Product product, string message, SocialPostOptions options)
    {
        await Task.Delay(1000); // Simulate API call
        return new SocialPostResult { Success = true, Message = "Đã đăng lên Fanpage", PlatformId = Id, PostUrl = "https://facebook.com/mock-post" };
    }
}

public class ShopeePlatform : ISocialPlatform
{
    public string Id => "shopee";
    public string Name => "Shopee";
    public string IconClass => "fas fa-shopping-bag";
    public string ColorClass => "#ee4d2d";
    public bool IsConfigured => true;

    public async Task<SocialPostResult> PublishProductAsync(Product product, string message, SocialPostOptions options)
    {
        await Task.Delay(1200);
        return new SocialPostResult { Success = true, Message = "Đã đồng bộ sản phẩm", PlatformId = Id, PostUrl = "https://shopee.vn/mock-product" };
    }
}

public class ZaloPlatform : ISocialPlatform
{
    public string Id => "zalo";
    public string Name => "Zalo OA";
    public string IconClass => "fas fa-comment";
    public string ColorClass => "#0068ff";
    public bool IsConfigured => true;

    public async Task<SocialPostResult> PublishProductAsync(Product product, string message, SocialPostOptions options)
    {
        await Task.Delay(800);
        return new SocialPostResult { Success = true, Message = "Đã gửi Broadcast", PlatformId = Id };
    }
}

public class TikTokPlatform : ISocialPlatform
{
    public string Id => "tiktok";
    public string Name => "TikTok Shop";
    public string IconClass => "fab fa-tiktok";
    public string ColorClass => "#000000";
    public bool IsConfigured => true;

    public async Task<SocialPostResult> PublishProductAsync(Product product, string message, SocialPostOptions options)
    {
        await Task.Delay(1500);
        return new SocialPostResult { Success = true, Message = "Đã tải lên TikTok Shop", PlatformId = Id };
    }
}

public class YouTubePlatform : ISocialPlatform
{
    public string Id => "youtube";
    public string Name => "YouTube Shorts";
    public string IconClass => "fab fa-youtube";
    public string ColorClass => "#ff0000";
    public bool IsConfigured => true;

    public async Task<SocialPostResult> PublishProductAsync(Product product, string message, SocialPostOptions options)
    {
        await Task.Delay(2000);
        return new SocialPostResult { Success = true, Message = "Đã tạo Video Short từ ảnh", PlatformId = Id };
    }
}
