using Domain.Models;

namespace Domain.Services;

public class ExpirationCalculator : IExpirationCalculator
{
    public DateTime? GetEffectiveExpirationDate(InventoryItem item)
    {
        DateTime? paoExpiration = null;

        if (item.OpenedDate.HasValue && item.PaoMonths.HasValue)
        {
            paoExpiration = item.OpenedDate.Value.AddMonths(item.PaoMonths.Value);
        }

        if (item.ExpirationDate.HasValue && paoExpiration.HasValue)
        {
            return item.ExpirationDate.Value < paoExpiration.Value
                ? item.ExpirationDate.Value
                : paoExpiration.Value;
        }

        return item.ExpirationDate ?? paoExpiration;
    }

    public bool IsExpired(InventoryItem item, DateTime asOf)
    {
        var effective = GetEffectiveExpirationDate(item);
        return effective.HasValue && effective.Value <= asOf;
    }

    public bool IsExpiringSoon(InventoryItem item, DateTime asOf, int withinDays)
    {
        var effective = GetEffectiveExpirationDate(item);
        if (!effective.HasValue) return false;

        return effective.Value > asOf && effective.Value <= asOf.AddDays(withinDays);
    }
}