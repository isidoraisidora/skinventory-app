namespace Domain.Config;

public class ProductEtlOptions
{
    public List<string> CategoryTags { get; set; } = new() { "en:face-care", "en:body-care", "en:sun-care", "en:cosmetics" };
    public int PageSize { get; set; } = 20;
    public int MaxPages { get; set; } = 15;
}