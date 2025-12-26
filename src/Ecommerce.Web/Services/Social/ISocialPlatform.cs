using Ecommerce.Infrastructure.Entities;

namespace Ecommerce.Web.Services.Social;

public interface ISocialPlatform
{
    string Id { get; }
    string Name { get; }
    string IconClass { get; } // e.g. "fab fa-facebook"
    string ColorClass { get; } // e.g. "btn-facebook" where CSS is defined
    bool IsConfigured { get; }

    Task<SocialPostResult> PublishProductAsync(Product product, string message, SocialPostOptions options);
}

public class SocialPostResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? PostUrl { get; set; }
    public string PlatformId { get; set; } = string.Empty;
}

public class SocialPostOptions
{
    public bool IncludePrice { get; set; }
    public bool IncludeLink { get; set; }
    public bool IncludeHashtags { get; set; }
}
