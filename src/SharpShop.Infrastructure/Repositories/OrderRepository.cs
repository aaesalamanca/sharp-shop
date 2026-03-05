using Microsoft.EntityFrameworkCore;
using SharpShop.Domain.Entities;
using SharpShop.Domain.Interfaces;
using SharpShop.Infrastructure.Data;

namespace SharpShop.Infrastructure.Repositories;

public class OrderRepository(AppDbContext context) : IOrderRepository
{
    public async Task<Order?> GetByIdAsync(Guid id)
    {
        return await context.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await context.Orders.Include(o => o.Items).ToListAsync();
    }

    public async Task AddAsync(Order order)
    {
        await context.Orders.AddAsync(order);

        foreach (var item in order.Items)
        {
            context.OrderItems.Add(item);
        }

        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Order order)
    {
        var trackedOrder = await context
            .Orders.Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == order.Id);

        if (trackedOrder is null)
            return;

        var trackedBookIds = trackedOrder.Items.Select(i => i.BookId).ToHashSet();
        var newBookIds = order.Items.Select(i => i.BookId).ToHashSet();

        var itemsToRemove = trackedOrder.Items.Where(i => !newBookIds.Contains(i.BookId)).ToList();
        foreach (var item in itemsToRemove)
        {
            context.OrderItems.Remove(item);
        }

        foreach (var newItem in order.Items)
        {
            var existingItem = trackedOrder.Items.FirstOrDefault(i => i.BookId == newItem.BookId);
            if (existingItem is not null)
            {
                existingItem.SetQuantity(newItem.Quantity);
            }
            else
            {
                var newOrderItem = new OrderItem(newItem.BookId, trackedOrder.Id, newItem.Quantity);
                context.OrderItems.Add(newOrderItem);
            }
        }

        trackedOrder.UpdateStatus(order.Status, order.ConfirmedAt, order.ShippedAt);

        await context.SaveChangesAsync();
    }
}
