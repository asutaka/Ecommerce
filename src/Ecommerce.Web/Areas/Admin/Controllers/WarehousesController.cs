using Ecommerce.Infrastructure.Entities;
using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Web.Areas.Admin.ViewModels;
using Ecommerce.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(AuthenticationSchemes = "AdminAuth")]
public class WarehousesController(EcommerceDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(string? searchTerm, int? pageNumber)
    {
        var query = dbContext.Warehouses.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(x => x.Code.ToLower().Contains(term) || 
                                     x.Name.ToLower().Contains(term) ||
                                     x.Address.ToLower().Contains(term));
        }

        query = query.OrderByDescending(x => x.CreatedAt);

        int pageSize = 10;
        var warehouses = await PaginatedList<Warehouse>.CreateAsync(query.AsNoTracking(), pageNumber ?? 1, pageSize);

        ViewBag.SearchTerm = searchTerm;
        return View(warehouses);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new WarehouseFormViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(WarehouseFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Check if code already exists
        if (await dbContext.Warehouses.AnyAsync(x => x.Code == model.Code))
        {
            ModelState.AddModelError("Code", "Mã kho đã tồn tại");
            return View(model);
        }

        var validPhoneNumbers = model.PhoneNumbers.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

        var warehouse = new Warehouse
        {
            Code = model.Code,
            Name = model.Name,
            Address = model.Address,
            PhoneNumbers = validPhoneNumbers
        };

        dbContext.Warehouses.Add(warehouse);
        await dbContext.SaveChangesAsync();

        TempData["Success"] = "Thêm kho hàng thành công";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var warehouse = await dbContext.Warehouses.FindAsync(id);
        if (warehouse == null)
        {
            return NotFound();
        }

        var model = new WarehouseFormViewModel
        {
            Id = warehouse.Id,
            Code = warehouse.Code,
            Name = warehouse.Name,
            Address = warehouse.Address,
            PhoneNumbers = new List<string>(warehouse.PhoneNumbers)
        };

        while (model.PhoneNumbers.Count < 5)
        {
            model.PhoneNumbers.Add(string.Empty);
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Guid id, WarehouseFormViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var warehouse = await dbContext.Warehouses.FindAsync(id);
        if (warehouse == null)
        {
            return NotFound();
        }

        // Check if code already exists (excluding current warehouse)
        if (await dbContext.Warehouses.AnyAsync(x => x.Code == model.Code && x.Id != id))
        {
            ModelState.AddModelError("Code", "Mã kho đã tồn tại");
            return View(model);
        }

        var validPhoneNumbers = model.PhoneNumbers.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

        warehouse.Code = model.Code;
        warehouse.Name = model.Name;
        warehouse.Address = model.Address;
        warehouse.PhoneNumbers = validPhoneNumbers;
        warehouse.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();

        TempData["Success"] = "Cập nhật kho hàng thành công";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        var warehouse = await dbContext.Warehouses.FindAsync(id);
        if (warehouse == null)
        {
            return NotFound();
        }

        dbContext.Warehouses.Remove(warehouse);
        await dbContext.SaveChangesAsync();

        TempData["Success"] = "Xóa kho hàng thành công";
        return RedirectToAction(nameof(Index));
    }
}
