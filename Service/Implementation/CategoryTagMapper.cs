namespace Service.Implementation;

public class CategoryTagMapper
{
    private static readonly Dictionary<string, string> TagToDisplayName = new()
    {
        { "en:face-care", "Skincare" },
        { "en:skin-care", "Skincare" },
        { "en:skincare", "Skincare" },
        { "en:body-care", "Bodycare" },
        { "en:cosmetics", "Cosmetics" },
        { "en:sun-care", "Sun Care" }
    };

    public static string ToDisplayName(string tag) =>
        TagToDisplayName.TryGetValue(tag, out var name) ? name : tag;
}