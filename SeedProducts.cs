using Ecommerce.Infrastructure.Entities;
using Ecommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

// Script để seed 100 sản phẩm mẫu với ảnh từ Unsplash
// Chạy: dotnet run --project src/Ecommerce.SeedProducts

Console.WriteLine("🌱 Bắt đầu seed products...");

var connectionString = "Host=localhost;Database=ecommerce_db;Username=postgres;Password=your_password;Schema=ecommerce";

var optionsBuilder = new DbContextOptionsBuilder<EcommerceDbContext>();
optionsBuilder.UseNpgsql(connectionString, 
    o => o.MigrationsHistoryTable("__ef_migrations_history", "ecommerce"));
optionsBuilder.EnableDynamicJson();

using var dbContext = new EcommerceDbContext(optionsBuilder.Options);

Console.WriteLine("📦 Đang xóa products cũ...");

// Xóa ProductCategories trước (foreign key)
await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM ecommerce.\"ProductCategories\"");
// Xóa Products
await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM ecommerce.\"Products\"");

Console.WriteLine("✅ Đã xóa products cũ");

// Lấy danh sách categories
var categories = await dbContext.Categories.ToListAsync();
if (!categories.Any())
{
    Console.WriteLine("⚠️ Không có category nào! Vui lòng tạo categories trước.");
    return;
}

Console.WriteLine($"📂 Tìm thấy {categories.Count} categories");

// Danh sách ảnh từ Unsplash (quần áo nam/nữ)
var fashionImages = new List<string>
{
    // Ảnh nam
    "https://images.unsplash.com/photo-1490114538077-0a7f8cb49891",
    "https://images.unsplash.com/photo-1617127365659-c47fa864d8bc",
    "https://images.unsplash.com/photo-1594938298603-c8148c4dae35",
    "https://images.unsplash.com/photo-1602810318383-e386cc2a3ccf",
    "https://images.unsplash.com/photo-1574015974293-817f0ebebb74",
    "https://images.unsplash.com/photo-1605518216938-7c31b7b14ad0",
    "https://images.unsplash.com/photo-1520975954732-35dd22299614",
    "https://images.unsplash.com/photo-1621072156002-e2fccdc0b176",
    "https://images.unsplash.com/photo-1596755094514-f87e34085b2c",
    "https://images.unsplash.com/photo-1564859228273-274232fdb516",
    
    // Ảnh nữ
    "https://images.unsplash.com/photo-1515886657613-9f3515b0c78f",
    "https://images.unsplash.com/photo-1591047139829-d91aecb6caea",
    "https://images.unsplash.com/photo-1509631179647-0177331693ae",
    "https://images.unsplash.com/photo-1485968579580-b6d095142e6e",
    "https://images.unsplash.com/photo-1487412720507-e7ab37603c6f",
    "https://images.unsplash.com/photo-1544441893-675973e31985",
    "https://images.unsplash.com/photo-1558769132-cb1aea8f6b96",
    "https://images.unsplash.com/photo-1572804013309-59a88b7e92f1",
    "https://images.unsplash.com/photo-1539008835657-9e8e9680c956",
    "https://images.unsplash.com/photo-1502716119720-b23a93e5fe1b",
    "https://images.unsplash.com/photo-1469334031218-e382a71b716b",
    "https://images.unsplash.com/photo-1483181957632-8bda974cbc91",
    "https://images.unsplash.com/photo-1595777457583-95e059d581b8",
    "https://images.unsplash.com/photo-1566174053879-31528523f8ae",
    "https://images.unsplash.com/photo-1585487000160-6ebcfceb0d03",
    "https://images.unsplash.com/photo-1596783074918-c84cb06531ca",
    "https://images.unsplash.com/photo-1619784299229-e84d9b6e9c0f",
    "https://images.unsplash.com/photo-1612423284934-2850a4ea6b0f"
};

// Danh sách tên sản phẩm
var productNames = new[]
{
    "Áo thun basic", "Áo sơ mi", "Quần jean", "Quần kaki", "Áo khoác", 
    "Váy dài", "Váy ngắn", "Đầm dự tiệc", "Áo polo", "Áo hoodie",
    "Quần short", "Quần âu", "Áo len", "Áo ba lỗ", "Áo croptop",
    "Chân váy", "Đầm maxi", "Áo blazer", "Quần jogger", "Áo cardigan",
    "Đầm suông", "Quần culottes", "Áo phông form rộng", "Váy midi", "Áo kiểu"
};

var adjectives = new[] { "cao cấp", "thời trang", "sang trọng", "trẻ trung", "năng động", "thanh lịch", "hiện đại", "cổ điển", "vintage", "minimalist" };
var colors = new[] { "đen", "trắng", "xám", "be", "xanh navy", "xanh denim", "hồng", "đỏ", "nâu", "vàng" };

var random = new Random();
var products = new List<Product>();

Console.WriteLine("🎨 Đang tạo 100 sản phẩm...");

for (int i = 1; i <= 100; i++)
{
    var baseName = productNames[random.Next(productNames.Length)];
    var adj = adjectives[random.Next(adjectives.Length)];
    var color = colors[random.Next(colors.Length)];
    var name = $"{baseName} {adj} {color}";
    
    // Random 1-5 ảnh cho mỗi sản phẩm
    var imageCount = random.Next(1, 6);
    var images = new List<string>();
    for (int j = 0; j < imageCount; j++)
    {
        var randomImage = fashionImages[random.Next(fashionImages.Count)];
        // Thêm query params để có ảnh khác nhau
        images.Add($"{randomImage}?w=800&q=80&sig={i}{j}");
    }
    
    var category = categories[random.Next(categories.Count)];
    var price = random.Next(100, 2000) * 1000; // 100k - 2000k
    var isFeatured = random.Next(0, 10) < 3; // 30% nổi bật
    var isActive = random.Next(0, 10) < 9; // 90% active
    
    var product = new Product
    {
        Id = Guid.NewGuid(),
        Name = name,
        Description = $"Sản phẩm {name} chất lượng cao, thiết kế {adj}, phù hợp cho mọi lứa tuổi. Chất liệu tốt, bền đẹp.",
        Price = price,
        Images = images,
        IsFeatured = isFeatured,
        IsActive = isActive,
        PrimaryCategoryId = category.Id,
        CreatedAt = DateTime.UtcNow.AddDays(-random.Next(0, 90)),
        UpdatedAt = DateTime.UtcNow
    };
    
    products.Add(product);
    
    if (i % 20 == 0)
    {
        Console.WriteLine($"  ✓ Đã tạo {i}/100 sản phẩm...");
    }
}

Console.WriteLine("💾 Đang lưu vào database...");

await dbContext.Products.AddRangeAsync(products);
await dbContext.SaveChangesAsync();

Console.WriteLine("📋 Đang tạo ProductCategories...");

var productCategories = products.Select(p => new ProductCategory
{
    ProductId = p.Id,
    CategoryId = p.PrimaryCategoryId!.Value
}).ToList();

await dbContext.ProductCategories.AddRangeAsync(productCategories);
await dbContext.SaveChangesAsync();

Console.WriteLine("✨ Hoàn thành!");
Console.WriteLine($"📊 Đã seed {products.Count} sản phẩm thành công");
Console.WriteLine($"   - {products.Count(p => p.IsFeatured)} sản phẩm nổi bật");
Console.WriteLine($"   - {products.Count(p => p.IsActive)} sản phẩm active");
Console.WriteLine($"   - {products.Count(p => !p.IsActive)} sản phẩm inactive");
