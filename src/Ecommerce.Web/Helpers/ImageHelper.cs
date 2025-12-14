using System;

namespace Ecommerce.Web.Helpers
{
    public static class ImageHelper
    {
        /// <summary>
        /// Optimizes external image URL with WebP format, quality, and size parameters
        /// </summary>
        /// <param name="url">Original image URL</param>
        /// <param name="width">Desired width in pixels (default: 800)</param>
        /// <param name="quality">Image quality 0-100 (default: 80)</param>
        /// <returns>Optimized image URL</returns>
        public static string OptimizeImageUrl(string url, int width = 800, int quality = 80)
        {
            if (string.IsNullOrEmpty(url))
                return url;

            // Unsplash
            if (url.Contains("unsplash.com"))
            {
                // Remove existing parameters
                var baseUrl = url.Split('?')[0];
                return $"{baseUrl}?auto=format&fit=crop&w={width}&q={quality}";
            }

            // Picsum
            if (url.Contains("picsum.photos"))
            {
                // Picsum format: https://picsum.photos/800/800
                return $"https://picsum.photos/{width}/{width}?quality={quality}";
            }

            // Cloudinary
            if (url.Contains("cloudinary.com") && url.Contains("/upload/"))
            {
                return url.Replace("/upload/", $"/upload/w_{width},q_{quality},f_auto/");
            }

            // Imgix
            if (url.Contains("imgix.net"))
            {
                var separator = url.Contains("?") ? "&" : "?";
                return $"{url}{separator}w={width}&q={quality}&auto=format";
            }

            // Default: return original URL if CDN not recognized
            return url;
        }

        /// <summary>
        /// Generates srcset attribute for responsive images
        /// </summary>
        /// <param name="url">Original image URL</param>
        /// <param name="quality">Image quality 0-100 (default: 80)</param>
        /// <returns>srcset attribute value</returns>
        public static string GenerateSrcSet(string url, int quality = 80)
        {
            if (string.IsNullOrEmpty(url))
                return string.Empty;

            var url400 = OptimizeImageUrl(url, 400, quality);
            var url800 = OptimizeImageUrl(url, 800, quality);
            var url1200 = OptimizeImageUrl(url, 1200, quality);

            return $"{url400} 400w, {url800} 800w, {url1200} 1200w";
        }

        /// <summary>
        /// Generates sizes attribute for responsive images
        /// </summary>
        /// <returns>sizes attribute value</returns>
        public static string GetDefaultSizes()
        {
            return "(max-width: 600px) 400px, (max-width: 1000px) 800px, 1200px";
        }

        /// <summary>
        /// Gets WebP version of image URL if supported by CDN
        /// </summary>
        /// <param name="url">Original image URL</param>
        /// <param name="width">Desired width</param>
        /// <param name="quality">Image quality</param>
        /// <returns>WebP image URL or original if not supported</returns>
        public static string GetWebPUrl(string url, int width = 800, int quality = 80)
        {
            if (string.IsNullOrEmpty(url))
                return url;

            // Unsplash supports auto format (includes WebP)
            if (url.Contains("unsplash.com"))
            {
                var baseUrl = url.Split('?')[0];
                return $"{baseUrl}?auto=format&fit=crop&w={width}&q={quality}&fm=webp";
            }

            // Cloudinary
            if (url.Contains("cloudinary.com") && url.Contains("/upload/"))
            {
                return url.Replace("/upload/", $"/upload/w_{width},q_{quality},f_webp/");
            }

            // Imgix
            if (url.Contains("imgix.net"))
            {
                var separator = url.Contains("?") ? "&" : "?";
                return $"{url}{separator}w={width}&q={quality}&fm=webp";
            }

            // Default: use optimize function
            return OptimizeImageUrl(url, width, quality);
        }
    }
}
