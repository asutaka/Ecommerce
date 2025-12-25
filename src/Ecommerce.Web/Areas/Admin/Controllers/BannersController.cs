using Ecommerce.Infrastructure.Entities;
using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Web.Areas.Admin.ViewModels;
using Ecommerce.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(AuthenticationSchemes = "AdminAuth")]
public class BannersController(EcommerceDbContext context, IWebHostEnvironment environment, IBannerAnalyticsService analyticsService) : Controller
{
    private readonly EcommerceDbContext _context = context;
    private readonly IWebHostEnvironment _environment = environment;
    private readonly IBannerAnalyticsService _analyticsService = analyticsService;

    // GET: Admin/Banners
    public async Task<IActionResult> Index(BannerType? bannerType, BannerPosition? position, bool? isActive)
    {
        var query = _context.Banners.Include(b => b.Category).AsQueryable();

        // Apply filters
        if (bannerType.HasValue)
            query = query.Where(b => b.BannerType == bannerType.Value);

        if (position.HasValue)
            query = query.Where(b => b.Position == position.Value);

        if (isActive.HasValue)
            query = query.Where(b => b.IsActive == isActive.Value);

        var banners = await query
            .OrderBy(b => b.DisplayOrder)
            .ThenByDescending(b => b.CreatedAt)
            .Select(b => new BannerViewModel
            {
                Id = b.Id,
                Title = b.Title,
                Description = b.Description,
                ImageUrl = b.ImageUrl,
                MobileImageUrl = b.MobileImageUrl,
                BannerType = b.BannerType,
                Position = b.Position,
                LinkUrl = b.LinkUrl,
                OpenInNewTab = b.OpenInNewTab,
                DisplayOrder = b.DisplayOrder,
                IsActive = b.IsActive,
                StartDate = b.StartDate,
                EndDate = b.EndDate,
                CategoryId = b.CategoryId,
                CategoryName = b.Category != null ? b.Category.Name : null,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt
            })
            .ToListAsync();

        // Pass filter values to view
        ViewBag.BannerType = bannerType;
        ViewBag.Position = position;
        ViewBag.IsActive = isActive;

        return View(banners);
    }

    // GET: Admin/Banners/Create
    public async Task<IActionResult> Create()
    {
        var model = new BannerViewModel();
        await PopulateDropdowns(model);
        return View(model);
    }

    // POST: Admin/Banners/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BannerViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // Upload desktop image
                if (model.ImageFile != null)
                {
                    model.ImageUrl = await SaveImageAsync(model.ImageFile);
                }

                // Upload mobile image
                if (model.MobileImageFile != null)
                {
                    model.MobileImageUrl = await SaveImageAsync(model.MobileImageFile);
                }

                // Validate that at least desktop image is provided
                if (string.IsNullOrEmpty(model.ImageUrl))
                {
                    ModelState.AddModelError("ImageFile", "Ảnh desktop là bắt buộc");
                    await PopulateDropdowns(model);
                    return View(model);
                }

                var banner = new Banner
                {
                    Title = model.Title,
                    Description = model.Description,
                    ImageUrl = model.ImageUrl,
                    MobileImageUrl = model.MobileImageUrl,
                    BannerType = model.BannerType,
                    Position = model.Position,
                    LinkUrl = model.LinkUrl,
                    OpenInNewTab = model.OpenInNewTab,
                    DisplayOrder = model.DisplayOrder,
                    IsActive = model.IsActive,
                    StartDate = model.StartDate.HasValue ? DateTime.SpecifyKind(model.StartDate.Value, DateTimeKind.Utc) : null,
                    EndDate = model.EndDate.HasValue ? DateTime.SpecifyKind(model.EndDate.Value, DateTimeKind.Utc) : null,
                    CategoryId = model.CategoryId
                };

