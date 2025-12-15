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
public class SuppliersController(EcommerceDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(string? searchTerm, int? pageNumber)
    {
        var query = dbContext.Suppliers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(x => x.Code.ToLower().Contains(term) || 
                                     x.Name.ToLower().Contains(term) ||
                                     x.Address.ToLower().Contains(term));
        }

        query = query.OrderByDescending(x => x.CreatedAt);

        int pageSize = 10;
        var suppliers = await PaginatedList<Supplier>.CreateAsync(query.AsNoTracking(), pageNumber ?? 1, pageSize);

        ViewBag.SearchTerm = searchTerm;
        return View(suppliers);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new SupplierFormViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(SupplierFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Check if code already exists
        if (await dbContext.Suppliers.AnyAsync(x => x.Code == model.Code))
        {
            ModelState.AddModelError("Code", "Mã nhà cung cấp đã tồn tại");
            return View(model);
        }

        var validPhoneNumbers = model.PhoneNumbers.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

        var supplier = new Supplier
        {
            Code = model.Code,
            Name = model.Name,
            Address = model.Address,
            PhoneNumbers = validPhoneNumbers
        };

        dbContext.Suppliers.Add(supplier);
        await dbContext.SaveChangesAsync();

        TempData["Success"] = "Thêm nhà cung cấp thành công";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var supplier = await dbContext.Suppliers.FindAsync(id);
        if (supplier == null)
        {
            return NotFound();
        }

        var model = new SupplierFormViewModel
        {
            Id = supplier.Id,
            Code = supplier.Code,
            Name = supplier.Name,
            Address = supplier.Address,
            PhoneNumbers = new List<string>(supplier.PhoneNumbers)
        };

        while (model.PhoneNumbers.Count < 5)
        {
            model.PhoneNumbers.Add(string.Empty);
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Guid id, SupplierFormViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var supplier = await dbContext.Suppliers.FindAsync(id);
        if (supplier == null)
        {
            return NotFound();
        }

        // Check if code already exists (excluding current supplier)
        if (await dbContext.Suppliers.AnyAsync(x => x.Code == model.Code && x.Id != id))
        {
            ModelState.AddModelError("Code", "Mã nhà cung cấp đã tồn tại");
            return View(model);
        }

        var validPhoneNumbers = model.PhoneNumbers.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

        supplier.Code = model.Code;
        supplier.Name = model.Name;
        supplier.Address = model.Address;
        supplier.PhoneNumbers = validPhoneNumbers;
        supplier.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();

        TempData["Success"] = "Cập nhật nhà cung cấp thành công";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        var supplier = await dbContext.Suppliers.FindAsync(id);
        if (supplier == null)
        {
            return NotFound();
        }

        dbContext.Suppliers.Remove(supplier);
        await dbContext.SaveChangesAsync();

        TempData["Success"] = "Xóa nhà cung cấp thành công";
        return RedirectToAction(nameof(Index));
    }
}
