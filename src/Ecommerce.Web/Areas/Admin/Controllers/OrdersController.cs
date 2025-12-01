using Ecommerce.Infrastructure.Entities;
using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Web.Areas.Admin.ViewModels;
using Ecommerce.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MassTransit;
using Ecommerce.Contracts;
using OfficeOpenXml;

namespace Ecommerce.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class OrdersController(EcommerceDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(int? pageNumber, OrderStatus? status, string? keyword, DateTime? fromDate, DateTime? toDate)
    {
        var query = dbContext.Orders.AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            keyword = keyword.Trim();
            query = query.Where(x => x.CustomerName.Contains(keyword) || x.CustomerEmail.Contains(keyword));
        }

        if (fromDate.HasValue)
        {
            query = query.Where(x => x.CreatedAt >= fromDate.Value.ToUniversalTime());
        }

        if (toDate.HasValue)
        {
            // Add 1 day to include the end date fully if it's just a date without time
            var endDate = toDate.Value.Date.AddDays(1).ToUniversalTime();
            query = query.Where(x => x.CreatedAt < endDate);
        }

        query = query.OrderByDescending(x => x.CreatedAt);

        int pageSize = 10;
        var orders = await PaginatedList<Order>.CreateAsync(query.AsNoTracking(), pageNumber ?? 1, pageSize);

        var model = new OrderListViewModel
        {
            Orders = orders,
            CurrentStatus = status,
            Keyword = keyword,
            FromDate = fromDate,
            ToDate = toDate
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(Guid id, [FromServices] IPublishEndpoint publishEndpoint)
    {
        var order = await dbContext.Orders.FindAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        if (order.Status == OrderStatus.Completed || order.Status == OrderStatus.Cancelled || order.Status == OrderStatus.Failed)
        {
            TempData["Error"] = "Không thể hủy đơn hàng này.";
            return RedirectToAction(nameof(Details), new { id });
        }

        await publishEndpoint.Publish(new CancelOrder(order.Id, "Admin cancelled"));
        
        TempData["Success"] = "Đã gửi yêu cầu hủy đơn hàng.";
        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var order = await dbContext.Orders
            .Include(x => x.Items)
            .Include(x => x.Payment)
            .Include(x => x.Notification)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        var model = new OrderDetailsViewModel
        {
            Id = order.Id,
            CustomerName = order.CustomerName,
            CustomerEmail = order.CustomerEmail,
            Status = order.Status,
            Total = order.Total,
            CreatedAt = order.CreatedAt,
            Items = order.Items.Select(x => new OrderItemViewModel
            {
                ProductName = x.ProductName,
                UnitPrice = x.UnitPrice,
                Quantity = x.Quantity
            }).ToList(),
            Payment = order.Payment != null ? new PaymentViewModel
            {
                Amount = order.Payment.Amount,
                Reference = order.Payment.Reference,
                ProcessedAt = order.Payment.ProcessedAt
            } : null,
            Notification = order.Notification != null ? new NotificationLogViewModel
            {
                Channel = order.Notification.Channel,
                Destination = order.Notification.Destination,
                SentAt = order.Notification.SentAt,
                Message = $"Order confirmation sent via {order.Notification.Channel}"
            } : null
        };

        return View(model);
    }
    [HttpGet]
    public async Task<IActionResult> Export(OrderStatus? status, string? keyword, DateTime? fromDate, DateTime? toDate)
    {
        var query = dbContext.Orders.AsNoTracking().AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            keyword = keyword.Trim();
            query = query.Where(x => x.CustomerName.Contains(keyword) || x.CustomerEmail.Contains(keyword));
        }

        if (fromDate.HasValue)
        {
            query = query.Where(x => x.CreatedAt >= fromDate.Value.ToUniversalTime());
        }

        if (toDate.HasValue)
        {
            var endDate = toDate.Value.Date.AddDays(1).ToUniversalTime();
            query = query.Where(x => x.CreatedAt < endDate);
        }

        query = query.OrderByDescending(x => x.CreatedAt);
        var orders = await query.Include(x => x.Payment).ToListAsync();

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Orders");

        // Headers
        worksheet.Cells[1, 1].Value = "Id";
        worksheet.Cells[1, 2].Value = "Customer Name";
        worksheet.Cells[1, 3].Value = "Customer Email";
        worksheet.Cells[1, 4].Value = "Status";
        worksheet.Cells[1, 5].Value = "Total";
        worksheet.Cells[1, 6].Value = "Created At";
        worksheet.Cells[1, 7].Value = "Payment Reference";

        using (var range = worksheet.Cells[1, 1, 1, 7])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        }

        // Data
        for (int i = 0; i < orders.Count; i++)
        {
            var order = orders[i];
            var row = i + 2;

            worksheet.Cells[row, 1].Value = order.Id;
            worksheet.Cells[row, 2].Value = order.CustomerName;
            worksheet.Cells[row, 3].Value = order.CustomerEmail;
            worksheet.Cells[row, 4].Value = order.Status.ToString();
            worksheet.Cells[row, 5].Value = order.Total;
            worksheet.Cells[row, 6].Value = order.CreatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
            worksheet.Cells[row, 7].Value = order.Payment?.Reference;
        }

        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;

        var fileName = $"Orders_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    [HttpGet]
    public IActionResult Import()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Import(IFormFile? file, [FromServices] IPublishEndpoint publishEndpoint)
    {
        if (file == null || file.Length == 0)
        {
            ModelState.AddModelError("File", "Vui lòng chọn file Excel");
            return View();
        }

        if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError("File", "Chỉ chấp nhận file .xlsx");
            return View();
        }

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        try
        {
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            using var package = new ExcelPackage(stream);
            
            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension.Rows;
            
            int successCount = 0;
            int errorCount = 0;

            // Cache products for lookup
            var allProducts = await dbContext.Products
                .Select(x => new { x.Id, x.Name, x.Price })
                .ToListAsync();

            // Assume header is row 1
            for (int row = 2; row <= rowCount; row++)
            {
                var customerName = worksheet.Cells[row, 1].Value?.ToString();
                var customerEmail = worksheet.Cells[row, 2].Value?.ToString();
                var itemsStr = worksheet.Cells[row, 3].Value?.ToString();

                if (string.IsNullOrWhiteSpace(customerName) || string.IsNullOrWhiteSpace(customerEmail) || string.IsNullOrWhiteSpace(itemsStr))
                {
                    continue;
                }

                var orderLines = new List<OrderLine>();
                var itemParts = itemsStr.Split(';', StringSplitOptions.RemoveEmptyEntries);
                
                bool rowValid = true;
                foreach (var part in itemParts)
                {
                    var segments = part.Split(':');
                    if (segments.Length != 2) continue;

                    var productName = segments[0].Trim();
                    if (!int.TryParse(segments[1].Trim(), out var quantity)) continue;

                    var product = allProducts.FirstOrDefault(p => p.Name.Equals(productName, StringComparison.OrdinalIgnoreCase));
                    if (product == null)
                    {
                        rowValid = false;
                        break; 
                    }

                    orderLines.Add(new OrderLine(product.Id, product.Name, product.Price, quantity));
                }

                if (!rowValid || !orderLines.Any())
                {
                    errorCount++;
                    continue;
                }

                var orderId = Guid.NewGuid();
                await publishEndpoint.Publish(new CreateInvoice(
                    orderId,
                    customerName,
                    customerEmail,
                    orderLines));
                
                successCount++;
            }

            if (successCount > 0)
            {
                TempData["Success"] = $"Đã import thành công {successCount} đơn hàng. {errorCount} dòng lỗi bỏ qua.";
            }
            else
            {
                TempData["Warning"] = "Không import được đơn hàng nào. Vui lòng kiểm tra dữ liệu.";
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("File", $"Lỗi khi xử lý file: {ex.Message}");
            return View();
        }
    }
}
