using Service.Interface;
using Web.Extensions;
using Web.Request;
using Web.Response;

namespace Web.Mappers;

public class IngredientMapper
{
    private readonly IIngredientService _ingredientService;

    public IngredientMapper(IIngredientService ingredientService)
    {
        _ingredientService = ingredientService;
    }

    public async Task<List<IngredientResponse>> GetAllAsync()
    {
        var result = await _ingredientService.GetAllAsync();
        return result.Select(x => x.ToResponse()).ToList();
    }

    public async Task<IngredientResponse> GetByIdAsync(Guid id)
    {
        var result = await _ingredientService.GetByIdNotNullAsync(id);
        return result.ToResponse();
    }

    public async Task<IngredientResponse> CreateAsync(IngredientRequest request)
    {
        var result = await _ingredientService.CreateAsync(request.Name, request.InciName);
        return result.ToResponse();
    }

    public async Task<IngredientResponse> UpdateAsync(Guid id, IngredientRequest request)
    {
        var result = await _ingredientService.UpdateAsync(id, request.Name, request.InciName);
        return result.ToResponse();
    }

    public async Task<IngredientResponse> DeleteAsync(Guid id)
    {
        var result = await _ingredientService.DeleteAsync(id);
        return result.ToResponse();
    }
}