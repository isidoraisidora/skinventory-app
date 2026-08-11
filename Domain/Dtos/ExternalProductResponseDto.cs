using System.Text.Json.Serialization;

namespace Domain.Dtos;

public class ExternalProductResponseDto
{
    [JsonPropertyName("products")]
    public List<ExternalProductDto> Products { get; set; } = new();

    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("page")]
    public int Page { get; set; }
}

public class ExternalProductDto
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("product_name")]
    public string? ProductName { get; set; }

    [JsonPropertyName("brands")]
    public string? Brands { get; set; }

    [JsonPropertyName("ingredients_text")]
    public string? IngredientsText { get; set; }

    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }
}