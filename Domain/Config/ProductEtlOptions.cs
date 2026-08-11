namespace Domain.Config;

public class ProductEtlOptions
{
    public string CategoryTag { get; set; } = "en:skincare";
    public int PageSize { get; set; } = 50;
    public int MaxPages { get; set; } = 5;
}