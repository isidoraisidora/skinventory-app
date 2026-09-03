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
    private readonly IEmailSender _emailSender;

    private const int ReminderWindowDays = 7;

    public ExpirationCheckService(
        IRepository<InventoryItem> inventoryItemRepository,
        IExpirationCalculator expirationCalculator,
        IEmailSender emailSender)
    {
        _inventoryItemRepository = inventoryItemRepository;
        _expirationCalculator = expirationCalculator;
        _emailSender = emailSender;
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

                await _emailSender.SendAsync(
                    item.User.Email,
                    "Your product is expiring soon",
                    $"Hi {item.User.FirstName},\n\n" +
                    $"Your product \"{item.Product.Name}\" is set to expire in {daysLeft} day(s), on {effectiveExpiration.Value:yyyy-MM-dd}.\n\n" +
                    $"Consider using it up soon or replacing it.\n\n" +
                    $"— Skincare Inventory"
                );

                item.ReminderSent = true;
                await _inventoryItemRepository.UpdateAsync(item);
            }
        }
    }
}