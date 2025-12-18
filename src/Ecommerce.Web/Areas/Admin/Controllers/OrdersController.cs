using Ecommerce.Infrastructure.Entities;
using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Web.Areas.Admin.ViewModels;
using Ecommerce.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MassTransit;
using Ecommerce.Contracts;
using ClosedXML.Excel;

namespace Ecommerce.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(AuthenticationSchemes = "AdminAuth")]
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
                ProcessedAt = order.Payment.ProcessedAt ?? DateTime.MinValue
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

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Orders");

        // Headers
        var headers = new[]
        {
            "Mã đơn hàng",
            "Khách hàng",
            "Email",
            "Số điện thoại",
            "Phương thức TT",
            "Nhà cung cấp TT",
            "Trạng thái",
            "Tổng tiền",
            "Phí ship",
            "Giảm giá",
            "Mã giảm giá",
            "Số lần thử TT",
            "Ngày tạo",
            "Ngày thanh toán",
            "Mã tham chiếu TT"
        };

        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cell(1, i + 1).Value = headers[i];
        }

        // Style header
        var headerRange = worksheet.Range(1, 1, 1, headers.Length);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Font.FontSize = 12;
        headerRange.Style.Fill.BackgroundColor = XLColor.FromArgb(68, 114, 196);
        headerRange.Style.Font.FontColor = XLColor.White;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        // Data
        for (int i = 0; i < orders.Count; i++)
        {
            var order = orders[i];
            var row = i + 2;

            worksheet.Cell(row, 1).Value = order.Id.ToString().Substring(0, 8);
            worksheet.Cell(row, 2).Value = order.CustomerName;
            worksheet.Cell(row, 3).Value = order.CustomerEmail;
            worksheet.Cell(row, 4).Value = order.CustomerPhone ?? "";
            worksheet.Cell(row, 5).Value = order.PaymentMethod;
            worksheet.Cell(row, 6).Value = order.PaymentProvider ?? "";
            worksheet.Cell(row, 7).Value = GetStatusText(order.Status);
            worksheet.Cell(row, 8).Value = order.Total;
            worksheet.Cell(row, 8).Style.NumberFormat.Format = "#,##0 ₫";
            worksheet.Cell(row, 9).Value = order.ShippingFee;
            worksheet.Cell(row, 9).Style.NumberFormat.Format = "#,##0 ₫";
            worksheet.Cell(row, 10).Value = order.Discount;
            worksheet.Cell(row, 10).Style.NumberFormat.Format = "#,##0 ₫";
            worksheet.Cell(row, 11).Value = order.CouponCode ?? "";
            worksheet.Cell(row, 12).Value = order.PaymentAttempts;
            worksheet.Cell(row, 13).Value = order.CreatedAt.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
            worksheet.Cell(row, 14).Value = order.PaymentDate?.ToLocalTime().ToString("dd/MM/yyyy HH:mm") ?? "";
            worksheet.Cell(row, 15).Value = order.Payment?.Reference ?? "";

            // Alternate row colors
            if (i % 2 == 0)
            {
                var rowRange = worksheet.Range(row, 1, row, headers.Length);
                rowRange.Style.Fill.BackgroundColor = XLColor.FromArgb(242, 242, 242);
            }

            // Color code status
            var statusCell = worksheet.Cell(row, 7);
            statusCell.Style.Font.Bold = true;
            switch (order.Status)
            {
                case OrderStatus.Completed:
                    statusCell.Style.Font.FontColor = XLColor.Green;
                    break;
                case OrderStatus.Failed:
                case OrderStatus.Cancelled:
                    statusCell.Style.Font.FontColor = XLColor.Red;
                    break;
                case OrderStatus.PaymentProcessing:
                    statusCell.Style.Font.FontColor = XLColor.Orange;
                    break;
            }
        }

        // Auto-fit columns
        worksheet.Columns().AdjustToContents();

        // Add borders to all cells
        var dataRange = worksheet.Range(1, 1, orders.Count + 1, headers.Length);
        dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        // Freeze header row
        worksheet.SheetView.FreezeRows(1);

        var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        var fileName = $"DonHang_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    private static string GetStatusText(OrderStatus status)
    {
        return status switch
        {
            OrderStatus.PendingInvoice => "Chờ hóa đơn",
            OrderStatus.Invoiced => "Đã xuất HĐ",
            OrderStatus.PaymentProcessing => "Đang xử lý TT",
            OrderStatus.Paid => "Đã thanh toán",
            OrderStatus.Notified => "Đã thông báo",
            OrderStatus.Completed => "Hoàn thành",
            OrderStatus.Cancelled => "Đã hủy",
            OrderStatus.Failed => "Thất bại",
            _ => status.ToString()
        };
    }

    [HttpGet]
    public IActionResult DownloadTemplate()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Orders");

        // Header
        worksheet.Cell(1, 1).Value = "Tên khách hàng *";
        worksheet.Cell(1, 2).Value = "Email *";
        worksheet.Cell(1, 3).Value = "Sản phẩm *";

        // Style header
        var headerRange = worksheet.Range(1, 1, 1, 3);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // Example rows
        worksheet.Cell(2, 1).Value = "Nguyễn Văn A";
        worksheet.Cell(2, 2).Value = "nguyenvana@example.com";
        worksheet.Cell(2, 3).Value = "iPhone 15:1; Ốp lưng:2";

        worksheet.Cell(3, 1).Value = "Trần Thị B";
        worksheet.Cell(3, 2).Value = "tranthib@example.com";
        worksheet.Cell(3, 3).Value = "MacBook Pro:1";

        // Instructions
        worksheet.Cell(5, 1).Value = "Hướng dẫn:";
        worksheet.Cell(5, 1).Style.Font.Bold = true;
        worksheet.Cell(6, 1).Value = "- Các cột có dấu * là bắt buộc";
        worksheet.Cell(7, 1).Value = "- Cột Sản phẩm: Tên SP:Số lượng; Tên SP 2:Số lượng";
        worksheet.Cell(8, 1).Value = "- Tên sản phẩm phải khớp chính xác với tên trong hệ thống";
        worksheet.Cell(9, 1).Value = "- Ví dụ: iPhone 15:1; Ốp lưng:2 (mua 1 iPhone và 2 ốp lưng)";
        worksheet.Cell(10, 1).Value = "- Xóa các dòng ví dụ và hướng dẫn này trước khi import";

        // Auto-fit columns
        worksheet.Columns().AdjustToContents();

        // Return file
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        var content = stream.ToArray();

        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Orders_Import_Template.xlsx");
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

        try
        {
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;
            
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheet(1);
            var rowCount = worksheet.LastRowUsed()?.RowNumber() ?? 0;
            
            int successCount = 0;
            int errorCount = 0;

            // Cache products for lookup
            var allProducts = await dbContext.Products
                .Select(x => new { x.Id, x.Name, x.Price })
                .ToListAsync();

            // Assume header is row 1
            for (int row = 2; row <= rowCount; row++)
            {
                var customerName = worksheet.Cell(row, 1).GetString();
                var customerEmail = worksheet.Cell(row, 2).GetString();
                var itemsStr = worksheet.Cell(row, 3).GetString();

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
