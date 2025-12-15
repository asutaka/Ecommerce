using System.Data;
using Dapper;
using Ecommerce.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Ecommerce.Web.Controllers;

public class HomeDapperController(IConfiguration configuration) : Controller
{
    private IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
    }

    [HttpGet]
    [Route("dapper")]
    public async Task<IActionResult> Index()
    {
        // SQL query to fetch all required data in a SINGLE roundtrip
        // 1. Featured Products (Top 12)
        // 2. Featured Categories (Top 6 by product count)
        // 3. New Arrivals (Top 4)
        const string sql = @"
            -- 1. Featured Products
            SELECT ""Id"", ""Name"", ""Description"", ""Images"", ""Price"", ""IsFeatured""
            FROM ""Products""
            WHERE ""IsFeatured"" = true
            ORDER BY ""Name""
            LIMIT 12;

            -- 2. Featured Categories
            SELECT c.""Id"", c.""Name"", c.""Description"", 
                   (SELECT COUNT(*) FROM ""Products"" p WHERE p.""PrimaryCategoryId"" = c.""Id"") as ""ProductCount""
            FROM ""Categories"" c
            ORDER BY ""ProductCount"" DESC
            LIMIT 6;

            -- 3. New Arrivals
            SELECT ""Id"", ""Name"", ""Description"", ""Images"", ""Price"", ""IsFeatured""
            FROM ""Products""
            ORDER BY ""CreatedAt"" DESC
            LIMIT 4;
        ";

        using var connection = CreateConnection();
        
        // Execute multiple queries at once
        using var multi = await connection.QueryMultipleAsync(sql);

        var featuredProducts = await multi.ReadAsync<ProductViewModel>();
        var categories = await multi.ReadAsync<CategorySummaryViewModel>();
        var newArrivals = await multi.ReadAsync<ProductViewModel>();

        var viewModel = new HomeViewModel
        {
            FeaturedProducts = featuredProducts.ToList(),
            FeaturedCategories = categories.ToList(),
            NewArrivals = newArrivals.ToList()
        };

        // Return the same view as HomeController
        return View("~/Views/Home/Index.cshtml", viewModel);
    }
}
