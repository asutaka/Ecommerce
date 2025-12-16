using Ecommerce.Infrastructure.Entities;
using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Web.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Web.Areas.Admin.Controllers;

public static class ProductsControllerImportExtension
{
    public static async Task<(List<Product> products, List<string> errors)> ParseImportFile(
        this EcommerceDbContext dbContext,
        Stream fileStream)
    {
        using var workbook = new ClosedXML.Excel.XLWorkbook(fileStream);
        var worksheet = workbook.Worksheets.First();

        var categories = await dbContext.Categories.ToListAsync();
        var categoryDict = categories.ToDictionary(c => c.Name.ToLower(), c => c.Id);

        var products = new List<Product>();
        var errors = new List<string>();
        int row = 2; // Start from row 2 (skip header)

        while (!worksheet.Cell(row, 1).IsEmpty())
        {
            try
            {
                var name = worksheet.Cell(row, 1).GetString().Trim();
                var description = worksheet.Cell(row, 2).GetString().Trim();
                var categoryName = worksheet.Cell(row, 3).GetString().Trim();
                var priceStr = worksheet.Cell(row, 4).GetString().Trim();
                var isFeaturedStr = worksheet.Cell(row, 5).GetString().Trim();
                var isActiveStr = worksheet.Cell(row, 6).GetString().Trim();
                var imagesStr = worksheet.Cell(row, 7).GetString().Trim();

                // Validation
                if (string.IsNullOrWhiteSpace(name))
                {
                    errors.Add($"Dòng {row}: Tên sản phẩm không được để trống");
                    row++;
                    continue;
                }

                if (!categoryDict.TryGetValue(categoryName.ToLower(), out var categoryId))
                {
                    errors.Add($"Dòng {row}: Danh mục '{categoryName}' không tồn tại");
                    row++;
                    continue;
                }

                if (!decimal.TryParse(priceStr, out var price) || price <= 0)
                {
                    errors.Add($"Dòng {row}: Giá không hợp lệ");
                    row++;
                    continue;
                }

                // Parse images (newline separated)
                var images = imagesStr.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .ToList();

                if (!images.Any())
                {
                    errors.Add($"Dòng {row}: Cần ít nhất 1 link ảnh");
                    row++;
                    continue;
                }

                var isFeatured = isFeaturedStr.Equals("Có", StringComparison.OrdinalIgnoreCase) ||
                                isFeaturedStr.Equals("TRUE", StringComparison.OrdinalIgnoreCase);
                var isActive = string.IsNullOrWhiteSpace(isActiveStr) ||
                              isActiveStr.Equals("Có", StringComparison.OrdinalIgnoreCase) ||
                              isActiveStr.Equals("TRUE", StringComparison.OrdinalIgnoreCase);

                var product = new Product
                {
                    Name = name,
                    Description = description,
                    Price = price,
                    IsFeatured = isFeatured,
                    IsActive = isActive,
                    Images = images,
                    PrimaryCategoryId = categoryId
                };

                product.ProductCategories.Add(new ProductCategory
                {
                    CategoryId = categoryId
                });

                products.Add(product);
            }
            catch (Exception ex)
            {
                errors.Add($"Dòng {row}: Lỗi xử lý - {ex.Message}");
            }

            row++;
        }

        return (products, errors);
    }
}
