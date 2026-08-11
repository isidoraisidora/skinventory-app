using System.Net.Http.Json;
using Domain.Dtos;
using Domain.Models;
using Service.Interface;

namespace Service.Implementation;

public class ExternalProductApi : IExternalProductApi
{
    private readonly HttpClient _httpClient;

    public ExternalProductApi(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Product>> SearchProductsAsync(string categoryTag, int page, int pageSize)
    {
        var url = $"api/v2/search?categories_tags={categoryTag}&page={page}&page_size={pageSize}";

        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException("Cannot read from API.");

        var result = await response.Content.ReadFromJsonAsync<ExternalProductResponseDto>();

        if (result?.Products == null || result.Products.Count == 0)
            throw new InvalidOperationException("Cannot read products from the API.");

        return result.Products
            .Where(dto => !string.IsNullOrWhiteSpace(dto.Code) && !string.IsNullOrWhiteSpace(dto.ProductName))
            .Select(ExternalProductTransformer.ToProduct)
            .ToList();
    }
}