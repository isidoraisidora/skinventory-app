using Domain.Enums;
using Domain.Models;
using Domain.Services;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class ExpirationCheckService : IExpirationCheckService
{
    private readonly IRepository<InventoryItem> _inventoryItemRepository;
    private readonly IExpirationCalculator _expirationCalculator;
    private readonly IEmailQueue _queue;

    private const int ReminderWindowDays = 7;

    public ExpirationCheckService(
        IRepository<InventoryItem> inventoryItemRepository,
        IExpirationCalculator expirationCalculator,
        IEmailQueue queue)
    {
        _inventoryItemRepository = inventoryItemRepository;
        _expirationCalculator = expirationCalculator;
        _queue = queue;
    }

    public async Task RunAsync()
    {
        var now = DateTime.UtcNow;

        var items = await _inventoryItemRepository.GetAllAsync(
            selector: x => x,
            predicate: x => x.ProductStatus == ProductStatus.Active || x.ProductStatus == ProductStatus.Opened,
            include: q => q.Include(x => x.User).Include(x => x.Product));

        foreach (var item in items)
        {
            var effectiveExpiration = _expirationCalculator.GetEffectiveExpirationDate(item);
            if (effectiveExpiration == null)
                continue;

            if (effectiveExpiration.Value <= now)
            {
                item.ProductStatus = ProductStatus.Expired;
                await _inventoryItemRepository.UpdateAsync(item);
                continue;
            }

            var isExpiringSoon = effectiveExpiration.Value <= now.AddDays(ReminderWindowDays);

            if (isExpiringSoon && !item.ReminderSent)
            {
                var daysLeft = (effectiveExpiration.Value - now).Days;

                await _queue.EnqueueAsync(new EmailMessage(
                    item.User.Email,
                    "Your product is expiring soon",
                    $"Hi {item.User.FirstName}, your product \"{item.Product.Name}\" expires in {daysLeft} day(s)."
                ));

                item.ReminderSent = true;
                await _inventoryItemRepository.UpdateAsync(item);
            }
        }
    }
}