using Microsoft.EntityFrameworkCore;
using SharpShop.Domain.Entities;
using SharpShop.Domain.Interfaces;
using SharpShop.Infrastructure.Data;

namespace SharpShop.Infrastructure.Repositories;

public class OrderRepository(AppDbContext context) : IOrderRepository
{
    public async Task<Order?> GetByIdAsync(Guid id)
    {
        return await context
            .Orders.Include(o => o.Items)
                .ThenInclude(i => i.Book)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await context.Orders.Include(o => o.Items).ThenInclude(i => i.Book).ToListAsync();
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
        var existingOrder = await context
            .Orders.Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == order.Id);

        if (existingOrder is null)
            return;

        context.OrderItems.RemoveRange(existingOrder.Items);

        foreach (var item in order.Items)
        {
            context.OrderItems.Add(item);
        }

        context.Orders.Update(order);
        await context.SaveChangesAsync();
    }
}
