using Ecommerce.Infrastructure.Entities;
using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Web.Areas.Admin.ViewModels;
using Ecommerce.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class CouponsController(EcommerceDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(string? searchTerm, int? pageNumber)
    {
        var query = dbContext.Coupons.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(x => x.Code.ToLower().Contains(term) || 
                                     x.Description.ToLower().Contains(term));
        }

        query = query.OrderByDescending(x => x.CreatedAt);

        int pageSize = 10;
        var coupons = await PaginatedList<Coupon>.CreateAsync(query.AsNoTracking(), pageNumber ?? 1, pageSize);

        ViewBag.SearchTerm = searchTerm;
        return View(coupons);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CouponFormViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CouponFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Validation
        if (await dbContext.Coupons.AnyAsync(x => x.Code == model.Code))
        {
            ModelState.AddModelError("Code", "Mã khuyến mãi đã tồn tại");
            return View(model);
        }

        if (model.EndDate <= model.StartDate)
        {
            ModelState.AddModelError("EndDate", "Ngày kết thúc phải sau ngày bắt đầu");
            return View(model);
        }

        var coupon = new Coupon
        {
            Code = model.Code.ToUpper(),
            Description = model.Description,
            DiscountAmount = model.DiscountAmount,
            DiscountPercentage = model.DiscountPercentage,
            MinimumOrderAmount = model.MinimumOrderAmount,
            StartDate = DateTime.SpecifyKind(model.StartDate, DateTimeKind.Utc),
            EndDate = DateTime.SpecifyKind(model.EndDate, DateTimeKind.Utc),
            IsActive = model.IsActive,
            UsageLimit = model.UsageLimit,
            UsedCount = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        dbContext.Coupons.Add(coupon);
        await dbContext.SaveChangesAsync();

        TempData["Success"] = "Thêm mã khuyến mãi thành công";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var coupon = await dbContext.Coupons.FindAsync(id);
        if (coupon == null)
        {
            return NotFound();
        }

        var model = new CouponFormViewModel
        {
            Id = coupon.Id,
            Code = coupon.Code,
            Description = coupon.Description,
            DiscountAmount = coupon.DiscountAmount,
            DiscountPercentage = coupon.DiscountPercentage,
            MinimumOrderAmount = coupon.MinimumOrderAmount,
            StartDate = coupon.StartDate,
            EndDate = coupon.EndDate,
            IsActive = coupon.IsActive,
            UsageLimit = coupon.UsageLimit
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Guid id, CouponFormViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var coupon = await dbContext.Coupons.FindAsync(id);
        if (coupon == null)
        {
            return NotFound();
        }

        // Check duplicate code
        if (await dbContext.Coupons.AnyAsync(x => x.Code == model.Code && x.Id != id))
        {
            ModelState.AddModelError("Code", "Mã khuyến mãi đã tồn tại");
            return View(model);
        }

        if (model.EndDate <= model.StartDate)
        {
            ModelState.AddModelError("EndDate", "Ngày kết thúc phải sau ngày bắt đầu");
            return View(model);
        }

        coupon.Code = model.Code.ToUpper();
        coupon.Description = model.Description;
        coupon.DiscountAmount = model.DiscountAmount;
        coupon.DiscountPercentage = model.DiscountPercentage;
        coupon.MinimumOrderAmount = model.MinimumOrderAmount;
        coupon.StartDate = DateTime.SpecifyKind(model.StartDate, DateTimeKind.Utc);
        coupon.EndDate = DateTime.SpecifyKind(model.EndDate, DateTimeKind.Utc);
        coupon.IsActive = model.IsActive;
        coupon.UsageLimit = model.UsageLimit;
        coupon.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();

        TempData["Success"] = "Cập nhật mã khuyến mãi thành công";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        var coupon = await dbContext.Coupons.FindAsync(id);
        if (coupon == null)
        {
            return NotFound();
        }

        dbContext.Coupons.Remove(coupon);
        await dbContext.SaveChangesAsync();

        TempData["Success"] = "Xóa mã khuyến mãi thành công";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        var coupon = await dbContext.Coupons.FindAsync(id);
        if (coupon == null)
        {
            return NotFound();
        }

        coupon.IsActive = !coupon.IsActive;
        coupon.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();

        TempData["Success"] = coupon.IsActive ? "Đã kích hoạt mã khuyến mãi" : "Đã vô hiệu hóa mã khuyến mãi";
        return RedirectToAction(nameof(Index));
    }
}
