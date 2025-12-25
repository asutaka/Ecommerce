using Ecommerce.Infrastructure.Entities;
using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Web.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(AuthenticationSchemes = "AdminAuth")]
public class ProductsController(EcommerceDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(string? searchTerm, int? pageNumber)
    {
        var query = dbContext.Products.Include(x => x.PrimaryCategory).AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(term) || 
                                     x.Description.ToLower().Contains(term));
        }

        query = query.OrderByDescending(x => x.CreatedAt);

        int pageSize = 10;
        var products = await Models.PaginatedList<Infrastructure.Entities.Product>.CreateAsync(query.AsNoTracking(), pageNumber ?? 1, pageSize);

        ViewBag.SearchTerm = searchTerm;
        return View(products);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new ProductFormViewModel
        {
            Categories = await GetCategories()
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Categories = await GetCategories();
            return View(model);
        }

        // Filter out empty URLs
        var validImages = model.ImageUrls.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

        if (!validImages.Any())
        {
            ModelState.AddModelError("ImageUrls", "Vui lòng nhập ít nhất 1 link ảnh");
            model.Categories = await GetCategories();
            return View(model);
        }

        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            Name = model.Name,
            Slug = Helpers.SlugHelper.GenerateProductSlug(model.Name, productId),
            Description = model.Description,
            Price = model.Price,
            OriginalPrice = model.OriginalPrice,
            IsFeatured = model.IsFeatured,
            IsActive = model.IsActive,
            Images = validImages,
            PrimaryCategoryId = model.PrimaryCategoryId
        };

        // Add to many-to-many relation
        product.ProductCategories.Add(new ProductCategory
        {
            CategoryId = model.PrimaryCategoryId
        });

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var product = await dbContext.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        var model = new ProductFormViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Slug = product.Slug,
            Description = product.Description,
            Price = product.Price,
            OriginalPrice = product.OriginalPrice,
            IsFeatured = product.IsFeatured,
            IsActive = product.IsActive,
            ImageUrls = new List<string>(product.Images),
            PrimaryCategoryId = product.PrimaryCategoryId ?? Guid.Empty,
            Categories = await GetCategories()
        };

        // Pad with empty strings to ensure 5 inputs
        while (model.ImageUrls.Count < 5)
        {
            model.ImageUrls.Add(string.Empty);
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Guid id, ProductFormViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            model.Categories = await GetCategories();
            return View(model);
        }

        var product = await dbContext.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        // Filter out empty URLs
        var validImages = model.ImageUrls.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

        if (!validImages.Any())
        {
            ModelState.AddModelError("ImageUrls", "Vui lòng nhập ít nhất 1 link ảnh");
            model.Categories = await GetCategories();
            return View(model);
        }

        product.Name = model.Name;
        
        // Regenerate slug if name changed
        if (product.Name != model.Name)
        {
            product.Slug = Helpers.SlugHelper.GenerateProductSlug(model.Name, product.Id);
        }
        
        product.Description = model.Description;
        product.Price = model.Price;
        product.OriginalPrice = model.OriginalPrice;
        product.IsFeatured = model.IsFeatured;
        product.IsActive = model.IsActive;
        product.Images = validImages;
        product.PrimaryCategoryId = model.PrimaryCategoryId;
        
        // Update many-to-many: ensure it's in the list
        // Note: For full M-N support, we would clear and re-add list based on selection
        // For now, we just ensure Primary is in there.
        var existing = await dbContext.ProductCategories
            .FirstOrDefaultAsync(x => x.ProductId == id && x.CategoryId == model.PrimaryCategoryId);
            
        if (existing == null)
        {
            dbContext.ProductCategories.Add(new ProductCategory
            {
                ProductId = id,
                CategoryId = model.PrimaryCategoryId
            });
        }

        product.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        var product = await dbContext.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        var product = await dbContext.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        product.IsActive = !product.IsActive;
        product.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Export(string? searchTerm)
    {
        var query = dbContext.Products.Include(x => x.PrimaryCategory).AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(term) || 
                                     x.Description.ToLower().Contains(term));
        }

        var products = await query.OrderByDescending(x => x.CreatedAt).ToListAsync();

        using var workbook = new ClosedXML.Excel.XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Products");

        // Header
        worksheet.Cell(1, 1).Value = "STT";
        worksheet.Cell(1, 2).Value = "Tên sản phẩm";
        worksheet.Cell(1, 3).Value = "Mô tả";
        worksheet.Cell(1, 4).Value = "Danh mục";
        worksheet.Cell(1, 5).Value = "Giá (VNĐ)";
        worksheet.Cell(1, 6).Value = "Giá gốc (VNĐ)";
        worksheet.Cell(1, 7).Value = "Trạng thái";
        worksheet.Cell(1, 8).Value = "Hoạt động";
        worksheet.Cell(1, 9).Value = "Ngày tạo";
        worksheet.Cell(1, 10).Value = "Links ảnh";

        // Style header
        var headerRange = worksheet.Range(1, 1, 1, 10);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightBlue;
        headerRange.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;

        // Data
        int row = 2;
        foreach (var product in products)
        {
            worksheet.Cell(row, 1).Value = row - 1;
            worksheet.Cell(row, 2).Value = product.Name;
            worksheet.Cell(row, 3).Value = product.Description;
            worksheet.Cell(row, 4).Value = product.PrimaryCategory?.Name ?? "";
            worksheet.Cell(row, 5).Value = product.Price;
            worksheet.Cell(row, 6).Value = product.OriginalPrice ?? 0;
            worksheet.Cell(row, 7).Value = product.IsFeatured ? "Nổi bật" : "Thường";
            worksheet.Cell(row, 8).Value = product.IsActive ? "Hoạt động" : "Không hoạt động";
            worksheet.Cell(row, 9).Value = product.CreatedAt.ToString("dd/MM/yyyy HH:mm");
            worksheet.Cell(row, 10).Value = string.Join("\n", product.Images);
            row++;
        }

        // Auto-fit columns
        worksheet.Columns().AdjustToContents();

        // Return file
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        var content = stream.ToArray();

        var fileName = $"Products_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    [HttpGet]
    public IActionResult DownloadTemplate()
    {
        using var workbook = new ClosedXML.Excel.XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Products");

        // Header
        worksheet.Cell(1, 1).Value = "Tên sản phẩm *";
        worksheet.Cell(1, 2).Value = "Mô tả";
        worksheet.Cell(1, 3).Value = "Danh mục *";
        worksheet.Cell(1, 4).Value = "Giá (VNĐ) *";
        worksheet.Cell(1, 5).Value = "Giá gốc (VNĐ)";
        worksheet.Cell(1, 6).Value = "Nổi bật (Có/Không)";
        worksheet.Cell(1, 7).Value = "Hoạt động (Có/Không)";
        worksheet.Cell(1, 8).Value = "Links ảnh (mỗi link 1 dòng)";

        // Style header
        var headerRange = worksheet.Range(1, 1, 1, 8);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightGreen;
        headerRange.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;

        // Example row
        worksheet.Cell(2, 1).Value = "Áo thun nam";
        worksheet.Cell(2, 2).Value = "Áo thun cotton 100% chất lượng cao";
        worksheet.Cell(2, 3).Value = "Nam";
        worksheet.Cell(2, 4).Value = 250000;
        worksheet.Cell(2, 5).Value = 350000;
        worksheet.Cell(2, 6).Value = "Không";
        worksheet.Cell(2, 7).Value = "Có";
        worksheet.Cell(2, 8).Value = "https://example.com/image1.jpg\nhttps://example.com/image2.jpg";

        // Instructions
        worksheet.Cell(4, 1).Value = "Hướng dẫn:";
        worksheet.Cell(4, 1).Style.Font.Bold = true;
        worksheet.Cell(5, 1).Value = "- Các cột có dấu * là bắt buộc";
        worksheet.Cell(6, 1).Value = "- Danh mục phải tồn tại trong hệ thống (nhập tên danh mục)";
        worksheet.Cell(7, 1).Value = "- Giá nhập số, không có dấu phẩy hoặc chấm";
        worksheet.Cell(8, 1).Value = "- Giá gốc (tùy chọn): để trống nếu không có giảm giá";
        worksheet.Cell(9, 1).Value = "- Nổi bật/Hoạt động: nhập 'Có' hoặc 'Không'";
        worksheet.Cell(10, 1).Value = "- Links ảnh: mỗi link trên 1 dòng (Alt+Enter trong Excel)";
        worksheet.Cell(11, 1).Value = "- Xóa dòng ví dụ và hướng dẫn này trước khi import";

        // Auto-fit columns
        worksheet.Columns().AdjustToContents();

        // Return file
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        var content = stream.ToArray();

        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Products_Import_Template.xlsx");
    }

    [HttpGet]
    public IActionResult Import()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Import(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            TempData["Error"] = "Vui lòng chọn file để import.";
            return View();
        }

        if (!file.FileName.EndsWith(".xlsx"))
        {
            TempData["Error"] = "File phải có định dạng .xlsx";
            return View();
        }

        try
        {
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;

            var (products, errors) = await dbContext.ParseImportFile(stream);

            if (errors.Any())
            {
                TempData["Error"] = $"Import thất bại với {errors.Count} lỗi:\n" + string.Join("\n", errors.Take(10));
                if (errors.Count > 10)
                {
                    TempData["Error"] += $"\n... và {errors.Count - 10} lỗi khác";
                }
                return View();
            }

            if (!products.Any())
            {
                TempData["Error"] = "Không tìm thấy sản phẩm nào để import.";
                return View();
            }

            await dbContext.Products.AddRangeAsync(products);
            await dbContext.SaveChangesAsync();

            TempData["Success"] = $"Import thành công {products.Count} sản phẩm!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Lỗi khi import: {ex.Message}";
            return View();
        }
    }

    private async Task<List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>> GetCategories()
    {
        return await dbContext.Categories
            .OrderBy(x => x.Name)
            .Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            })
            .ToListAsync();
    }
}