                _context.Banners.Add(banner);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Tạo banner thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi khi tạo banner: {ex.Message}");
            }
        }

        await PopulateDropdowns(model);
        return View(model);
    }

    // GET: Admin/Banners/Edit/5
    public async Task<IActionResult> Edit(Guid id)
    {
        var banner = await _context.Banners
            .Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (banner == null)
        {
            TempData["ErrorMessage"] = "Không tìm thấy banner";
            return RedirectToAction(nameof(Index));
        }

        var model = new BannerViewModel
        {
            Id = banner.Id,
            Title = banner.Title,
            Description = banner.Description,
            ImageUrl = banner.ImageUrl,
            MobileImageUrl = banner.MobileImageUrl,
            BannerType = banner.BannerType,
            Position = banner.Position,
            LinkUrl = banner.LinkUrl,
            OpenInNewTab = banner.OpenInNewTab,
            DisplayOrder = banner.DisplayOrder,
            IsActive = banner.IsActive,
            StartDate = banner.StartDate,
            EndDate = banner.EndDate,
            CategoryId = banner.CategoryId,
            CategoryName = banner.Category?.Name,
            CreatedAt = banner.CreatedAt,
            UpdatedAt = banner.UpdatedAt
        };

        await PopulateDropdowns(model);
        return View(model);
    }

    // POST: Admin/Banners/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(BannerViewModel model)
    {
        if (!model.Id.HasValue)
        {
            TempData["ErrorMessage"] = "ID banner không hợp lệ";
            return RedirectToAction(nameof(Index));
        }

        if (ModelState.IsValid)
        {
            try
            {
                var banner = await _context.Banners.FindAsync(model.Id.Value);
                if (banner == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy banner";
                    return RedirectToAction(nameof(Index));
                }

                // Upload new desktop image if provided
                if (model.ImageFile != null)
                {
                    // Delete old image
                    if (!string.IsNullOrEmpty(banner.ImageUrl))
                    {
                        DeleteImage(banner.ImageUrl);
                    }
                    model.ImageUrl = await SaveImageAsync(model.ImageFile);
                }
                else
                {
                    model.ImageUrl = banner.ImageUrl; // Keep existing
                }

                // Upload new mobile image if provided
                if (model.MobileImageFile != null)
                {
                    // Delete old mobile image
                    if (!string.IsNullOrEmpty(banner.MobileImageUrl))
                    {
                        DeleteImage(banner.MobileImageUrl);
                    }
                    model.MobileImageUrl = await SaveImageAsync(model.MobileImageFile);
                }
                else
                {
                    model.MobileImageUrl = banner.MobileImageUrl; // Keep existing
                }

                // Update banner properties
                banner.Title = model.Title;
                banner.Description = model.Description;
                banner.ImageUrl = model.ImageUrl;
                banner.MobileImageUrl = model.MobileImageUrl;
                banner.BannerType = model.BannerType;
                banner.Position = model.Position;
                banner.LinkUrl = model.LinkUrl;
                banner.OpenInNewTab = model.OpenInNewTab;
                banner.DisplayOrder = model.DisplayOrder;
                banner.IsActive = model.IsActive;
                banner.StartDate = model.StartDate.HasValue ? DateTime.SpecifyKind(model.StartDate.Value, DateTimeKind.Utc) : null;
                banner.EndDate = model.EndDate.HasValue ? DateTime.SpecifyKind(model.EndDate.Value, DateTimeKind.Utc) : null;
                banner.CategoryId = model.CategoryId;
                banner.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cập nhật banner thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi khi cập nhật banner: {ex.Message}");
            }
        }

        await PopulateDropdowns(model);
        return View(model);
    }

    // POST: Admin/Banners/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var banner = await _context.Banners.FindAsync(id);
            if (banner == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy banner";
                return RedirectToAction(nameof(Index));
            }

            // Delete images
            if (!string.IsNullOrEmpty(banner.ImageUrl))
            {
                DeleteImage(banner.ImageUrl);
            }
            if (!string.IsNullOrEmpty(banner.MobileImageUrl))
            {
                DeleteImage(banner.MobileImageUrl);
            }

            _context.Banners.Remove(banner);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Xóa banner thành công!";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Lỗi khi xóa banner: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: Admin/Banners/ToggleActive/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        try
        {
            var banner = await _context.Banners.FindAsync(id);
            if (banner == null)
            {
                return Json(new { success = false, message = "Không tìm thấy banner" });
            }

            banner.IsActive = !banner.IsActive;
            banner.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Json(new { success = true, isActive = banner.IsActive });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    // GET: Admin/Banners/Analytics/5
    public async Task<IActionResult> Analytics(Guid id, DateTime? from, DateTime? to)
    {
        var banner = await _context.Banners.FindAsync(id);
        if (banner == null)
        {
            TempData["ErrorMessage"] = "Không tìm thấy banner";
            return RedirectToAction(nameof(Index));
        }

        // Default to last 30 days if not specified
        var fromDate = from ?? DateTime.UtcNow.AddDays(-30);
        var toDate = to ?? DateTime.UtcNow;

        var analytics = await _analyticsService.GetAnalyticsAsync(id, fromDate, toDate);

        var model = new BannerAnalyticsViewModel
        {
            BannerId = id,
            BannerTitle = banner.Title,
            BannerType = banner.BannerType.ToString(),
            FromDate = fromDate,
            ToDate = toDate,
            TotalViews = analytics.TotalViews,
            TotalClicks = analytics.TotalClicks,
            AverageCTR = analytics.AverageCTR,
            DailyData = analytics.DailyBreakdown.Select(d => new DailyAnalyticsData
            {
                Date = d.Date.ToString("yyyy-MM-dd"),
                Views = d.Views,
                Clicks = d.Clicks,
                CTR = d.CTR
            }).ToList()
        };

        return View(model);
    }

    #region Helper Methods

    private async Task PopulateDropdowns(BannerViewModel model)
    {
        // Banner Types
        model.BannerTypes = Enum.GetValues<BannerType>()
            .Select(bt => new SelectListItem
            {
                Value = ((int)bt).ToString(),
                Text = bt switch
                {
                    BannerType.Hero => "Hero Banner (Carousel)",
                    BannerType.TopBar => "Top Banner (Thông báo)",
                    BannerType.Promotional => "Promotional Banner",
                    BannerType.Category => "Category Banner",
                    _ => bt.ToString()
                },
                Selected = bt == model.BannerType
            })
            .ToList();

        // Positions
        model.Positions = Enum.GetValues<BannerPosition>()
            .Select(p => new SelectListItem
            {
                Value = ((int)p).ToString(),
                Text = p switch
                {
                    BannerPosition.All => "Tất cả trang",
                    BannerPosition.Home => "Trang chủ",
                    BannerPosition.CategoryPage => "Trang danh mục",
                    BannerPosition.ProductDetail => "Trang sản phẩm",
                    _ => p.ToString()
                },
                Selected = p == model.Position
            })
            .ToList();

        // Categories
        var categories = await _context.Categories
            .Where(c => c.ParentId == null)
            .OrderBy(c => c.Name)
            .ToListAsync();

        model.Categories = categories
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name,
                Selected = c.Id == model.CategoryId
            })
            .ToList();

        model.Categories.Insert(0, new SelectListItem { Value = "", Text = "-- Chọn danh mục (tùy chọn) --" });
    }

    private async Task<string> SaveImageAsync(IFormFile file)
    {
        var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "banners");
        Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        return $"/images/banners/{uniqueFileName}";
    }

    private void DeleteImage(string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl)) return;

        try
        {
            var filePath = Path.Combine(_environment.WebRootPath, imageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
        catch
        {
            // Ignore errors when deleting images
        }
    }

    #endregion
}
