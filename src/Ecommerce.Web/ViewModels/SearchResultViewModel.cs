namespace Ecommerce.Web.ViewModels;

public class SearchResultViewModel
{
    public string Query { get; set; } = string.Empty;
    public List<ProductViewModel> Results { get; set; } = new();
    public int TotalResults => Results.Count;
    public bool HasResults => Results.Any();
}
