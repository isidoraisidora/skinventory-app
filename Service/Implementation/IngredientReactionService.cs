using Domain.Enums;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class IngredientReactionService : IIngredientReactionService
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IRepository<IngredientReaction> _reactionRepository;
    private readonly IRepository<Product> _productRepository;

    private const int SignificantSeverityThreshold = 5;

    public IngredientReactionService(
        ICurrentUserService currentUserService,
        IRepository<IngredientReaction> reactionRepository,
        IRepository<Product> productRepository)
    {
        _currentUserService = currentUserService;
        _reactionRepository = reactionRepository;
        _productRepository = productRepository;
    }

    public async Task<List<IngredientReaction>> GetAllForUserAsync()
    {
        var user = _currentUserService.GetUserId();
        return await _reactionRepository.GetAllAsync(
            selector: x => x,
            predicate: x => x.CreatedById == user);
    }

    private async Task<IngredientReaction> GetOwnedReactionOrThrow(Guid id)
    {
        var user = _currentUserService.GetUserId();
        var reaction = await _reactionRepository.GetAsync(
            selector: x => x,
            predicate: x => x.Id == id && x.CreatedById == user);

        if (reaction == null)
            throw new InvalidOperationException("Reaction log not found.");

        return reaction;
    }

    public async Task<IngredientReaction> LogReactionAsync(Guid productId, Guid ingredientId, ReactionType type,
        int severity, string? note)
    {
        if (severity is < 1 or > 10)
            throw new ArgumentOutOfRangeException(nameof(severity), "Severity must be between 1 and 10.");

        var user = _currentUserService.GetUserId();

        var reaction = new IngredientReaction
        {
            CreatedById = user,
            ProductId = productId,
            IngredientId = ingredientId,
            ReactionType = type,
            ReactionSeverity = severity,
            Note = note
        };

        return await _reactionRepository.InsertAsync(reaction);
    }

    public async Task<IngredientReaction> UpdateAsync(Guid id, ReactionType? type, int? severity, string? note)
    {
        var reaction = await GetOwnedReactionOrThrow(id);

        if (severity is < 1 or > 10)
            throw new ArgumentOutOfRangeException(nameof(severity), "Severity must be between 1 and 10.");

        if (type != null) reaction.ReactionType = type.Value;
        if (severity != null) reaction.ReactionSeverity = severity.Value;
        if (note != null) reaction.Note = note;

        return await _reactionRepository.UpdateAsync(reaction);
    }

    public async Task<IngredientReaction> DeleteAsync(Guid id)
    {
        var reaction = await GetOwnedReactionOrThrow(id);
        return await _reactionRepository.DeleteAsync(reaction);
    }

    public async Task<List<Ingredient>> GetConflictingIngredientsAsync(Guid productId)
    {
        var user = _currentUserService.GetUserId();

        var reactedIngredientIds = (await _reactionRepository.GetAllAsync(
                selector: x => x.IngredientId,
                predicate: x => x.CreatedById == user && x.ReactionSeverity >= SignificantSeverityThreshold))
            .ToHashSet();

        if (reactedIngredientIds.Count == 0)
            return new List<Ingredient>();

        var product = await _productRepository.GetAsync(
            selector: x => x,
            predicate: x => x.Id == productId,
            include: q => q.Include(p => p.Ingredients));

        if (product == null)
            throw new InvalidOperationException("Product not found.");

        return product.Ingredients.Where(i => reactedIngredientIds.Contains(i.Id)).ToList();
    }
}