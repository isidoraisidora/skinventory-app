using Domain.Models;

namespace Domain.Services;

public interface IExpirationCalculator
{
    DateTime? GetEffectiveExpirationDate(InventoryItem item);
    bool IsExpired(InventoryItem item, DateTime asOf);
    bool IsExpiringSoon(InventoryItem item, DateTime asOf, int withinDays);
}