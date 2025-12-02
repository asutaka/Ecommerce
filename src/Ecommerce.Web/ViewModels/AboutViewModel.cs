namespace Ecommerce.Web.ViewModels;

public class AboutViewModel
{
    public string Title { get; set; } = "Về Moderno";
    public string Description { get; set; } = string.Empty;
    public AboutStatistics Statistics { get; set; } = new();
    public List<WhyChooseUs> Features { get; set; } = new();
}

public class AboutStatistics
{
    public int YearsInOperation { get; set; }
    public decimal CustomerRating { get; set; }
    public int CustomerSatisfaction { get; set; }
}

public class WhyChooseUs
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}
