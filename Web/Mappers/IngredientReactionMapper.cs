using Domain.Dtos;
using Service.Interface;
using Web.Extensions;
using Web.Request;
using Web.Response;

namespace Web.Mappers;

public class IngredientReactionMapper
{
    private readonly IIngredientReactionService _ingredientReactionService;

    public IngredientReactionMapper(IIngredientReactionService ingredientReactionService)
    {
        _ingredientReactionService = ingredientReactionService;
    }

    public async Task<List<IngredientReactionResponse>> GetAllAsync()
    {
        var result = await _ingredientReactionService.GetAllForUserAsync();
        return result.Select(x => x.ToResponse()).ToList();
    }

    public async Task<IngredientReactionResponse> LogAsync(LogReactionRequest request)
    {
        var dto = new IngredientReactionDto
        {
            ProductId = request.ProductId,
            IngredientId = request.IngredientId,
            Type = request.Type,
            Severity = request.Severity,
            Note = request.Note
        };

        var result = await _ingredientReactionService.LogReactionAsync(dto);
        return result.ToResponse();
    }

    public async Task<IngredientReactionResponse> UpdateAsync(Guid id, UpdateReactionRequest request)
    {
        var result = await _ingredientReactionService.UpdateAsync(id, request.Type, request.Severity, request.Note);
        return result.ToResponse();
    }

    public async Task DeleteAsync(Guid id)
    {
        await _ingredientReactionService.DeleteAsync(id);
    }

    public async Task<List<string>> GetConflictingIngredientsAsync(Guid productId)
    {
        var result = await _ingredientReactionService.GetConflictingIngredientsAsync(productId);
        return result.Select(i => i.Name).ToList();
    }
}